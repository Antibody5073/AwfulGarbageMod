using AwfulGarbageMod.Items.Weapons;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Buffs;
using gunrightsmod.Content.DamageClasses;

namespace AwfulGarbageMod.Projectiles
{


    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    [ExtendsFromMod("gunrightsmod")]

    public class SnakeOilMinion : ModProjectile
    {

        int atkTimer;
        float turnSpd;

        float offsetPos = 0;
        float offsetDistance = 24;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.extraUpdates = 1;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = ModContent.GetInstance<SummonStupidDamage>();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.damage = AGUtils.ScaleDamage(15, owner, ModContent.GetInstance<SummonStupidDamage>());
            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out int target);


            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            turnSpd = 0;
            target.AddBuff(ModContent.BuffType<SnakeOilBuff>(), 240);
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalTerMericaPlayer>().SnakeOil)
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 targetPos = owner.Center;

            vectorToIdlePosition = targetPos - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = targetPos;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out int target)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;
            target = -1;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                bool inRange = between < distanceFromTarget;
                bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                bool closeThroughWall = between < 75f || Collision.CanHitLine(owner.Center - new Vector2(2, 2), 4, 4, npc.Center - new Vector2(2, 2), 4, 4);

                if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall) && between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    target = owner.MinionAttackTargetNPC;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];


                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 75f || Collision.CanHitLine(owner.Center - new Vector2(2, 2), 4, 4, npc.Center - new Vector2(2, 2), 4, 4);

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            Player owner = Main.player[Projectile.owner];

            // Default movement parameters (here for attacking)
            float speed = 8f;
            float inertia = 1f;
            Vector2 direction;

            if (foundTarget)
            {
                speed = 10;
                turnSpd += 0.15f;
                if (Projectile.velocity.LengthSquared() < speed * speed)
                {
                    Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.One) * 0.2f;
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
                Vector2 targetPos = targetCenter;
                if (Vector2.DistanceSquared(targetPos, Projectile.Center) > 80 * 80)
                {
                    float turn = AGUtils.TurnTowards(turnSpd, targetPos, Projectile.velocity.ToRotation(), Projectile.Center);
                    Projectile.velocity = Projectile.velocity.RotatedBy(turn);
                }
            }
            else
            {
                turnSpd = 0;
                atkTimer--;
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 12f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 6f;
                }

                if (Projectile.velocity.LengthSquared() < speed * speed)
                {
                    Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.One) * 0.2f;
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
                Vector2 targetPos = owner.Center;
                if (Vector2.DistanceSquared(targetPos, Projectile.Center) > 80 * 80)
                {
                    float turn = AGUtils.TurnTowards(3, targetPos, Projectile.velocity.ToRotation(), Projectile.Center, 4);
                    Projectile.velocity = Projectile.velocity.RotatedBy(turn);
                }
            }
        }

        private void Visuals()
        {
            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
            offsetPos += 24;

            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.position + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * (float)Math.Sin((double)MathHelper.ToRadians(offsetPos)) * offsetDistance, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0, 0);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}