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
    public class ElectricToothbrush : ModItem
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
            Item.value = 2500000;
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
            
            player.runAcceleration *= 1.32f;
            player.runSlowdown *= 1.32f;
            player.jumpSpeedBoost += AGUtils.OnePerentJumpSpeed * 32;
            player.GetModPlayer<InfernusPlayer>().Ink_Storm_Equipped = true;

            player.npcTypeNoAggro[1] = true;
            player.npcTypeNoAggro[16] = true;
            player.npcTypeNoAggro[59] = true;
            player.npcTypeNoAggro[71] = true;
            player.npcTypeNoAggro[81] = true;
            player.npcTypeNoAggro[138] = true;
            player.npcTypeNoAggro[121] = true;
            player.npcTypeNoAggro[122] = true;
            player.npcTypeNoAggro[141] = true;
            player.npcTypeNoAggro[147] = true;
            player.npcTypeNoAggro[183] = true;
            player.npcTypeNoAggro[184] = true;
            player.npcTypeNoAggro[204] = true;
            player.npcTypeNoAggro[225] = true;
            player.npcTypeNoAggro[244] = true;
            player.npcTypeNoAggro[302] = true;
            player.npcTypeNoAggro[333] = true;
            player.npcTypeNoAggro[335] = true;
            player.npcTypeNoAggro[334] = true;
            player.npcTypeNoAggro[336] = true;
            player.npcTypeNoAggro[537] = true;
            player.npcTypeNoAggro[676] = true;
            player.npcTypeNoAggro[667] = true;
        }

        public override void AddRecipes()
        {/*
            CreateRecipe()
                .AddIngredient<Mouthwash>()
                .AddIngredient<LithiumIonBattery>()
                .AddIngredient<UnrealEssence>(50)
                .AddTile(TileID.LunarCraftingStation)
                .Register();*/
        }
    }
}