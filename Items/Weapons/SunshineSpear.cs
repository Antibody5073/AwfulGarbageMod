using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class SunshineSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sunshine Spear"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Striking an enemy summons sunlight from above");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Spear);
			Item.damage = 8;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.knockBack = 5;
			Item.value = 10000;
			Item.rare = 1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("SunshineSpearProj").Type;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IronBar, 8);
            recipe.AddIngredient(ItemID.Daybloom, 3);
			recipe.AddIngredient(ItemID.Torch, 50);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.LeadBar, 8);
            recipe2.AddIngredient(ItemID.Daybloom, 3);
            recipe2.AddIngredient(ItemID.Torch, 50);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}