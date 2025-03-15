using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.IO;
using AwfulGarbageMod.Items.Weapons.Summon;

namespace AwfulGarbageMod.Buffs
{
    public class IciclashBuff : ModBuff
    {
        public static readonly int TagDamage = 0;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class IciclashBuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<IciclashBuff>())
            {
                // Apply a flat bonus to every hit
                int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(4, 4) + new Vector2(0, -4), ModContent.ProjectileType<IciclashProj2>(), 5 + (int)(projectile.damage * 0.15f), 0, projectile.owner);
                Main.projectile[proj].ArmorPenetration += 30;
                if (Main.projectile[proj].damage > 14)
                {
                    Main.projectile[proj].damage = 14;
                }
            }
        }
    }
}