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
    public class GrassBasher : ModItem
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
            Item.damage = 23; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<GrassBasherProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 3; // The color of the name of your item
            Item.value = Item.sellPrice(gold: 1); // Sells for 1 gold 50 silver
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
                .AddIngredient(ItemID.Vine, 4)
                .AddIngredient(ItemID.JungleSpores, 18)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GrassBasherProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/GrassBasherProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 95;
        public override float SpinSpd => 1.03f;
        public override float SpinDistanceIncrease => 0.1f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.65f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 65;
        public override float RetractSpd => 0.15f;
        public override float MaxRetractSpd => 12f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Poisoned, 420);
            }
        }
        public override void Dusts()
        {
            int dustRate = 3;

            if (Main.rand.NextBool(dustRate))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity *= 0.2f;
                Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                Main.dust[dust].noGravity = true;
            }
        }
        public override void SpinEffect(Player player, float timer, float length, float maxLength)
        {
            if (length == maxLength && timer % 42 == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.SporeCloud, (int)(Projectile.damage * 0.25f), 0, player.whoAmI);
            }
        }
    }
}