using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class GlacierBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots an inaccurate, short-ranged ice shard along with an arrow");
		}

		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 25;
			Item.noMelee = true;
			Item.useAnimation = 25;
			Item.useStyle = 5;
			Item.knockBack = 2.5f;
			Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 6f;
            Item.crit = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity * 1.5f, type, damage, knockback, player.whoAmI);
            
            int proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(7)) * 1.2f, Mod.Find<ModProjectile>("FrostBowShard").Type, damage / 2, knockback / 2, player.whoAmI);
            proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-7)) * 1.2f, Mod.Find<ModProjectile>("FrostBowShard").Type, damage / 2, knockback / 2, player.whoAmI);
            proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-12)) * 0.8f, Mod.Find<ModProjectile>("FrostBowShard").Type, damage / 3, knockback / 2, player.whoAmI);
            proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(12)) * 0.8f, Mod.Find<ModProjectile>("FrostBowShard").Type, damage / 3, knockback / 2, player.whoAmI);

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(0, 0);
			return offset;
        }
        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FrostBow>());
            recipe.AddIngredient(ModContent.ItemType<FrigidiumBar>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}