using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace AwfulGarbageMod.Buffs
{
    public class SparklingStormbrewBuff : ModBuff
    {
        public static LocalizedText DeathMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false; // Causes this buff not to persist when exiting and rejoining the world    
        }

        // Allows you to make this buff give certain effects to the given player
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SparklingStormbrewBuffPlayer>().lifeRegenDebuff = true;
        }
    }
    public class SparklingStormbrewBuffPlayer : ModPlayer
    {
        // Flag checking when life regen debuff should be activated
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (lifeRegenDebuff)
            {
                for (int i = 0; i < Player.buffType.Length; i++)
                {
                    int type = Player.buffType[i];
                    if (type == ModContent.BuffType<SparklingStormbrewBuff>())
                    {
                        Player.buffTime[i] = 0;
                    }
                }
                Player.Heal((int)MathHelper.Clamp(info.Damage, 0, 150));
                Player.immuneTime = 60;
                Player.immune = true;
                SoundEngine.PlaySound(SoundID.Item4, Player.Center);
                return true;
            }
            return base.ConsumableDodge(info);
        }
        // Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
        // This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
        // The player will take damage at a rate of half the number you subtract per second
        public override void UpdateLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegen -= 10;
            }
        }
    }
}