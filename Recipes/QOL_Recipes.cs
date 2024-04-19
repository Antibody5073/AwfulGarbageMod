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
    public class QOL_Recipes : ModSystem
    {
        public override void AddRecipes()
        {
            if (ModContent.GetInstance<Config>().AreYouLazy)
            {
                Recipe.Create(ItemID.IceSkates)
                    .AddIngredient(ItemID.IronBar, 8)
                    .AddIngredient(ModContent.ItemType<FrostShard>(), 10)
                    .AddIngredient(ItemID.FlinxFur, 4)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.IceSkates)
                   .AddIngredient(ItemID.LeadBar, 8)
                   .AddIngredient(ModContent.ItemType<FrostShard>(), 10)
                   .AddIngredient(ItemID.FlinxFur, 4)
                   .AddTile(TileID.Anvils)
                   .Register();
                Recipe.Create(ItemID.BlizzardinaBottle)
                   .AddIngredient(ItemID.BottledWater, 1)
                   .AddIngredient(ModContent.ItemType<FrostShard>(), 16)
                   .AddIngredient(ItemID.IceBlock, 45)
                   .AddTile(TileID.IceMachine)
                   .Register();
                Recipe.Create(ItemID.LavaCharm)
                    .AddIngredient(ItemID.GoldBar, 10)
                    .AddIngredient(ItemID.HellstoneBar, 15)
                    .AddIngredient(ItemID.LavaBucket, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.LavaCharm)
                    .AddIngredient(ItemID.PlatinumBar, 10)
                    .AddIngredient(ItemID.HellstoneBar, 15)
                    .AddIngredient(ItemID.LavaBucket, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.WaterWalkingBoots)
                    .AddIngredient(ItemID.WaterBucket, 3)
                    .AddIngredient(ItemID.Silk, 15)
                    .AddIngredient(ModContent.ItemType<FrostShard>(), 12)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.ObsidianRose)
                    .AddIngredient(ItemID.Obsidian, 20)
                    .AddIngredient(ItemID.WaterBucket, 3)
                    .AddIngredient(ItemID.LavaBucket, 3)
                    .AddIngredient(ItemID.Fireblossom, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.FeralClaws)
                    .AddIngredient(ItemID.JungleSpores, 16)
                    .AddIngredient(ItemID.CopperBar, 8)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.FeralClaws)
                    .AddIngredient(ItemID.JungleSpores, 16)
                    .AddIngredient(ItemID.TinBar, 8)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.BandofRegeneration)
                    .AddIngredient(ItemID.LifeCrystal, 1)
                    .AddIngredient(ItemID.CopperBar, 8)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient(ItemID.Mushroom, 5)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.BandofRegeneration)
                    .AddIngredient(ItemID.LifeCrystal, 1)
                    .AddIngredient(ItemID.TinBar, 8)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient(ItemID.Mushroom, 5)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.DivingHelmet)
                    .AddIngredient(ItemID.SharkFin, 8)
                    .AddIngredient(ItemID.Chain, 30)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.GuideVoodooDoll)
                    .AddIngredient(ItemID.Silk, 7)
                    .AddIngredient(ItemID.HellstoneBar, 7)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ItemID.FrozenTurtleShell)
                .AddIngredient(ItemID.TurtleShell)
                .AddIngredient<Cryogem>(30)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            }
        }
    }
}