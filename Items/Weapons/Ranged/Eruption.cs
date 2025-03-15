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

    public class Eruption : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Normal bullets travel instantly");
        }

        int counter;

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = 5;
            Item.knockBack = 5f;
            Item.value = 5000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 5.5f;
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ProjectileID.ExplosiveBullet;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity * 2, Mod.Find<ModProjectile>("EruptionProj").Type, damage, knockback, player.whoAmI);
            for (var i = 0; i <= Main.rand.Next(3, 4); i++)
            {
                proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0.8f, 1.2f), type, damage, knockback, player.whoAmI);
                    if (type == ProjectileID.Bullet)
                    {
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 10;
                    }
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
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddIngredient(ItemID.HellstoneBar, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class EruptionProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Stream"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
            for (var i = 0; i < 15; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 15; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            if (Vector2.Distance(Projectile.Center, Main.player[Main.myPlayer].Center) < 1000)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.25f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity = Projectile.velocity * 0.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}