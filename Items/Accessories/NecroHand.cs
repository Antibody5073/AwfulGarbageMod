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

    public class NecroHand : ModItem
    {
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 25000;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.expert = true;
            Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().BoneGloveDamage += 27;
            player.GetModPlayer<GlobalPlayer>().FlatCrit += 10;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 1;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 0.33f;
            player.boneGloveItem = Item;
        }
	}
}