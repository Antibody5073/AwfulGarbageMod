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
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Weapons.Melee
{
    public class TheEyeOfTheStorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Eye of the Storm");
            // Tooltip.SetDefault("Zaps nearby enemies with lightning\nStriking enemies with the yoyo itself increases zap range and damage, up to double\nPenetrates some enemy armor");
           

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

            Item.damage = 15; // The amount of damage the item does to an enemy or player.
            Item.DamageType = DamageClass.MeleeNoSpeed; // The type of damage the weapon does. MeleeNoSpeed means the item will not scale with attack speed.
            Item.knockBack = 5.5f; // The amount of knockback the item inflicts.
            Item.crit = 0; // The percent chance for the weapon to deal a critical strike. Defaults to 4.
            Item.channel = true; // Set to true for items that require the attack button to be held out (e.g. yoyos and magic missile weapons)
            Item.rare = 3; // The item's rarity. This changes the color of the item's name.
            Item.value = Item.buyPrice(gold: 1); // The amount of money that the item is can be bought for.

            Item.shoot = ModContent.ProjectileType<TheEyeOfTheStormProj>(); // Which projectile this item will shoot. We set this to our corresponding projectile.
            Item.shootSpeed = 16f; // The velocity of the shot projectile.			
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("Cirrus").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 6);
            recipe.AddIngredient(ItemID.RainCloud, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class TheEyeOfTheStormProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 15f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 350f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
        }

        bool thing = false;
        float startingDmg;
        float dmg;
        float zapRange;
        int counter = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of the projectile's hitbox.
            Projectile.height = 16; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
            Projectile.ArmorPenetration = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
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
                    hitInfo.Damage = (int)(hitInfo.Damage *(2 + player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg));
                    hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit;
                }
            };
        }
        */

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)startingDmg;

            zapRange += 10;
            if (zapRange > 400)
            {
                zapRange = 400;
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
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Electric, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
        }

        
        public override bool PreAI()
        {
            DrawOffsetX = -8;
            DrawOriginOffsetY = -8;

            if (!thing)
            {
                startingDmg = Projectile.damage;
                dmg = startingDmg;
                zapRange = 200;
                thing = true;
            }
            Projectile.damage = (int)dmg;

            return true;
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Electric);
            }

            counter++;
            if (counter % 26 == 13)
            {
                NPC closestNPC = FindClosestNPC(zapRange);
                if (closestNPC != null)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12, Mod.Find<ModProjectile>("TheEyeOfTheStormProjLightning").Type, (int)(dmg * 1.25f), Projectile.knockBack / 5f, Projectile.owner);
                }
            }

            if (counter % 6 == 0)
            {
                for (var i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + new Vector2(zapRange, 0).RotatedBy(MathHelper.ToRadians(360 * i/20)), 0, 0, DustID.RainCloud, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
                for (var i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + new Vector2(zapRange, 0).RotatedBy(MathHelper.ToRadians(360 * i / 20 + 360/40)), 0, 0, DustID.Cloud, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }

            }

            return closestNPC;
        }
    }
    public class TheEyeOfTheStormProjLightning : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 90;
            Projectile.ArmorPenetration = 6;
        }

        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 direction = target.Center - Projectile.Center;
            direction.Normalize();
            for (var i = 0; i < 8; i++)
            {
                Vector2 vel = (direction * Main.rand.NextFloat(3, 5)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30) + 180));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Electric, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
        }

        public override void AI()
        {
            Projectile.aiStyle = -1;
            counter++;
            currentOffset += (newOffset - oldOffset) / 5;
            if (counter == 5)
            {
                oldOffset = newOffset;
                newOffset = Main.rand.NextFloat(-12, 12);
                counter = -1;
            }

            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);

            int dust = Dust.NewDust(Projectile.Center + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * currentOffset, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;


        }
    }
}