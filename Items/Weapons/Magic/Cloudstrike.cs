using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class Cloudstrike : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 23;
			Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = 5;
			Item.knockBack = 3;
			Item.value = 15000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("CloudstrikeProj").Type;
			Item.shootSpeed = 15f;
			Item.noMelee = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DemoniteBar, 11);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.Feather, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimtaneBar, 11);
            recipe2.AddIngredient(ItemID.Cloud, 20);
            recipe2.AddIngredient(ItemID.Feather, 4);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
		}
	}

    public class CloudstrikeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wind"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        Vector2 origVel;
        float spdMult;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            origVel = Projectile.velocity;
            spdMult = 1;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.98f);
            spdMult = 0.1f;
        }

        public override void AI()
        {
            Projectile.aiStyle = -1;
            int dust = Dust.NewDust(Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), 1, 1, DustID.Cloud, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

            spdMult += (1 - spdMult) / 36;
            Projectile.velocity = new Vector2(spdMult * Projectile.ai[0], spdMult * Projectile.ai[1]);

        }
    }
}