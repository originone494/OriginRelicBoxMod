using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace OriginRelicBox.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class BaoHuSheYao : RelicModelBase
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BaoHuSheYao_PoisonCount", 2m)];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner && Owner.Creature.CombatState!.RoundNumber == 1)
        {
            Flash();
            CardModel cardModel = base.Owner.Creature.CombatState.CreateCard<Snakebite>(base.Owner);
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
        }
    }
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == base.Owner && Owner.Creature.CombatState!.RoundNumber == 1)
        {
            return;
        }

        List<CardModel> list = PileType.Hand.GetPile(Owner).Cards.Where(c => c != null && c.Id.Entry == "SNAKEBITE").ToList();

        int time = list.Count;

        Flash();

        for (int i = 0; i < time; i++)
        {
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            foreach (Creature hittableEnemy in base.Owner.Creature.CombatState!.HittableEnemies)
            {
                NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemy);
                if (nCreature != null)
                {
                    NGaseousImpactVfx? child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("83eb85"));
                    NCombatRoom.Instance!.CombatVfxContainer.AddChildSafely(child);
                }
            }
            await PowerCmd.Apply<PoisonPower>(base.Owner.Creature.CombatState.HittableEnemies, DynamicVars["BaoHuSheYao_PoisonCount"].BaseValue, Owner.Creature, null);
        }
    }
}