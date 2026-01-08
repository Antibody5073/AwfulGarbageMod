using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Infernus;
using Infernus.Buffs;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.InfernusCrossMod
{

    [ExtendsFromMod("Infernus")]
    public class LithiumIonBattery : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stormy Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n12% decreased summon damage\n8% increased movement speed");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 600000;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
            Item.defense = 6;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.velocity.Y == 0)
            {
                player.endurance = 1f - (0.8f * (1f - player.endurance));
            }
            else
            {
                player.endurance = 1f - (0.9f * (1f - player.endurance));
            }
            if (player.HasBuff(BuffID.Electrified))
            {
                player.GetDamage(DamageClass.Generic) += 0.20f;
            }
            player.fireWalk = true;
            player.noFallDmg = true;
            player.GetModPlayer<Infernus.InfernusPlayer>().Enchanted_Femur = true;
            Main.LocalPlayer.GetModPlayer<InfernusPlayer>().Boulder_Equipped = true;

            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[BuffID.CursedInferno] = true;

            player.AddBuff(ModContent.BuffType<Boulder_Buff>(), 2);

        }

        public override void AddRecipes()
        {/*
            CreateRecipe()
                .AddIngredient<ShockAbsorber>()
                .AddIngredient<SoulflowerSeed>()
                .AddIngredient(ItemID.ObsidianHorseshoe)
                .AddIngredient<Infernus.Items.Weapon.HardMode.Accessories.Boulder_Expert>()
                .AddIngredient<Infernus.Items.Accesories.Enchanted_Femur>()
                .AddIngredient<UnrealEssence>(30)
                .AddTile(TileID.TinkerersWorkbench)
                .Register(); 
            CreateRecipe()
                .AddIngredient<ShockAbsorber>()
                .AddIngredient<BloodflowerSeed>()
                .AddIngredient(ItemID.ObsidianHorseshoe)
                .AddIngredient<Infernus.Items.Weapon.HardMode.Accessories.Boulder_Expert>()
                .AddIngredient<Infernus.Items.Accesories.Enchanted_Femur>()
                .AddIngredient<UnrealEssence>(30)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();*/
        }
    }
}