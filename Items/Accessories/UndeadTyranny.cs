using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Infernus;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using StramClasses;
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

    public class UndeadTyranny : ModItem
    {
        public static LocalizedText InfernusEffect { get; private set; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("Infernus", out Mod infernus))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "InfernusEffect", InfernusEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("24% increased critical strike damage");
            InfernusEffect = this.GetLocalization(nameof(InfernusEffect));

        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 3;
            Item.accessory = true;
            Item.defense = 4;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.whipRangeMultiplier += 0.1f;
            player.GetKnockback(DamageClass.Summon).Base += 1f;
            player.GetModPlayer<GlobalPlayer>().PercentArmorIgnore *= 0.75f;
            player.thorns += 1f;


            if (ModLoader.TryGetMod("Infernus", out Mod infernus))
            {
                DoInfernus(player);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IronFist>()
                .AddIngredient<SpikyThread>()
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient<ThornyShackle>()
                .AddIngredient<SoulOfIghtImaHeadOut>(20)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }



        [JITWhenModsEnabled("Infernus")]
        public static void DoInfernus(Player player)
        {
            player.GetModPlayer<InfernusPlayer>().Cryo_Gauntlet = true;
        }
    }
}