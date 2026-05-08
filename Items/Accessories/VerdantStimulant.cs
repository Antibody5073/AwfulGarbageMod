using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Accessories
{
    public class VerdantStimulant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 5, 45, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EarthenShield>().verdantStimulant = true;
            player.GetDamage<SentinelDamageClass>() += 0.08f;
            player.GetCritChance<SentinelDamageClass>() += 8f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

}