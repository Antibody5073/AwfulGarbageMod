using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Weapons.demo
{

    public class FreezeRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");

        }

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.mana = 11;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = 5;
            Item.knockBack = 3;
            Item.value = 15000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FreezeRayProj>();
            Item.shootSpeed = 6f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            return false;
        }
    }

    public class FreezeRayProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 vel = Main.rand.NextVector2CircularEdge(2, 2);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 1.5f);
                Main.dust[dust].position += vel * 6;
                Main.dust[dust].velocity = vel;
                Main.dust[dust].noGravity = true;
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FreezeRayProjPulse>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.4f, 0.8f);
                Main.dust[dust].velocity = Main.rand.NextVector2CircularEdge(0.4f, 0.4f);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    public class FreezeRayProjPulse : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Defense *= 0.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}