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
using AwfulGarbageMod.DamageClasses;
using System.Collections.Generic;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class FrigidSeed : ModItem
	{

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
        }


		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = 1;
			Item.accessory = true;

        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().FrigidSeed = true;
        }
    }
}