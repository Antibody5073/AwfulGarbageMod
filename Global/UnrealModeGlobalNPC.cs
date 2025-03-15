using AwfulGarbageMod.Items;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Summon;
using Microsoft.CodeAnalysis;
using AwfulGarbageMod.Items.Tools;
using System.Linq;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.NPCs.BossUnrealRework.KingSlime;
using AwfulGarbageMod.NPCs.BossUnrealRework.DukeFishron;

namespace AwfulGarbageMod.Global
{
    public class UnrealModeGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.SlimeCrown)
            {
                return !NPC.AnyNPCs(50) & !NPC.AnyNPCs(ModContent.NPCType<KingSlimePhase1>()) & !NPC.AnyNPCs(ModContent.NPCType<KingSlimePhase2>());

            }
            return base.CanUseItem(item, player);
        }
    }

    public class UnrealModeGlobalNPC : GlobalNPC
    {
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                if (npc.type == NPCID.KingSlime)
                {
                    NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<KingSlimePhase1>(), 0, npc.whoAmI);
                    npc.active = false;
                    npc.life = -1;
                    npc.HitEffect();
                }
                if (npc.type == NPCID.DukeFishron)
                {
                    NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DukeFishron>(), 0, npc.whoAmI);
                    npc.active = false;
                    npc.life = -1;
                    npc.HitEffect();
                }
            }
        }
    }
}