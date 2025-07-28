using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Ammo
{
    public class PoisonPellet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stone Pellet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Low damage, easy to obtain bullet");
        }

        public override void SetDefaults()
        {
            Item.damage = 5; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 2f;
            Item.value = 15;
            Item.rare = ItemRarityID.Green;
            Item.shoot = Mod.Find<ModProjectile>("PoisonPelletProj").Type; // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 5f; // The speed of the projectile.
            Item.ammo = AmmoID.Bullet; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(75);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiderLeg").Type, 1);
            recipe.AddIngredient(Mod.Find<ModItem>("StonePellet").Type, 75);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe(75);
            recipe2.AddIngredient(Mod.Find<ModItem>("SpiderLeg").Type, 1);
            recipe2.AddIngredient(ItemID.MusketBall, 75);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
    public class PoisonPelletProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fossil Bullet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        bool hitEnemy = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hitEnemy)
            {
                target.AddBuff(BuffID.Poisoned, 600);
                Projectile.damage = (int)(Projectile.damage * 0.8);
                hitEnemy = true;
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
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}