using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class BloodVolley : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = 5;
            Item.knockBack = 3;
            Item.value = 15000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = Mod.Find<ModProjectile>("BloodBoltProj").Type;
            Item.shootSpeed = 10f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;
            proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(15)), Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;
            proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-15)), Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(30, 30);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("BloodBolt").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("VeinJuice").Type, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}