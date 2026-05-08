using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class Phloem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets travel instantly");
		}

        int counter;

		public override void SetDefaults()
		{
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = 5000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 7f;
            Item.noMelee = true;
            Item.ArmorPenetration = 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            counter++;
            if (counter % 5 == 1)
            {
                Vector2 vel = velocity.SafeNormalize(Vector2.One) * 4;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PhloemProj>(), (int)(damage * 0.67), knockback / 2, player.whoAmI, vel.RotatedBy(MathHelper.ToRadians(-45)).X, vel.RotatedBy(MathHelper.ToRadians(-45)).Y);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PhloemProj>(), (int)(damage * 0.67), knockback / 2, player.whoAmI, vel.RotatedBy(MathHelper.ToRadians(-15)).X, vel.RotatedBy(MathHelper.ToRadians(-15)).Y);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PhloemProj>(), (int)(damage * 0.67), knockback / 2, player.whoAmI, vel.RotatedBy(MathHelper.ToRadians(15)).X, vel.RotatedBy(MathHelper.ToRadians(15)).Y);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PhloemProj>(), (int)(damage * 0.67), knockback / 2, player.whoAmI, vel.RotatedBy(MathHelper.ToRadians(45)).X, vel.RotatedBy(MathHelper.ToRadians(45)).Y);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(0, 0);
			return offset;
        }
	}

    public class PhloemProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 7;
            Projectile.height = 11;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        readonly int[] frameOrder = { 0, 1, 2, 1 };
        int frameIndex = 0;

        int StateTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[Type].Value;
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
            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.position += new Vector2(Projectile.ai[0], Projectile.ai[1]);
            Projectile.ai[0] *= 0.94f;
            Projectile.ai[1] *= 0.94f;

            if (StateTimer % 5 == 0)
            {

                frameIndex++;

                if (frameIndex >= frameOrder.Length)
                {
                    frameIndex = 0;
                }
                Projectile.frame = frameOrder[frameIndex];
            }
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 4; i++)
            {

                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}