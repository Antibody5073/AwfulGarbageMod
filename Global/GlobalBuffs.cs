using AwfulGarbageMod.Buffs;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using System;
using AwfulGarbageMod.Systems;
using Terraria.Localization;

namespace AwfulGarbageMod.Global.GlobalBuffs
{
    // Showcases how to work with all buffs
    public class ExampleGlobalBuff : GlobalBuff
    {
        float timer = 1;

        public override void Update(int type, Player player, ref int buffIndex)
        {
            //Magic power potion adjustment
            if (type == BuffID.MagicPower && ModContent.GetInstance<Config>().MagicPowerMana)
            {
                player.statManaMax2 += 100;
                player.GetDamage(DamageClass.Magic) -= 0.2f;
            }

            //Lightning ring
            if (type == BuffID.ManaSickness && player.GetModPlayer<GlobalPlayer>().lightningRingPrevious == true)
            {
                player.buffTime[buffIndex] -= 1;
            }
            if (DifficultyModes.Difficulty > 0 && !player.GetModPlayer<GlobalPlayer>().DisabledUnrealBuffNerfs)
            {
                if (type == BuffID.Regeneration)
                {
                    player.lifeRegen -= 2;
                }
                if (type == BuffID.Ironskin)
                {
                    player.statDefense -= 3;
                }
                if (type == BuffID.Endurance)
                {
                    player.endurance = 1f - (1.05f * (1f - player.endurance));
                }
                if (type == BuffID.Wrath)
                {
                    player.GetDamage(DamageClass.Generic) -= 0.05f;
                }
                if (type == BuffID.Rage)
                {
                    player.GetCritChance(DamageClass.Generic) -= 5;
                }
                if (type == BuffID.Archery)
                {
                    player.arrowDamage -= 0.05f;
                }
            }
        }

        
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            
            Player player = Main.LocalPlayer;

            if (BuffID.Sets.IsATagBuff[type])
            {
                npc.buffTime[buffIndex] += 1;

                timer -= 1 / player.GetModPlayer<GlobalPlayer>().WhipDebuffDurationPrev;
                if (timer <= 0)
                {
                    timer += 1;
                    npc.buffTime[buffIndex] -= 1;
                }
            }
        }

        
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;

            

            if (type == BuffID.ManaSickness && player.GetModPlayer<GlobalPlayer>().MyceliumHoodBonus == true)
            {
                tip += $"\nReduced mana cost by " + Math.Ceiling(player.manaSickReduction * 100) + "%";
            }
            if (type == BuffID.MagicPower && ModContent.GetInstance<Config>().MagicPowerMana)
            {
                tip = "Increased max mana by 100";
            }
            
            if (DifficultyModes.Difficulty > 0 && !player.GetModPlayer<GlobalPlayer>().DisabledUnrealBuffNerfs)
            {
                if (type == BuffID.Regeneration)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Regeneration");
                }
                if (type == BuffID.Ironskin)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Ironskin");
                }
                if (type == BuffID.Endurance)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Endurance");
                }
                if (type == BuffID.Wrath)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Wrath");

                }
                if (type == BuffID.Rage)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Rage");
                }
                if (type == BuffID.Archery)
                {
                    tip += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Archery");
                }
            }
        }
    }
}