using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
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
    public class CelestitePlating : ModItem
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
            Item.value = 4500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 4;
            player.GetCritChance(DamageClass.Magic) += 4f;
            player.manaCost *= 0.96f;
            player.statManaMax2 += 30;
            player.GetDamage(DamageClass.Magic) += 0.04f;
            player.magicCuffs = true;
            player.manaMagnet = true;
            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalResistance(player, ElementID.Celestial, 0.08f, mod);
                IncreaseElementalResistance(player, ElementID.Earth, 0.08f, mod);
            }
        }


        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient<CelestiteShard>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}