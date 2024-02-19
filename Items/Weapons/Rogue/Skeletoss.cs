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
using StramClasses;
using StramClasses.Classes.Rogue;
using StramClasses.Classes.Rogue.Projectiles;
using AwfulGarbageMod.Global;
using Terraria.GameContent;
using Terraria.Audio;

namespace AwfulGarbageMod.Items.Weapons.Rogue
{
    [ExtendsFromMod("StramClasses")]

    public class Skeletoss : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.toRogueItem(12);
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 13;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.5f;
            Item.value = 5000;
            Item.rare = 2;
            SoundStyle soundStyle = new SoundStyle("StramClasses/Assets/Sounds/RogueKnife");
            soundStyle.Volume = 3f;
            soundStyle.PitchVariance = 0.5f;
            SoundStyle value = soundStyle;
            base.Item.UseSound = value; Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("SkeletossProj").Type;
            Item.shootSpeed = 0.1f;
            Item.ArmorPenetration = 8;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            RoguePlayer roguePlayer = player.rogue();

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].rogueProjectile().critDamage = (int)Math.Round(roguePlayer.critDamage * (float)base.Item.rogueItem().baseCritDamage * base.Item.rogueItem().prefixCritDamage + (float)roguePlayer.critDamageFlat);
            return false;
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class SkeletossProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        int aiState = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = StramUtils.rogueDamage();
            Projectile.rogueProjectile().splittableWeapon = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 15;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 32;
            Projectile.extraUpdates = 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity.Normalize();
            Projectile.velocity *= 0.1f;
            for (var i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                Main.dust[dust].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(20)) * -8;
                Main.dust[dust].noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 32; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].velocity = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians((360 / 32) * i));
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(8f, 8f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
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
            base.Projectile.rotation += 0.3f * (float)base.Projectile.direction;
            base.Projectile.spriteDirection = base.Projectile.direction;
            base.Projectile.ai[0] += 1f;
            Projectile.velocity += (Projectile.velocity.SafeNormalize(Vector2.Zero)) * 0.05f;
            if (Vector2.Distance(Projectile.velocity, Vector2.Zero) > 8)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 8;
            }
            
            if (Projectile.timeLeft % 5 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 0.7f;
                Main.dust[dust].velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.6f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}