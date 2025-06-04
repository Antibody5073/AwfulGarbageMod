using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using System.Collections.Generic;
using static Humanizer.In;
using System.Linq;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Items.Armor;

namespace AwfulGarbageMod.Global
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    public class ItemTypes : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool Empowerment = false;

        public bool Unreal = false;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Unreal)
            {
                TooltipLine tooltip = new TooltipLine(Mod, "Unreal", "Unreal");
                tooltips.Add(tooltip);
            }
        }
    }
}
