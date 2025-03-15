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

    public class FrostFlareBoomerang : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tsunami Throw"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Bursts into water on enemy impact");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 30;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 4.5f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("FrostFlareBoomerangProj").Type;
            Item.shootSpeed = 16f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<FrigidiumBar>(12)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Flamarang)
                .AddIngredient(ItemID.IceBoomerang)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}

    public class FrostFlareBoomerangProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tsunami Throw"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
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
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 1;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 5; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, new Vector2(11, 0).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("FrostFlareBoomerangProjFlame").Type, (int)(Projectile.damage * 0.8f), 0, Projectile.owner);
                Main.projectile[proj].CritChance = Projectile.CritChance;
            }
            for (var i = 0; i < 3; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(Main.rand.NextFloat(-120, 120), Main.rand.NextFloat(-600, -720)), Vector2.Zero, Mod.Find<ModProjectile>("FrostFlareBoomerangProjIcicle").Type, (int)(Projectile.damage * 0.7f), 0, Projectile.owner, ai1: target.Bottom.Y);
                Main.projectile[proj].CritChance = Projectile.CritChance;
                Main.projectile[proj].velocity = (target.Center - Main.projectile[proj].Center).SafeNormalize(Vector2.Zero) * 10f;
                Main.projectile[proj].velocity += (target.velocity / 3);


            }
        }
        public override void AI()
        {
            StateTimer++;

            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Water, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(80, 115) * 0.013f;
            Main.dust[dust].noGravity = true;
        }
    }
    public class FrostFlareBoomerangProjFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Stream"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.aiStyle = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
            for (var i = 0; i < 15; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].noGravity = true;
            }
        }



        public override void AI()
        {
            if (Vector2.Distance(Projectile.Center, Main.player[Main.myPlayer].Center) < 1000)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.25f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity = Projectile.velocity * 0.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
    public class FrostFlareBoomerangProjIcicle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bone Toothpick"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(18))
            {
                target.AddBuff(BuffID.Frostburn, 240);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 8; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Ice, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            if (Projectile.position.Y < Projectile.ai[1])
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.timeLeft % 2 == 0)
            {
                int dust;
                Vector2 vel = Projectile.velocity;

                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = vel * -0.5f;
            }
        }
    }
}