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

namespace AwfulGarbageMod.Items.Accessories
{

    public class RegalCrystalEmblem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dualism Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting player line.
			// Tooltip.SetDefault("16% increased magic damage when not moving horizontally\n21% reduced mana cost when not moving vertically\n3% increased magic crit chance\n\"Aside from the pepperoni-pizza smell, it's perfect!\"");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.expert = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().IlluminantString = true;
            }
            player.GetDamage(DamageClass.Summon) += 6 / 100f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.maxMinions += 1; 
            player.maxTurrets += 1;
            player.moveSpeed += 0.12f;
            player.VolatileGelatin(Item);
            player.volatileGelatin = true;
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
                 .AddIngredient<CrystalEmblem>()
                 .AddIngredient(ItemID.RoyalGel)
                 .AddIngredient(ItemID.VolatileGelatin)
                 .AddTile(TileID.TinkerersWorkbench)
                 .Register();
        }
	}
}