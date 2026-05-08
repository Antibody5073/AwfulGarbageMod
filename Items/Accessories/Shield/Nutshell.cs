using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Magic;
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
	public class Nutshell : ModItem
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


        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<SentinelDamageClass>();
            Item.damage = 10;
            Item.knockBack = 1f;

            Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 5);
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;


            Item.shieldItem().shield = true;
			Item.shieldItem().durability = 50;
            Item.shieldItem().absorption = 0.25f;
            Item.shieldItem().cooldown = (int)(120 * 17.5f);
			Item.shieldItem().shieldType = "Nutshell";
            Item.shieldItem().shieldBreakColor = Color.OliveDrab;

            Item.defense = 2;
		}
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 -= 40;
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
                .AddIngredient<WoodShield>()
                .AddIngredient<AcornStaff>()
                .AddIngredient(ItemID.Emerald, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}