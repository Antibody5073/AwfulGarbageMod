using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class GelatinousGlock : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gelatinous Glock"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Normal bullets turn into splattering slime");
		}

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = 5;
			Item.knockBack = 0.2f;
			Item.value = 5000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.crit = 23;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 7f;
			Item.noMelee = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
            {
                type = Mod.Find<ModProjectile>("GlockSlime").Type;
				damage = damage / 4 * 3;
            }

        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Gel, 30);
            recipe.AddIngredient(ItemID.GoldBar, 12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register(); 
			Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Gel, 30);
            recipe2.AddIngredient(ItemID.PlatinumBar, 12);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
	}
}