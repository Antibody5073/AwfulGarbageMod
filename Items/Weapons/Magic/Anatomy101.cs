using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using System.Runtime.Serialization.Formatters;
using AwfulGarbageMod.Projectiles;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class Anatomy101 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloudstrike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots a gust of wind that temporarily slows down on enemy hits");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = 5;
            Item.knockBack = 3;
            Item.value = 15000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.crit = 0;
            Item.shootSpeed = 12f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int seed = Main.rand.Next(4);
            int proj;
            switch (seed)
            {
                case 0:
                    for (var i = 0; i < Main.rand.Next(3, 4); i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0.6f, 1.2f), ModContent.ProjectileType<Anatomy101Proj>(), (int)(damage * 0.9f), knockback * 0.7f, player.whoAmI);
                    }
                    break;
                case 1:
                    for (var i = 0; i < 5; i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(8)) * Main.rand.NextFloat(1.2f, 1.3f), ModContent.ProjectileType<TissueStormProj>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                        Main.projectile[proj].extraUpdates += 1;
                    }
                    break;
                case 2:
                    proj = Projectile.NewProjectile(source, position, velocity * 0.4f, ModContent.ProjectileType<NecropotenceProj>(), (int)(damage * 2.5f), knockback * 1.75f, player.whoAmI);
                    Main.projectile[proj].extraUpdates += 1;
                    break;
                case 3:
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-12)), ModContent.ProjectileType<OcularScepterProj>(), (int)(damage), 0, player.whoAmI);
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-36)), ModContent.ProjectileType<OcularScepterProj>(), (int)(damage), 0, player.whoAmI);
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(12)), ModContent.ProjectileType<OcularScepterProj>(), (int)(damage), 0, player.whoAmI);
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(36)), ModContent.ProjectileType<OcularScepterProj>(), (int)(damage), 0, player.whoAmI);
                    break;
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ToothpickTome>()
                .AddIngredient<TissueStorm>()
                .AddIngredient<BloodVolley>()
                .AddIngredient<NaturalSpell>()
                .AddTile(TileID.DemonAltar)
                .Register();
            CreateRecipe()
                .AddIngredient<ToothpickTome>()
                .AddIngredient<ScaleShardStorm>()
                .AddIngredient<BloodVolley>()
                .AddIngredient<NaturalSpell>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class Anatomy101Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wind"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
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
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            origVel = Projectile.velocity;
            spdMult = 1;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void AI()
        {
            StateTimer++;

            if (Projectile.timeLeft % 6 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90);
        }
    }
}