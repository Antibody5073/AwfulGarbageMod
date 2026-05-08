using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{
    public class ReflexiveCharm : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(10, player, ModContent.GetInstance<SentinelDamageClass>()).ToString()));

            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 13;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        int Timer = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EarthenShield>().reflexiveCharm = true;
            player.GetModPlayer<EarthenShield>().ShieldEffectPower += 0.05f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(24)
                .AddIngredient(ItemID.Chain, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

}