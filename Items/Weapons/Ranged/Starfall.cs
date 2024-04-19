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

    public class Starfall : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 130;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 7;
			Item.noMelee = true;
			Item.scale = 0f;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5f;
			Item.value = 10000;
			Item.rare = 10;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<StarfallProj>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
            pointPoisition.Y -= 100 * 1;
            float num90 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            float num101 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
            float ai2 = num101 + pointPoisition.Y;
            if (num101 < 0f)
            {
                num101 *= -1f;
            }
            if (num101 < 20f)
            {
                num101 = 20f;
            }
            float num112 = (float)Math.Sqrt(num90 * num90 + num101 * num101);
            num112 = Item.shootSpeed / num112;
            num90 *= num112;
            num101 *= num112;
            Vector2 vector5 = new Vector2(num90, num101) / 2f;
            Projectile.NewProjectile(source, pointPoisition, vector5.RotatedByRandom(MathHelper.ToRadians(12)), type, damage, knockback, player.whoAmI, 0f, ai2);

            return false;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 18)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class StarfallProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        float spinSpd;
        float projSpeed = 0.3f;
        float maxSpd;
        bool didit = false;
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
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }

        public override void OnSpawn(IEntitySource source)
        {
            spinSpd = (Main.rand.Next(2) * 2 - 1) * 0.2f;
            maxSpd = Vector2.Distance(Vector2.Zero, Projectile.velocity);
        }

        int StateTimer = 0;

        public override bool PreDraw(ref Color lightColor)
        {
            if (!didit)
            {
                Player player = Main.player[Projectile.owner];
                SpriteEffects spriteEffects = SpriteEffects.None; // Flip the sprite based on the direction it is facing.

                Texture2D value123 = TextureAssets.Projectile[Projectile.type].Value;
                Rectangle rectangle25 = new Rectangle(0, 0, value123.Width, value123.Height);
                Vector2 origin33 = rectangle25.Size() / 2f;
                Color alpha13 = Projectile.GetAlpha(lightColor);
                Texture2D value124 = TextureAssets.Extra[91].Value;
                Rectangle value125 = value124.Frame();
                Vector2 origin10 = new Vector2((float)value125.Width / 2f, 10f);
                Vector2 vector122 = new Vector2(0f, Projectile.gfxOffY);
                Vector2 spinningpoint = new Vector2(0f, -10f);
                float num189 = (float)Main.timeForVisualEffects / 60f;
                Vector2 vector123 = Projectile.Center + Projectile.velocity;
                Color color119 = Color.Blue * 0.2f;
                Color color120 = Color.White * 0.5f;
                color120.A = 0;
                float num190 = 0f;
                if (Main.tenthAnniversaryWorld)
                {
                    color119 = Color.HotPink * 0.3f;
                    color120 = Color.White * 0.75f;
                    color120.A = 0;
                    num190 = -0.1f;
                }
                if (Projectile.type == 728)
                {
                    color119 = Color.Orange * 0.2f;
                    color120 = Color.Gold * 0.5f;
                    color120.A = 50;
                    num190 = -0.2f;
                }
                Color color121 = color119;
                color121.A = 0;
                Color color122 = color119;
                color122.A = 0;
                Color color123 = color119;
                color123.A = 0;
                alpha13.A = 0;
                float direction = Projectile.velocity.ToRotation();


                Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189), value125, new Color(color121.R, color121.G, color121.B, Projectile.alpha), direction + (float)Math.PI / 2f, origin10, 1.5f + num190, SpriteEffects.None);
                Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + (float)Math.PI * 2f / 3f), value125, new Color(color122.R, color122.G, color122.B, Projectile.alpha), direction + (float)Math.PI / 2f, origin10, 1.1f + num190, SpriteEffects.None);
                Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + 4.1887903f), value125, new Color(color123.R, color123.G, color123.B, Projectile.alpha), direction + (float)Math.PI / 2f, origin10, 1.3f + num190, SpriteEffects.None);
                Vector2 vector124 = Projectile.Center - Projectile.velocity * 0.5f;
                for (float num191 = 0f; num191 < 1f; num191 += 0.5f)
                {
                    float num192 = num189 % 0.5f / 0.5f;
                    num192 = (num192 + num191) % 1f;
                    float num193 = num192 * 2f;
                    if (num193 > 1f)
                    {
                        num193 = 2f - num193;
                    }
                    Main.EntitySpriteDraw(value124, vector124 - Main.screenPosition + vector122, value125, new Color(color120.R, color120.G, color120.B, Projectile.alpha) * num193, direction + (float)Math.PI / 2f, origin10, 0.3f + num192 * 0.5f, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(value123, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle25, new Color(alpha13.R, alpha13.G, alpha13.B, Projectile.alpha), Projectile.rotation, origin33, Projectile.scale + 0.25f, spriteEffects);

            }

            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects2 = SpriteEffects.None;

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
            spriteEffects2 = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(5f, 5f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 3.5f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects2, 0f);
            }
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects2, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void AI()
        {
            StateTimer++;

            Projectile.rotation += spinSpd;

            if (Projectile.penetrate > 0)
            {
                float maxDetectRadius = 360; // The maximum radius at which a projectile can detect a target

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                // If found, change the velocity of the projectile and turn it in the direction of the target
                // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                projSpeed += 0.05f;
                if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > maxSpd)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= maxSpd;
                }
            }
            else
            {
                if (!didit)
                {
                    didit = true;
                    Projectile.timeLeft = 12;
                    Projectile.extraUpdates = 0;
                }
                Projectile.alpha += 24;
            }

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