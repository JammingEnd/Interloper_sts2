using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Interloper.InterloperCode.Relics;

// the first time each turn you play a colorless card, gain 2 void reach. gain a colorless card reward on pickup
public class FamilyPictureRelic : InterloperRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    private bool _usedThisTurn;
    private bool UsedThisTurn
    {
        get => this._usedThisTurn;
        set
        {
            this.AssertMutable();
            this._usedThisTurn = value;
        }
    }
    public override async Task AfterObtained()
    {
        var colorlessPool = ModelDb.CardPool<ColorlessCardPool>();
        var options = new CardCreationOptions(
            new List<CardPoolModel> { colorlessPool },
            CardCreationSource.Other,
            CardRarityOddsType.Uniform);

        var reward = new CardReward(
            options, 3, Owner, RunManager.Instance.PlayerChoiceSynchronizer);

        await RewardsCmd.OfferCustom(Owner, new List<Reward> { reward });
    }
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return;
        this.UsedThisTurn = false;
        this.Status = RelicStatus.Active;
    }
    public override async Task AfterCombatEnd(CombatRoom _)
    {
        this.UsedThisTurn = false;
        this.Status = RelicStatus.Normal;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            this.Status = RelicStatus.Active;
            this.UsedThisTurn = false;
        }
        
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        if (_usedThisTurn)
            return;

        if (cardPlay.Card.Pool?.IsColorless != true)
            return;

        UsedThisTurn = true;
        await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, 2, Owner.Creature, null);
    }
}