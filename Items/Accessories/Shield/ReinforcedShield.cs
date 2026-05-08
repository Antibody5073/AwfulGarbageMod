using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using Terraria.Localization;
using System.Linq;
using AwfulGarbageMod.Global;


namespace AwfulGarbageMod.Items.Accessories.Shield
{
    public class ReinforcedShield : ModItem
	{
        public static LocalizedText TooltipWithVar { get; private set; }
        public override void SetStaticDefaults()
        {
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(Item.shieldItem().durability * player.GetModPlayer<EarthenShield>().shieldDurabilityMult, Math.Round(Item.shieldItem().absorption * 10000) / 100 + "%", Math.Round(Item.shieldItem().cooldown / player.GetModPlayer<EarthenShield>().shieldCooldownMult / 60f * 100) / 100 + " seconds"));

            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (!player.GetModPlayer<EarthenShield>().shieldEquipped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 2);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;

            Item.shieldItem().shield = true;
			Item.shieldItem().absorption = 0.14f;
			Item.shieldItem().durability = 33;
			Item.shieldItem().cooldown = 120 * 13;
			Item.shieldItem().shieldType = "ReinforcedShield";
            Item.shieldItem().shieldBreakColor = Color.DarkSlateGray;

            Item.defense = 1;
		}
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.95f * (1f - player.endurance));
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}