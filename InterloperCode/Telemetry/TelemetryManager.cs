using Interloper.InterloperCode.Telemetry;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Telemetry;

/// <summary>
/// High-level telemetry API. All network work is fire-and-forget and fails silently,
/// so it never blocks gameplay or crashes the game. Only tracks Interloper runs.
/// </summary>
public static class TelemetryManager
{
    private static SupabaseClient? _client;
    private static string? _runId;
    private static IRunState? _runState;
    private static bool _active;
    private static readonly Dictionary<Reward, Guid> _offerGroups = new();
    private static readonly Dictionary<string, int> _deckSnapshot = new();

    public static bool Enabled => _client != null;

    public static void Configure(string baseUrl, string anonKey)
    {
        _client = new SupabaseClient(baseUrl, anonKey);
    }

    public static void StartRun(IRunState runState)
    {
        if (_client == null)
            return;

        var player = runState.Players.FirstOrDefault();
        if (player == null || player.Character is not InterloperCharacter)
            return;

        _runState = runState;
        _runId = Guid.NewGuid().ToString();
        _active = true;

        var body = new
        {
            id = _runId,
            started_at = Now(),
            character_id = "Interloper",
            ascension = runState.AscensionLevel
        };
        FireAndForget(() => _client.PostAsync("runs", body));
    }

    public static void EndRun(bool victory)
    {
        if (!_active || _client == null || _runId == null)
            return;

        string runId = _runId;
        var runState = _runState;

        FireAndForget(async () =>
        {
            await _client.PatchAsync("runs", runId, new { ended_at = Now(), victory });
            if (runState != null)
                await RecordFinalDeck(runState, runId);
        });

        Reset();
    }

    public static void RecordCardOffer(IRunState runState, Player player, CardReward reward)
    {
        if (!_active || _client == null || _runId == null)
            return;

        var offerGroup = Guid.NewGuid();
        _offerGroups[reward] = offerGroup;
        _deckSnapshot.Clear();

        foreach (var card in reward.Cards)
        {
            _deckSnapshot[card.Id.Entry] = CountInDeck(player, card.Id.Entry);
            string cardId = card.Id.Entry;
            Guid group = offerGroup;
            FireAndForget(() => PostCardEvent(runState, cardId, "offered", group));
        }
    }

    public static void RecordCardPick(IRunState runState, Player player, CardReward reward)
    {
        if (!_active || _client == null || _runId == null)
            return;

        if (!_offerGroups.TryGetValue(reward, out var offerGroup))
            return;
        _offerGroups.Remove(reward);

        foreach (var card in reward.Cards)
        {
            string key = card.Id.Entry;
            int before = _deckSnapshot.GetValueOrDefault(key, 0);
            int after = CountInDeck(player, key);
            if (after > before)
            {
                FireAndForget(() => PostCardEvent(runState, key, "picked", offerGroup));
                break;
            }
        }

        _deckSnapshot.Clear();
    }

    public static void RecordCardPlay(CardModel card, IRunState runState, int turn)
    {
        if (!_active || _client == null || _runId == null)
            return;

        string cardId = card.Id.Entry;
        string runId = _runId;
        int act = runState.CurrentActIndex + 1;
        int floor = runState.ActFloor;

        FireAndForget(() => _client.PostAsync("card_plays", new
        {
            run_id = runId,
            card_id = cardId,
            act,
            floor,
            turn
        }));
    }

    private static async Task PostCardEvent(IRunState runState, string cardId, string eventType, Guid offerGroup)
    {
        if (_client == null || _runId == null)
            return;

        await _client.PostAsync("run_cards", new
        {
            run_id = _runId,
            card_id = cardId,
            event_type = eventType,
            act = runState.CurrentActIndex + 1,
            floor = runState.ActFloor,
            offer_group_id = offerGroup
        });
    }

    private static async Task RecordFinalDeck(IRunState runState, string runId)
    {
        if (_client == null)
            return;

        var player = runState.Players.FirstOrDefault(p => p.Character is InterloperCharacter);
        if (player == null)
            return;

        foreach (var card in player.Deck.Cards)
        {
            await _client.PostAsync("run_decks", new
            {
                run_id = runId,
                card_id = card.Id.Entry,
                upgraded = card.IsUpgraded
            });
        }
    }

    private static int CountInDeck(Player player, string cardId)
    {
        return player.Deck.Cards.Count(c => c.Id.Entry == cardId);
    }

    private static string Now() => DateTime.UtcNow.ToString("o");

    private static void FireAndForget(Func<Task> action)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await action();
            }
            catch
            {
                // Fail silently; telemetry must never crash the game.
            }
        });
    }

    private static void Reset()
    {
        _active = false;
        _runId = null;
        _runState = null;
        _offerGroups.Clear();
        _deckSnapshot.Clear();
    }
}