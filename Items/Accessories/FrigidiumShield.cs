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
    [AutoloadEquip(EquipType.Shield)]
    public class FrigidiumShield : ModItem
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
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 4;
            player.buffImmune[BuffID.Chilled] = true;
            player.GetCritChance(DamageClass.Melee) += 4f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.04f;
            player.GetDamage(DamageClass.Melee) += 0.04f;
            player.GetModPlayer<GlobalPlayer>().iceCrystalGeode = true;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.1f;


            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalResistance(player, ElementID.Ice, 0.08f, mod);
                IncreaseElementalResistance(player, ElementID.Fire, 0.08f, mod);
            }
        }


        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IceCrystalGeode>());
            recipe.AddIngredient(ModContent.ItemType<FrigidiumBar>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}