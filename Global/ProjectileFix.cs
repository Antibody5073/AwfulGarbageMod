using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Printing;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using Mono.Cecil;
using Terraria.Audio;
using Terraria.Localization;
using AwfulGarbageMod.Items.Accessories;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Global
{

    public class TempestStaffProjectileFix : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        int StateTimer = 0;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ProjectileID.MiniSharkron;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[ProjectileID.MiniSharkron] = 2;
            ProjectileID.Sets.TrailCacheLength[ProjectileID.MiniSharkron] = 6;
        }
        public override void SetDefaults(Projectile entity)
        {
            entity.alpha = 0;
            entity.aiStyle = -1;
            entity.extraUpdates = 1;
        }
        public override void AI(Projectile projectile)
        {
            StateTimer++;
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.frame = 1;
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            projectile.velocity /= 2;
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.frame = 1;

        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[ProjectileID.MiniSharkron].Value;

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;
            // Applying lighting and draw current frame
            Color drawColor = projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            for (int k = 0; k < projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY) + new Vector2(projectile.height / 2, projectile.width / 2);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}