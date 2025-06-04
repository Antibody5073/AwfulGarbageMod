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
    public class PhantomFlail : ModItem
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
            Item.damage = 136; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<PhantomFlailProj>(); // The flail projectile
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 26)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class PhantomFlailProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/PhantomFlailProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 150;
        public override float SpinSpd => 1.15f;
        public override float SpinDistanceIncrease => 0.4f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.35f;
        public override bool canHitThroughWalls => true;
        public override int spinHitCooldown => 80;
        public override float RetractSpd => 0.4f;
        public override float MaxRetractSpd => 8f;
        public override float flailHeadRotationRetract => 180;
        public override float flailHeadRotation => 90;

        public override void Dusts()
        {
            int dustRate = 3;

            if (Main.rand.NextBool(dustRate))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity *= 0.25f;
                Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                Main.dust[dust].noGravity = true;
            }
        }
        public override void HitEffect(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 6; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0, 8).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("IceSpiritPikeSpiritProj").Type, Projectile.damage / 3, 0, Projectile.owner, 2, 15);
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].penetrate -= 1;
            }
        }
        public override void WidthHeight()
        {
            Projectile.width = 48;
            Projectile.height = 48;
        }

    }
}