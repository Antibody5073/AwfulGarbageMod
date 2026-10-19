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
using AwfulGarbageMod.Tiles.Furniture;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class VirtuousVambrace : ModItem
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
            Item.value = 750000;
            Item.rare = 8;
            Item.accessory = true;
            Item.defense = 3;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetDamage(DamageClass.Melee) += 0.08f;
            player.GetCritChance(DamageClass.Melee) += 8f;
            player.GetModPlayer<GlobalPlayer>().Enlightenment += 300;
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.08f;
            if (player.HeldItem.DamageType == DamageClass.MeleeNoSpeed || (player.HeldItem.CountsAsClass(DamageClass.Melee) && player.HeldItem.attackSpeedOnlyAffectsWeaponAnimation) || ItemID.Sets.Yoyo[player.HeldItem.type])
            {
                player.GetAttackSpeed(DamageClass.Melee) += 0.4f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeteoricManica>()
                .AddIngredient<Enlightenment>()
                .AddIngredient(ItemID.SoulofLight, 30)
                .AddIngredient(ItemID.ButterflyDust)
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}