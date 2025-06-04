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
    public class ShootingStar : ModItem
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
            Item.knockBack = 5f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
            Item.width = 32; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.damage = 28; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<ShootingStarProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 1; // The color of the name of your item
            Item.value = Item.sellPrice(silver: 50); // Sells for 1 gold 50 silver
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

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 7)
                .AddIngredient(ItemID.Chain, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShootingStarProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/ShootingStarProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 110;
        public override float SpinSpd => 0.95f;
        public override float SpinDistanceIncrease => 0.35f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.7f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 70;
        public override float RetractSpd => 0.05f;
        public override float MaxRetractSpd => 6f;

        public override void Dusts()
        {
            Projectile.light = 0.9f;

            if (StateTimer % 2 == 0) 
            {
                Vector2 vector2 = new Vector2(Main.screenWidth, Main.screenHeight);
                if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector2 / 2f, vector2 + new Vector2(400f))) && Main.rand.Next(6) == 0)
                {
                    int num932 = Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17);
                    if (Main.tenthAnniversaryWorld)
                    {
                        num932 = Utils.SelectRandom<int>(Main.rand, 16, 16, 16, 17);
                    }
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0.2f, num932);
                }
                if (Main.rand.Next(20) == 0 || (Main.tenthAnniversaryWorld && Main.rand.Next(15) == 0))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
                } 
            }
        }

        Vector2 offset;

        public override bool PreDraw(ref Color lightColor)
        {

            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = ((player.direction < 0)) ? SpriteEffects.FlipVertically : SpriteEffects.None; // Flip the sprite based on the direction it is facing.

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
            float direction = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
            {
                direction = offset.ToRotation() + MathHelper.ToRadians(player.direction * 80);
            }

            Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189), value125, color121, direction + (float)Math.PI / 2f, origin10, 1.5f + num190, SpriteEffects.None);
            Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + (float)Math.PI * 2f / 3f), value125, color122, direction + (float)Math.PI / 2f, origin10, 1.1f + num190, SpriteEffects.None);
            Main.EntitySpriteDraw(value124, vector123 - Main.screenPosition + vector122 + spinningpoint.RotatedBy((float)Math.PI * 2f * num189 + 4.1887903f), value125, color123, direction + (float)Math.PI / 2f, origin10, 1.3f + num190, SpriteEffects.None);
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
                Main.EntitySpriteDraw(value124, vector124 - Main.screenPosition + vector122, value125, color120 * num193, direction + (float)Math.PI / 2f, origin10, 0.3f + num192 * 0.5f, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(value123, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle25, alpha13, Projectile.rotation, origin33, Projectile.scale + 0.1f, spriteEffects);


            return base.PreDraw(ref lightColor);
        }

        public override void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
            offset = offsetFromPlayer;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            Color newColor7 = Color.CornflowerBlue;
            SoundEngine.PlaySound(in SoundID.Item10, Projectile.position);

            newColor7 = Color.HotPink;
            newColor7.A /= 2;
            for (int num635 = 0; num635 < 7; num635++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float num636 = 0f; num636 < 1f; num636 += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num636 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num637 = 0f; num637 < 1f; num637 += 0.25f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num637 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector54 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector54 / 2f, vector54 + new Vector2(400f))))
            {
                for (int num638 = 0; num638 < 7; num638++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
        }
    }
}