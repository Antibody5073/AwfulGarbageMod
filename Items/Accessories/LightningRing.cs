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

    public class LightningRing : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lightning Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Mana potions are automatically consumed\nDisable visibility to disable auto-drinking mana potions\nMana sickness wears off twice as fast\nDrinking a mana potion will electrify you for two seconds\n5% reduced mana usage");
		}

		public int counter;

		public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 50000;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost *= 0.95f;
            if (hideVisual)
            {
                player.manaFlower = false;
            }
            else
            {
                player.manaFlower = true;
            }
            player.GetModPlayer<GlobalPlayer>().lightningRing = true;
        }
    }
}