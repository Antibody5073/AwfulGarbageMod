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
using Terraria.Audio;
using Terraria.Localization;
using AwfulGarbageMod.Items.Accessories;

namespace AwfulGarbageMod.Global
{

    public class GlobalProjectiles : GlobalProjectile
    {
        public static class Sets
        {
            public static bool[] IsScepterProjectile = ProjectileID.Sets.Factory.CreateBoolSet();
        }

        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ProjectileID.ThrowingKnife)
            {
                entity.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (entity.type == ProjectileID.PoisonedKnife)
            {
                entity.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (entity.type == 599)
            {
                entity.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (entity.type == ProjectileID.FrostDaggerfish)
            {
                entity.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            //Illuminant String
            if (player.GetModPlayer<GlobalPlayer>().IlluminantString && ProjectileID.Sets.IsAWhip[projectile.type])
            {
                Lighting.AddLight(projectile.Center, 0.75f, 0.4f, 1f);
            }
            base.AI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];

            //Poison Pendant
            if (projectile.DamageType == DamageClass.Ranged && player.GetModPlayer<GlobalPlayer>().spiderPendant == true && Main.rand.NextBool(15))
            {
                target.AddBuff(BuffID.Poisoned, 600);

            }
            //Corrupted Pendant
            if (projectile.DamageType == DamageClass.Ranged && player.GetModPlayer<GlobalPlayer>().corruptedPendant == true && hit.Crit && Main.rand.NextBool(5))
            {
                target.AddBuff(BuffID.CursedInferno, 300);
            }
            //Crimson Pendant
            if (projectile.DamageType == DamageClass.Ranged && player.GetModPlayer<GlobalPlayer>().crimsonPendant == true && hit.Crit && Main.rand.NextBool(5))
            {
                target.AddBuff(BuffID.Ichor, 300);
            }

            //Storm Hood
            if (projectile.DamageType == GetInstance<KnifeDamageClass>() && player.GetModPlayer<GlobalPlayer>().StormHoodBonus == true)
            {
                int[] npcsBlacklisted = { target.whoAmI };
                NPC closestNPC = FindClosestNPC(400, target.Center, npcsBlacklisted);
                if (closestNPC != null)
                {
                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, (closestNPC.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 12, Mod.Find<ModProjectile>("StormLightningProj").Type, projectile.damage / 4, 0, player.whoAmI, target.whoAmI, -1);
                    Main.projectile[proj].penetrate = 2;
                    Main.projectile[proj].localNPCImmunity[target.whoAmI] = -1;

                }
            }
            if (projectile.DamageType == GetInstance<KnifeDamageClass>() && player.GetModPlayer<GlobalPlayer>().poisonSigil > 0 && hit.Crit && Main.rand.NextBool(3))
            {
                for (int j = 0; j < 2; j++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), projectile.Center, Main.rand.NextVector2CircularEdge(5, 5), ProjectileID.SporeCloud, player.GetModPlayer<GlobalPlayer>().poisonSigil, 0, player.whoAmI);
                    Main.projectile[proj].DamageType = DamageClass.Ranged;
                    Main.projectile[proj].usesIDStaticNPCImmunity = true;
                    Main.projectile[proj].idStaticNPCHitCooldown = 10;
                }
            }
            if (projectile.DamageType == GetInstance<KnifeDamageClass>() && player.GetModPlayer<GlobalPlayer>().shadowSigil > 0 && hit.Crit && Main.rand.NextBool(3))
            {
                for (int j = 0; j < 2; j++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), projectile.Center, Main.rand.NextVector2CircularEdge(5, 5), ModContent.ProjectileType<SigilOfShadowsSporeProj>(), player.GetModPlayer<GlobalPlayer>().shadowSigil, 0, player.whoAmI);
                }
            }

            if (projectile.type == ProjectileID.MoonlordBullet && projectile.ai[1] == -15f)
            {
                projectile.damage += 2;
            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];

            //bee damage boosts
            if (projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.HornetStinger)
            {
                projectile.damage = (int)(projectile.damage * player.GetModPlayer<GlobalPlayer>().beeDmg);
            }
            if (projectile.type == ProjectileID.HornetStinger)
            {
                projectile.damage = (int)(projectile.damage * ((player.GetModPlayer<GlobalPlayer>().beeDmg - 1) * 1.5f + 1));
            }

            //Velocity boosts
            if (AGUtils.AnyRangedDmg(projectile.DamageType))
            {
                projectile.velocity *= player.GetModPlayer<GlobalPlayer>().rangedVelocity;
            }
            if (projectile.DamageType == ModContent.GetInstance<KnifeDamageClass>())
            {
                projectile.velocity *= player.GetModPlayer<GlobalPlayer>().knifeVelocity;
            }

            //Bone Glove recode
            if (projectile.type == ProjectileID.BoneGloveProj)
            {
                projectile.damage = player.GetModPlayer<GlobalPlayer>().BoneGloveDamage;
            }

            if (projectile.type == ProjectileID.VolatileGelatinBall)
            {
                projectile.usesIDStaticNPCImmunity = false;
                projectile.localNPCHitCooldown = -2;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 15;
            }

            //Starfury nerf
            if (projectile.type == ProjectileID.Starfury && ModContent.GetInstance<Config>().StarfuryNerf)
            {
                projectile.damage = (int)(projectile.damage * 0.80f);
            }

            //Demon scythe nerf
            if (projectile.type == ProjectileID.DemonScythe && ModContent.GetInstance<Config>().DemonScytheNerf)
            {
                if (!NPC.downedBoss3)
                {
                    projectile.damage = (int)(projectile.damage * 0.50f);
                }
            }

            //Star wrath
            if (projectile.type == ProjectileID.StarWrath && ModContent.GetInstance<Config>().MoonLordBalance)
            {
                projectile.damage = (int)(projectile.damage * 0.5f);
            }
            //Terrarian
            if (projectile.type == ProjectileID.TerrarianBeam && ModContent.GetInstance<Config>().MoonLordBalance)
            {
                projectile.velocity *= 1.3f;
            }
            //Chlorophyte bullet
            if (projectile.type == ProjectileID.ChlorophyteBullet && ModContent.GetInstance<Config>().ChlorophyteBulletNerf)
            {
                projectile.damage = (int)(projectile.damage * 0.75f);
            }
        }
        public NPC FindClosestNPC(float maxDetectDistance, Vector2 projCenter, int[] npcsBlacklisted)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)

                if (target.CanBeChasedBy() && !npcsBlacklisted.Contains(k))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projCenter);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }

            }

            return closestNPC;
        }
    }
}