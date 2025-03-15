using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Ammo
{
    public class FrigidArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stone Pellet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Low damage, easy to obtain bullet");
        }

        public override void SetDefaults()
        {
            Item.damage = 6; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 3f;
            Item.value = 15000;
            Item.rare = 2;
            Item.shoot = Mod.Find<ModProjectile>("FrigidArrowProj").Type; // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 5.5f; // The speed of the projectile.
            Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient<Placeable.OresBars.FrigidiumBar>(1)
                .AddIngredient<FrostShard>(1)
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe(150)
                .AddIngredient<Placeable.OresBars.FrigidiumBar>(1)
                .AddIngredient<FrostShard>(1)
                .AddIngredient(ItemID.WoodenArrow, 75)
                .AddIngredient(ItemID.FrostburnArrow, 50)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class FrigidArrowProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fossil Bullet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.arrow = true;
        }
        Vector2 spawnPos;
        int temp;
        public override void OnSpawn(IEntitySource source)
        {
            spawnPos = Projectile.Center;
            temp = 4;
        }
        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 6; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Ice, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 1.2f);
            }
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
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            temp--;
            if (temp == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity * 0.8f, Mod.Find<ModProjectile>("FrigidArrowProj2").Type, (int)(Projectile.damage * 0.4f), Projectile.knockBack / 3, Projectile.owner, 0);
            }
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
    public class FrigidArrowProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fossil Bullet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        float maxSpd;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.arrow = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            maxSpd = Vector2.Distance(Projectile.velocity, Vector2.Zero);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 8; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-3, 3);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Frost, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(10))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }


        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 0.8f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
            dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].velocity = Projectile.velocity * 0.5f;
            Main.dust[dust].noGravity = true;

            float maxDetectRadius = 800f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 0.3f; // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > maxSpd)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= maxSpd;
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