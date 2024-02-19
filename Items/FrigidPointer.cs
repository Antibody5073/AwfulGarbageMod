using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Systems;
using Terraria.DataStructures;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items
{
    public class FrigidPointer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("\"Doesn't seem to melt\""); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }

        public override void HoldItem(Player player)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("FrigidPointerProj").Type, 0, 0, player.whoAmI);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FrostShard").Type, 5);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 5);
            recipe.AddIngredient(Mod.Find<ModItem>("FrigidiumOre").Type, 20);
            recipe.AddIngredient(ItemID.IceBlock, 45);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class FrigidPointerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            /*
            float offsetX = 0;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 0;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            */

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void AI()
        {
            Vector2 IcePalacePos = new Point16(Structures.IcePalacePosX, Structures.IcePalacePosY).ToWorldCoordinates();

            Player player = Main.player[Projectile.owner];

            float DistanceToPalace = Vector2.Distance(IcePalacePos, player.Center);
            if (DistanceToPalace > 125)
            {
                DistanceToPalace = 125;
            }

            Vector2 targetPos = player.MountedCenter + (IcePalacePos - player.Center).SafeNormalize(new Vector2(1, 0)) * DistanceToPalace
                - new Vector2(Projectile.width / 2, Projectile.height / 2);

            Projectile.rotation = (IcePalacePos - player.Center).SafeNormalize(new Vector2(1, 0)).ToRotation();

            Projectile.position = targetPos;

        }
    }
}