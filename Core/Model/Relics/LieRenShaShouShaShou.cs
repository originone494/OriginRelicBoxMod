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

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Unpowered),
         new CardsVar(2), 
         new EnergyVar(1),
         new DynamicVar("LieRenShaShouShaShou_GetBlockAmount",8m),
         new DynamicVar("LieRenShaShouShaShou_GetHeal",16m),
         new DynamicVar("LieRenShaShouShaShou_GetHealAmount", 1m),
         new DynamicVar("LieRenShaShouShaShou_GetPower",25m),
         new DynamicVar("LieRenShaShouShaShou_ExtraDraw",32m),
         new DynamicVar("LieRenShaShouShaShou_ExtraEnergy",40m),
         ];


    private int _counter;

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
        }
    }

    private bool _isGetBlock = false;

    private bool _isGetHeal = false;

    private bool _isGetPower = false;


    public override decimal ModifyHandDraw(Player player, decimal count)
    {

        if (player != Owner)
        {
            return count;
        }

        if (DisplayAmount < DynamicVars["LieRenShaShouShaShou_ExtraDraw"].BaseValue)
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


        if (DisplayAmount >= DynamicVars["LieRenShaShouShaShou_GetBlockAmount"].BaseValue && !_isGetBlock)
        {
            _isGetBlock = true;
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            Flash();
        }

        if (DisplayAmount >= DynamicVars["LieRenShaShouShaShou_GetHeal"].BaseValue && !_isGetHeal)
        {
            _isGetHeal = true;
            await CreatureCmd.Heal(Owner.Creature, DynamicVars["LieRenShaShouShaShou_GetHealAmount"].BaseValue);
            Flash();
        }

        if (DisplayAmount >= DynamicVars["LieRenShaShouShaShou_GetPower"].BaseValue && !_isGetPower)
        {
            _isGetPower = true;

            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, null);
        }


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

        if (DisplayAmount >= DynamicVars["LieRenShaShouShaShou_ExtraEnergy"].BaseValue)
        {
            PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, this.Owner);
        }

        if (player.Creature.CombatState!.RoundNumber > 1)
        {
            return Task.CompletedTask;

        }

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

        _isGetBlock = false;
        _isGetHeal = false;
        _isGetPower = false;

        ShaShou_Counter = 0;

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