using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class HydroString : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hydro String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Shoots two streams of water along with an arrow");
		}

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 26;
			Item.noMelee = true;
			Item.useAnimation = 26;
			Item.useStyle = 5;
			Item.knockBack = 2.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 6f;
            Item.crit = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = -1; i < 2; i++)
            {
                if (i == 0)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, velocity * 1.5f, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                }
                else
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, velocity.RotatedBy(MathHelper.ToRadians(10 * i)), Mod.Find<ModProjectile>("WaterStreamRanged").Type, damage / 3 * 2, knockback / 2, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;

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
            recipe.AddIngredient(ItemID.Coral, 3);
            recipe.AddIngredient(ItemID.Seashell, 3);
            recipe.AddIngredient(ItemID.Starfish, 6);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}