using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Placeable.Furniture;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Drawing;
using Terraria.GameContent;
using AwfulGarbageMod.Items.Weapons.Melee;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class Icewave : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shatter Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a shattering icicle \nShards have armor piercing capabilities");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 20;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = 5;
            Item.knockBack = 2;
            Item.value = 50000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = ModContent.ProjectileType<IcewaveProj>();
            Item.shootSpeed = 0.01f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.direction); 
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShatterTome>()
                .AddIngredient<FrigidiumBar>(19)
                .AddTile<Tiles.Furniture.FrigidForge>()
                .Register();
        }
    }

    public class IcewaveProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Icicle"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        int timer;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void AI()
        {
            if (timer > 60)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.02f;
            }
            if (timer % 90 == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IcewaveProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 45, 1);
                Main.projectile[proj].flailProjectile().shadeVortexDirection = Projectile.ai[0];
                Main.projectile[proj].flailProjectile().ownerProj = Projectile;
                proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IcewaveProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + MathHelper.Pi, 45, 1);
                Main.projectile[proj].flailProjectile().shadeVortexDirection = Projectile.ai[0];
                Main.projectile[proj].flailProjectile().ownerProj = Projectile;

            }
            timer++;

        }

        public override void OnKill(int timeLeft)
        {
        }
    }
    public class IcewaveProj2 : ModProjectile
    {

        // We could use a vanilla texture if we want instead of supplying our own.
        // public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Excalibur;

        public override void SetStaticDefaults()
        {
            // If a Jellyfish is zapping and we attack it with this projectile, it will deal damage to us.
            // This set has the projectiles for the Night's Edge, Excalibur, Terra Blade (close range), and The Horseman's Blade (close range).
            // This set does not have the True Night's Edge, True Excalibur, or the long range Terra Beam projectiles.
            Main.projFrames[Type] = 4; // This projectile has 4 frames.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer = 0;
        public override void SetDefaults()
        {
            // The width and height don't really matter here because we have custom collision.
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10; // The projectile can hit 3 enemies.
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true; // A line of sight check so the projectile can't deal damage through tiles.
            Projectile.ownerHitCheckDistance = 300; // The maximum range that the projectile can hit a target. 300 pixels is 18.75 tiles.
            Projectile.usesOwnerMeleeHitCD = false; // This will make the projectile apply the standard number of immunity frames as normal melee attacks.
                                                    // Normally, projectiles die after they have hit all the enemies they can.
                                                    // But, for this case, we want the projectile to continue to live so we can have the visuals of the swing.
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

            // We will be using custom AI for this projectile. The original Excalibur uses aiStyle 190.
            Projectile.aiStyle = -1;
            // Projectile.aiStyle = ProjAIStyleID.NightsEdge; // 190
            // AIType = ProjectileID.Excalibur;

            // If you are using custom AI, add this line. Otherwise, visuals from Flasks will spawn at the center of the projectile instead of around the arc.
            // We will spawn the visuals around the arc ourselves in the AI().
            Projectile.noEnchantmentVisuals = true;
            Projectile.alpha = 100;
        }

        public override void AI()
        {
            StateTimer++;

            // In our item, we spawn the projectile with the direction, max time, and scale
            // Projectile.ai[0] == direction
            // Projectile.ai[1] == max time
            // Projectile.ai[2] == scale
            // Projectile.localAI[0] == current time

            // Terra Blade makes an extra sound when spawning.
            // if (Projectile.localAI[0] == 0f) {
            // 	SoundEngine.PlaySound(SoundID.Item60 with { Volume = 0.65f }, Projectile.position);
            // }

            Projectile.localAI[0]++; // Current time that the projectile has been alive.
            Player player = Main.player[Projectile.owner];
            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
            float direction = Projectile.ai[0];
            float adjustedRotation = direction + MathHelper.TwoPi * percentageOfLife * Projectile.flailProjectile().shadeVortexDirection;
            Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.

            float scaleMulti = 1.3f; // Excalibur, Terra Blade, and The Horseman's Blade is 0.6f; True Excalibur is 1f; default is 0.2f 
            float scaleAdder = 1f; // Excalibur, Terra Blade, and The Horseman's Blade is 1f; True Excalibur is 1.2f; default is 1f 

            Projectile.Center = Projectile.flailProjectile().ownerProj.Center - Projectile.velocity;
            Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;

            // The other sword projectiles that use AI Style 190 have different effects.
            // This example only includes the Excalibur.
            // Look at AI_190_NightsEdge() in Projectile.cs for the others.

            // Here we spawn some dust inside the arc of the swing.
            float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();


            Projectile.scale *= Projectile.ai[2]; // Set the scale of the projectile to the scale of the item.

            // If the projectile is as old as the max animation time, kill the projectile.
            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.Kill();
            }
            if (!Projectile.flailProjectile().ownerProj.active)
            {
                Projectile.Kill();
            }

        }

        // Here is where we have our custom collision.
        // This collision will only run if the projectile is within range of target with the range being Projectile.ownerHitCheckDistance
        // Or if the projectile hasn't already hit all of the targets it can with Projectile.penetrate
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // This is how large the circumference is, aka how big the range is. Vanilla uses 94f to match it to the size of the texture.
            float coneLength = 94f * Projectile.scale;
            // This number affects how much the start and end of the collision will be rotated.
            // Bigger Pi numbers will rotate the collision counter clockwise.
            // Smaller Pi numbers will rotate the collision clockwise.
            // (Projectile.ai[0] is the direction)
            float collisionRotation = MathHelper.PiOver2 * Projectile.flailProjectile().shadeVortexDirection;
            float maximumAngle = MathHelper.Pi; // The maximumAngle is used to limit the rotation to create a dead zone.
            float coneRotation = Projectile.rotation + collisionRotation;

            // Uncomment this line for a visual representation of the cone. The dusts are not perfect, but it gives a general idea.
            // Dust.NewDustPerfect(Projectile.Center + coneRotation.ToRotationVector2() * coneLength, DustID.Pixie, Vector2.Zero);
            // Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, new Vector2((float)Math.Cos(maximumAngle) * Projectile.ai[0], (float)Math.Sin(maximumAngle)) * 5f); // Assumes collisionRotation was not changed

            // First, we check to see if our first cone intersects the target.
            if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
            {
                return true;
            }

            return false;
        }

        public override void CutTiles()
        {
            // Here we calculate where the projectile can destroy grass, pots, Queen Bee Larva, etc.
            Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            float width = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Vanilla has several particles that can easily be used anywhere.
            // The particles from the Particle Orchestra are predefined by vanilla and most can not be customized that much.
            // Use auto complete to see the other ParticleOrchestraType types there are.
            // Here we are spawning the Excalibur particle randomly inside of the target's hitbox.
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
                Projectile.owner);

            // You could also spawn dusts at the enemy position. Here is simple an example:
            // Dust.NewDust(Main.rand.NextVector2FromRectangle(target.Hitbox), 0, 0, ModContent.DustType<Content.Dusts.Sparkle>());

            // Set the target's hit direction to away from the player so the knockback is in the correct direction.
            hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);

            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
                Projectile.owner);

            info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
        }

        // Taken from Main.DrawProj_Excalibur()
        // Look at the source code for the other sword types.
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = texture.Frame(1, 4); // The sourceRectangle says which frame to use.
            Vector2 origin = sourceRectangle.Size() / 2f;
            float scale = Projectile.scale * 1.1f;
            SpriteEffects spriteEffects = ((Projectile.flailProjectile().shadeVortexDirection < 0)) ? SpriteEffects.FlipVertically : SpriteEffects.None; // Flip the sprite based on the direction it is facing.
            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
            float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

            Color backDarkColor = new Color(43, 208, 226); // Original Excalibur color: Color(180, 160, 60)
            Color middleMediumColor = new Color(21, 255, 240); // Original Excalibur color: Color(255, 255, 80)
            Color frontLightColor = new Color(104, 238, 253); // Original Excalibur color: Color(255, 240, 150)

            backDarkColor.A = (byte)0.5f;
            middleMediumColor.A = (byte)0.5f;
            frontLightColor.A = (byte)0.5f;


            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            // Back part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation, origin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation, origin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(8f, 8f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.oldRot[k], origin, scale, spriteEffects, 0f);
                // Very faint part affected by the light color
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, faintLightingColor * 0.15f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k], origin, scale, spriteEffects, 0f);
                // Middle part
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, middleMediumColor * lerpTime * 0.3f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k], origin, scale, spriteEffects, 0f);
                // Front part
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, frontLightColor * lerpTime * 0.5f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k], origin, scale * 0.975f, spriteEffects, 0f);
                // Thin top line (final frame)
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * lerpTime * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k] + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
                // Thin middle line (final frame)
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * lerpTime * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k] + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
                // Thin bottom line (final frame)
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * lerpTime * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.oldRot[k] + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            }

            return false;
        }

        // Copied from Main.DrawPrettyStarSparkle() which is private
        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D sparkleTexture = TextureAssets.Extra[98].Value;
            Color bigColor = shineColor * opacity * 0.5f;
            bigColor.A = 0;
            Vector2 origin = sparkleTexture.Size() / 2f;
            Color smallColor = drawColor * 0.5f;
            float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
            Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
            bigColor *= lerpValue;
            smallColor *= lerpValue;
            // Bright, large part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
            // Dim, small part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
        }
    }
}