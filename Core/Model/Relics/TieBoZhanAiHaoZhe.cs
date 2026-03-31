using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using OriginRelicBox.Extensions;


namespace OriginRelicBox.Relics;


[Pool(typeof(SharedRelicPool))]
public sealed class TieBoZhanAiHaoZhe : RelicModelBase
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [

        HoverTipFactory.FromCard<IronWave>()
    ];

    public override async Task AfterObtained()
    {

        await ToolExtensions.AddCardToDeck<IronWave>(Owner);

        List<CardModel> list1 = PileType.Deck.GetPile(Owner).Cards.Where(c => c != null && c.Tags.Contains(CardTag.Strike)).ToList();
        List<CardModel> list2 = PileType.Deck.GetPile(Owner).Cards.Where(c => c != null && c.Tags.Contains(CardTag.Defend)).ToList();

        int count = list1.Count + list2.Count;

        foreach (CardModel item in list1.Concat(list2))
        {
            await CardPileCmd.RemoveFromDeck(item);
        }
        await Cmd.Wait(0.75f);
        await ToolExtensions.AddCardToDeck<IronWave>(Owner, count);


    }
}