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

    public class FrostDartgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Oceanic Pistol"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Normal bullets gain damage and turn into a water stream");
        }

        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = 5;
            Item.knockBack = 3f;
            Item.value = 5000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Dart;
            Item.shootSpeed = 16f;
            Item.noMelee = true;
            Item.crit = 8;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(20)) * 0.75f, type, (int)(damage * 0.7f), knockback, player.whoAmI);

            return false;
        }


        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Cryogem>(40)
                //.AddIngredient<FrigidiumBar>(16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}