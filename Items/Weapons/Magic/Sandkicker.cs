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

    public class Sandkicker : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Starkicker"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots stardust");
		}

		public override void SetDefaults()
		{
			Item.damage = 14;
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
			Item.shoot = Mod.Find<ModProjectile>("SandkickerProj").Type;
			Item.shootSpeed = 16f;
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
            recipe.AddIngredient(Mod.Find<ModItem>("Starkicker").Type);
            recipe.AddIngredient(ItemID.FossilOre, 12);
            recipe.AddIngredient(ItemID.SandBlock, 66);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SandkickerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stardust"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }


        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 8;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 16;
            height = 16;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.usesLocalNPCImmunity = false;
            if (Vector2.Distance(Vector2.Zero, Projectile.velocity) > 2)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 2;
            }
        }

        public override void AI()
        {
            Projectile.velocity *= new Vector2(0.98f, 0.98f);

            for (var i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}