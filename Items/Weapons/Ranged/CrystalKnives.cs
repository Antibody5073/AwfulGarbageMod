using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class CrystalKnives : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 33;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 14;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3.25f;
			Item.value = 10000;
			Item.rare = 5;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrystalKnivesProj>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0);

            return false;
        }
    }

    public class CrystalKnivesProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        float spinSpd;
        int StateTimer;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 450;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)TextureAssets.Projectile[864];

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * 1;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            startY = frameHeight * 0;
            sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;


            // Applying lighting and draw current frame
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void AI()
        {
            StateTimer++;
            if (Projectile.ai[0] == 0)
            {
                float distanceToMouse = Vector2.Distance(Projectile.Center, new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y));
                if (distanceToMouse < 180)
                {
                    Player player = Main.player[Projectile.owner];
                    Vector2 vel = Projectile.velocity / player.GetModPlayer<GlobalPlayer>().rangedVelocity / player.GetModPlayer<GlobalPlayer>().knifeVelocity;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel * 0.8f, ModContent.ProjectileType<CrystalKnivesProj>(), Projectile.damage, 0, Projectile.owner, 1);
                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().Empowerments = Projectile.GetGlobalProjectile<KnifeProjectile>().Empowerments;
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(12)) * 0.8f, ModContent.ProjectileType<CrystalKnivesProj>(), Projectile.damage * 2 / 3, 0, Projectile.owner, 1);
                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
                    proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(-12)) * 0.8f, ModContent.ProjectileType<CrystalKnivesProj>(), Projectile.damage * 2 / 3, 0, Projectile.owner, 1);
                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}