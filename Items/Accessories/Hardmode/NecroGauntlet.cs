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

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class NecroGauntlet : ModItem
    {
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

        }
        public int counter;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(16, Main.LocalPlayer, DamageClass.Summon), AGUtils.ScaleDamage(20, Main.LocalPlayer, DamageClass.Summon)));
            tooltips.Add(tooltip);
        }
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
            player.GetModPlayer<GlobalPlayer>().necroPotence += 16;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 20;
            player.boneGloveItem = Item;
            player.GetAttackSpeed(DamageClass.Melee) += 0.16f;
            player.GetDamage(DamageClass.Melee) += 0.08f;
            player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit += 12;
            player.moveSpeed += 0.16f;
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