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

    public class ChloroplastCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meteorite Geode"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Melee damage summons a small meteor chunk from the sky\nMeteors summoned by sword strikes deal 25% more damage\nHas a two second cooldown");
        }


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 1500;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
            Item.lifeRegen = 1;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.velocity.Y < 0)
            {
                player.endurance = 1f - (0.85f * (1f - player.endurance));
            }
            player.GetModPlayer<GlobalPlayer>().ChloroplastCore = true;
        }
    }
}