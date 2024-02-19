using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class OrganicGreatsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Organic Greatsword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots leaves every other swing");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.damage = 33;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("OrganicGreatswordProj").Type;
			Item.shootSpeed = 13f;
			Item.crit = 0;
            Item.scale = 1.15f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;
			if (counter % 2 == 1)
			{
				for (var i = -1; i < 2; i++)
				{
					int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(6*i)), Mod.Find<ModProjectile>("OrganicGreatswordProj").Type, (int)(damage * 0.5f), knockback / 2, player.whoAmI);

                }
            }
			return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("LeafBlade").Type);
            recipe.AddIngredient(ItemID.JungleSpores, 18);
            recipe.AddIngredient(ItemID.Stinger, 12);
            recipe.AddIngredient(ItemID.Vine, 10);
            recipe.AddIngredient(ItemID.CorruptSeeds, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("LeafBlade").Type);
            recipe2.AddIngredient(ItemID.JungleSpores, 18);
            recipe2.AddIngredient(ItemID.Stinger, 12);
            recipe2.AddIngredient(ItemID.Vine, 10);
            recipe2.AddIngredient(ItemID.CrimsonSeeds, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }

    public class OrganicGreatswordProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leaf"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 7;
            Projectile.height = 11;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.aiStyle = 0;
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Grass, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

        }
    }
}