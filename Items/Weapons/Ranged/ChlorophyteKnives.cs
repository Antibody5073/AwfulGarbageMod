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
using System.Collections.Generic;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class ChlorophyteKnives : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 60;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 26;
			Item.noMelee = true;
			Item.scale = 0f;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5f;
			Item.value = 10000;
			Item.rare = 7;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ChlorophyteKnivesProj>();
            Item.shootSpeed = 13f;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 18)
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ChlorophyteKnivesProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        float spinSpd;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 450;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 11; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-18, 18)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.ChlorophyteWeapon, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].noGravity = true;
            }
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.SporeCloud, Projectile.damage / 3, 0.5f, Projectile.owner, 0);
            Main.projectile[proj].DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Main.projectile[proj].usesIDStaticNPCImmunity = true;
            Main.projectile[proj].idStaticNPCHitCooldown = 12;
            Main.projectile[proj].ArmorPenetration += 12;
            Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
            Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().Empowerments = Projectile.GetGlobalProjectile<KnifeProjectile>().Empowerments;

        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 11; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.ChlorophyteWeapon, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].noGravity = true;
            }
        }
        int StateTimer = 0;

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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 3.5f);
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

            if (Projectile.timeLeft == 420)
            {
                spinSpd = Projectile.velocity.X * 0.06f;
                if (spinSpd > 0.2f) { spinSpd = 0.2f; }
                if (spinSpd < -0.2f) { spinSpd = -0.2f; }
            }
            else if (Projectile.timeLeft < 420)
            {
                Projectile.rotation += spinSpd;
                Projectile.velocity.Y += 0.15f;
                Projectile.velocity.X *= 0.99f;
                if (Projectile.velocity.Y > 14)
                {
                    Projectile.velocity.Y = 14;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(30);
            }
        }
    }
}