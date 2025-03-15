using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class DeathStringsSealed : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots an inaccurate, short-ranged ice shard along with an arrow");
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "Sealed", "\n[c/521864:(Sealed)]");
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "ItemName" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }
        public override void SetDefaults()
		{
			Item.damage = 27;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.noMelee = true;
			Item.useAnimation = 20;
			Item.useStyle = 5;
			Item.knockBack = 1f;
			Item.value = 200000;
            Item.rare = -1;
            Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(12)), ModContent.ProjectileType<DeathStringsProj>(), damage * 2/3, knockback / 5, player.whoAmI, 0, 0.4f);

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(0, 0);
			return offset;
        }
	}
    public class DeathStrings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frost Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots an inaccurate, short-ranged ice shard along with an arrow");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "Sealed", "\n[c/521864:(Unsealed)]");
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "ItemName" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }
        public override void SetDefaults()
        {
            Item.damage = 401;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.noMelee = true;
            Item.useAnimation = 26;
            Item.useStyle = 5;
            Item.knockBack = 1f;
            Item.value = 200000;
            Item.rare = -1;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].GetGlobalProjectile<ProjectileWeaponEffect>().DeathStrings = true;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }
    }

    public class DeathStringsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer = 0;
        float maxSpd;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 660;
            Projectile.light = 1f;
            Projectile.alpha = 160;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            maxSpd = Vector2.Distance(Projectile.velocity, Vector2.Zero);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]--;
                return false;
            }
            return base.CanHitNPC(target);
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

        public override bool ShouldUpdatePosition()
        {
            if (Projectile.timeLeft < 600)
            {
                return true;
            }
            else
            {
                Projectile.position += Projectile.velocity * (1-(((float)Projectile.timeLeft - 600) / 60));
                return false;
            }
        }

        public override void AI()
        {
            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90);

            float maxDetectRadius = 1200f; // The maximum radius at which a projectile can detect a target
            float projSpeed = Projectile.ai[1]; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null || Projectile.ai[0] > 0)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > maxSpd)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= maxSpd;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90);

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
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
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