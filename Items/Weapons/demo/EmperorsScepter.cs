using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Weapons.demo
{

    public class EmperorsScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 222;
            Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 3;
            Item.value = 15000;
            Item.rare = 8;
            Item.UseSound = SoundID.Item29;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = ModContent.ProjectileType<EmperorsScepterProj>();
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.One) * 72, velocity.RotatedByRandom(MathHelper.ToRadians(30)), Item.shoot, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EmperorsScepterHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class EmperorsScepterHoldout : ModProjectile
    {
        public override string Texture => "AwfulGarbageMod/Items/Weapons/demo/EmperorsScepter";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[base.Type] = true;
            ProjectileID.Sets.CanDistortWater[base.Type] = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public int duration;
        int visualTimer;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
        }

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

            Vector2 origin = new Vector2(0, frameHeight);

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
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation + MathHelper.ToRadians(45), origin, Projectile.scale, spriteEffects, 0);
            
            Texture2D texture2 = (Texture2D)ModContent.Request<Texture2D>("AwfulGarbageMod/Items/Weapons/demo/EmperorsScepterSparkle");
            visualTimer++;
            sourceRectangle = new Rectangle(0, startY, texture2.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;


            Main.EntitySpriteDraw(texture2,
                Projectile.Center - Main.screenPosition + new Vector2(64 * Projectile.scale, 0).RotatedBy(Projectile.rotation),
                sourceRectangle, Color.White, 0, origin, Projectile.scale, spriteEffects, 0);

            if (visualTimer < 10)
            {

                Main.EntitySpriteDraw(texture2,
                    Projectile.Center - Main.screenPosition + new Vector2(64 * Projectile.scale, 0).RotatedBy(Projectile.rotation),
                    sourceRectangle, Color.White * ((10 - visualTimer) / 10f), 0, origin, Projectile.scale * (1 + visualTimer / 10f), spriteEffects, 0);
            }
            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[base.Projectile.owner];
            duration = player.itemAnimationMax;
            player.heldProj = base.Projectile.whoAmI;
            if (base.Projectile.timeLeft > duration)
            {
                base.Projectile.timeLeft = duration;
            }
            Projectile.Center = player.HandPosition.Value;
            Projectile.spriteDirection = player.direction;
            Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation(); 
            player.ChangeDir((Main.MouseWorld.X > Projectile.Center.X).ToDirectionInt());

        }
    }
    public class EmperorsScepterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wind"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer = 0;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 480;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 2;
        }

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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void AI()
        {
            StateTimer++;

            if (Projectile.timeLeft % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            Player player = Main.player[Projectile.owner];
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135);

            float maxDetectRadius = 300; // The maximum radius at which a projectile can detect a target
            float projSpeed = 2f; // The speed at which the projectile moves towards the target
            if (Projectile.timeLeft > 450)
            {
                projSpeed = 0.5f;
            }
            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 8f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 8;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135);
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance && lineOfSight)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}