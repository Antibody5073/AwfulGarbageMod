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
using System.Linq;

namespace AwfulGarbageMod.Items.Accessories
{

    [AutoloadEquip(EquipType.Wings)]
    public class ThrushGliders : ModItem
    {

        public override void SetStaticDefaults()
        {
            // These wings use the same values as the solar wings
            // Fly time: 90 ticks = 1 seconds
            // Fly speed: 9
            // Acceleration multiplier: 1.25
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(75, 6f, 1.5f);

        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().ThrushGliders = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f; // Falling glide speed
            ascentWhenRising = 0.25f; // Rising speed
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 1f;
            constantAscend = 0.15f;
        }
    }
}
