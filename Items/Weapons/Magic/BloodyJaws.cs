using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class BloodyJaws : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Great Bite"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Creates shark teeth near the player");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.mana = 9;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = 5;
			Item.knockBack = 3f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("BloodyJawsProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<GreatBite>()
               .AddIngredient(ItemID.TissueSample, 18)
               .AddIngredient(ItemID.CrimtaneBar, 15)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }

    public class BloodyJawsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tooth"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 2; i++)
            {
                Vector2 pos = target.Center + new Vector2(0, 300).RotatedByRandom(MathHelper.Pi);
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Normalize(target.Center - pos) * 12f, Mod.Find<ModProjectile>("WormBiteProj").Type, (int)(damageDone * 0.75), hit.Knockback, Projectile.owner);
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = 90;
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Water, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].noGravity = true;
        }
    }
}