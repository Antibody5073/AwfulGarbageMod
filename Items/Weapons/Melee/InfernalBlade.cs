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
using Terraria.Audio;
using static Terraria.ModLoader.PlayerDrawLayer;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class InfernalBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Infernal Blade"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots an exploding fireball every other swing");
		}

		public int counter;

		public override void SetDefaults()
		{
            Item.damage = 20;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = 1;
			Item.knockBack = 5;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("LeafBladeProj").Type;
			Item.shootSpeed = 8f;
			Item.crit = 0;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			counter++;

            if (counter % 2 == 1)
            {
                int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("InfernalBladeProj").Type, (int)(damage * 0.85f), knockback / 3, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Fireblossom, 3)
                .AddIngredient<CandesciteOre>(50)
                .AddIngredient(ItemID.SilverBroadsword)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Fireblossom, 3)
                .AddIngredient<CandesciteOre>(50)
                .AddIngredient(ItemID.TungstenBroadsword)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class InfernalBladeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

            for (var i = 0; i < 6; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(60 * i)), Mod.Find<ModProjectile>("InfernalBladeExplosion").Type, Projectile.damage / 3 * 2, Projectile.knockBack / 2, Projectile.owner);

            }
        }
            

        public override void AI()
        {
            Projectile.aiStyle = 0;
            int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 2f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

        }
    }
    public class InfernalBladeExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override void AI()
        {
            Projectile.aiStyle = 0;
            int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

        }
    }
}