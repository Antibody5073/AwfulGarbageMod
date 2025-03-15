using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class HeavenlyBurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavenly Burst"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Hold down the attack button to charge up the bow, increasing arrows shot to a max of 5 charges(8 arrows)");
        }

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.noMelee = true;
            Item.useAnimation = 30;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = 15000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 14f;
            Item.crit = 0;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("HeavenlyBurstDummy").Type, damage, knockback, player.whoAmI, Item.shootSpeed, type);
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
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.GoldBow);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe2.AddIngredient(ItemID.Cloud, 20);
            recipe2.AddIngredient(ItemID.GoldBow);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.DemoniteBar, 8);
            recipe3.AddIngredient(ItemID.Cloud, 20);
            recipe3.AddIngredient(ItemID.PlatinumBow);
            recipe3.AddTile(TileID.Anvils);
            recipe3.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe4.AddIngredient(ItemID.Cloud, 20);
            recipe4.AddIngredient(ItemID.PlatinumBow);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
    }

    public class HeavenlyBurstDummy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("a"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
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
                if (chargeLevel < 5)
                {
                    chargeLevel++;
                    if (chargeLevel == 5)
                    {
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
                    }
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
            }
            float dir = (-3 * (arrowsShot - 1)) / 2;
            for (var i = 0; i < arrowsShot; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(dir) + Projectile.rotation), (int)Projectile.ai[1], Projectile.damage, Projectile.knockBack, Projectile.owner);
                dir += 3;

            }
        }
    }
}