using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Tiles.Furniture;


namespace ExampleMod.Content
{
    // This class contains thoughtful examples of item recipe creation.
    // Recipes are explained in detail on the https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes and https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes wiki pages. Please visit the wiki to learn more about recipes if anything is unclear.
    public class MiscRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.BottledWater, 3)
                .AddIngredient(ItemID.Bottle, 3)
                .AddIngredient(ItemID.IceBlock, 2)
                .AddTile(TileID.Campfire)
                .Register();
            Recipe.Create(ItemID.PutridScent)
                .AddIngredient(ItemID.FleshKnuckles)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            Recipe.Create(ItemID.FleshKnuckles)
                .AddIngredient(ItemID.PutridScent)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            Recipe.Create(ItemID.FlintlockPistol)
                .AddIngredient<Flint>(18)
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.IceSickle)
                .AddIngredient<Cryogem>(25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.FrostStaff)
                .AddIngredient<Cryogem>(20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.Amarok)
                .AddIngredient<Cryogem>(25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            if (ModContent.GetInstance<Config>().HardmodeMetalRework)
            {
                Recipe.Create(ItemID.MythrilBar)
                    .AddIngredient(ItemID.MythrilOre, 8)
                    .AddTile(TileID.Furnaces)
                    .Register();
                Recipe.Create(ItemID.MythrilBar)
                    .AddIngredient(ItemID.MythrilOre, 2)
                    .AddIngredient<Flint>()
                    .AddTile<ElementalSmeltery>()
                    .Register();
                Recipe.Create(ItemID.OrichalcumBar)
                    .AddIngredient(ItemID.OrichalcumOre, 8)
                    .AddTile(TileID.Furnaces)
                    .Register();
                Recipe.Create(ItemID.OrichalcumBar)
                    .AddIngredient(ItemID.OrichalcumOre, 2)
                    .AddIngredient<FrostShard>()
                    .AddTile<ElementalSmeltery>()
                    .Register();
                Recipe.Create(ItemID.AdamantiteBar)
                    .AddIngredient(ItemID.AdamantiteOre, 8)
                    .AddTile(TileID.AdamantiteForge)
                    .Register();
                Recipe.Create(ItemID.AdamantiteBar)
                    .AddIngredient(ItemID.AdamantiteOre, 2)
                    .AddIngredient<VeinJuice>()
                    .AddTile<ElementalSmeltery>()
                    .Register();
                Recipe.Create(ItemID.TitaniumBar)
                    .AddIngredient(ItemID.TitaniumOre, 8)
                    .AddTile(TileID.AdamantiteForge)
                    .Register();
                Recipe.Create(ItemID.TitaniumBar)
                    .AddIngredient(ItemID.TitaniumOre, 2)
                    .AddIngredient<SpiderLeg>()
                    .AddTile<ElementalSmeltery>()
                    .Register();
                Recipe.Create(ItemID.ChlorophyteBar)
                    .AddIngredient(ItemID.ChlorophyteOre, 8)
                    .AddTile(TileID.AdamantiteForge)
                    .Register();
                Recipe.Create(ItemID.ChlorophyteBar)
                    .AddIngredient(ItemID.ChlorophyteOre, 2)
                    .AddIngredient<EnchantedLeaf>(2)
                    .AddTile<ElementalSmeltery>()
                    .Register();
            }
        }

        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                if (ModContent.GetInstance<Config>().HardmodeMetalRework)
                {
                    if (recipe.Mod == null && recipe.HasIngredient(ItemID.MythrilOre) && recipe.HasResult(ItemID.MythrilBar) && recipe.createItem.stack == 1)
                    {
                        recipe.DisableRecipe();
                    }
                    if (recipe.Mod == null && recipe.HasIngredient(ItemID.OrichalcumOre) && recipe.HasResult(ItemID.OrichalcumBar) && recipe.createItem.stack == 1)
                    {
                        recipe.DisableRecipe();
                    }
                    if (recipe.Mod == null && recipe.HasIngredient(ItemID.AdamantiteOre) && recipe.HasResult(ItemID.AdamantiteBar) && recipe.createItem.stack == 1)
                    {
                        recipe.DisableRecipe();
                    }
                    if (recipe.Mod == null && recipe.HasIngredient(ItemID.TitaniumOre) && recipe.HasResult(ItemID.TitaniumBar) && recipe.createItem.stack == 1)
                    {
                        recipe.DisableRecipe();
                    }
                    if (recipe.Mod == null && recipe.HasIngredient(ItemID.ChlorophyteOre) && recipe.HasResult(ItemID.ChlorophyteBar) && recipe.createItem.stack == 1)
                    {
                        recipe.DisableRecipe();
                    }
                }
            }
        }
    }
}