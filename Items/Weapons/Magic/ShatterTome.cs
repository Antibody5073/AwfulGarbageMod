using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class ShatterTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shatter Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots a shattering icicle \nShards have armor piercing capabilities");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 22;
			Item.mana = 4;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 29;
			Item.useAnimation = 29;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("ShatterTomeProj").Type;
			Item.shootSpeed = 8f;
			Item.noMelee = true;
		}
        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FrostShard").Type, 14);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 3);
            recipe.AddIngredient(ItemID.BorealWood, 30);
			recipe.AddIngredient(ItemID.IceBlock, 30);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

    public class ShatterTomeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Icicle"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[Type].Value;

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
            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 4; i++)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * -3 + Main.rand.NextVector2Circular(8, 8);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + vel, vel, ModContent.ProjectileType<ShatterTomeSplit>(), Projectile.damage / 2, 0f, Projectile.owner);
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.Center, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(80, 115) * 0.013f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position += new Vector2(-2, -2);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 4; i++)
            {
                Vector2 vel = oldVelocity.SafeNormalize(Vector2.Zero) * -3 + Main.rand.NextVector2Circular(8, 8);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + vel, vel, ModContent.ProjectileType<ShatterTomeSplit>(), Projectile.damage / 2, 0f, Projectile.owner);
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 12;
            height = 12;
            return true;
        }
    }

    public class ShatterTomeSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 15;
        }
        float spin = 0;
        public override void OnSpawn(IEntitySource source)
        {
            spin = Main.rand.NextFloat(-0.15f, 0.15f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[Type].Value;

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
            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position += new Vector2(-2, -2);
            Projectile.rotation += spin;
        }
    }
}