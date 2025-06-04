using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class AmethystGemcornStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.mana = 16;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 21;
			Item.useAnimation = 21;
			Item.useStyle = 5;
			Item.knockBack = 4f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("AcornStaffProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText((int)(Item.mana * player.manaCost));
            int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("AmethystGemcornStaffProj").Type, damage, knockback, player.whoAmI, 0, (int)(Item.mana * player.manaCost));
            return false;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<AcornStaff>()
                .AddIngredient(ItemID.AmethystStaff)
                .AddIngredient(ItemID.GemTreeAmethystSeed, 5)
                .AddIngredient(ItemID.FallenStar, 20)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
	}

    public class AmethystGemcornStaffProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }

        bool regenMana = true;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.LocalPlayer;
            if (regenMana)
            {
                player.ManaEffect((int)Projectile.ai[1]);
                player.statMana += (int)Projectile.ai[1];
                //Main.NewText((int)Projectile.ai[1]);
                regenMana = false;
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AmethystGemcornStaffExplosion>(), hit.SourceDamage / 2, 2, player.whoAmI);

                for (int j = 0; j < 3; j++)
                {
                    float rotat = Main.rand.NextFloatDirection();
                    for (int i = 0; i < 32; i++)
                    {
                        Vector2 vel = new Vector2((float)Math.Cos(MathHelper.TwoPi * i / 16), (float)Math.Sin(MathHelper.TwoPi * i / 16) * 5);
                        vel = vel.RotatedBy(rotat);
                        int dust = Dust.NewDust(Projectile.position + new Vector2(3, 3), Projectile.width - 3, Projectile.height - 3, DustID.GemAmethyst, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
                        Main.dust[dust].velocity = vel;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].alpha = 120;
                    }
                }
            }
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(225f);
            if (Projectile.timeLeft % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, 0f, 0f, 0, default(Color), 1f);
            }
        }
    }
    public class AmethystGemcornStaffExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }
}