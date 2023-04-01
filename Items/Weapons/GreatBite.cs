using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class GreatBite : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Great Bite"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Creates shark teeth near the player");
		}

		public override void SetDefaults()
		{
			Item.damage = 19;
			Item.mana = 7;
			Item.DamageType = DamageClass.Magic;
			Item.width = 33;
			Item.height = 30;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = 3;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("GreatBiteProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
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
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), vector28, v4, Item.shoot, damage, knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                }
            }
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Coral, 3);
            recipe.AddIngredient(ItemID.Seashell, 6);
            recipe.AddIngredient(ItemID.Starfish, 3);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}