using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Tools
{
    public class FlintHandAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7;
            Item.value = Item.buyPrice(silver: 50); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;


            Item.axe = 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}