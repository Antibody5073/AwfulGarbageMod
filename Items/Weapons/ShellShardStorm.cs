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

    public class ShellShardStorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shell Shard Storm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Shoots many shell shards \nShards have armor piercing capabilities");
		}

		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.mana = 2;
			Item.DamageType = DamageClass.Magic;
			Item.width = 33;
			Item.height = 30;
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = 3;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("ShellProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            {
                for (var i = 0; i < Main.rand.Next(1, 4); i++)
                {
					int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, velocity.RotatedByRandom(MathHelper.ToRadians(10)) * Main.rand.Next(80, 120) * 0.01f, Mod.Find<ModProjectile>("ShellProj").Type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;

                }
            }
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Coral, 3);
            recipe.AddIngredient(ItemID.Seashell, 6);
            recipe.AddIngredient(ItemID.Starfish, 3);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}