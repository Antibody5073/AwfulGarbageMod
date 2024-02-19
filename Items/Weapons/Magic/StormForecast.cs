using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static Terraria.ModLoader.PlayerDrawLayer;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class StormForecast : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Forecast"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Rapidly shoots erratic lightning bolts\nSignificantly less effective at longer ranges");

        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.mana = 4;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 6;
            Item.useAnimation = 18;
            Item.useStyle = 5;
            Item.knockBack = 1;
            Item.value = 15000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item93;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = Mod.Find<ModProjectile>("StormForecastProj").Type;
            Item.shootSpeed = 24f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(4, -2);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("CloudBlaster").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 6);
            recipe.AddIngredient(ItemID.RainCloud, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class StormForecastProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
        }

        int counter = 0;

        public override void AI()
        {
            counter++;
            if (counter % 3 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
            }

            int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
        }
        
    }
}