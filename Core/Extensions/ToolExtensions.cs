using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace OriginRelicBox.Extensions;

public static class ToolExtensions
{

    public static async Task AddCardToDeck<T>(Player player, int amount = 1) where T : CardModel
    {
        if (player == null || amount < 1) return;

        var resultList = new List<CardPileAddResult>();

        for (int i = 0; i < amount; i++)
        {
            CardModel card = player.RunState.CreateCard<T>(player);

            var addResult = await CardPileCmd.Add(card, PileType.Deck);
            resultList.Add(addResult);
        }
        CardCmd.PreviewCardPileAdd(resultList, 2f);

    }

}