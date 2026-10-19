using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Summon;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{
    public class Foliaggregate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Constellate");
            // Tooltip.SetDefault("Strike enemies to gain damage and yoyo speed, up to double");
           

            // These are all related to gamepad controls and don't seem to affect anything else
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24; // The width of the item's hitbox.
            Item.height = 24; // The height of the item's hitbox.

            Item.useStyle = ItemUseStyleID.Shoot; // The way the item is used (e.g. swinging, throwing, etc.)
            Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
            Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
            Item.noMelee = true; // This makes it so the item doesn't do damage to enemies (the projectile does that).
            Item.noUseGraphic = true; // Makes the item invisible while using it (the projectile is the visible part).
            Item.UseSound = SoundID.Item1; // The sound that will play when the item is used.

            Item.damage = 33; // The amount of damage the item does to an enemy or player.
            Item.DamageType = DamageClass.MeleeNoSpeed; // The type of damage the weapon does. MeleeNoSpeed means the item will not scale with attack speed.
            Item.knockBack = 7f; // The amount of knockback the item inflicts.
            Item.crit = 0; // The percent chance for the weapon to deal a critical strike. Defaults to 4.
            Item.channel = true; // Set to true for items that require the attack button to be held out (e.g. yoyos and magic missile weapons)
            Item.rare = 1; // The item's rarity. This changes the color of the item's name.
            Item.value = Item.buyPrice(gold: 1); // The amount of money that the item is can be bought for.

            Item.shoot = ModContent.ProjectileType<FoliaggregateProj>(); // Which projectile this item will shoot. We set this to our corresponding projectile.
            Item.shootSpeed = 16f; // The velocity of the shot projectile.			
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.JungleYoyo)
                .AddIngredient(ItemID.Valor)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FoliaggregateProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 215f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12f;
        }
        int hits = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of the projectile's hitbox.
            Projectile.height = 16; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        int counter = 0;
        float dir;
        // notes for aiStyle 99: 
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.
        /*
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                hitInfo.Damage = (int)dmg;
                
                if (hitInfo.Crit)
                {
                    hitInfo.Damage = (int)(hitInfo.Damage * (2 + player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg));
                    hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit;
                }
            };
        }
        */

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
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }


        public override void PostAI()
        {
            counter++;
            if (counter % 6 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FoliaggregateProj2>(), Projectile.damage / 4, Projectile.knockBack / 1.5f, Projectile.owner, dir, Projectile.Center.X > Main.player[Projectile.owner].Center.X ? 2 : -2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FoliaggregateProj2>(), Projectile.damage / 4, Projectile.knockBack / 1.5f, Projectile.owner, dir + 180, Projectile.Center.X > Main.player[Projectile.owner].Center.X ? 2 : -2);
                dir += Projectile.Center.X > Main.player[Projectile.owner].Center.X ? -10f : 10f;
            }
        }

    }
    public class FoliaggregateProj2 : ModProjectile
    {
        public override string Texture => "AwfulGarbageMod/Items/Weapons/Melee/LeafBladeProj";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            
        }
        int bounces;
        readonly int[] frameOrder = { 0, 1, 2, 1 };
        int frameIndex = 0;
        Vector2 origin;
        float distance;
        public override void OnSpawn(IEntitySource source)
        {
            origin = Projectile.Center;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Defense *= 0.4f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (StateTimer > 0)
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
                // Applying lighting and draw current frame
                Color drawColor = Projectile.GetAlpha(lightColor);

                Texture2D projectileTexture = texture;
                Vector2 drawOrigin = origin;
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = (byte)(color.A * 0.75f);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
                }

                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
                // It's important to return false, otherwise we also draw the original texture.
                return false;
        }
        public override void AI()
        {
            StateTimer++;


            if (StateTimer % 5 == 0)
            {

                frameIndex++;

                if (frameIndex >= frameOrder.Length)
                {
                    frameIndex = 0;
                }
                Projectile.frame = frameOrder[frameIndex];
            }
            Projectile.Center = origin + new Vector2(distance, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[0]));
            Projectile.ai[0] += Projectile.ai[1];
            distance += 2f;
            if (StateTimer > 0)
            {
                Projectile.rotation = (Projectile.oldPos[0] - Projectile.oldPos[1]).ToRotation() + MathHelper.PiOver2;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
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