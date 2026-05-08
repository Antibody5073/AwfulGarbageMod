using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Projectiles
{

    public class IceShardFrigidiusSpin : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        Vector2 origVel;
        float spdMult;
        int StateTimer = 0;


        public override void SetDefaults()
		{
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            if (AGUtils.IsOnScreen(Projectile, new Vector2(32, 32) * Vector2.Distance(Vector2.Zero, Projectile.velocity)))
            {
                SpriteEffects spriteEffects = SpriteEffects.None;

                // Getting texture of projectile
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

                Vector2 origin = sourceRectangle.Size() / 2f;
                Color drawColor = Projectile.GetAlpha(lightColor);

                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = origin;
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(6f, 6f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
                }

                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

                // It's important to return false, otherwise we also draw the original texture.
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Player player = FindClosestPlayer(4000);
            if (player != null)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, Projectile.Center);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f, ModContent.ProjectileType<IceShardFrigidiusAccelerate>(), 35, 0, -1, 1, 0);
            }
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];

            StateTimer++;
            Projectile.rotation += 0.15f;
            Projectile.velocity *= 0.97f;

            if (owner.globalEnemyBossInfo().killOrbitals || !owner.active)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 3;
            }

            Projectile.damage = 26;
            if (Main.expertMode)
            {
                Projectile.damage = 24;
                if (Main.masterMode)
                {
                    Projectile.damage = 22;
                }
            }
        }
        public Player FindClosestPlayer(float maxDetectDistance)
        {
            Player closestPlayer = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestPlayer = target;
                }
            }

            return closestPlayer;
        }
    }
}