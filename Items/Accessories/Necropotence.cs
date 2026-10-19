using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{

    public class Necropotence : ModItem
    {

        public override void SetStaticDefaults()
        {

        }
        public int counter;

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = ItemRarityID.Green;
			Item.accessory = true;
            Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().necroPotence += 1;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 0.33f;
        }
	}
}