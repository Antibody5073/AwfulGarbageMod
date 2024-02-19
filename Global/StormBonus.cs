using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using System.Collections.Generic;
using static Humanizer.In;
using System.Linq;

namespace AwfulGarbageMod.Global
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    public class StormBonus : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int shotNumber = 0;
        
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<GlobalPlayer>().StormHelmetBonus)
            {
                if (item.useAmmo == AmmoID.Arrow)
                {
                    shotNumber++;

                    if (shotNumber % 5 == 0)
                    {
                        for (int i = 0; i < Main.rand.Next(3, 5); i++)
                        {
                            Vector2 pos = new Vector2(Main.MouseWorld.X + Main.rand.NextFloat(-200, 200), player.Center.Y - 800);
                            Vector2 toMouse = Vector2.Normalize(Main.MouseWorld - pos);
                            float spd = Vector2.Distance(new Vector2(0, 0), velocity);
                            int proj = Projectile.NewProjectile(item.GetSource_ReleaseEntity(), pos, toMouse * spd * Main.rand.NextFloat(0.8f, 1.2f), Mod.Find<ModProjectile>("LightningBurstProj").Type, damage, knockback, player.whoAmI);
                            Main.projectile[proj].tileCollide = false;
                        }
                    }
                }
            }
            if (player.GetModPlayer<GlobalPlayer>().StormHeadgearBonus)
            {
                if (item.useAmmo == AmmoID.Bullet)
                {
                    shotNumber++;

                    if (shotNumber % 3 == 0)
                    {
                        type = Mod.Find<ModProjectile>("StormLightningProj").Type;
                        damage = (int)(damage * 1.25f);
                        Vector2 toMouse = Vector2.Normalize(Main.MouseWorld - player.Center);
                        velocity = toMouse * 12f;
                    }
                }
            }
        }
    }
    public class StormLightningProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 200;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 200;
        }

        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;


        int[] npcsHit = { -1, -1, -1, -1, -1, -1, -1 };
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == -1)
            {
                npcsHit[1] = (int)Projectile.ai[0];
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.ArmorPenetration = 0;
            Projectile.damage = (int)(Projectile.damage * 0.95);

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k] == target)
                {
                    npcsHit[Projectile.penetrate] = k;
                    break;
                }
            }

            float maxDetectRadius = 800f; // The maximum radius at which a projectile can detect a target
            float projSpeed = Vector2.Distance(new Vector2(0, 0), Projectile.velocity); // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
            {
                Projectile.timeLeft = 1;
                return;
            }
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            Projectile.aiStyle = -1;
            counter++;
            currentOffset += (newOffset - oldOffset) / 5;
            if (counter == 5)
            {
                oldOffset = newOffset;
                newOffset = Main.rand.NextFloat(-12, 12);
                counter = -1;
            }

            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);

            int dust = Dust.NewDust(Projectile.Center + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * currentOffset, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;


        }
        public NPC FindClosestNPC(float maxDetectDistance)
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
                if (!npcsHit.Contains(k))
                {
                    if (target.CanBeChasedBy())
                    {
                        // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                        float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                        // Check if it is within the radius
                        if (sqrDistanceToTarget < sqrMaxDetectDistance)
                        {
                            sqrMaxDetectDistance = sqrDistanceToTarget;
                            closestNPC = target;
                        }
                    }
                }
            }

            return closestNPC;
        }
    }
    
}
