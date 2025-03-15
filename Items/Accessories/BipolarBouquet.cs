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
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Accessories
{

    public class BipolarBouquet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lightning Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Mana potions are automatically consumed\nDisable visibility to disable auto-drinking mana potions\nMana sickness wears off twice as fast\nDrinking a mana potion will electrify you for two seconds\n5% reduced mana usage");
		}

		public int counter;

		public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 30000;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetCritChance(DamageClass.Magic) += 5;

            if (player.velocity.X == 0)
            {
                player.GetDamage(DamageClass.Magic) += 15 / 100f;
            }
            if (player.velocity.Y == 0)
            {
                player.manaCost *= 0.75f;
            }
            else
            {
                player.manaCost *= 0.9f;
            }
            player.GetDamage(DamageClass.Magic) += 10 / 100f;
            player.GetModPlayer<GlobalPlayer>().FrozenLotus += 0.07f;
            player.GetModPlayer<GlobalPlayer>().MoltenRose += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BipolarEmblem>()
                .AddIngredient<FrozenLotus>()
                .AddIngredient<MoltenRose>()
                .AddIngredient<Cryogem>(10)
                .AddIngredient<Pyrogem>(10)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}