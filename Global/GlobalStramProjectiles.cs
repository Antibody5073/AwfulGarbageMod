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
using StramClasses;
using Terraria.Audio;

namespace AwfulGarbageMod.Global;

[ExtendsFromMod("StramClasses")]
public class GlobaStramlProjectiles : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source) 
    {
        Player player = Main.player[projectile.owner];

        //Velocity boosts
        if (projectile.DamageType == StramUtils.rogueDamage())
        {
            projectile.velocity *= player.GetModPlayer<GlobalStramPlayer>().rogueVelocity;
        }
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[projectile.owner];
        if (projectile.DamageType == StramUtils.rogueDamage() && player.GetModPlayer<GlobalStramPlayer>().CloudAssassinBonus)
        {
            if (hit.Crit)
            {
                SoundEngine.PlaySound(SoundID.Item94, target.Center);

                int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), target.Top + new Vector2(0, -800), new Vector2(0, 12), Mod.Find<ModProjectile>("ThundercrackLightning").Type, (int)(8 + hit.SourceDamage * 0.50), 0, projectile.owner);
                Main.projectile[proj].penetrate = 1;

                for (var i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(target.Top, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.2f);
                    Main.dust[dust].velocity = new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(24));
                }
            }
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

