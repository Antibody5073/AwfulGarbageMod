using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AwfulGarbageMod.Items.Tools
{
    public class SacredSkeletool : ModItem
    {
        int toolState = 1;
        public override void LoadData(TagCompound tag)
        {
            toolState = 1;
            Item.hammer = 0;
            Item.axe = 35;
            Item.pick = 235;
            base.LoadData(tag);
        }
        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 6;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 36); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
            Item.rare = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.tileBoost = 3;

            Item.hammer = 0;
            Item.axe = 35;
            Item.pick = 235;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;

        }

        public override void HoldItem(Player player)
        {
            if (toolState == 1)
            {
                Main.instance.MouseText("Pickaxe/Axe", Item.rare);
            }
            else
            {
                Main.instance.MouseText("Hammer/Axe", Item.rare);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            string key;
            if (toolState == 1)
            {
                Item.pick = 0;
                Item.hammer = 125;
                toolState = 2;
                key = "Hamaxe Form";
            }
            else
            {
                Item.pick = 235;
                Item.hammer = 0;
                toolState = 1;
                key = "Pickaxe Axe Form";
            }
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(200, 200, 200), key, dramatic: true);

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("Skeletool").Type);
            recipe.AddIngredient(ItemID.Picksaw);
            recipe.AddIngredient(ItemID.Ectoplasm, 18); 
            recipe.AddIngredient(ItemID.FrostCore, 1);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 1);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();
        }
    }
}