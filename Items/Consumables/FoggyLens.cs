﻿using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;

namespace AwfulGarbageMod.Items.Consumables
{
    //imported from my tAPI mod because I'm lazy
    public class FoggyLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Foggy Lens"); 
            // Tooltip.SetDefault("Calls forth the storm observer");
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
            Item.consumable = false;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            // "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
            return !NPC.AnyNPCs(NPCType<EyeOfTheStorm>());
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCType<EyeOfTheStorm>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Lens, 5);
            recipe.AddIngredient(ItemID.RainCloud, 50);
            recipe.AddIngredient(ItemID.SnowBlock, 50);
            recipe.AddIngredient(ItemID.Feather, 5);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Lens, 5);
            recipe2.AddIngredient(ItemID.RainCloud, 50);
            recipe2.AddIngredient(ItemID.SnowBlock, 50);
            recipe2.AddIngredient(ItemID.Feather, 5);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe2.AddTile(TileID.DemonAltar);
            recipe2.Register();
        }
    }
}