using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class Backbone : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Backbone"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Launches bone shards upwards on impact \n\"I've got a strong backbone, you know?\"");
		}

		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = 1;
			Item.knockBack = 5.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 23;
            Item.scale = 1.1f;
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			for (var i = 0; i < Main.rand.Next(4, 6); i++)
			{
				Vector2 ProjVel = new Vector2(0, Main.rand.NextFloat(-7, -4)).RotatedByRandom(MathHelper.ToRadians(13));
                Projectile.NewProjectile(player.GetSource_OnHit(target), target.Top + ProjVel * 2, ProjVel, Mod.Find<ModProjectile>("BackboneProj").Type, damageDone * 3 / 4, hit.Knockback, player.whoAmI);
			}
		}
	}

    public class BackboneProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bone Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.BoneTorch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
        }
    }
}