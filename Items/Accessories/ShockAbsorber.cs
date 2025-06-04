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

    public class ShockAbsorber : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 1500;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.velocity.Y == 0)
            {
                player.endurance = 1f - (0.85f * (1f - player.endurance));
            }
            if (player.HasBuff(BuffID.Electrified))
            {
                player.GetDamage(DamageClass.Generic) += 0.15f;
            }
            player.GetModPlayer<GlobalPlayer>().ShockAbsorber = true;
        }
    }
}