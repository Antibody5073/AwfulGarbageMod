using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class SlimeSplatterShark : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slime Splatter Shark"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets turn into splattering slime\nFires bullets extremely quickly");
		}

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = 5;
			Item.knockBack = 0.2f;
			Item.value = 5000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 6f;
			Item.noMelee = true;
		}

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(-8, 0);
            return offset;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
            {
                type = Mod.Find<ModProjectile>("GlockSlime").Type;
                damage /= 3;
            }
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("GelatinousGlock").Type);
            recipe.AddIngredient(ItemID.Minishark);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.DemoniteBar, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("GelatinousGlock").Type);
            recipe2.AddIngredient(ItemID.Minishark);
            recipe2.AddIngredient(ItemID.IllegalGunParts);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 30);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}

    public class SlimeSplatterSharkSlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 4;
            Projectile.height = 20;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.scale = 0.7f;
            Projectile.extraUpdates = 1;
        }



        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 11; i++)
            {
                float xv = (0f - Projectile.velocity.X) * (float)Main.rand.Next(35, 50) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.3f;
                float yv = (0f - Projectile.velocity.Y) * (float)Main.rand.Next(35, 50) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.3f;
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + xv, Projectile.position.Y + xv), new Vector2(xv, yv), Mod.Find<ModProjectile>("SlimeSplatterSharkSplatter").Type, Projectile.damage, 0f, Projectile.owner);
                Main.projectile[proj].CritChance = -80;
            }
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Water, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;

            }
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Water, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
            Main.dust[dust].noGravity = true;
            Projectile.aiStyle = 0;
        }
    }

    public class SlimeSplatterSharkSplatter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 4;
            Projectile.height = 20;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.scale = 0.7f;
            Projectile.extraUpdates = 1;
            Projectile.CritChance = 0;
        }


        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Water, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
            Main.dust[dust].noGravity = true;
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.aiStyle = 1;
            return false;
        }

    }
}