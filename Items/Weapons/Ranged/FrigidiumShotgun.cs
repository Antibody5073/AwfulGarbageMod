using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class FrigidiumShotgun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets travel instantly");
		}

        int counter;

		public override void SetDefaults()
		{
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 49;
            Item.useAnimation = 49;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = 5000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 8f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = 0; i <= Main.rand.Next(2, 4); i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(12)) * Main.rand.NextFloat(0.8f, 1.2f), type, damage, knockback, player.whoAmI);
            }
            for (var i = 0; i < Main.rand.Next(3, 6); i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0.8f, 1.2f), Mod.Find<ModProjectile>("FrostBowShard").Type, (int)(damage * 0.5), knockback, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(8, 0);
			return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ModContent.ItemType<FrigidiumBar>(), 18);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}