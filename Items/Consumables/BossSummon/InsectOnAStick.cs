using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;

namespace AwfulGarbageMod.Items.Consumables.BossSummon
{
    //imported from my tAPI mod because I'm lazy
    public class InsectOnAStick : ModItem
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
            return !NPC.AnyNPCs(NPCType<TreeToad>());
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCType<NPCs.Boss.TreeToad>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.Bugs, 1);
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Grasshopper, 1);
            recipe2.AddIngredient(ItemID.Wood, 15);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.LadyBug, 1);
            recipe3.AddIngredient(ItemID.Wood, 15);
            recipe3.AddTile(TileID.WorkBenches);
            recipe3.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddRecipeGroup(RecipeGroupID.Butterflies, 1);
            recipe4.AddIngredient(ItemID.Wood, 15);
            recipe4.AddTile(TileID.WorkBenches);
            recipe4.Register();
            Recipe recipe5 = CreateRecipe();
            recipe5.AddRecipeGroup(RecipeGroupID.Fireflies, 1);
            recipe5.AddIngredient(ItemID.Wood, 15);
            recipe5.AddTile(TileID.WorkBenches);
            recipe5.Register();
            Recipe recipe6 = CreateRecipe();
            recipe6.AddRecipeGroup(RecipeGroupID.Dragonflies, 1);
            recipe6.AddIngredient(ItemID.Wood, 15);
            recipe6.AddTile(TileID.WorkBenches);
            recipe6.Register();
        }
    }
}