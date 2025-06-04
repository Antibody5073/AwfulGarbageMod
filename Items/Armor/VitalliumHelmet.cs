using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class VitalliumHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Sanguine Hood");
			// Tooltip.SetDefault("80 increased max mana");

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true; Item.defense = 5; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetJumpState<VitalliumJump>().Enable();
            player.noFallDmg = true;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<VitalliumBreastplate>() && legs.type == ModContent.ItemType<VitalliumLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Reverts the Unreal Mode buff changes"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().DisabledUnrealBuffNerfs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VitalliumBar>(14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class VitalliumJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(BlizzardInABottle);


        public override float GetDurationMultiplier(Player player)
        {
            // Use this hook to set the duration of the extra jump
            // The XML summary for this hook mentions the values used by the vanilla extra jumps
            return 3.5f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            // Use this hook to modify "player.runAcceleration" and "player.maxRunSpeed"
            // The XML summary for this hook mentions the values used by the vanilla extra jumps
            player.runAcceleration *= 8f;
            player.maxRunSpeed *= 1.5f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            // Use this hook to trigger effects that should appear at the start of the extra jump
            // This example mimics the logic for spawning the puff of smoke from the Cloud in a Bottle
            int offsetY = player.height;
            if (player.gravDir == -1f)
                offsetY = 0;

            offsetY -= 16;

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position + new Vector2(-34f, offsetY), 102, 32, DustID.LifeCrystal, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, Scale: 1.5f);
                dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
            }
        }


        public override void ShowVisuals(Player player)
        {
            // Use this hook to trigger effects that should appear throughout the duration of the extra jump
            // This example mimics the logic for spawning the dust from the Blizzard in a Bottle
            int offsetY = player.height - 6;
            if (player.gravDir == -1f)
                offsetY = 6;

            Vector2 spawnPos = new Vector2(player.position.X, player.position.Y + offsetY);

            Dust dust = Dust.NewDustDirect(spawnPos, player.width, 12, DustID.LifeCrystal, player.velocity.X * -0.4f, player.velocity.Y * -0.6f);
            dust.fadeIn = 1.5f;
            dust.noGravity = false;
            dust.noLight = true;

        }
    }
}
