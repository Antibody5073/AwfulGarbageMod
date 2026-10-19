using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Consumables
{
    public class AccursedElixir : ModItem
    {
        public static LocalizedText RestoreLifeText { get; private set; }

        public override void SetStaticDefaults()
        {
            RestoreLifeText = this.GetLocalization(nameof(RestoreLifeText));

            Item.ResearchUnlockCount = 30;
        }
        
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(gold: 30);
            Item.mana = 1;

            Item.healLife = 50; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
            Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
        }


        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 50 + player.statMana / 2;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            mult = player.statMana;
        }
        public override void OnConsumeItem(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().AccursedElixir = true;
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(15)
                .AddIngredient(ItemID.GreaterHealingPotion, 15)
                .AddIngredient(ItemID.Ectoplasm, 3)
                .AddIngredient<HeartOfBlight>()
                .AddTile(TileID.Bottles) // Making this recipe be crafted at bottles will automatically make Alchemy Table's effect apply to its ingredients.
                .Register();
        }
    }
    public class AccursedMana : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.healMana > 0;
        }
        public override bool ConsumeItem(Item item, Player player)
        {
            return !player.HasItem(ModContent.ItemType<AccursedElixir>());
        }
    }
}