using AwfulGarbageMod.Global;
using StramClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class WorthlessJunkBreastplate : ModItem
	{
        public static LocalizedText StramEffect { get; private set; }

        public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("WorthlessJunk Breastplate");
            // Tooltip.SetDefault("20 increased maximum health\nIncreases maximum number of minions by 1");
            StramEffect = this.GetLocalization(nameof(StramEffect));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "StramEffect", StramEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 6; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.statManaMax2 += 40;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.2f;
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                DoStramClasses(player);
            }
        }
        [JITWhenModsEnabled("StramClasses")]
        public static void DoStramClasses(Player player)
        {
            player.rogue().critDamageFlat += 6;
        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Garbage>(90)
                .AddTile<Tiles.Furniture.TrashMelter>()
                .Register();
        }
    }
}
