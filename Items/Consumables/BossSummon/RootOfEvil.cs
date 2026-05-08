using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Subworlds;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Tiles.OresBars;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Items.Consumables.BossSummon
{
    //imported from my tAPI mod because I'm lazy
    public class RootOfEvil : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Insect on a Stick"); 
            // Tooltip.SetDefault("Will attract a great forest-dwelling amphibian");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13; // This helps sort inventory know this is a boss summoning Item.
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            // "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
            return (!NPC.AnyNPCs(NPCType<Blightdrasil>())) && SubworldSystem.IsActive<EbonwoodBossSubworld>();
        }

        public override bool? UseItem(Player player)
        {

            Vector2 pos = new Point16(Main.maxTilesX / 2, Main.maxTilesY / 2).ToWorldCoordinates();
            NPC.NewNPC(Item.GetSource_ReleaseEntity(), (int)pos.X, (int)pos.Y + 225, ModContent.NPCType<Blightdrasil>(), 150, Target: player.whoAmI);
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {

        }
    }
}