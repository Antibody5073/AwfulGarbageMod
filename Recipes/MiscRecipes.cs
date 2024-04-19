using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;


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
            Recipe.Create(ItemID.ChlorophyteBar)
                .AddIngredient(ItemID.ChlorophyteOre, 2)
                .AddIngredient<EnchantedLeaf>(2)
                .AddTile(TileID.AdamantiteForge)
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
        }
    }
}