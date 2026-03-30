using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;


namespace OriginRelicBox.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class LieRenShaShouShaShou : RelicModelBase
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool ShowCounter => true;

    public override int DisplayAmount => ShaShou_Counter;

    private int _counter;

    [SavedProperty]
    public int ShaShou_Counter
    {
        get => _counter;
        set
        {
            AssertMutable();
            _counter = value;
            InvokeDisplayAmountChanged();
        }
    }


    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Unpowered), new CardsVar(2), new EnergyVar(1)];

    private bool _hunterKillerLived = false;

    private int _cardsPlayedThisTurn;

    private int CardsPlayedThisTurn
    {
        get
        {
            return _cardsPlayedThisTurn;
        }
        set
        {
            AssertMutable();
            _cardsPlayedThisTurn = value;
            InvokeDisplayAmountChanged();
        }
    }
    public override async Task BeforeCombatStart()
    {

        if (DisplayAmount >= 10)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            Flash();
        }

        if (DisplayAmount >= 20)
        {

            await CreatureCmd.Heal(Owner.Creature, 2);
            Flash();
        }

        if (DisplayAmount >= 30)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, null);
        }
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {

        if (player != Owner)
        {
            return count;
        }
        if (player.Creature.CombatState!.RoundNumber > 1)
        {
            return count;
        }
        if (DisplayAmount < 40)
        {
            return count;
        }
        return count + DynamicVars.Cards.BaseValue;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }
        if (!CombatManager.Instance.IsInProgress)
        {
            return;

        }
        ShaShou_Counter++;
        




        if (_hunterKillerLived)
        {
            CardsPlayedThisTurn++;
            Flash();
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1m, Owner.Creature, null, silent: true);
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1m, Owner.Creature, null, silent: true);
        }


    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState!.RoundNumber > 1)
        {
            return Task.CompletedTask;
        }
        if (DisplayAmount >= 50)
        {
            PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, this.Owner);
        }
        ShaShou_Counter = 0;


        IReadOnlyList<Creature> creature = Owner.Creature.CombatState!.HittableEnemies;
        foreach (var monster in creature)
        {
            if (monster.ModelId.Entry == "HUNTER_KILLER")
            {

                _hunterKillerLived = true;
            }
        }

        return Task.CompletedTask;
    }

    public override Task BeforeDeath(Creature creature)
    {
        if (creature.ModelId.Entry == "HUNTER_KILLER")
        {

            _hunterKillerLived = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatVictoryEarly(CombatRoom room)
    {
        _hunterKillerLived = false;

        

        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, -CardsPlayedThisTurn, Owner.Creature, null, silent: true);
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, -CardsPlayedThisTurn, Owner.Creature, null, silent: true);
            CardsPlayedThisTurn = 0;
        }
    }
}