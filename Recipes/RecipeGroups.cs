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
    public class RecipeGroups : ModSystem
    {
        public static RecipeGroup AnyHerb;

        public override void Unload()
        {
            AnyHerb = null;
        }
        public override void AddRecipeGroups()
        {
            // Create a recipe group and store it
            // Language.GetTextValue("LegacyMisc.37") is the word "Any" in English, and the corresponding word in other languages
            AnyHerb = new RecipeGroup(() => "Any Herb",
                ItemID.Daybloom, ItemID.Blinkroot, ItemID.Shiverthorn, ItemID.Moonglow, ItemID.Waterleaf, ItemID.Fireblossom);

            // To avoid name collisions, when a modded items is the iconic or 1st item in a recipe group, name the recipe group: ModName:ItemName
            RecipeGroup.RegisterGroup("AwfulGarbageMod:AnyHerb", AnyHerb);
        }

    }
}