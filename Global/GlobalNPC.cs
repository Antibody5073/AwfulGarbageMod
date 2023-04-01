using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Global;
public class ExampleGlobalNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.WallCreeper || npc.type == NPCID.WallCreeperWall || npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall)
        {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderLeg>(), 1));
        }
        if (npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.IceBat)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostShard>(), 1));
        }
    }










    public override bool InstancePerEntity => true;

    public bool ExampleDebuff;
    public int BoneSkewerBleed;
    public int BoneSkewerTimer;

    public override void ResetEffects(NPC npc)
    {
        ExampleDebuff = false;
        BoneSkewerTimer -= 1;
        if (BoneSkewerTimer < 1)
        {
            BoneSkewerBleed = 0;
        }
    }

    public override void SetDefaults(NPC npc)
    {
        // We want our ExampleJavelin buff to follow the same immunities as BoneJavelin
        npc.buffImmune[BuffType<Buffs.BoneSkewerBleed>()] = npc.buffImmune[BuffID.BoneJavelin];
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (BoneSkewerBleed > 0)
        {
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen -= BoneSkewerBleed * 3 * 2;
            if (damage < BoneSkewerBleed * 2)
            {
                damage = BoneSkewerBleed * 2;
            }
        }
        if (ExampleDebuff)
        {
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen -= 16;
            if (damage < 2)
            {
                damage = 2;
            }
        }
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (BoneSkewerBleed > 0)
        {
            if (Main.rand.Next(4) < 3)
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                Main.dust[dust].velocity *= 1.8f;
                Main.dust[dust].velocity.Y -= 0.5f;
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.5f;
                
            }
        }
    }
}


