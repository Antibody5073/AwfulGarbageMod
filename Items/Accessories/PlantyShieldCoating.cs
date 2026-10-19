using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption;

namespace AwfulGarbageMod.Items.Accessories
{
    public class PlantyShieldCoating : ModItem
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
        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 13;
            Item.value = Item.buyPrice(0, 4, 50);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        int Timer = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EarthenShield>().shieldDurabilityMult += 0.1f;
            player.GetModPlayer<EarthenShield>().shieldRegenStrength += 0.75f;

            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalResistance(player, ElementID.Nature, 0.12f, mod);
                IncreaseElementalResistance(player, ElementID.Fire, -0.12f, mod);
            }
        }
    }

}