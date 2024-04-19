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

    public class NecroGauntlet : ModItem
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
            Item.value = 25000;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.expert = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 2;
            player.GetModPlayer<GlobalPlayer>().BoneGloveDamage += 26;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 31;
            player.boneGloveItem = Item;
            player.GetAttackSpeed(DamageClass.Melee) += 0.14f;
            player.GetDamage(DamageClass.Melee) += 0.07f;
            player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit += 12;
            player.moveSpeed *= 1.06f;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient<NecroHand>()
                .AddIngredient<MyceliumGauntlet>()
                .AddIngredient<SoulOfIghtImaHeadOut>(6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}