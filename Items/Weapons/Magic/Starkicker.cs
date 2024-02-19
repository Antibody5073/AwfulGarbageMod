using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class Starkicker : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Starkicker"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots stardust");
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
            Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("StarkickerProj").Type;
			Item.shootSpeed = 9f;
			Item.noMelee = true;
		}

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(-4, 0);
            return offset;
        }
        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 8);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddIngredient(ItemID.VilePowder, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.FallenStar, 8);
            recipe2.AddIngredient(ItemID.LeadBar, 5);
            recipe2.AddIngredient(ItemID.VilePowder, 15);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.FallenStar, 8);
            recipe3.AddIngredient(ItemID.IronBar, 5);
            recipe3.AddIngredient(ItemID.ViciousPowder, 15);
            recipe3.AddTile(TileID.Anvils);
            recipe3.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.FallenStar, 8);
            recipe4.AddIngredient(ItemID.LeadBar, 5);
            recipe4.AddIngredient(ItemID.ViciousPowder, 15);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
	}

    public class StarkickerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stardust"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 4;
            Projectile.alpha = 255;
        }


        public override void AI()
        {
            Projectile.velocity *= new Vector2(0.97f, 0.97f);

            int dust = Dust.NewDust(Projectile.Center + new Vector2(Main.rand.Next(-18, 19), Main.rand.Next(-18, 19)), 1, 1, DustID.YellowStarDust, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}