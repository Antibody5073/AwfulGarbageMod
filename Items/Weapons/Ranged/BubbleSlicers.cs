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
using Terraria.ModLoader.Config;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class BubbleSlicers : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Throw 2-3 knives at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 69;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 30;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = 30000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BubbleSlicersProj>();
            Item.shootSpeed = 10f;
        }
    }

    public class BubbleSlicersProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        float spinSpd;
        int aiState = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
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
            if (aiState == 1)
            {
                Projectile.velocity.Y += 0.33f;
                Projectile.velocity.X *= 0.99f;
                if (Projectile.velocity.Y > 15)
                {
                    Projectile.velocity.Y = 15;
                }
            }
                if (Projectile.timeLeft % 10 == 5)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(5, 5), ProjectileID.FlaironBubble, Projectile.damage / 3, 0, Projectile.owner, -10);
                    Main.projectile[proj].ArmorPenetration += 5;
                    Main.projectile[proj].DamageType = ModContent.GetInstance<KnifeDamageClass>();

                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
                    Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().Empowerments = Projectile.GetGlobalProjectile<KnifeProjectile>().Empowerments;
                }

            if (Projectile.timeLeft == 420)
            {
                aiState = 1;
            }

            Projectile.rotation += MathHelper.ToRadians(22);
        }
    }

}