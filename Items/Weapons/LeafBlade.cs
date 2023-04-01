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

namespace AwfulGarbageMod.Items.Weapons
{

    public class LeafBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Leaf Blade"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Shoots leaves every other swing");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = 1;
			Item.knockBack = 3;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("LeafBladeProj").Type;
			Item.shootSpeed = 8f;
			Item.crit = 0;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			if (counter % 2 == 1)
			{
				for (var i = -1; i < 2; i++)
				{
					int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, velocity.RotatedBy(MathHelper.ToRadians(6*i)), Mod.Find<ModProjectile>("LeafBladeProj").Type, (int)(damage * 0.4f), knockback, player.whoAmI);
					Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Melee) + (int)player.GetCritChance(DamageClass.Generic) + 4;

                }
            }
			return false;
        }


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Coral, 6);
			recipe.AddIngredient(ItemID.Seashell, 3);
            recipe.AddIngredient(ItemID.Starfish, 3);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}