using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.Shield
{
	public class Microwave : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(Item.shieldItem().durability * player.GetModPlayer<EarthenShield>().shieldDurabilityMult, Math.Round(Item.shieldItem().absorption * 10000)/100 + "%", Math.Round(Item.shieldItem().cooldown / player.GetModPlayer<EarthenShield>().shieldCooldownMult / 60f * 100) / 100 + " seconds"));

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
            Item.DamageType = ModContent.GetInstance<SentinelDamageClass>();
            Item.damage = 34;
            Item.knockBack = 0f;

            Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 12, silver: 50);
			Item.accessory = true;
            Item.rare = ItemRarityID.Orange;

            Item.shieldItem().shield = true;
			Item.shieldItem().absorption = 0.12f;
			Item.shieldItem().durability = 70;
			Item.shieldItem().cooldown = 120 * 15;
			Item.shieldItem().shieldType = "Microwave";
            Item.shieldItem().shieldBreakColor = Color.OrangeRed;

            Item.defense = 3;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReinforcedShield>()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}