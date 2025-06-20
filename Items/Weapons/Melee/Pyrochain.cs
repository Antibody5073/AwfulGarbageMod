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
    public class Pyrochain : ModItem
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
            Item.knockBack = 5.5f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
            Item.width = 32; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.damage = 118; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<PyrochainProj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 4; // The color of the name of your item
            Item.value = Item.sellPrice(gold: 3); // Sells for 1 gold 50 silver
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
                .AddIngredient<CandesciteBar>(34)
                .AddIngredient<Pyrogem>(23)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class PyrochainProj : BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Melee/PyrochainProjectileChain"; 
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 180;
        public override float SpinSpd => 0.7f;
        public override float SpinDistanceIncrease => 0.1f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.5f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 65;
        public override float RetractSpd => 0.2f;
        public override float MaxRetractSpd => 8f;

        int linkedProjectile;

        public override void Dusts()
        {
            int dustRate = 4;

            if (Main.rand.NextBool(dustRate))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, default(Color), 1.3f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.OnFire3, 240);
        }

        public override void SpinEffect(Player player, float timer, float length, float maxLength)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PyrochainProjHitbox>()] < 1)
            {
                linkedProjectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PyrochainProjHitbox>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack / 3, player.whoAmI);
                Main.projectile[linkedProjectile].localNPCHitCooldown = Projectile.localNPCHitCooldown;
            }
            else
            {
                Main.projectile[linkedProjectile].timeLeft = 8;
                Main.projectile[linkedProjectile].Center = Projectile.Center;
            }
        }
    }

    public class PyrochainProjHitbox : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 8;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.noEnchantmentVisuals = true;

        }
        int hitCounter = 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint6 = 0f;
            Vector2 vector3 = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(-1.5707963705062866) * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Main.player[Projectile.owner].Center, 16f * Projectile.scale, ref collisionPoint6))
            {
                bool lineOfSight = Collision.CanHitLine(Main.player[Projectile.owner].Center, Projectile.width, Projectile.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height);
                if (lineOfSight)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}