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
using rail;
using Infernus;

namespace AwfulGarbageMod.Items.Accessories.VitalityCrossMod
{

    [ExtendsFromMod("VitalityMod")]
    public class CrimsonLantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stormy Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n12% decreased summon damage\n8% increased movement speed");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 300000;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

    }
}