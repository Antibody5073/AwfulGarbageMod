using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Printing;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using Mono.Cecil;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics.Metrics;
using AwfulGarbageMod.Items.Accessories;

namespace AwfulGarbageMod.Global
{
    public class VenomEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Venom Empowerment!";
        public override int empowermentCooldown => 90;
        public override int empowermentID => Empowerments.VenomEmpowerment;
        public override Color textColor => new Color(104, 43, 196);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<VenomEmpowermentPlayer>().hasSigil && player.GetModPlayer<VenomEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<VenomEmpowermentPlayer>().sigilCooldown = (int)(90 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {
                projectile.damage = (int)(projectile.damage * 1.35f);
            }
        }
        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;
        public override void Visuals(Projectile projectile, Player player)
        {
            Vector2 vel1 = projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 5;
            Vector2 vel2 = projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2) * 5;

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Venom, vel1.X, vel1.Y, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Venom, vel2.X, vel2.Y, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            target.AddBuff(BuffID.Venom, 300);
            int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), player.Center, new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(15)), ProjectileID.BabySpider, 30, 0, player.whoAmI);
            Main.projectile[proj].DamageType = DamageClass.Ranged;
            proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), player.Center, new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(15)), ProjectileID.BabySpider, 30, 0, player.whoAmI);
            Main.projectile[proj].DamageType = DamageClass.Ranged;


        }
    }

    public class VenomEmpowermentPlayer : ModPlayer
    {
        public bool hasSigil = false;
        public int sigilCooldown = 0;

        public override void ResetEffects()
        {
            hasSigil = false;
            sigilCooldown -= 1;
        }
    }
}
