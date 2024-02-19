using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class RottingHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Rotting Mask");
			// Tooltip.SetDefault("20 reduced maximum health\n5% increased summon damage");

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 -= 20;
            player.GetDamage(DamageClass.Summon).Flat += 2;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<RottingBreastplate>() && legs.type == ModContent.ItemType<RottingLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summons an Eater of Souls that attaches to enemies, reducing their defense by 3 and increases their damage taken by 8%.\n2 defense\n12% increased movement speed"; // This is the setbonus tooltip
            player.moveSpeed += 0.12f; // Increase the movement speed of the player
            player.maxRunSpeed += 0.12f;
            player.statDefense += 2;
            player.GetModPlayer<GlobalPlayer>().RottingBonus = true;
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("RottingMinion").Type] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("RottingMinion").Type, 0, 0, Main.myPlayer);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.RottenChunk, 5);
            recipe.AddIngredient(ItemID.Ebonwood, 20);
            recipe.AddIngredient(ItemID.VilePowder, 10);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.RottenChunk, 5);
            recipe2.AddIngredient(ItemID.Ebonwood, 20);
            recipe2.AddIngredient(ItemID.VilePowder, 10);
            recipe2.AddIngredient(ItemID.TinBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
