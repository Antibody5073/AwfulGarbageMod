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

    public class OrganicGreatsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Organic Greatsword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots leaves every other swing");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<LeafBladeProj>();
			Item.shootSpeed = 13f;
			Item.crit = 0;
            Item.scale = 1.15f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			if (counter % 2 == 1)
			{
				for (var i = -1; i < 2; i++)
				{
					int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(6*i)), type, (int)(damage * 0.5f), knockback / 2, player.whoAmI);

                }
            }
			return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("LeafBlade").Type);
            recipe.AddIngredient(ItemID.JungleSpores, 20);
            recipe.AddIngredient(ItemID.Stinger, 15);
            recipe.AddIngredient(ItemID.Vine, 8);
            recipe.AddIngredient(ItemID.CorruptSeeds, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("LeafBlade").Type);
            recipe2.AddIngredient(ItemID.JungleSpores, 20);
            recipe2.AddIngredient(ItemID.Stinger, 15);
            recipe2.AddIngredient(ItemID.Vine, 8);
            recipe2.AddIngredient(ItemID.CrimsonSeeds, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}