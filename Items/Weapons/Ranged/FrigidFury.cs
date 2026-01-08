using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class FrigidFury : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots an inaccurate, short-ranged ice shard along with an arrow");
		}
		float vel;
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 7;
			Item.noMelee = true;
			Item.useAnimation = 35;
			Item.useStyle = 5;
			Item.knockBack = 0.5f;
			Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item34;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Flames;
			Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 9f;
			Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            vel += 0.15f;

            int proj = Projectile.NewProjectile(source, position, velocity * vel, type, damage, knockback, player.whoAmI, 1);
            

            return false;
        }
        public override void UseAnimation(Player player)
        {
			vel = 0.25f;
        }

        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(0, 0);
			return offset;
        }
	}
}