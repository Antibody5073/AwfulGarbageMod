using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class MagnosBlessing : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 250000;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().LifeMagnet = true;
            player.GetModPlayer<GlobalPlayer>().Proboscis = true;
            player.GetModPlayer<GlobalPlayer>().BloodMagnet = true;
            player.GetModPlayer<GlobalPlayer>().MagnosBlessing = true;
            player.hasLuckyCoin = true;
            player.goldRing = true;
            player.treasureMagnet = true;
            player.manaMagnet = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodMagnet>()
                .AddIngredient(ItemID.CelestialMagnet)
                .AddIngredient(ItemID.TreasureMagnet)
                .AddIngredient(ItemID.CoinRing)
                .AddIngredient<UnrealEssence>(50)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class ManaStarItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == 184 || entity.type == 1735 || entity.type == 1868;
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().MagnosBlessing)
            {
                player.AddBuff(ModContent.BuffType<ManaStarBuff>(), 60 * 5);
            }
            return base.OnPickup(item, player);
        }
    }
    public class CoinItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.IsACoin;
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().MagnosBlessing)
            {
                player.AddBuff(ModContent.BuffType<CoinBuff>(), 60 * 5);
            }
            return base.OnPickup(item, player);
        }
    }
}