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
using AwfulGarbageMod.Items.Accessories;

namespace AwfulGarbageMod.Global
{
    public class RadiantEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Radiance Empowerment!";
        public override int empowermentCooldown => 240;
        public override int empowermentID => Empowerments.RadiantEmpowerment;
        public override Color textColor => new Color(255, 255, 0);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<RadiantEmpowermentPlayer>().hasSigil && player.GetModPlayer<RadiantEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<RadiantEmpowermentPlayer>().sigilCooldown = (int)(240 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {
                projectile.damage = (int)(projectile.damage * 1.7f);
            }
            if (projectile.penetrate > 0)
            {
                if (projectile.usesLocalNPCImmunity == false)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                }
                projectile.penetrate += 2;
            }
        }

        public override void Visuals(Projectile projectile, Player player)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Pixie, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            List<NPCandValue> npcDistances = new List<NPCandValue> { };


            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = 1280 * 1280;

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
                        npcDistances.Add(
                            new NPCandValue { npc = target2, value = sqrDistanceToTarget }
                            );
                    }
                }
            }

            npcDistances.Sort();

            int npcs = npcDistances.Count;
            if (npcs > 3)
            {
                npcs = 3;
            }
            if (npcs > 0)
            {
                for (int i = 0; i < npcs; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), npcDistances[i].npc.Center - new Vector2(npcDistances[i].npc.width + 16, 0), Vector2.Zero, ModContent.ProjectileType<RadiantPrismSwordProj>(), damageDone / 3, 2, player.whoAmI, npcDistances[i].npc.whoAmI);
                }
            }
        }
    }

    public class RadiantEmpowermentPlayer : ModPlayer
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
