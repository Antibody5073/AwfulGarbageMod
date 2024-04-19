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
using AwfulGarbageMod.Projectiles;

namespace AwfulGarbageMod.Items.Accessories
{

    public class FourEyes : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Toad Eyes"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("8% increased crit chance\n4% further increased crit chance when staying still");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = Item.sellPrice(platinum: 1);

            Item.rare = ItemRarityID.Purple;
            Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
                Item.accessory = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().GlacialEye = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GlacialEyeProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<GlacialEyeProj>(), 0, 0, Main.myPlayer);
            }

            player.GetModPlayer<GlobalPlayer>().MoltenEye = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<MoltenEyeProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<MoltenEyeProj>(), 0, 0, Main.myPlayer);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.92f * (1f - player.endurance));
            player.GetModPlayer<GlobalPlayer>().GlacialEye = true;
            player.GetModPlayer<GlobalPlayer>().GlacialEyeDmg += 55;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GlacialEyeProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<GlacialEyeProj>(), 0, 0, Main.myPlayer);
            }
            player.GetDamage(DamageClass.Generic) += 0.08f;
            player.GetCritChance(DamageClass.Generic) += 8f;
            player.GetModPlayer<GlobalPlayer>().MoltenEye = true;
            player.GetModPlayer<GlobalPlayer>().MoltenEyeDmg += 70;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MoltenEyeProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<MoltenEyeProj>(), 0, 0, Main.myPlayer);
            }


            if (player.velocity == new Vector2(0, 0))
            {
                player.GetCritChance(DamageClass.Generic) += 8f;
            }
            player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg += 0.12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OpposingViews>()
                .AddIngredient(ItemID.EyeoftheGolem)
                .AddIngredient<ToadFocus>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}