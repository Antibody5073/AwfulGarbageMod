using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{
    public class PlantyShieldCoating : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 13;
            Item.value = Item.buyPrice(0, 4, 50);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        int Timer = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EarthenShield>().shieldDurabilityMult += 0.1f;
            player.GetModPlayer<EarthenShield>().shieldRegenStrength += 0.75f;
        }
    }

}