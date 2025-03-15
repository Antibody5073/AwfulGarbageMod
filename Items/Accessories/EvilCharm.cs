using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Accessories
{

    public class EvilCharm : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Corrupted Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("15% increased ranged velocity\n2% increased ranged crit chance\nRanged crits have a 20% chance to inflict cursed flame");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = 2;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.maxTurrets += 1;
        }
	}
}