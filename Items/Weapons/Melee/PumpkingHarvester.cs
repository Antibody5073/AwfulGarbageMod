using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using System.Diagnostics.Contracts;
using StramClasses.Classes.Rogue;
using StramClasses;
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Weapons.Melee
{
    public class PumpkingHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            // This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
            // When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
            Item.knockBack = 4f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
            Item.width = 32; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.damage = 388; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<PumpkingHarvesterProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 8; // The color of the name of your item
            Item.value = Item.sellPrice(gold: 10); // Sells for 1 gold 50 silver
            Item.DamageType = ModContent.GetInstance<FlailDamageClass>();
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation());
            Main.projectile[proj].originalDamage = Item.damage;

            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class PumpkingHarvesterProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/PumpkingHarvesterProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 160;
        public override float SpinSpd => 0.8f;
        public override float SpinDistanceIncrease => 0.4f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.35f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 80;
        public override float RetractSpd => 0.5f;
        public override float MaxRetractSpd => 6f;
        public override float flailHeadRotationRetract => 180;
        public override float flailHeadRotation => 90;

        public override void Dusts()
        {
            int dustRate = 3;

            if (Main.rand.NextBool(dustRate))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity *= 0.25f;
                Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override void WidthHeight()
        {
            Projectile.width = 64;
            Projectile.height = 64;
        }

        public override void SpinEffect(Player player, float timer, float length, float maxLength)
        {
            if (length == maxLength && timer % (int)(60 / spinSpdMultiplier) == 0)
            {
                Vector2 mouse = new Vector2((float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X, (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y);
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, mouse.SafeNormalize(Vector2.Zero) * 0.1f, ProjectileID.FlamingScythe, (int)(Projectile.damage * 0.2f), 0, player.whoAmI);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
                Main.projectile[proj].penetrate = 8;
                Main.projectile[proj].usesIDStaticNPCImmunity = true;
                Main.projectile[proj].idStaticNPCHitCooldown = 8; 
                Main.projectile[proj].rotation = Projectile.rotation;
                Main.projectile[proj].timeLeft = Main.projectile[proj].timeLeft * 2 / 3;
            }
        }


        public override void SetRotation(Vector2 offsetFromPlayer, float rotation, Player player)
        {
            Projectile.rotation = offsetFromPlayer.ToRotation() * -2 * player.direction + MathHelper.ToRadians(-90) + MathHelper.ToRadians(rotation) * player.direction * (Projectile.flailProjectile().spinSpdMultiplier >= 0 ? 1 : -1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile) + offsetCenter;

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath());

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                var chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    // Use normal chainTexture and lighting, no changes
                }

                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            Texture2D projectileTexture = TextureAssets.Projectile[ProjectileID.FlamingScythe].Value;


            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, projectileTexture.Width, frameHeight);
            Color drawColor = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            // Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)

            if (CurrentAIState == AIState.Spinning)
            {
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }

            projectileTexture = TextureAssets.Npc[NPCID.PumpkingBlade].Value;


            frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
             startY = frameHeight * Projectile.frame;
            sourceRectangle = new Rectangle(0, startY, projectileTexture.Width, frameHeight);
            drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(projectileTexture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}