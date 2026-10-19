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

    public class GoblinVambrace : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("24% increased critical strike damage");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
			Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			if (player.HeldItem.DamageType == DamageClass.MeleeNoSpeed || (player.HeldItem.CountsAsClass(DamageClass.Melee) && player.HeldItem.attackSpeedOnlyAffectsWeaponAnimation) || ItemID.Sets.Yoyo[player.HeldItem.type])
			{
				player.GetAttackSpeed(DamageClass.Melee) += 0.3f;
			}

        }
	}
}