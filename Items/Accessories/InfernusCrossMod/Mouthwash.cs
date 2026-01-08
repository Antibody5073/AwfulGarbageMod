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
using rail;
using Infernus;

namespace AwfulGarbageMod.Items.Accessories.InfernusCrossMod
{

    [ExtendsFromMod("Infernus")]
    public class Mouthwash : ModItem
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
            Item.value = 300000;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.runAcceleration *= 1.24f;
            player.runSlowdown *= 1.24f;
            player.jumpSpeedBoost += AGUtils.OnePerentJumpSpeed * 24;
            player.GetJumpState(ExtraJump.GoatMount).Enable();
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
        {
            CreateRecipe()
                .AddIngredient<WitheredLeaf>()
                .AddIngredient(ItemID.RoyalGel)
                .AddIngredient<Infernus.Items.Accesories.Squid_Expert>()
                .AddIngredient<Infernus.Items.Accesories.Shiny>()
                .AddIngredient<UnrealEssence>(30)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}