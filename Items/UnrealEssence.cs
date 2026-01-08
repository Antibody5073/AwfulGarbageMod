using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;

namespace AwfulGarbageMod.Items
{
    public class UnrealEssence : ModItem
    {
		public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 45));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 1000; // Makes the item worth 1 gold.
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
        }
        
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AwfulGarbageMod/Items/UnrealEssenceGlow").Value;
            for (int i = 0; i < 4; i++)
            {
                Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2;
                spriteBatch.Draw(texture, position + offsetPositon, null, new Color((byte)MathHelper.Lerp(69, 199, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(25, 41, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(112, 255, (Main.mouseTextColor - 190) / 65f), 100), 0, origin, scale, SpriteEffects.None, 0f);
            }

            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AwfulGarbageMod/Items/UnrealEssenceGlow").Value;
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f);
            // We redraw the item's sprite 4 times, each time shifted 2 pixels on each direction, using Main.DiscoColor to give it the color changing effect
            for (int i = 0; i < 4; i++)
            {
                Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 2 + new Vector2(0, 2);
                spriteBatch.Draw(texture, position + offsetPositon, null, new Color((byte)MathHelper.Lerp(69, 199, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(25, 41, (Main.mouseTextColor - 190) / 65f), (byte)MathHelper.Lerp(112, 255, (Main.mouseTextColor - 190) / 65f), 100), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
            // Return true so the original sprite is drawn right after
            return true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, new Color(215, 71, 255).ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255); // Makes this item render at full brightness.
        }
    }
}
