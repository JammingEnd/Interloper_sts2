using BaseLib.Abstracts;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Telemetry;

public class TelemetryRewardHook() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom room)
    {
        if (player.Character is not InterloperCharacter)
            return false;

        foreach (var reward in rewards.OfType<CardReward>())
            TelemetryManager.RecordCardOffer(player.RunState, player, reward);

        return false;
    }

    public override async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (player.Character is not InterloperCharacter)
            return;

        if (reward is CardReward cardReward)
            TelemetryManager.RecordCardPick(player.RunState, player, cardReward);
    }
}

public class TelemetryCombatHook() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is not InterloperCharacter)
            return;

        int turn = cardPlay.Card.Owner.PlayerCombatState?.TurnNumber ?? 1;
        TelemetryManager.RecordCardPlay(cardPlay.Card, cardPlay.Card.Owner.RunState, turn);
    }
}