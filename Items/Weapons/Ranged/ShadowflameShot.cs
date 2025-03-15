using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
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

    public class ShadowflameShot : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets travel instantly");
		}

        int counter;

		public override void SetDefaults()
		{
            Item.damage = 66;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 20);
            Item.rare = 2;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 4;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 9f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].GetGlobalProjectile<ProjectileWeaponEffect>().Shadowflame = true;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(8, 0);
			return offset;
        }
        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient<Shadowfuel>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}