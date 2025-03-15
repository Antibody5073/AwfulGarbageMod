using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class LeafyDisc : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tsunami Throw"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Bursts into water on enemy impact");
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 20;
			Item.noMelee = true;
			Item.scale = 0f;
            Item.maxStack = 2;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 4.5f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<LeafyDiscProj>();
            Item.shootSpeed = 14f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < Item.stack;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<EnchantedLeaf>(7)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
	}

    public class LeafyDiscProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tsunami Throw"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        int StateTimer = 0;

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
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            StateTimer++;

        }
    }
}