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

    public class MeteoricManica : ModItem
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
            Item.value = 750000;
            Item.rare = 3;
            Item.accessory = true;
            Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().meteoriteGeode = true;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.1f;
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            if (player.HeldItem.DamageType == DamageClass.MeleeNoSpeed || (player.HeldItem.CountsAsClass(DamageClass.Melee) && player.HeldItem.attackSpeedOnlyAffectsWeaponAnimation) || ItemID.Sets.Yoyo[player.HeldItem.type])
            {
                player.GetAttackSpeed(DamageClass.Melee) += 0.3f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeteoriteGeode>()
                .AddIngredient<GoblinVambrace>()
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}