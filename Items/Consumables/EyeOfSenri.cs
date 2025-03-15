using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using AwfulGarbageMod.Systems;
using Terraria.Audio;

namespace AwfulGarbageMod.Items.Consumables
{
    //imported from my tAPI mod because I'm lazy
    public class EyeOfSenri : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.value = 0;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.consumable = false;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            return Main.expertMode;
        }

        public override bool? UseItem(Player player)
        {
            SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/rei_drain_00")
            {
                Volume = 0.5f,
            };
            SoundEngine.PlaySound(impactSound); 
            
            impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/seal_00")
            {
                Volume = 0.5f,
            };
            SoundEngine.PlaySound(impactSound);
            DifficultyModes.Difficulty++;
            if (Main.masterMode)
            {
                DifficultyModes.Difficulty %= 3;
            }
            else
            {
                DifficultyModes.Difficulty %= 2;
            }
            switch (DifficultyModes.Difficulty)
            {
                case 0:
                    Main.NewText("All special difficulty modes turned off.");
                    break;
                case 1:
                    Main.NewText("Unreal Mode: Welcome to unreality.", new Color(184, 65, 220));
                    break;
                case 2:
                    Main.NewText("Absurdly Extra Mode: This is ridiculous!!!", new Color(210, 173 ,173));
                    break;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}