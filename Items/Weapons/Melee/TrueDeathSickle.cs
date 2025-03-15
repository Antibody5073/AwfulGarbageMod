using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Transactions;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class TrueDeathSickle : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Star"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Stacks up to 3");
		}

		public override void SetDefaults()
		{
			Item.damage = 532;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 42;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 42;
			Item.useStyle = 1;
			Item.knockBack = 3.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrueDeathSickleProj>();
            Item.shootSpeed = 24f;
        }

	}

    public class TrueDeathSickleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        bool returning = false;
        float spinSpd;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.scale = 1.5f;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        int StateTimer = 0;
        float RotateDir;
        bool didit;
        float projSpd;
        bool emitSpirits = false;
        public override bool PreDraw(ref Color lightColor)
        {

            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            /*
            float offsetX = 0;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 0;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            */

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(18f, 18f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 681)
            {
                Projectile.timeLeft = 681;
            }
            Projectile.tileCollide = false;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!emitSpirits)
            {
                for (var i = 0; i < 6; i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0, 8).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("IceSpiritPikeSpiritProj").Type, Projectile.damage / 6  , 0, Projectile.owner, 2, 15);
                    Main.projectile[proj].tileCollide = false;
                    Main.projectile[proj].penetrate -= 1;
                }
                emitSpirits = true;
            }
        }

        public override void AI()
        {
            if (!didit)
            {
                projSpd = Projectile.velocity.Length();
                didit = true;
            }

            Player player = Main.player[Projectile.owner];
            StateTimer++;


            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0;
            Main.dust[dust].noGravity = true;
            Projectile.rotation += Main.rand.NextFloat(0.35f, 0.5f);
            if (Projectile.timeLeft > 680)
            {
                if (StateTimer % 3 == 0)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.DeathSickle, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].penetrate += 3;
                }
            }
            else
            {
                if (Projectile.timeLeft == 680 && !emitSpirits)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(45)) * -8, Mod.Find<ModProjectile>("IceSpiritPikeSpiritProj").Type, Projectile.damage / 6, 0, Projectile.owner, 2);
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].penetrate -= 1;
                    }
                    emitSpirits = true;
                }
                if (player.active)
                {
                    RotateDir += AGUtils.TurnTowards(180, player.Center, RotateDir, Projectile.Center);
                    Projectile.velocity = new Vector2(projSpd, 0).RotatedBy(RotateDir);

                    if (Vector2.DistanceSquared(player.Center, Projectile.Center) < projSpd * projSpd)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
    }
}