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

namespace AwfulGarbageMod.Global
{
    public class HolyEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Holy Empowerment!";
        public override int empowermentCooldown => 360;
        public override int empowermentID => Empowerments.HolyEmpowerment; 
        public override Color textColor => new Color(253, 246, 39);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<HolyEmpowermentPlayer>().hasSigil && player.GetModPlayer<HolyEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<HolyEmpowermentPlayer>().sigilCooldown = (int)(360 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {

                projectile.damage += 80;
            }
        }

        public override void Visuals(Projectile projectile, Player player)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.HallowedWeapons, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            hasThisEmpowerment = false;

            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = new Vector2(target.Center.X + Main.rand.NextFloat(-200, 200), target.Center.Y - Main.rand.NextFloat(800, 1100));
                Vector2 toMouse = Vector2.Normalize(target.Center - pos);
                float spd = 12;
                int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), pos, (toMouse * spd), ProjectileID.StarVeilStar, projectile.damage * 3/4, projectile.knockBack / 2, player.whoAmI);
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].extraUpdates = 1;
                Main.projectile[proj].velocity += target.velocity / (Main.projectile[proj].extraUpdates + 1);
                Main.projectile[proj].DamageType = ModContent.GetInstance<KnifeDamageClass>();

            }

            float sqrMaxDetectDistance = 160 * 160;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target2 = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)

                if (target2.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target2.Center, projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        target2.AddBuff(ModContent.BuffType<HolyFireBuff>(), 720);
                    }
                }

            }
        }
    }

    public class HolyEmpowermentPlayer : ModPlayer
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
