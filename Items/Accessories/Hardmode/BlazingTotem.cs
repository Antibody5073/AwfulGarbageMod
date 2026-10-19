using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Tiles.Furniture;
using ExampleMod.Content;
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

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class BlazingTotem : ModItem
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
            Item.value = 80000;
            Item.rare = 5;
			Item.accessory = true;
            Item.defense = 7;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxTurrets += 1;
            player.GetDamage(DamageClass.Summon) += 0.1f;

            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalDamage(player, ElementID.Fire, 0.08f, mod);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroups.AnyOldOneArmySentryAccessory)
                .AddIngredient(ItemID.FleshKnuckles)
                .AddIngredient<CandesciteBar>(20)
                .AddTile<CandescentCrucible>()
                .Register();
        }
    }
}