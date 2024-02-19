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
using Terraria.GameContent.Items;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class TidalSlicer : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Tidal Slicer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots water every other swing \nSmall chance to shoot three");
        }

        public int counter;

		

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = 1;
			Item.knockBack = 4;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("WaterStreamMelee").Type;
			Item.shootSpeed = 12f;
			Item.crit = 0;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			if (counter % 2 == 1)
			{
				if (Main.rand.Next(6) < 1)
				{
					for (var i = -1; i < 2; i++)
					{
						int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(10*i)), Mod.Find<ModProjectile>("WaterStreamMelee").Type, damage, knockback / 2, player.whoAmI);

                    }
                } 
				else
				{
                    int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("WaterStreamMelee").Type, damage, knockback, player.whoAmI);
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