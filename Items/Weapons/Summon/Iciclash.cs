using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Weapons.Summon
{
    public class Iciclash : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloud Chain"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Enemies struck by summons take 8% increased damage");
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<IciclashProj>(), 20, 1.5f, 5.5f);
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.rare = 2;
            Item.autoReuse = true;
            Item.value = 15000;
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrigidiumBar>(22)
                .AddIngredient<FrostShard>(16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IciclashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        List<Vector2> postions = new List<Vector2>();

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();
            // use these to change from the vanilla defaults
            Projectile.WhipSettings.Segments = 27;
            // Projectile.WhipSettings.RangeMultiplier = 1f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        // This example uses PreAI to implement a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<IciclashBuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.6f);
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {

            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                
                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            postions.Clear();

            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            //Main.DrawWhip_WhipBland(Projectile, list);
            // The code below is for custom drawing.
            // If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
            // However, you must adhere to how they draw if you do.

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 14, 26);
                Vector2 origin = new Vector2(5, 8);
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (i == list.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                postions.Add(pos);

                pos += diff;
            }
            return false;
        }
        public override void AI()
        {
            Player player = Main.LocalPlayer;


            for (int i = 0; i < postions.Count; i++)
            {
                if (i == 1)
                {
                    int dust = Dust.NewDust(postions[postions.Count - 1], 1, 1, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true; 
                }
            }

        }
    }
    public class IciclashProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        Vector2 origVel;
        float spdMult;
        int StateTimer = 0;


        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            if (AGUtils.IsOnScreen(Projectile, new Vector2(32, 32) * Vector2.Distance(Vector2.Zero, Projectile.velocity)))
            {
                SpriteEffects spriteEffects = SpriteEffects.None;

                // Getting texture of projectile
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

                Vector2 origin = sourceRectangle.Size() / 2f;
                Color drawColor = Projectile.GetAlpha(lightColor);

                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = origin;
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(6f, 6f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
                }

                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

                // It's important to return false, otherwise we also draw the original texture.
            }
            return false;
        }
        public override void AI()
        {
            StateTimer++;
                Projectile.velocity.Y += 0.15f;
                if (Projectile.velocity.Y > 8)
                {
                    Projectile.velocity.Y = 8;
                }
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(315);

        }
    }
}