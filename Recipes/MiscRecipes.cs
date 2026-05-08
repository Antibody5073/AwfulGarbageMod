using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Melee.Hardmode;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Items.Weapons.Summon.Hardmode;
using AwfulGarbageMod.Tiles.Furniture;
using Infernus.Items.Accesories;
using Infernus.Items.Materials;
using Infernus.Items.Weapon.HardMode.Accessories;
using Infernus.Items.Weapon.HardMode.Ranged;
using Infernus.Items.Weapon.HardMode.Summon;
using Infernus.Items.Weapon.Magic;
using Infernus.Items.Weapon.Melee;
using Infernus.Items.Weapon.Ranged;
using Infernus.Items.Weapon.Summon;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace AwfulGarbageMod.Recipes
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
                .AddIngredient<Flint>(25)
                .AddRecipeGroup(RecipeGroupID.IronBar, 20)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.FlintlockPistol)
                .AddIngredient<Flint>(25)
                .AddRecipeGroup(RecipeGroupID.IronBar, 20)
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.CelestialMagnet)
                .AddIngredient<Flint>(20)
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddIngredient<CelestiteShard>(8)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.CreativeWings)
                .AddIngredient<CelestiteShard>(8)
                .AddIngredient(ItemID.Feather, 3)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.Extractinator)
                .AddIngredient<Flint>(15)
                .AddIngredient<Garbage>(45)
                .AddIngredient(ItemID.SiltBlock, 40)
                .AddTile<TrashMelter>()
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
                if (recipe.Mod == null && recipe.HasResult(ItemID.EndurancePotion) && recipe.createItem.stack == 1)
                {
                    recipe.AddIngredient<Frosshroom>();
                }
                if (recipe.Mod == null && recipe.HasResult(ItemID.MiningPotion) && recipe.createItem.stack == 1)
                {
                    recipe.AddIngredient<Frosshroom>();
                }
                if (ModLoader.TryGetMod("Infernus", out Mod infernus))
                {
                    DoInfernusRecipe(recipe);
                }
                if (ModContent.GetInstance<Config>().MinorProgressionChanges)
                {
                    if (recipe.HasResult(ItemID.LightningBoots) && recipe.createItem.stack == 1)
                    {
                        recipe.AddIngredient<StormEssence>(5);
                        recipe.AddTile<StormAnvil>();
                    }
                    if (recipe.HasResult(ItemID.TerraBlade) && recipe.createItem.stack == 1)
                    {
                        recipe.AddIngredient<HeartOfBlight>();
                    }
                }
            }
        }


        [JITWhenModsEnabled("Infernus")]
        public static void DoInfernusRecipe(Recipe recipe)
        {
            if (recipe.Mod == null) { return; }
            if (recipe.HasResult<UndeadTyranny>())
            {
                recipe.AddIngredient<Cryo_Gauntlet>();
            }
            if (recipe.HasResult<DriedBlossom>())
            {
                recipe.AddIngredient<Ocean_Crest>();
            }
            if (recipe.HasResult<BipolarBouquet>())
            {
                recipe.AddIngredient<Undying_Ember>();
                recipe.AddIngredient<Scroll>();
            }
            if (recipe.HasResult<Elemental_Lash>())
            {
                recipe.AddIngredient<BlazingTotem>();
            }
            if (recipe.HasResult<Blood_Quiver>())
            {
                recipe.AddIngredient<ThrushFeather>();
                recipe.AddIngredient<PoisonPendant>();
                recipe.AddIngredient<VeinJuice>(20);
            }
            if (recipe.HasResult<ShellOfVengefulSpirits>())
            {
                recipe.AddIngredient<Effigy_of_might>();
            }
            if (recipe.HasResult<Ice_Banner>())
            {
                recipe.AddIngredient<FrigidiumBar>(20);
            }
            if (recipe.HasResult<CryoCleaver>())
            {
                recipe.AddIngredient<Infernus.Items.Weapon.Melee.Ice>();
            }
            if (recipe.HasResult<Hellslash>())
            {
                recipe.AddIngredient<Infernus.Items.Weapon.Melee.Fire>();
            }
            if (recipe.HasResult<TidesTurn>())
            {
                recipe.AddIngredient<Infernus.Items.Weapon.Melee.Seashellsword>();
            }
            if (recipe.HasResult<Infernus.Items.Weapon.HardMode.Melee.bronzesword>())
            {
                recipe.AddIngredient<Items.Weapons.Melee.InfernalBlade>();
            }
            if (recipe.HasResult<Infernus.Items.Weapon.HardMode.Melee.Frisbe>())
            {
                recipe.AddIngredient<LeafyDisc>(2);
            }
            if (recipe.HasResult<Infernus.Items.Weapon.HardMode.Ranged.SoulBow>())
            {
                recipe.AddIngredient<LightningBurst>();
            }
            if (recipe.HasResult<Eruption>())
            {
                recipe.AddIngredient<Mini>();
            }
            if (recipe.HasResult<Bomb>())
            {
                recipe.AddIngredient<FrigidiumShotgun>();
            }
            if (recipe.HasResult<Buther>())
            {
                recipe.AddIngredient<FlintStaff>();
            }
            if (recipe.HasResult<Storm>())
            {
                recipe.AddIngredient<StormForecast>();
            }
            if (recipe.HasResult<TerraStaff>())
            {
                recipe.AddIngredient<VerdantStaff>();
            }
            if (recipe.HasResult<BlazingTotem>())
            {
                recipe.AddIngredient<Hot>(15);
            }
            if (recipe.HasResult<HeatTippedToothpickTome>())
            {
                recipe.AddIngredient<Hot>(20);
            }
            if (recipe.HasResult<IceElementalStaff>())
            {
                recipe.AddIngredient<BlizStaff>();
            }
            if (recipe.HasResult<Light>())
            {
                recipe.AddIngredient<SoulOfIghtImaHeadOut>(15);
            }
            if (recipe.HasResult<Hellion_Spite>())
            {
                recipe.AddIngredient<CandesciteBar>(17);
            }
            if (recipe.HasResult<Tree_Band>())
            {
                recipe.AddIngredient<EnchantedLeaf>(20);
            }
            if (recipe.HasResult<CHARD>())
            {
                recipe.AddIngredient<StormEssence>(7);
                recipe.AddIngredient<SoulOfIghtImaHeadOut>(7);
            }
        }
    }
}