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
    public class VortexStorm : ModItem
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
            Item.damage = 693; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<VortexStormProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 1; // The color of the name of your item
            Item.value = Item.sellPrice(silver: 20); // Sells for 1 gold 50 silver
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
                .AddIngredient(ItemID.FragmentVortex, 18)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class VortexStormProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/VortexStormProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public Vector2 centerPos;
        float rotation = 0;

        public override float MaxSpinDistance => 145;
        public override float SpinSpd => 1f;
        public override float SpinDistanceIncrease => 0.3f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.25f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 60;
        public override float RetractSpd => 0.35f;
        public override float MaxRetractSpd => 12f;
        public override float flailHeadRotationRetract => 180;
        public override float flailHeadRotation => 90;


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void SpawnEffect()
        {
            centerPos = Projectile.Center;
        }
        public override void Dusts()
        {
            int dustRate = 3;

            if (Main.rand.NextBool(dustRate))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity *= 0.25f;
                Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                Main.dust[dust].noGravity = true;
            }
            rotation -= (float)Math.PI / 20f;

        }
        public override void PostDraw(Color lightColor)
        {
            SpriteEffects dir = SpriteEffects.None;

            Vector2 vector161 = centerPos + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition - new Vector2(16, 16);
            Texture2D value180 = TextureAssets.Projectile[578].Value;
            Color color155 = Projectile.GetAlpha(lightColor);
            Vector2 origin21 = new Vector2(value180.Width, value180.Height) / 2f;
            float num314 = rotation;
            Vector2 vector162 = Vector2.One * Projectile.scale;
            Rectangle? sourceRectangle5 = null;

            Color color158 = color155 * 0.8f;
            color158.A /= 2;
            Color color159 = Color.Lerp(color155, Color.Black, 0.5f);
            color159.A = color155.A;
            float num317 = 0.95f + (rotation * 0.75f).ToRotationVector2().Y * 0.1f;
            color159 *= num317;
            float scale22 = 0.6f + Projectile.scale * 0.6f * num317;
            Texture2D value183 = TextureAssets.Extra[50].Value;
            bool flag36 = true;
            Vector2 origin24 = value183.Size() / 2f;
            Main.EntitySpriteDraw(value183, vector161, null, color159, 0f - num314 + 0.35f, origin24, scale22, dir ^ SpriteEffects.FlipHorizontally);
            Main.EntitySpriteDraw(value183, vector161, null, color155, 0f - num314, origin24, Projectile.scale, dir ^ SpriteEffects.FlipHorizontally);
            if (flag36)
            {
                Main.EntitySpriteDraw(value180, vector161, null, color158, (0f - num314) * 0.7f, origin21, Projectile.scale, dir ^ SpriteEffects.FlipHorizontally);
            }
            Main.EntitySpriteDraw(value183, vector161, null, color155 * 0.8f, num314 * 0.5f, origin24, Projectile.scale * 0.9f, dir);
        }
        public override void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {

            Vector2 mouse = new Vector2((float)Main.mouseX + Main.screenPosition.X, (float)Main.mouseY + Main.screenPosition.Y);
            float centerSpd = (Vector2.Distance(centerPos, mouse) - spinDistance * 3f / 2f) / 60;
            centerPos += (mouse - centerPos).SafeNormalize(Vector2.One) * centerSpd;

            offsetCenter = centerPos - player.Center;
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
        }
    }
}