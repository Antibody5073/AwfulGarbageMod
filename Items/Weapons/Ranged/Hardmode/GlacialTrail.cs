using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Ammo;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged.Hardmode
{

    public class GlacialTrail : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arachnid's String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Periodically shoots arrows in 8 directions");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 55;
			Item.DamageType = ModContent.GetInstance<MageRangedClass>();
			Item.mana = 17;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 35;
			Item.noMelee = true;
			Item.useAnimation = 35;
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
			Projectile proj = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * 6, velocity, type, damage, knockback, player.whoAmI);

			Vector2 offset = Main.rand.NextVector2Circular(96, 96);
			Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * 6 + offset, velocity, ModContent.ProjectileType<GlacialEcho2>(), damage * 2 / 3, knockback, player.whoAmI, proj.whoAmI, offset.X, offset.Y);

			for (int i = 0; i < 12; i++) {

                int dust = Dust.NewDust(position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * 6 + offset - new Vector2(2, 2), 1, 1, DustID.IceTorch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity = new Vector2(0, 2).RotatedBy(MathHelper.TwoPi * i / 12);
                Main.dust[dust].noGravity = true;
            }
		
			proj = Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * -6, velocity, type, damage, knockback, player.whoAmI);

            offset = Main.rand.NextVector2Circular(96, 96);
            Projectile.NewProjectileDirect(source, position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * 6 + offset, velocity, ModContent.ProjectileType<GlacialEcho2>(), damage * 2 / 3, knockback, player.whoAmI, proj.whoAmI, offset.X, offset.Y);

            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(position + velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.One) * -6 + offset - new Vector2(2, 2), 1, 1, DustID.IceTorch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity = new Vector2(0, 2).RotatedBy(MathHelper.TwoPi * i / 12);
                Main.dust[dust].noGravity = true;
            }
            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextBool(1, 5);
        }
        public override Vector2? HoldoutOffset()
        {
			Vector2 offset = new Vector2(8, 0);
			return offset;
        }
        public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient<GlacierBow>()
				.AddIngredient(ItemID.SkyFracture)
                .AddIngredient<Cryogem>(25)
                .AddIngredient(ItemID.FrostCore)
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}