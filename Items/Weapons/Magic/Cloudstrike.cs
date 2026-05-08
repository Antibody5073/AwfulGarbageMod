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

    public class Cloudstrike : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 23;
			Item.mana = 6;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 5;
			Item.knockBack = 3;
			Item.value = 15000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("CloudstrikeProj").Type;
			Item.shootSpeed = 15f;
			Item.noMelee = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DemoniteBar, 11);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.Feather, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimtaneBar, 11);
            recipe2.AddIngredient(ItemID.Cloud, 20);
            recipe2.AddIngredient(ItemID.Feather, 4);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
		}
	}

    public class CloudstrikeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wind"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 3;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        Vector2 origVel;
        float spdMult;
        int StateTimer = 0;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            origVel = Projectile.velocity;
            spdMult = 1;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.98f);
            spdMult = 0.1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AwfulGarbageMod/Items/Weapons/Magic/CloudstrikeProjTrail");

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height;
            int startY = frameHeight;


            // Get this frame on texture
            Rectangle sourceRectangle;

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin; 
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                if (k > 0)
                {
                    sourceRectangle = new Rectangle(0, startY, texture.Width, (int)Math.Ceiling(frameHeight * Vector2.Distance(Projectile.oldPos[k], Projectile.oldPos[k-1]) / 20 - 0.5f));
                    origin = sourceRectangle.Size() / 2f;
                    Vector2 drawPos = (Projectile.oldPos[k] + Projectile.oldPos[k-1])/2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = (byte)(color.A * 0.75f);
                    Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale, spriteEffects, 0f);
                }
            }
            texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            startY = frameHeight * Projectile.frame;
            sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            StateTimer++;
            if (StateTimer % 5 == 0)
            {
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.aiStyle = -1;
            int dust = Dust.NewDust(Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), 1, 1, DustID.Cloud, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

            spdMult += (1 - spdMult) / 36;
            Projectile.velocity = new Vector2(spdMult * Projectile.ai[0], spdMult * Projectile.ai[1]);

        }
    }
}