using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace AwfulGarbageMod.Projectiles
{
    public class GeodeMeteor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 2;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
        }


        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 0, 0, DustID.Meteorite, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

            if (Main.rand.NextBool(3))
            {
                int dust2 = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 0, 0, DustID.Torch);
                Main.dust[dust2].scale = 1.25f;
                Main.dust[dust2].noGravity = true;
            }
        }
    }
}