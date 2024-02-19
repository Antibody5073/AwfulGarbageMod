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

namespace AwfulGarbageMod.Items.Weapons.Melee
{
    public class Cirrus : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cirrus");
            // Tooltip.SetDefault("Strike enemies to gain damage and yoyo speed, up to double");
           

            // These are all related to gamepad controls and don't seem to affect anything else
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24; // The width of the item's hitbox.
            Item.height = 24; // The height of the item's hitbox.

            Item.useStyle = ItemUseStyleID.Shoot; // The way the item is used (e.g. swinging, throwing, etc.)
            Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
            Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
            Item.noMelee = true; // This makes it so the item doesn't do damage to enemies (the projectile does that).
            Item.noUseGraphic = true; // Makes the item invisible while using it (the projectile is the visible part).
            Item.UseSound = SoundID.Item1; // The sound that will play when the item is used.

            Item.damage = 14; // The amount of damage the item does to an enemy or player.
            Item.DamageType = DamageClass.MeleeNoSpeed; // The type of damage the weapon does. MeleeNoSpeed means the item will not scale with attack speed.
            Item.knockBack = 3.5f; // The amount of knockback the item inflicts.
            Item.crit = 0; // The percent chance for the weapon to deal a critical strike. Defaults to 4.
            Item.channel = true; // Set to true for items that require the attack button to be held out (e.g. yoyos and magic missile weapons)
            Item.rare = 2; // The item's rarity. This changes the color of the item's name.
            Item.value = Item.buyPrice(gold: 1); // The amount of money that the item is can be bought for.

            Item.shoot = ModContent.ProjectileType<CirrusProj>(); // Which projectile this item will shoot. We set this to our corresponding projectile.
            Item.shootSpeed = 16f; // The velocity of the shot projectile.			
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DemoniteBar, 9);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(Mod.Find<ModItem>("BlingString").Type);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimtaneBar, 9);
            recipe2.AddIngredient(ItemID.Cloud, 20);
            recipe2.AddIngredient(Mod.Find<ModItem>("BlingString").Type);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }

    public class CirrusProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 15f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 340f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 10f;
        }

        bool thing = false;
        float startingDmg;
        float dmg;

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of the projectile's hitbox.
            Projectile.height = 16; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 10f;

        }

        // notes for aiStyle 99: 
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.
        /*
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                hitInfo.Damage = (int)dmg;
                
                if (hitInfo.Crit)
                {
                    hitInfo.Damage = (int)(hitInfo.Damage * (2 + player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg));
                    hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit;
                }
            };
        }
        */
        

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.Damage = (int)dmg;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] += 0.5f;
            if (ProjectileID.Sets.YoyosTopSpeed[Projectile.type] > 20)
            {
                ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
            }
            dmg += startingDmg / 20;
            if (dmg > startingDmg * 2)
            {
                dmg = startingDmg * 2;
            }

            Vector2 direction = target.Center - Projectile.Center;
            direction.Normalize();

            for (var i = 0; i < 8; i++)
            {
                Vector2 vel = (direction * Main.rand.NextFloat(3, 5)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30) + 180));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Cloud, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
            Projectile.damage = (int)startingDmg;
        }

        public override bool PreAI()
        {
            if (!thing)
            {
                startingDmg = Projectile.damage;
                dmg = startingDmg;
                thing = true;
            }

            Projectile.damage = (int)dmg;

            return true;
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Cloud);
            }
        }
    }
}