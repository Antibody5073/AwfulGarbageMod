using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class DarkStar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Star"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Only one can be out at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 10;
			Item.useStyle = 1;
			Item.knockBack = 2.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("DarkStarProj").Type;
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.IronBar, 8);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(ItemID.Deathweed, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.LeadBar, 8);
            recipe2.AddIngredient(ItemID.FallenStar, 5);
            recipe2.AddIngredient(ItemID.Deathweed, 3);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}