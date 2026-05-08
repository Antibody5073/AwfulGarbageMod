using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using Terraria.Localization;
using System.Linq;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Accessories.Shield
{
	//[AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class EnchantedShield : ModItem
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
            Item.damage = 25;
            Item.knockBack = 5f;
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 8);
			Item.accessory = true;
            Item.rare = ItemRarityID.Green;
			Item.shieldItem().shield = true;
			Item.shieldItem().absorption = 0.12f;
			Item.shieldItem().durability = 25;
			Item.shieldItem().cooldown = 120 * 12;
			Item.shieldItem().shieldType = "EnchantedShield";
            Item.shieldItem().shieldBreakColor = new Color(13, 81, 209);

            Item.defense = 2;
		}
        public override bool WeaponPrefix()
        {
            return true;
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ModContent.ItemType<StoneShield>())
                .AddTile(TileID.Anvils)
                .Register();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}