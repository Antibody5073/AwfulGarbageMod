using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.DamageClasses;
using StramClasses;
using StramClasses.Classes.Rogue;
using StramClasses.Classes.Rogue.Projectiles;
using AwfulGarbageMod.Global;
using Terraria.GameContent;
using StramClasses.Classes.Rogue.Weapons;
using Terraria.Audio;

namespace AwfulGarbageMod.Items.Weapons.Rogue
{
    [ExtendsFromMod("StramClasses")]

    public class SkyDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.toRogueItem(19);
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 18;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.5f;
            Item.value = 1000;
            Item.rare = 2;
            SoundStyle soundStyle = new SoundStyle("StramClasses/Assets/Sounds/RogueKnife");
            soundStyle.Volume = 3f;
            soundStyle.PitchVariance = 0.5f;
            SoundStyle value = soundStyle;
            base.Item.UseSound = value; Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("SkyDaggerProj").Type;
            Item.shootSpeed = 8f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            RoguePlayer roguePlayer = player.rogue();

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].rogueProjectile().critDamage = (int)Math.Round(roguePlayer.critDamage * (float)base.Item.rogueItem().baseCritDamage * base.Item.rogueItem().prefixCritDamage + (float)roguePlayer.critDamageFlat);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 20)
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.GoldDagger>()
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 20)
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.GoldDagger>()
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 20)
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.PlatinumDagger>()
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 20)
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.PlatinumDagger>()
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddIngredient(ItemID.Feather, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class SkyDaggerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        int aiState = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = StramUtils.rogueDamage();
            Projectile.rogueProjectile().splittableWeapon = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RoguePlayer roguePlayer = player.rogue();

            int proj;

            proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(4)) * 1.6f, Mod.Find<ModProjectile>("SkyDaggerProj2").Type, Projectile.damage / 2, 0.5f, player.whoAmI);
            Main.projectile[proj].rogueProjectile().critDamage = Projectile.rogueProjectile().critDamage / 2;
            Main.projectile[proj].rogueProjectile().critDamageModifier /= 2;
            Main.projectile[proj].CritChance = Projectile.CritChance;
            Main.projectile[proj].velocity = Main.projectile[proj].velocity / player.GetModPlayer<GlobalStramPlayer>().rogueVelocity;

            proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(-4)) * 1.6f, Mod.Find<ModProjectile>("SkyDaggerProj2").Type, Projectile.damage / 2, 0.5f, player.whoAmI);
            Main.projectile[proj].rogueProjectile().critDamage = Projectile.rogueProjectile().critDamage / 2;
            Main.projectile[proj].rogueProjectile().critDamageModifier /= 2;
            Main.projectile[proj].CritChance = Projectile.CritChance;
            Main.projectile[proj].velocity = Main.projectile[proj].velocity / player.GetModPlayer<GlobalStramPlayer>().rogueVelocity;
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
            base.Projectile.rotation += 0.3f * (float)base.Projectile.direction;
            base.Projectile.spriteDirection = base.Projectile.direction;
            base.Projectile.ai[0] += 1f;
            

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.6f);
            Main.dust[dust].noGravity = true;
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class SkyDaggerProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = StramUtils.rogueDamage();
            Projectile.rogueProjectile().splittableWeapon = false;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Harpy, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity = new Vector2(2, 0).RotatedByRandom(MathHelper.ToRadians(360));
                Main.dust[dust].noGravity = true;
            }
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            float num = (Main.player[base.Projectile.owner].rogue().moonMagnet ? (-1f) : 1f);
            base.Projectile.velocity.Y = base.Projectile.velocity.Y + num * 0.15f;
            base.Projectile.velocity.X = base.Projectile.velocity.X * 0.97f;
        }
    }


}