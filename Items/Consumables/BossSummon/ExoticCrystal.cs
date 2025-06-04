using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Tiles.OresBars;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Consumables.BossSummon
{
    //imported from my tAPI mod because I'm lazy
    public class ExoticCrystal : ModItem
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
            Item.rare = ItemRarityID.Cyan;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = true;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            // "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
            return (!NPC.AnyNPCs(NPCType<FrigidiusHead>())) && player.GetModPlayer<GlobalPlayer>().ImportantHoveredTile == "FrigidAltar";
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCType<NPCs.Boss.FrigidiusHead>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient<Placeable.OresBars.FrigidiumOre>(5)
                .AddIngredient<FrostShard>(2)
                .AddIngredient<SpiritItem>(1)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}