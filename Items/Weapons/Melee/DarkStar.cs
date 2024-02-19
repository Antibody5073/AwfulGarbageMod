using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class DarkStar : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Star"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Stacks up to 3");
		}

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 10;
			Item.useStyle = 1;
			Item.knockBack = 3.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("DarkStarProj").Type;
            Item.shootSpeed = 15f;
            Item.maxStack = 3;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < Item.stack;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.IronBar, 5);
			recipe.AddIngredient(ItemID.FallenStar, 2);
			recipe.AddIngredient(ItemID.Deathweed, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.LeadBar, 5);
            recipe2.AddIngredient(ItemID.FallenStar, 2);
            recipe2.AddIngredient(ItemID.Deathweed, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}

    public class DarkStarProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Star"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        bool returning = false;
        float spinSpd;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.scale = 0.7f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            returning = true;
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;
            }
            Vector2 vel = new Vector2(Main.player[Projectile.owner].Center.X - Projectile.Center.X,
            Main.player[Projectile.owner].Center.Y - Projectile.Center.Y);
            vel.Normalize();
            Projectile.velocity = vel * 12f;
            Projectile.extraUpdates = Projectile.timeLeft;
            Projectile.tileCollide = false;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;
            }
            if (!returning)
            {
                returning = true;
                Vector2 vel = new Vector2(Main.player[Projectile.owner].Center.X - Projectile.Center.X,
                Main.player[Projectile.owner].Center.Y - Projectile.Center.Y);
                vel.Normalize();
                Projectile.velocity = vel * 12f;
                Projectile.extraUpdates = Projectile.timeLeft;
                Projectile.tileCollide = false;
            }
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
            if (returning)
            {
                if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) < 15)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.timeLeft == 400)
                {
                    spinSpd = Projectile.velocity.X * 0.06f;
                    if (spinSpd > 0.2f) { spinSpd = 0.2f; }
                    if (spinSpd < -0.2f) { spinSpd = -0.2f; }
                }
                else if (Projectile.timeLeft < 385)
                {
                    Projectile.velocity.Y += 1f;
                    if (Projectile.velocity.Y > 20)
                    {
                        Projectile.velocity.Y = 20;
                    }
                }
                Projectile.rotation += spinSpd;
            }
        }
    }
}