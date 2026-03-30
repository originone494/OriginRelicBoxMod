using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OriginRelicBox.Powers;

public abstract class JianYingPower : PowerModelBase
{
    private int _cardsPlayedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => CardsPlayedThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

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

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner.Player)
        {
            CardsPlayedThisTurn++;
            Flash();
            await PowerCmd.Apply<StrengthPower>(base.Owner, 1m, base.Applier, null, silent: true);
            await PowerCmd.Apply<DexterityPower>(base.Owner, 1m, base.Applier, null, silent: true);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            await PowerCmd.Apply<StrengthPower>(base.Owner, -CardsPlayedThisTurn, base.Applier, null, silent: true);
            await PowerCmd.Apply<DexterityPower>(base.Owner, -CardsPlayedThisTurn, base.Applier, null, silent: true);
            CardsPlayedThisTurn = 0;
        }
    }

}