using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class ReanimatedAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 40000;
            Item.rare = 5;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<GlobalPlayer>().EnchantedSword == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<HallowedMetalChunkProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<HallowedMetalChunkProj>(), 0, 0, Main.myPlayer);
            }

            player.GetModPlayer<GlobalPlayer>().EnchantedSword += 35;


            if (player.GetModPlayer<GlobalPlayer>().CrimsonAxe == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonMetalChunkProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<CrimsonMetalChunkProj>(), 0, 0, Main.myPlayer);
            }

            player.GetModPlayer<GlobalPlayer>().CrimsonAxe += 35;

            if (player.GetModPlayer<GlobalPlayer>().CursedHammer == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<CorruptedMetalChunkProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<CorruptedMetalChunkProj>(), 0, 0, Main.myPlayer);
            }

            player.GetModPlayer<GlobalPlayer>().CursedHammer += 35;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HallowedMetalChunk>()
                .AddIngredient<CorruptedMetalChunk>()
                .AddIngredient(ItemID.SoulofFright, 15)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowedMetalChunk>()
                .AddIngredient<CrimsonMetalChunk>()
                .AddIngredient(ItemID.SoulofFright, 15)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}