using AwfulGarbageMod;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Items.Consumables
{
    //imported from my tAPI mod because I'm lazy
    public class EyeOfSenri : ModItem
    {
        public static LocalizedText DifficultyOff { get; private set; }
        public static LocalizedText Unreal { get; private set; }
        public static LocalizedText AbEX { get; private set; }

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
            return Main.masterMode;
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
            switch (DifficultyModes.Difficulty)
            {
                case 0:
                    DifficultyModes.Difficulty = 1;
                    Main.NewText("Unreal Mode: Welcome to unreality.", new Color(184, 65, 220));
                    //ChatHelper.BroadcastChatMessage(Unreal.ToNetworkText(), new Color(184, 65, 220));
                    break;
                case 1:
                    DifficultyModes.Difficulty = 2;
                    Main.NewText("Absurdly Extra Mode: This is ridiculous!!!", new Color(210, 173, 173));
                    //ChatHelper.BroadcastChatMessage(AbEX.ToNetworkText(), new Color(210, 173, 173));
                    break;
                case 2:
                    DifficultyModes.Difficulty = 0;
                    Main.NewText("All special difficulty modes turned off.", Color.White);
                    //ChatHelper.BroadcastChatMessage(DifficultyOff.ToNetworkText(), Color.White);
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