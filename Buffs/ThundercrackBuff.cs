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
using Terraria.Audio;

namespace AwfulGarbageMod.Buffs
{
    public class ThundercrackBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ThundercrackBuffNPC>().markedByThundercrack = true;
        }
    }

    public class ThundercrackBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool markedByThundercrack;

        public override void ResetEffects(NPC npc)
        {
            markedByThundercrack = false;
        }

        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (markedByThundercrack && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                SoundEngine.PlaySound(SoundID.Item94, npc.Center);

                Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Top + new Vector2(0, -800), new Vector2(0, 12), Mod.Find<ModProjectile>("ThundercrackLightning").Type, projectile.damage, 0, projectile.owner);
                for (int i = 0; i < npc.buffType.Length; i++)
                {
                     int type = npc.buffType[i];
                    if (type == ModContent.BuffType<ThundercrackBuff>())
                    {
                        npc.buffTime[i] = 0;
                    }

                }
            }
        }
    }
}