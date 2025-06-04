using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class Sprinkler : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sprinkler"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 16;
            Item.mana = 6;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("SprinklerProj").Type;
			Item.shootSpeed = 9f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int proj = Projectile.NewProjectile(source, position, new Vector2(0, Main.rand.NextFloat(-4f, -7f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 31))), type, damage, knockback, player.whoAmI);

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
            recipe.AddIngredient(ItemID.Waterleaf, 3);
            recipe.AddIngredient(ItemID.SilverBar, 5);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Waterleaf, 3);
            recipe2.AddIngredient(ItemID.TungstenBar, 5);
            recipe2.AddIngredient(ItemID.WaterBucket, 5);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}

    public class SprinklerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterStream);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 300)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                return false;
            }
            return true;
        }


        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.DungeonWater, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
            if (Projectile.timeLeft > 300)
            {
                Projectile.velocity *= 0.98f;
            }
            else if (Projectile.timeLeft == 300)
            {
                Vector2 vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - Projectile.position.X, (float)Main.mouseY + Main.screenPosition.Y - Projectile.position.Y);
                vel.Normalize();
                Projectile.velocity = vel * 10f;
            }
            else
            {
                Projectile.velocity.Y += 0.05f;
            }
        }
    }
}