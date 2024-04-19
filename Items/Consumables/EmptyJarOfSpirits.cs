using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;

namespace AwfulGarbageMod.Items.Consumables
{
    //imported from my tAPI mod because I'm lazy
    public class EmptyJarOfSpirits : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Foggy Lens"); 
            // Tooltip.SetDefault("Calls forth the storm observer");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13; // This helps sort inventory know this is a boss summoning Item.
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.Gray;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.value = Item.buyPrice(gold: 15);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
        }
    }
}