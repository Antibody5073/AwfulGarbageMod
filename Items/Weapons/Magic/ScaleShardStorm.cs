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

    public class ScaleShardStorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shell Shard Storm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots many shell shards \nShards have armor piercing capabilities");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 8;
			Item.mana = 3;
			Item.DamageType = DamageClass.Magic;
			Item.width = 33;
			Item.height = 30;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("ShellProj").Type;
			Item.shootSpeed = 14f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            {
                for (var i = 0; i < Main.rand.Next(1, 5); i++)
                {
					int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)) * Main.rand.Next(80, 120) * 0.01f, Mod.Find<ModProjectile>("ScaleShardStormProj").Type, damage, knockback, player.whoAmI);

                }
            }
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("ShellShardStorm").Type);
            recipe.AddIngredient(ItemID.ShadowScale, 18);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}

    public class ScaleShardStormProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shell Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.friendly = true;
            Projectile.penetrate = Main.rand.Next(3) + 1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 24;
        }
    }
}