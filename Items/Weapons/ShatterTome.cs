using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class ShatterTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shatter Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Shoots a shattering icicle \nShards have armor piercing capabilities");
		}

		public override void SetDefaults()
		{
			Item.damage = 19;
			Item.mana = 4;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("ShatterTomeProj").Type;
			Item.shootSpeed = 5f;
			Item.noMelee = true;
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FrostShard").Type, 16);
            recipe.AddIngredient(ItemID.BorealWood, 30);
			recipe.AddIngredient(ItemID.IceBlock, 30);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}