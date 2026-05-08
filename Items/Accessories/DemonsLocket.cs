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
using StramClasses;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{

    public class DemonsLocket : ModItem
	{
        public override void SetStaticDefaults()
		{
        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.9f * (1f - player.endurance));
            player.statLifeMax2 += 25;
            player.GetModPlayer<GlobalPlayer>().bonusluck += 0.05f;
        }
    }
}