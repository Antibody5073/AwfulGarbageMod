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

namespace AwfulGarbageMod.Items.Weapons.Paladin
{
    [ExtendsFromMod("StramClasses")]

    public class SlimyFlail2 : ModItem
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
            Item.damage = 15; // The damage of your flail, this is dynamically adjusted in the projectile code.
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.shoot = ModContent.ProjectileType<SlimyFlail2Proj>(); // The flail projectile
            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
            Item.rare = 1; // The color of the name of your item
            Item.value = Item.sellPrice(silver: 20); // Sells for 1 gold 50 silver
            Item.toPaladinItem();
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].flailProjectile().spinOffset = MathHelper.ToDegrees(velocity.ToRotation());
            Main.projectile[proj].originalDamage = Item.damage;
            Main.projectile[proj].paladinProjectile().smiteChargeOnHit = player.paladin().itemSmiteCharge + Item.paladinItem().prefixSmiteCharge + Item.paladinItem().baseSmiteCharge;
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
                .AddIngredient(ItemID.Wood, 18)
                .AddIngredient(ItemID.Gel, 12)
                .AddIngredient(ItemID.Chain, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class SlimyFlail2Proj : Melee.BaseFlailProjectile
    {
        public override string ChainTexturePath()
        {
            return "AwfulGarbageMod/Items/Weapons/Paladin/SlimyFlail2ProjectileChain";
        }
        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override float MaxSpinDistance => 70;
        public override float SpinSpd => 0.9f;
        public override float SpinDistanceIncrease => 0.3f;
        public override bool MeleeSpdToRange => true;
        public override float MeleeSpdEffectiveness => 0.65f;
        public override bool canHitThroughWalls => false;
        public override int spinHitCooldown => 80;
        public override float RetractSpd => 0.05f;
        public override float MaxRetractSpd => 6f;
        public override float flailHeadRotationRetract => 180;
        public override float flailHeadRotation => 90;
        public override bool dealDamageWhileSmiting => true;
        public override bool paladin => true;


        public override void HitEffect(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[Projectile.owner].paladin().smiting)
            {
                target.AddBuff(BuffID.OnFire3, 240);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[Projectile.owner].paladin().smiting)
            {
                modifiers.FinalDamage *= 0.01f;
            }
        }

        public override void Dusts()
        {
            int dustRate = 3;

            if (Main.player[Projectile.owner].paladin().smiting)
            {
                for (int i = 0; i < 4; i++)
                { }
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity *= 0.25f;
                Main.dust[dust].scale = (float)Main.rand.Next(75, 150) * 0.013f;
                Main.dust[dust].noGravity = true;
            }
            else
            {
                if (Main.rand.NextBool(dustRate))
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].velocity *= 0.25f;
                    Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                    Main.dust[dust].noGravity = true;
                }
            }

        }
    }
}