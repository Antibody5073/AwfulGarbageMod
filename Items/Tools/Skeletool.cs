using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AwfulGarbageMod.Items.Tools
{
    public class Skeletool : ModItem
    {
        int toolState = 1;
        public override void LoadData(TagCompound tag)
        {
            toolState = 1;
            Item.hammer = 0;
            Item.axe = 24;
            Item.pick = 145;
            base.LoadData(tag);
        }
        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 11;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 12); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
            Item.rare = 4;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;


            Item.tileBoost = 1;

            Item.hammer = 0;
            Item.axe = 24;
            Item.pick = 145;
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
                Item.hammer = 85;
                toolState = 2;
                key = "Hamaxe Form";
            }
            else
            {
                Item.pick = 145;
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
            recipe.AddIngredient(Mod.Find<ModItem>("Fraxeture").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Toothpick").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Bonecrush").Type);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}