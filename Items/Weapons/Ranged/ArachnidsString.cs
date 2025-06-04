using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class ArachnidsString : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arachnid's String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Periodically shoots arrows in 8 directions");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 17;
			Item.noMelee = true;
			Item.useAnimation = 17;
			Item.useStyle = 5;
			Item.knockBack = 2.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 9f;
			Item.crit = 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            counter++;
            if (counter % 3 == 1)
            {
                for (var i = 0; i < 8; i++)
                {
                    int proj2 = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(45 * i + 22.5f)) / 1.5f, type, damage / 2, knockback / 2, player.whoAmI);

                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(8, 0);
			return offset;
        }
        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod.Find<ModItem>("SpiderLeg").Type, 16);
            recipe.AddIngredient(ItemID.Cobweb, 75);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}