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
using Terraria.Localization;

namespace AwfulGarbageMod.Buffs
{
    public class SanguineRegenerationBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false; // Causes this buff not to persist when exiting and rejoining the world    
        }

        // Allows you to make this buff give certain effects to the given player
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SanguineBuffPlayer>().lifeRegenDebuff = true;
        }
    }
    public class SanguineBuffPlayer : ModPlayer
    {
        // Flag checking when life regen debuff should be activated
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        // Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
        // This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
        // The player will take damage at a rate of half the number you subtract per second
        public override void UpdateLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                Player.lifeRegen += (int)(Player.statLifeMax2 * 0.16f / 7.5f);
                Player.manaRegenBonus += 50;
                Player.manaRegenDelayBonus += 3f;
            }
        }
    }
}