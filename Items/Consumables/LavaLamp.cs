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
    public class LavaLamp : ModItem
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
            Item.rare = 4;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.value = Item.sellPrice(platinum: 1, gold: 50);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            // "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
            return !NPC.AnyNPCs(NPCType<Candescence>());
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCType<Candescence>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Pyrogem>(6)
                .Register();
        }
    }
}