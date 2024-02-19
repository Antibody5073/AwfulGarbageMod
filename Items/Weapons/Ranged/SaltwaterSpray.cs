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

    public class SaltwaterSpray : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Oceanic Pistol"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets gain damage and turn into a water stream");
		}

		public override void SetDefaults()
		{
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = 5000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 6f;
            Item.noMelee = true;
            Item.crit = 6;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));
            if (type == ProjectileID.Bullet)
            {
                type = Mod.Find<ModProjectile>("WaterStreamRanged").Type;
                damage = (int)(damage * 1.2f);
                velocity = velocity * new Vector2(1.5f, 1.5f);
            }

        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(0, 0);
			return offset;
        }
        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("OceanicPistol").Type);
            recipe.AddIngredient(ItemID.ShadowScale, 18);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("OceanicPistol").Type);
            recipe2.AddIngredient(ItemID.TissueSample, 18);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}