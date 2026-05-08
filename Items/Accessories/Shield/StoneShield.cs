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
    public class StoneShield : ModItem
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
			Item.value = Item.buyPrice(silver: 10);
			Item.accessory = true;
            Item.rare = ItemRarityID.White;

            Item.shieldItem().shield = true;

			Item.shieldItem().absorption = 0.12f;
			Item.shieldItem().durability = 20;
			Item.shieldItem().cooldown = 120 * 15;

			Item.shieldItem().shieldType = "StoneShield";
            Item.shieldItem().shieldBreakColor = new Color(199, 195, 190);


            Item.defense = 1;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<EarthenShield>().shieldsActive)
            {
                player.moveSpeed *= 0.75f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}