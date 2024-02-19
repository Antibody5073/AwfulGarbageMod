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
    public class RemixRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            if (ModContent.GetInstance<Config>().RemixRecipes)
            {
                    Recipe.Create(ItemID.Cloud, 18)
                        .AddIngredient(ItemID.GlowingMushroom, 3)
                        .AddIngredient(ItemID.JungleSpores, 1)
                        .AddIngredient(ItemID.DirtBlock, 18)
                        .AddCondition(Condition.NearWater)
                        .AddCondition(Condition.DownedEyeOfCthulhu)
                        .AddCondition(Condition.InSpace)
                        .AddCondition(Language.GetOrRegister("Mods.AwfulGarbageMod.Conditions.RemixSeed"), () => Main.remixWorld || Main.zenithWorld)
                        .Register();
                    Recipe.Create(ItemID.RainCloud, 18)
                        .AddIngredient(ItemID.GlowingMushroom, 3)
                        .AddIngredient(ItemID.JungleSpores, 1)
                        .AddIngredient(ItemID.BottledWater, 6)
                        .AddCondition(Condition.NearWater)
                        .AddCondition(Condition.DownedEyeOfCthulhu)
                        .AddCondition(Condition.InSpace)
                        .AddCondition(Language.GetOrRegister("Mods.AwfulGarbageMod.Conditions.RemixSeed"), () => Main.remixWorld || Main.zenithWorld)
                        .Register();
                    Recipe.Create(ItemID.RainCloud, 24)
                        .AddIngredient(ItemID.GlowingMushroom, 3)
                        .AddIngredient(ItemID.JungleSpores, 1)
                        .AddIngredient(ItemID.WaterBucket, 1)
                        .AddCondition(Condition.NearWater)
                        .AddCondition(Condition.DownedEyeOfCthulhu)
                        .AddCondition(Condition.InSpace)
                        .AddCondition(Language.GetOrRegister("Mods.AwfulGarbageMod.Conditions.RemixSeed"), () => Main.remixWorld || Main.zenithWorld)
                        .Register();
                    Recipe.Create(ItemID.SnowCloudBlock, 18)
                        .AddIngredient(ItemID.GlowingMushroom, 3)
                        .AddIngredient(ItemID.JungleSpores, 1)
                        .AddIngredient(ItemID.SnowBlock, 18)
                        .AddCondition(Condition.NearWater)
                        .AddCondition(Condition.DownedEyeOfCthulhu)
                        .AddCondition(Condition.InSpace)
                        .AddCondition(Language.GetOrRegister("Mods.AwfulGarbageMod.Conditions.RemixSeed"), () => Main.remixWorld || Main.zenithWorld)
                        .Register();
            }
        }
    }
}