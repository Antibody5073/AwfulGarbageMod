using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static AssGen.Assets;
using Terraria.GameContent;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Consumables.BossSummon
{
    //imported from my tAPI mod because I'm lazy
    public class SlimeCrownUnreal : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Insect on a Stick"); 
            // Tooltip.SetDefault("Will attract a great forest-dwelling amphibian");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13; // This helps sort inventory know this is a boss summoning Item.
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;

            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true; Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
        }

        // We use the CanUseItem hook to prevent a player from using this Item while the boss is present in the world.
        public override bool CanUseItem(Player player)
        {
            // "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
            return !NPC.AnyNPCs(NPCID.KingSlime);
        }
        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.KingSlime);
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
        
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AwfulGarbageMod/Items/Consumables/BossSummon/SlimeCrownUnreal").Value;
            for (int i = 0; i < 4; i++)
            {
                Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2;
                spriteBatch.Draw(texture, position + offsetPositon, null, new Color((byte)MathHelper.Lerp(69, 199, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(25, 41, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(112, 255, (Main.mouseTextColor - 190) / 65f), 100), 0, origin, scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AwfulGarbageMod/Items/Consumables/BossSummon/SlimeCrownUnreal").Value;
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f);
            // We redraw the item's sprite 4 times, each time shifted 2 pixels on each direction, using Main.DiscoColor to give it the color changing effect
            for (int i = 0; i < 4; i++)
            {
                Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2;
                spriteBatch.Draw(texture, position + offsetPositon, null, new Color((byte)MathHelper.Lerp(69, 199, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(25, 41, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(112, 255, (Main.mouseTextColor - 190) / 65f), 100), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SlimeCrown)
                .AddIngredient<UnrealEssence>(20)
                .Register();
        }
    }
}