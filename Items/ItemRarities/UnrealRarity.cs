using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework.Input;
using AwfulGarbageMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AwfulGarbageMod.Items
{
    public class UnrealRarity : ModRarity
    {
        public override Color RarityColor => new Color((byte)MathHelper.Lerp(69, 199, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(25, 41, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(112, 255, (Main.mouseTextColor - 190) / 65f));

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
}