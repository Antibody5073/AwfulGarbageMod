using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ShellOfVengefulSpirits : ModItem
	{
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(30, Main.LocalPlayer, DamageClass.Ranged), AGUtils.ScaleDamage(100, Main.LocalPlayer, DamageClass.Ranged), AGUtils.ScaleDamage(15, Main.LocalPlayer, DamageClass.Magic), AGUtils.ScaleDamage(120, Main.LocalPlayer, DamageClass.Magic), AGUtils.ScaleDamage(30, Main.LocalPlayer, DamageClass.Summon), AGUtils.ScaleDamage(110, Main.LocalPlayer, DamageClass.Ranged)));
            tooltips.Add(tooltip);
        }


        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 12;
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            if (player.GetModPlayer<GlobalPlayer>().OverflowingVenom < 1.1f)
            {
                player.GetModPlayer<GlobalPlayer>().OverflowingVenom = 1.1f;
            }
            player.hasPaladinShield = true;
            player.noKnockback = true;

            player.thorns += 0.75f;
            player.GetModPlayer<GlobalPlayer>().cactusShell += 30;
            player.GetModPlayer<GlobalPlayer>().cactusShellPercent += 100;
            player.GetModPlayer<GlobalPlayer>().VenomOnDamanged += 180;
            player.GetModPlayer<GlobalPlayer>().HellShellSparks += 15;
            player.GetModPlayer<GlobalPlayer>().HellShellSparksPercent += 120;
            player.longInvince = true;
            player.starCloakItem = Item;

            player.GetModPlayer<GlobalPlayer>().MeatShield += 0.3f;
            player.GetModPlayer<GlobalPlayer>().ScaledShadeShield += 12f;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 30;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 110;
            player.buffImmune[46] = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[156] = true;
            player.fireWalk = true;

            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.AddBuff(BuffID.IceBarrier, 3);
            }
            player.GetModPlayer<GlobalPlayer>().HoneyOnDamaged += 300;
            if (hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().Bees = false;
            }
            else
            {
                player.GetModPlayer<GlobalPlayer>().Bees = true;
            }
        }


        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<NecroAegis>()
                .AddIngredient<VenomWard>()
                .AddIngredient<ShellOfRetaliation>()

                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
	}
}