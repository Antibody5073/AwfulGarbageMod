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

    public class ThrushFeather : ModItem
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
            Item.value = 7500;
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.25f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;


            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalResistance(player, ElementID.Ice, -0.1f, mod);
                IncreaseElementalResistance(player, ElementID.Fire, -0.1f, mod);
                IncreaseElementalResistance(player, ElementID.Thunder, -0.1f, mod);
                IncreaseElementalResistance(player, ElementID.Earth, 0.1f, mod);
                IncreaseElementalDamage(player, ElementID.Wind, 0.15f, mod);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfIghtImaHeadOut>(8)
                .AddIngredient(ItemID.Feather)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}