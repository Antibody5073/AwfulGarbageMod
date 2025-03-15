using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class LavaLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Spirit Pike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a homing spirit every third use");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Spear);
            Item.damage = 72;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.knockBack = 7;
            Item.value = 10000;
            Item.rare = 2;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("LavaLanceProj").Type;
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }

    public class LavaLanceProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Lava, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
            }
        }


        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Lava, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            Projectile.rotation += Vector2.Distance(Projectile.velocity, Vector2.Zero) / 10;
            Projectile.velocity *= 0.97f;
        }
    }

    public class LavaLanceProj : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 36f;
        protected virtual float HoldoutRangeMax => 148f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Spirit Pike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1.25f;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 19;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;


        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }
            if (Projectile.timeLeft == (int)(player.itemAnimationMax * 0.75f))
            {
                for (var i = 0; i < Main.rand.Next(2, 5); i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(8, 0).RotatedBy(Projectile.velocity.ToRotation()) + Main.rand.NextVector2Circular(2.5f, 2.5f), ModContent.ProjectileType<LavaLanceProj2>(), Projectile.damage / 2, Projectile.knockBack / 4, Projectile.owner);

                }
            }
            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }



            return false; // Don't execute vanilla AI.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }
    }
}