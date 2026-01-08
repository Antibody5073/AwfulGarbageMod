using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Infernus;
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

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class BipolarBouquet : ModItem
	{
        public static LocalizedText InfernusEffect { get; private set; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("Infernus", out Mod infernus))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "InfernusEffect", InfernusEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("24% increased critical strike damage");
            InfernusEffect = this.GetLocalization(nameof(InfernusEffect));

        }


        public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 30000;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetCritChance(DamageClass.Magic) += 5;

            if (player.velocity.X == 0)
            {
                player.GetDamage(DamageClass.Magic) += 15 / 100f;
            }
            if (player.velocity.Y == 0)
            {
                player.manaCost *= 0.75f;
            }
            else
            {
                player.manaCost *= 0.9f;
            }
            player.GetDamage(DamageClass.Magic) += 10 / 100f;
            player.GetModPlayer<GlobalPlayer>().FrozenLotus += 0.07f;
            player.GetModPlayer<GlobalPlayer>().MoltenRose += 0.07f;

            if (ModLoader.TryGetMod("Infernus", out Mod infernus))
            {
                DoInfernus(player);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BipolarEmblem>()
                .AddIngredient<FrozenLotus>()
                .AddIngredient<MoltenRose>()
                .AddIngredient<Cryogem>(10)
                .AddIngredient<Pyrogem>(10)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        [JITWhenModsEnabled("Infernus")]
        public static void DoInfernus(Player player)
        {
            player.GetModPlayer<InfernusPlayer>().Ice_Scroll = true;
        }
    }
}