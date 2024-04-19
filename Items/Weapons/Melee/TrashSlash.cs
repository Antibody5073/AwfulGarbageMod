using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using Terraria.GameContent.Items;
using AwfulGarbageMod.Items.Placeable.Furniture;
using Microsoft.Xna.Framework.Graphics;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class TrashSlash : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Tidal Slicer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots water every other swing \nSmall chance to shoot three");
        }

        public int counter;

		

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 40;
			Item.useAnimation = 22;
			Item.useStyle = 1;
			Item.knockBack = 5;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("TrashSlashProj").Type;
			Item.shootSpeed = 10f;
			Item.crit = 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("TrashSlashProj").Type, (int)(damage * 1.35f), knockback / 2, player.whoAmI);
            player.statLife -= 5;
            if (player.statLife < 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} littered too much."), 5, 0);
            }    
            else
            {
                player.HealEffect(-5);
            }
            return false;
		}


		public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<Garbage>(75)
                .AddTile<Tiles.Furniture.TrashMelter>()
                .Register();
        }
	}

    public class TrashSlashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Star"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 3;

        }

        float spinSpd;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 3);
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

            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity *= 0.7f;
            if (Vector2.Distance(oldVelocity, Vector2.Zero) > 2) {
                for (var i = 0; i < 5; i++)
                {
                    float xv = Projectile.velocity.X * Main.rand.NextFloat(0.2f, 0.7f);
                    float yv = Projectile.velocity.Y * Main.rand.NextFloat(0.2f, 0.7f);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, xv, yv, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].noGravity = true;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            for (var i = 0; i < 7; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Stone, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;
            }
            player.Heal(5);
            
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft < 570)
            {
                if (Vector2.Distance(player.Center, Projectile.Center) < 24)
                {
                    player.Heal(5);
                    Projectile.Kill();
                }
            }
            if (Projectile.velocity.X > 0)
            {
                spinSpd = 1;
            }
            else
            {
                spinSpd = -1;
            }

            spinSpd *= Vector2.Distance(Projectile.velocity, Vector2.Zero) * 0.08f;

            Projectile.rotation += spinSpd;

            if (Projectile.velocity.Y < 7)
            {
                Projectile.velocity.Y += 0.12f;
            }

        }
    }
}