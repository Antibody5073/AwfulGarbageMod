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
    public class LeeNunchucks : ModItem
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
            Item.damage = 84; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<LeeNunchucksProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 7; // The color of the name of your item
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
            proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation()) + 180;
            Main.projectile[proj].originalDamage = Item.damage;
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class LeeNunchucksProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/LeeNunchucksProjectileChain";
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 75;
        public override float SpinSpd => 1.6f;
        public override float SpinDistanceIncrease => 0.5f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.5f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 35;
        public override float RetractSpd => 0.3f;
        public override float MaxRetractSpd => 15f;
        public override void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {

            Vector2 mouse = new Vector2((float)Main.mouseX + Main.screenPosition.X - player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - player.Center.Y);
            if (player.direction == 1 && mouse.X < 0) { mouse.X = 0; }
            if (player.direction == -1 && mouse.X > 0) { mouse.X = 0; }
            offsetCenter = mouse.SafeNormalize(Vector2.Zero) * Math.Clamp(mouse.Length() - 1, 0, 300);
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
        }


        public override void Dusts()
        {
            Player player = Main.player[Projectile.owner];
            int i = 0;
            while (i < offsetCenter.Length())
            {
                i += 4;
                if (Main.rand.NextBool(20))
                {
                    int dust = Dust.NewDust(player.Center - new Vector2(2, 2) + offsetCenter.SafeNormalize(Vector2.Zero) * i, 0, 0, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
    public class LeeNunchucksProj2 : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/LeeNunchucksProjectileChain";
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 75;
        public override float SpinSpd => 1.6f;
        public override float SpinDistanceIncrease => 0.5f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.5f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 35;
        public override float RetractSpd => 0.3f;
        public override float MaxRetractSpd => 15f;
        public override void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {

            Vector2 mouse = new Vector2((float)Main.mouseX + Main.screenPosition.X - player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - player.Center.Y);
            offsetCenter = -mouse.SafeNormalize(Vector2.Zero) * Math.Clamp(mouse.Length() - 1, 0, 300) * 0.66f;
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
        }


        public override void Dusts()
        {
            Player player = Main.player[Projectile.owner];
            int i = 0;
            while (i < offsetCenter.Length())
            {
                i += 4;
                if (Main.rand.NextBool(20))
                {
                    int dust = Dust.NewDust(player.Center - new Vector2(2, 2) + offsetCenter.SafeNormalize(Vector2.Zero) * i, 0, 0, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}