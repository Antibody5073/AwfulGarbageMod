using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class LightningBurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning Burst"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Hold down the attack button to charge up the bow, increasing arrows shot to a max of 6 charges(13 arrows)\nAt maximum charge, the center three arrows are converted into Electric arrows that fly fast and pierce");
        }

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.noMelee = true;
            Item.useAnimation = 26;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = 15000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 11f;
            Item.crit = 0;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("LightningBurstDummy").Type, damage, knockback, player.whoAmI, Item.shootSpeed, type);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("HeavenlyBurst").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 6);
            recipe.AddIngredient(ItemID.RainCloud, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class LightningBurstDummy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("a"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
        }
        int ChargeTime = 0;
        int chargeLevel = 0;

        public override bool PreAI()
        {
            Player owner = Main.player[Projectile.owner];

            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            Projectile.position = owner.Center;
            if (!owner.channel)
            {
                return true; // Let the vanilla whip AI run.
            }
            Projectile.timeLeft = 3;
            Projectile.rotation = (Main.MouseWorld - Projectile.position).ToRotation();

            if (ChargeTime % owner.HeldItem.useTime == 0) // 1 segment per 12 ticks of charge.
            {
                if (chargeLevel < 6)
                {
                    chargeLevel++;
                    for (var i = 0; i < 8; i++)
                    {
                        float xv = Main.rand.NextFloat(-3, 3);
                        float yv = Main.rand.NextFloat(-2, -5);
                        int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.MagicMirror, xv, yv, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                    }
                }
            }
            ChargeTime++;
            // Reset the animation and item timer while charging.
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;
            owner.itemRotation = Projectile.rotation;
            if (owner.Center.X < Main.MouseWorld.X)
            {
                owner.direction = (int)MathHelper.ToRadians(90);
            }
            else
            {
                owner.direction = (int)MathHelper.ToRadians(-90);
                owner.itemRotation += (int)MathHelper.ToRadians(180);
            }


            return false; // Prevent the vanilla whip AI from running.
        }

        public override void OnKill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];

            int arrowsShot = 0;
            switch (chargeLevel)
            {
                case 1:
                    arrowsShot = 1;
                    break;
                case 2:
                    arrowsShot = 2;
                    break;
                case 3:
                    arrowsShot = 3;
                    break;
                case 4:
                    arrowsShot = 5;
                    break;
                case 5:
                    arrowsShot = 8;
                    break;
                case 6:
                    arrowsShot = 13;
                    if (owner.GetModPlayer<GlobalPlayer>().StormHelmetBonus)
                    {
                        for (int j = 0; j < Main.rand.Next(3, 5); j++)
                        {
                            Vector2 pos = new Vector2(Main.MouseWorld.X + Main.rand.NextFloat(-200, 200), owner.Center.Y - 800);
                            Vector2 toMouse = Vector2.Normalize(Main.MouseWorld - pos);
                            float spd = Projectile.ai[0];
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, toMouse * spd * Main.rand.NextFloat(0.8f, 1.2f), Mod.Find<ModProjectile>("LightningBurstProj").Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Main.projectile[proj].tileCollide = false;
                        }
                        owner.HeldItem.GetGlobalItem<StormBonus>().shotNumber = 0;
                    }
                    break;
            }
            float dir = (-2.5f * (arrowsShot - 1)) / 2;
            for (var i = 0; i < arrowsShot; i++)
            {
                if (chargeLevel == 6 && (i == 5 || i == 6 || i == 7))
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(dir) + Projectile.rotation), Mod.Find<ModProjectile>("LightningBurstProj").Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].CritChance = Projectile.CritChance;
                    

                }
                else
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(dir) + Projectile.rotation), (int)Projectile.ai[1], Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[proj].CritChance = Projectile.CritChance;
                }

                dir += 2.5f;

            }
        }
    }

    public class LightningBurstProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electric Arrow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.arrow = true;

        }

        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            DrawOffsetX = -3;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90);

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