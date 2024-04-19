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
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Weapons.Melee
{
    public class FrigidiumFlails : ModItem
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
            Item.knockBack = 2.5f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
            Item.width = 32; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.damage = 30; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<FrigidiumFlailsProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 2; // The color of the name of your item
            Item.value = Item.sellPrice(gold: 3); // Sells for 1 gold 50 silver
            Item.DamageType = ModContent.GetInstance<FlailDamageClass>();
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: -12);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation()) - 120;
            proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation());
            proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: 12);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation()) + 120;

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
                .AddIngredient<Icebreaker>()
                .AddIngredient<FrigidiumBar>(24)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FrigidiumFlailsProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/FrigidiumFlailsProjectileChain";
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 180;
        public override float SpinSpd => 0.85f;
        public override float SpinDistanceIncrease => 0.075f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.5f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 70;
        public override float RetractSpd => 0.15f;
        public override float MaxRetractSpd => 12f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity = new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(20));
                Main.dust[dust].scale = 1;
                Main.dust[dust].noGravity = true;
            }
            target.AddBuff(BuffID.Frostburn, 120);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
            offsetFromPlayer.Y /= 4.5f;
            Vector2 mouse = new Vector2((float)Main.mouseX + Main.screenPosition.X - player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - player.Center.Y);
            offsetFromPlayer = offsetFromPlayer.RotatedBy(mouse.ToRotation() + MathHelper.ToRadians(Projectile.ai[2]));

        }
    }
}