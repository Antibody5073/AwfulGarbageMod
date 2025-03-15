using AwfulGarbageMod.Global;
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

    public class HotGlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Normal bullets travel instantly");
        }

        int counter;

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = 5000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 9f;
            Item.noMelee = true;
            Item.crit = 6;
            Item.ArmorPenetration = 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].GetGlobalProjectile<ProjectileWeaponEffect>().HotGlock = true;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(8, 0);
            return offset;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CandesciteOre>(50)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }


    public class HotGlockProj : ModProjectile
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
            Projectile.extraUpdates = 3;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 720;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
            for (var i = 0; i < 15; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(12, 12);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 15; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(12, 12);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            return false;
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