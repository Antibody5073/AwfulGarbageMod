using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VitalityMod.Content.NPCs.Bosses.AnarchulesBeetle;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.Vitality.Infestation
{
    [ExtendsFromMod("VitalityMod")]

    public class VerticalCursedOrb : ModProjectile
    {
        private int animationTimer;

        private int animationFrame;

        private int Timer;

        public override string Texture => "VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/CursedOrb";

        public override void SetDefaults()
        {
            base.Projectile.aiStyle = -1;
            Main.projFrames[base.Projectile.type] = 4;
            base.Projectile.width = 30;
            base.Projectile.height = 30;
            base.Projectile.ignoreWater = true;
            base.Projectile.hostile = true;
            base.Projectile.friendly = false;
            base.Projectile.tileCollide = false;
            base.Projectile.timeLeft = 360;
            base.Projectile.penetrate = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            base.Projectile.ai[0] += 1f;
            if (++animationTimer >= 4)
            {
                animationTimer = 0;
                if (++animationFrame >= 4)
                {
                    animationFrame = 0;
                }
            }

            base.Projectile.velocity.Y = 10f;
            if (Timer < 20)
            {
                base.Projectile.velocity.X -= 0.4f;
            }

            if (Timer >= 20 && Timer < 60)
            {
                base.Projectile.velocity.X += 0.4f;
            }

            if (Timer >= 60 && Timer < 80)
            {
                base.Projectile.velocity.X -= 0.4f;
            }

            Timer++;
            if (Timer >= 80)
            {
                Timer = 0;
            }

            for (int i = 0; i < 1; i++)
            {
                int num = Dust.NewDust(base.Projectile.Center, 0, 0, 75, (float)((double)base.Projectile.velocity.X * 0.8), (float)((double)base.Projectile.velocity.Y * 0.8), 0, default(Color), 1.4f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position = base.Projectile.Center;
                Main.dust[num].scale = (float)Main.rand.Next(25, 70) * 0.04f;
                Main.dust[num].velocity *= 0.2f;
            }

            if (++base.Projectile.frameCounter >= 4)
            {
                base.Projectile.frameCounter = 0;
                if (++base.Projectile.frame >= 4)
                {
                    base.Projectile.frame = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(39, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vector = new Vector2(0f, 0f);
            float collisionPoint = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<VerticalCursedOrb2>() && projectile.ai[0] >= base.Projectile.ai[0] - 2f && projectile.ai[0] <= base.Projectile.ai[0] + 2f)
                {
                    vector = projectile.Center;
                }
            }

            if (vector == new Vector2(0f, 0f))
            {
                return false;
            }

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), base.Projectile.Center, vector, 16f, ref collisionPoint);
        }

        public override void PostDraw(Color lightColor)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.type != ModContent.ProjectileType<VerticalCursedOrb2>() || !(projectile.ai[0] >= base.Projectile.ai[0] - 2f) || !(projectile.ai[0] <= base.Projectile.ai[0] + 2f))
                {
                    continue;
                }

                Vector2 center = projectile.Center;
                Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/CursedOrbChain");
                Vector2 center2 = base.Projectile.Center;
                Vector2 vector = center - center2;
                float rotation = vector.ToRotation() - MathF.PI / 2f;
                while (true)
                {
                    float num = vector.Length();
                    if (num < 25f || float.IsNaN(num))
                    {
                        break;
                    }

                    center2 += vector * 28f / num;
                    vector = center - center2;
                    Main.EntitySpriteDraw(texture2D, center2 - Main.screenPosition, new Rectangle(0, 32 * animationFrame, 14, 32), lightColor, rotation, new Vector2(texture2D.Size().X * 0.5f, 16f), 1f, SpriteEffects.None);
                }
            }
        }
    }
    [ExtendsFromMod("VitalityMod")]

    public class VerticalCursedOrb2 : ModProjectile
    {
        private int Timer;

        public override string Texture => "VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/CursedOrb";

        public override void SetDefaults()
        {
            base.Projectile.aiStyle = -1;
            Main.projFrames[base.Projectile.type] = 4;
            base.Projectile.width = 30;
            base.Projectile.height = 30;
            base.Projectile.ignoreWater = true;
            base.Projectile.hostile = true;
            base.Projectile.friendly = false;
            base.Projectile.tileCollide = false;
            base.Projectile.timeLeft = 360;
            base.Projectile.penetrate = -1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            base.Projectile.ai[0] += 1f;
            base.Projectile.velocity.Y = 10f;
            if (Timer < 20)
            {
                base.Projectile.velocity.X += 0.4f;
            }

            if (Timer >= 20 && Timer < 60)
            {
                base.Projectile.velocity.X -= 0.4f;
            }

            if (Timer >= 60 && Timer < 80)
            {
                base.Projectile.velocity.X += 0.4f;
            }

            Timer++;
            if (Timer >= 80)
            {
                Timer = 0;
            }

            for (int i = 0; i < 1; i++)
            {
                int num = Dust.NewDust(base.Projectile.Center, 0, 0, 75, (float)((double)base.Projectile.velocity.X * 0.8), (float)((double)base.Projectile.velocity.Y * 0.8), 0, default(Color), 1.4f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position = base.Projectile.Center;
                Main.dust[num].scale = (float)Main.rand.Next(25, 70) * 0.04f;
                Main.dust[num].velocity *= 0.2f;
            }

            if (++base.Projectile.frameCounter >= 4)
            {
                base.Projectile.frameCounter = 0;
                if (++base.Projectile.frame >= 4)
                {
                    base.Projectile.frame = 0;
                }
            }
        }
    }
}
