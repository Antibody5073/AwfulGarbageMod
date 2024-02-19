using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace AwfulGarbageMod.Projectiles
{

    public class VilethornHostileBase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Main.netMode != 2 && Projectile.ai[1] == 0f && Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundStyle legacySoundStyle = SoundID.Item8;
                SoundEngine.PlaySound(in legacySoundStyle, Projectile.Center);
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                if (Projectile.type == ModContent.ProjectileType<VilethornHostileBase>() && Main.myPlayer == Projectile.owner)
                {
                    int num755 = Projectile.type;
                    if (Projectile.ai[1] >= 10f)
                    {
                        num755 = ModContent.ProjectileType<VilethornHostileTip>();
                    }
                    int num766 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2), Projectile.velocity.X, Projectile.velocity.Y, num755, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[num766].damage = Projectile.damage;
                    Main.projectile[num766].ai[1] = Projectile.ai[1] + 1f;
                    NetMessage.SendData(27, -1, -1, null, num766);
                }
                return;
            }
            if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int num877 = 0; num877 < 3; num877++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default(Color), 1.2f);
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 170, default(Color), 1.1f);

            }

            Projectile.alpha += 2;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
    public class VilethornHostileTip : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Main.netMode != 2 && Projectile.ai[1] == 0f && Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundStyle legacySoundStyle = SoundID.Item8;
               
                SoundEngine.PlaySound(in legacySoundStyle, Projectile.Center);
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                if (Projectile.type == ModContent.ProjectileType<VilethornHostileBase>() && Main.myPlayer == Projectile.owner)
                {
                    int num755 = Projectile.type;
                    if (Projectile.ai[1] >= 10f)
                    {
                        num755 = ModContent.ProjectileType<VilethornHostileTip>();
                    }
                    int num766 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2), Projectile.velocity.X, Projectile.velocity.Y, num755, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[num766].damage = Projectile.damage;
                    Main.projectile[num766].ai[1] = Projectile.ai[1] + 1f;
                    NetMessage.SendData(27, -1, -1, null, num766);
                }
                return;
            }
            if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int num877 = 0; num877 < 3; num877++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default(Color), 1.2f);
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 170, default(Color), 1.1f);

            }

            Projectile.alpha += 2;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}