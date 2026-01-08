using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using System.Collections.Generic;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Placeable;

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
                shop.Add<Nunchucks>(Condition.DownedKingSlime);

            }
            if (shop.NpcType == NPCID.Pirate)
            {
                // Adding an item to a vanilla NPC is easy:
                // This item sells for the normal price.
                shop.Add<AndHeSawThatTheWorldWasTastyOnceMore>();

            }
        }
    }
}