using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.Shield
{
	public class WaterVeil : ModItem
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


        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<SentinelDamageClass>();
            Item.damage = 27;
            Item.knockBack = 5f;

            Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 15);
			Item.accessory = true;
            Item.rare = ItemRarityID.Green;

            Item.shieldItem().overrideShieldDamage = false;
            Item.shieldItem().shield = true;

			Item.shieldItem().absorption = 0.30f;
			Item.shieldItem().durability = 40;
			Item.shieldItem().cooldown = 120 * 20;

			Item.shieldItem().shieldType = "WaterVeil";
            Item.shieldItem().shieldBreakColor = Color.LightCyan;

            Item.defense = 1;
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
        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.statLifeMax2 -= 40;
            if (player.GetModPlayer<EarthenShield>().shieldsActive)
            {
                player.endurance = 1f - (0.95f * (1f - player.endurance));
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StormEssence>(7)
                .AddIngredient(ItemID.Coral, 5)
                .AddIngredient(ItemID.Seashell, 5)
                .AddTile<StormAnvil>()
                .Register();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}