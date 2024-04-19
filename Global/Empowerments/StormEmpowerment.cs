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

namespace AwfulGarbageMod.Global
{
    public class StormEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Storm Empowerment!";
        public override int empowermentCooldown => 300;
        public override int empowermentID => Empowerments.StormEmpowerment;
        public override Color textColor => new Color(107, 176, 255);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<StormEmpowermentPlayer>().hasSigil && player.GetModPlayer<StormEmpowermentPlayer>().sigilCooldown <= 0;
        }

        int startingTimeLeft;
        bool increasedDamage = false;

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<StormEmpowermentPlayer>().sigilCooldown = (int)(300 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            startingTimeLeft = projectile.timeLeft;
            if (ApplyDmgKb)
            {
                projectile.damage += 70;
            }
            projectile.penetrate += 1;
            increasedDamage = true;
        }
        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;
        public override void Visuals(Projectile projectile, Player player)
        {
            counter++;
            currentOffset += (newOffset - oldOffset) / 5;
            if (counter == 5)
            {
                oldOffset = newOffset;
                newOffset = Main.rand.NextFloat(-12, 12);
                counter = -1;
            }

            Vector2 normalizedVel = projectile.velocity.SafeNormalize(Vector2.Zero);

            int dust = Dust.NewDust(projectile.Center + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * currentOffset, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            hasThisEmpowerment = false;
            if (increasedDamage)
            {
                projectile.damage -= 80;
                projectile.timeLeft = startingTimeLeft;
            }
            for (var i = 0; i < 16; i++)
            {
                float xv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(projectile.Center, 1, 1, DustID.Electric, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].noGravity = true;
            }

            float maxDetectRadius = 1200f; // The maximum radius at which a projectile can detect a target
            float projSpeed = Vector2.Distance(new Vector2(0, 0), projectile.velocity);

            // Trying to find NPC closest to the projectile
            NPC closestNPC = AGUtils.GetClosestNPC(projectile.Center, maxDetectRadius, target);
            if (closestNPC != null)
            {

                // If found, change the velocity of the projectile and turn it in the direction of the target
                // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                projectile.velocity = (closestNPC.Center - projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            }
        }
    }

    public class StormEmpowermentPlayer : ModPlayer
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
