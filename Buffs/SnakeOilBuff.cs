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
using gunrightsmod.Content.DamageClasses;

namespace AwfulGarbageMod.Buffs
{
    [ExtendsFromMod("gunrightsmod")]

    public class SnakeOilBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;

        }
        public override void Update(NPC npc, ref int buffIndex)
        {

            npc.GetGlobalNPC<SnakeOilBuffNPC>().hasDebuff = true;
        }
    }
    [ExtendsFromMod("gunrightsmod")]

    public class SnakeOilBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool hasDebuff;

        public override void ResetEffects(NPC npc)
        {
            hasDebuff = false;
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (hasDebuff)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.DungeonSpirit, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].scale *= 1.2f;
                    Main.dust[dust].alpha /= 3;
                }
            }
        }
        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (hasDebuff && projectile.CountsAsClass<StupidDamage>())
            {
                modifiers.SourceDamage *= 1.12f;

                for (int i = 0; i < 12; i++)
                {
                    int dust = Dust.NewDust(npc.Center, 1, 1, DustID.DungeonSpirit, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity = new Vector2(0, 5).RotatedBy(MathHelper.TwoPi * i / 12);
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < npc.buffType.Length; i++)
                {
                    int type = npc.buffType[i];
                    if (type == ModContent.BuffType<SnakeOilBuff>())
                    {
                        npc.buffTime[i] = 0;
                    }
                }
            }
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (hasDebuff && item.DamageType.CountsAsClass<StupidDamage>())
            {
                modifiers.SourceDamage *= 1.12f;

                for (int i = 0; i < 12; i++)
                {
                    int dust = Dust.NewDust(npc.Center, 1, 1, DustID.DungeonSpirit, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity = new Vector2(0, 5).RotatedBy(MathHelper.TwoPi * i / 12);
                    Main.dust[dust].noGravity = true;
                }

                for (int i = 0; i < npc.buffType.Length; i++)
                {
                    int type = npc.buffType[i];
                    if (type == ModContent.BuffType<SnakeOilBuff>())
                    {
                        npc.buffTime[i] = 0;
                    }
                }
            }
        }
    }
}