using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Buffs
{
    public class NightmareRaze : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bone Skewer Bleed"); // Buff display name
            // Description.SetDefault("Losing life"); // Buff description
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NightmareRazeNPC>().hasHolyFire = true;
        }
    }
    public class NightmareRazeNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool hasHolyFire;

        public override void ResetEffects(NPC npc)
        {
            hasHolyFire = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (hasHolyFire)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 10 * 2;
                if (damage < 5)
                {
                    damage = 5;
                }
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (hasHolyFire)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Wraith, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 1.2f;
                }
            }
        }
    }
}