using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Ammo;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static AssGen.Assets;
using static Redemption;

namespace AwfulGarbageMod.Items.Accessories
{

    public class EarthenAmulet : ModItem
    {
        public static LocalizedText RedemptionEffect { get; private set; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "RedemptionEffect", RedemptionEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("24% increased critical strike damage");
            RedemptionEffect = this.GetLocalization(nameof(RedemptionEffect));

        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 1;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.12f;
            player.manaCost *= 0.95f;
            player.statManaMax2 += 20;
            player.manaRegenDelayBonus -= 0.5f;
            player.manaRegenBonus -= 40;
            player.GetModPlayer<GlobalPlayer>().EarthenAmulet = true;

            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalDamage(player, ElementID.Earth, 0.05f, mod);
                IncreaseElementalResistance(player, ElementID.Earth, 0.05f, mod);
                IncreaseElementalResistance(player, ElementID.Nature, -0.1f, mod);
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(6)
                .AddIngredient(ItemID.DirtBlock, 18)
                .AddIngredient(ItemID.ClayBlock, 12)
                .AddIngredient(ItemID.StoneBlock, 12)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}