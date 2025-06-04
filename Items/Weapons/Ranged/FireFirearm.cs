using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
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

    public class FireFirearm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moon Sniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Normal bullets travel instantly");
        }

        int counter;

        public override void SetDefaults()
        {
            Item.damage = 29;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = 5;
            Item.knockBack = 2f;
            Item.value = 5000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 11f;
            Item.noMelee = true;
            Item.crit = 6;
            Item.ArmorPenetration = 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].GetGlobalProjectile<ProjectileWeaponEffect>().FireFirearm = true;
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
                .AddIngredient<HotGlock>()
                .AddIngredient<CandesciteBar>(25)
                .AddIngredient<Pyrogem>(35)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}