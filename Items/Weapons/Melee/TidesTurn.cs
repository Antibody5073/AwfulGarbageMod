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

    public class TidesTurn : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Tidal Slicer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots water every other swing \nSmall chance to shoot three");
        }

        public int counter;

		

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = 1;
			Item.knockBack = 5;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("WaterStreamMelee").Type;
			Item.shootSpeed = 15f;
			Item.crit = 0;
			Item.scale = 1.2f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			if (counter % 2 == 1)
			{
                    int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("WaterStreamMelee").Type, damage, knockback / 2, player.whoAmI);
            }
			return false;
        }


		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("TidalSlicer").Type);
            recipe.AddIngredient(ItemID.ShadowScale, 18);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("TidalSlicer").Type);
            recipe2.AddIngredient(ItemID.TissueSample, 18);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}