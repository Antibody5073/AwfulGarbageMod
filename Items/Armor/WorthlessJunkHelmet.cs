using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using StramClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class WorthlessJunkHelmet : ModItem
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


        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("WorthlessJunk Mask");
			// Tooltip.SetDefault("40 increased maximum health");

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = false; // Draw all hair as normal. Used by Mime Mask, Sunglasses
                                                                   // ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
                                                                   // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
            StramEffect = this.GetLocalization(nameof(StramEffect));

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
            player.manaCost *= 0.82f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.3f;
            player.whipRangeMultiplier += 0.3f;
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                DoStramClasses(player);
            }
        }
        [JITWhenModsEnabled("StramClasses")]
        public static void DoStramClasses(Player player)
        {
            player.GetModPlayer<GlobalStramPlayer>().rogueVelocity += 0.25f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<WorthlessJunkBreastplate>() && legs.type == ModContent.ItemType<WorthlessJunkLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Striking an enemy with a weapon increases the damage of other classes by 10% for 10 seconds.\nFor example, hitting an emeny with a melee weapon increases ranged, magic, and summon damage by 10%.\nThis effect stacks for each class.\n Removes the non-magic damage penalties of scepters"; // This is the setbonus tooltip
			player.GetModPlayer<GlobalPlayer>().IgnoreScepterDmgPenalties = true;
			player.GetModPlayer<GlobalPlayer>().WorthlessJunkBonus = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<Garbage>(150)
               .AddTile<Tiles.Furniture.TrashMelter>()
               .Register();
        }
    }
}
