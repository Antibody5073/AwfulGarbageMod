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
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Buffs
{
    public class HarujionPetalBuff : ModBuff
    {
        public static LocalizedText SpiritEnergy { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            SpiritEnergy = this.GetLocalization(nameof(SpiritEnergy));
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = SpiritEnergy.Format((int)Main.LocalPlayer.GetModPlayer<GlobalPlayer>().HarujionLevel, Main.LocalPlayer.GetModPlayer<GlobalPlayer>().HarujionPetal);
            
        }
    }
   
}