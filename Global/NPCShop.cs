using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using System.Collections.Generic;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Weapons.Ranged;

namespace AwfulGarbageMod.Global.GlobalItems
{
    class ChirumiruShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
            {
                // Adding an item to a vanilla NPC is easy:
                // This item sells for the normal price.
                shop.Add<ChirumiruCan>(Condition.TimeNight);
            }
        }
    }
}