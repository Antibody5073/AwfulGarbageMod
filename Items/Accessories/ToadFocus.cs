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
using StramClasses;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{

    public class ToadFocus : ModItem
	{
        public static LocalizedText StramEffect { get; private set; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "StramEffect", StramEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Toad Focus"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("6% increased crit chance\n4% further increased crit chance when staying still\nCritical strikes deal 20% more damage");
            StramEffect = this.GetLocalization(nameof(StramEffect));

        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 4;

            if (player.velocity == new Vector2(0, 0))
            {
                player.GetCritChance(DamageClass.Generic) += 4;
            }
            player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg += 0.12f;
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                DoStramClasses(player);
            }
        }

        [JITWhenModsEnabled("StramClasses")]
        public static void DoStramClasses(Player player)
        {
            player.rogue().critDamage += 0.06f;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod.Find<ModItem>("ContactLens").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("ToadEyes").Type);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}