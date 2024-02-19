using AwfulGarbageMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class WormBite : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Great Bite"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Creates shark teeth near the player");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = 5;
			Item.knockBack = 3f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("GreatBiteProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = 0; i < 4; i++)
            {
                {
                    Vector2 pointPoisition = player.Center;
                    float num90 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                    float num101 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                    float f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                    float value18 = 20f;
                    float value19 = 60f;
                    Vector2 vector28 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value18, value19, Main.rand.NextFloat());
                    for (int num78 = 0; num78 < 75; num78++)
                    {
                        vector28 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value18, value19, Main.rand.NextFloat());
                        if (Collision.CanHit(pointPoisition, 0, 0, vector28 + (vector28 - pointPoisition).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                        {
                            break;
                        }
                        f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                    }
                    Vector2 v4 = Main.MouseWorld - vector28;
                    Vector2 vector29 = new Vector2(num90, num101).SafeNormalize(Vector2.UnitY) * Item.shootSpeed;
                    v4 = v4.SafeNormalize(vector29) * Item.shootSpeed;
                    v4 = Vector2.Lerp(v4, vector29, 0.25f);
                    int proj = Projectile.NewProjectile(source, vector28, v4, Item.shoot, damage/2, knockback, player.whoAmI);
                }
            }
            for (var i = 0; i < 2; i++)
            {
                {
                    Vector2 pointPoisition = player.Center;
                    float num90 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                    float num101 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                    float f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                    float value18 = 20f;
                    float value19 = 60f;
                    Vector2 vector28 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value18, value19, Main.rand.NextFloat());
                    for (int num78 = 0; num78 < 75; num78++)
                    {
                        vector28 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value18, value19, Main.rand.NextFloat());
                        if (Collision.CanHit(pointPoisition, 0, 0, vector28 + (vector28 - pointPoisition).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                        {
                            break;
                        }
                        f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
                    }
                    Vector2 v4 = Main.MouseWorld - vector28;
                    Vector2 vector29 = new Vector2(num90, num101).SafeNormalize(Vector2.UnitY) * Item.shootSpeed;
                    v4 = v4.SafeNormalize(vector29) * Item.shootSpeed;
                    v4 = Vector2.Lerp(v4, vector29, 0.25f);
                    int proj = Projectile.NewProjectile(source, vector28, v4, Mod.Find<ModProjectile>("WormBiteProj").Type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
               .AddIngredient<GreatBite>()
               .AddIngredient(ItemID.ShadowScale, 18)
               .AddIngredient(ItemID.DemoniteBar, 15)
               .AddTile(TileID.Anvils)
               .Register();
        }
	}

    public class WormBiteProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tooth"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Water, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].noGravity = true;
        }
    }
}