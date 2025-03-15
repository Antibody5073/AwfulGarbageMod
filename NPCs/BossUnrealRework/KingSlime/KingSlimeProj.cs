using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.KingSlime
{
    public class KingSlimeProjSlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        bool didit = false;
        Vector2 startpos;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 475;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.CornflowerBlue * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)TextureAssets.Npc[1];

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * 1;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;


            Color drawColor = Projectile.GetAlpha(lightColor);

            if (Projectile.timeLeft <= 400)
            {
                Texture2D texture2 = ModContent.Request<Texture2D>("AwfulGarbageMod/NPCs/BossUnrealRework/KingSlime/KingSlimeProjTele").Value;
                DrawLaser(texture2, Projectile.Center + new Vector2(1, 1), new Vector2(0, 1), 30, 1200, lightColor, -1.57f, 1f, 2000f, Color.MidnightBlue, 0);
            }
            startY = frameHeight * 0;
            sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;


            // Applying lighting and draw current frame
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, float distance, Color lightColor, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            Color drawColor = Projectile.GetAlpha(lightColor);

            float amt = 0;
            for (float max = 0; max <= distance; max += step)
            {
                amt += 1;
            }
            float current = 0;
            // Draws the laser 'body'
            for (float i = transDist; i <= distance; i += step)
            {
                Color c = drawColor * ((float)(amt - current) / (float)amt);
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 24, 28, 30), c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
                current++;
            }
        }
        public override void AI()
        {
            if (!didit)
            {
                startpos = Projectile.Center;
                didit = true;
            }


            if (Projectile.timeLeft > 415)
            {
                Projectile.Center = new Vector2(MathHelper.Lerp(Projectile.ai[0], startpos.X, (Projectile.timeLeft - 415) / 60f), MathHelper.Lerp(Projectile.ai[1], startpos.Y, (Projectile.timeLeft - 415) / 60f));
            }
            if (Projectile.timeLeft <= 400)
            {
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 24)
                {
                    Projectile.velocity.Y = 24;
                }
            }
        }
    }
    public class KingSlimeProjSlam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 92;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            NPC owner = Main.npc[(int)Projectile.ai[1]];

            owner.globalEnemyBossInfo().finishedAtk = true;
            Projectile.active = false;
            return base.OnTileCollide(oldVelocity);
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[1]];
            owner.Center = Projectile.Center;


            if (Main.player[(int)Projectile.ai[0]].Top.Y - 30 > Projectile.Bottom.Y)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
        }
    }
    public class KingSlimeProjGround : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }   

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 47;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        int counter = 0;
        Vector2 teleportPos;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            if (counter++ % 6 == 0)
            {
                Projectile.position += Projectile.velocity;
                teleportPos = GetTeleportPos();
                Projectile.position = teleportPos - new Vector2(Projectile.width / 2, Projectile.height / 2);
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<KingSlimeProjSlimeSpawner>(), 0, 0, Main.myPlayer, Projectile.ai[0]);
                Main.projectile[proj].timeLeft = 120;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        private Vector2 GetTeleportPos()
        {
            Vector2 teleportPosition = Projectile.Center;

            Tile tile = Framing.GetTileSafely(teleportPosition);
            if (!tile.HasTile || !AGUtils.IsNotAmbientObject(tile.TileType))
            {
                for (int j = 0; j < 24; j++)
                {
                    if (tile.HasTile && !AGUtils.IsNotAmbientObject(tile.TileType))
                    {
                        return teleportPosition;
                    }
                    else
                    {
                        teleportPosition.Y += 16;
                        tile = Framing.GetTileSafely(teleportPosition);
                    }
                }
            }
            else
            {
                for (int j = 0; j < 24; j++)
                {
                    if (!tile.HasTile || !AGUtils.IsNotAmbientObject(tile.TileType))
                    {
                        return teleportPosition + new Vector2(0, 16);
                    }
                    else
                    {
                        teleportPosition.Y -= 16;
                        tile = Framing.GetTileSafely(teleportPosition);
                    }
                }
            }
            return Projectile.Center;

        }
    }
    public class KingSlimeProjSlimeSpawner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 110;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(1.2f, 1.7f);
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].alpha *= 0;
            Main.dust[dust].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            int type = DifficultyModes.Difficulty == 2 ? Main.rand.NextFromList(NPCID.MotherSlime, NPCID.BlackSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime) : NPCID.BlueSlime;
            int nPC = NPC.NewNPC(NPC.GetBossSpawnSource(Projectile.owner), (int)Projectile.Center.X, (int)Projectile.Center.Y, type);

            Main.npc[nPC].lifeMax *= (int)Projectile.ai[0];
            Main.npc[nPC].life *= (int)Projectile.ai[0];

            Main.npc[nPC].lifeMax /= 75;
            Main.npc[nPC].life /= 75;

            Main.npc[nPC].damage = (int)(Main.npc[nPC].damage * 2f);
            Main.npc[nPC].velocity = new Vector2(0, -12);
        }
    }

}
