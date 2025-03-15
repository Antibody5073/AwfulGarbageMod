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

namespace AwfulGarbageMod.Items.Weapons.Melee
{

	public class TriElement : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Organic Greatsword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots leaves every other swing");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = 1;
			Item.knockBack = 4;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<LeafBladeProj>();
			Item.shootSpeed = 1f;
			Item.crit = 0;
			Item.scale = 1.05f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			counter++;
			if (counter % 3 == 0)
			{
				for (var i = -1; i < 2; i++)
				{
					int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(6 * i)) * 13, ModContent.ProjectileType<LeafBladeProj>(), (int)(damage * 0.5f), knockback, player.whoAmI);

				}
			}
			else if (counter % 3 == 1)
			{
				int proj = Projectile.NewProjectile(source, position, velocity * 15, Mod.Find<ModProjectile>("WaterStreamMelee").Type, (int)(damage * 0.8f), knockback, player.whoAmI);
			}
			else
			{
				int proj = Projectile.NewProjectile(source, position, velocity * 8, Mod.Find<ModProjectile>("HellslashProj").Type, (int)(damage * 0.7f), knockback, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod.Find<ModItem>("OrganicGreatsword").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("TidesTurn").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Hellslash").Type);

            recipe.AddIngredient(ItemID.Bone, 35);
			recipe.AddIngredient(ItemID.JungleSpores, 12);
            recipe.AddIngredient(ItemID.HellstoneBar, 8);

            recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}