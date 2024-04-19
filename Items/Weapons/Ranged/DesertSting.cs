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

    public class DesertSting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arachnid's String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Periodically shoots arrows in 8 directions");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 7;
			Item.noMelee = true;
			Item.useAnimation = 7;
			Item.useStyle = 5;
			Item.knockBack = 2.5f;
			Item.value = 10000;
			Item.rare = 5;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(4)), type, damage, knockback, player.whoAmI);

            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			position += Main.rand.NextVector2Circular(6, 6);
			if (counter % 8 == 0)
			{
				type = ProjectileID.VenomArrow;
			}
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextBool(3 ,4);
        }
        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(8, 0);
			return offset;
        }
        public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient<CactusBow>()
                .AddIngredient<DesertScale>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}