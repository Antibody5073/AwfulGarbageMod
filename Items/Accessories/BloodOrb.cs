using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption;

namespace AwfulGarbageMod.Items.Accessories
{

    public class BloodOrb : ModItem
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
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 1;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().PercentArmorIgnoreSummon *= 0.7f;
            player.GetModPlayer<GlobalPlayer>().MaxPercentArmorIgnoreSummon += 12;
            player.statLifeMax2 -= (int)Math.Ceiling(player.slotsMinions) * 30;
			Item.lifeRegen = (int)Math.Ceiling(player.slotsMinions);


            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalDamage(player, ElementID.Blood, 0.10f, mod);
                IncreaseElementalDamage(player, ElementID.Poison, -0.08f, mod);
                IncreaseElementalDamage(player, ElementID.Holy, -0.08f, mod);
            }
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Glass, 15);
            recipe.AddIngredient(Mod.Find<ModItem>("CloudRelic").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("VeinJuice").Type, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}