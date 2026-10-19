using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class HeavenlyBurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavenly Burst"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Hold down the attack button to charge up the bow, increasing arrows shot to a max of 5 charges(8 arrows)");
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.noMelee = true;
            Item.useAnimation = 30;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = 15000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 14f;
            Item.crit = 0;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, Vector2.Zero, Mod.Find<ModProjectile>("HeavenlyBurstDummy").Type, damage, knockback, player.whoAmI, Item.shootSpeed, type);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.GoldBow);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe2.AddIngredient(ItemID.Cloud, 20);
            recipe2.AddIngredient(ItemID.GoldBow);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.DemoniteBar, 8);
            recipe3.AddIngredient(ItemID.Cloud, 20);
            recipe3.AddIngredient(ItemID.PlatinumBow);
            recipe3.AddTile(TileID.Anvils);
            recipe3.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe4.AddIngredient(ItemID.Cloud, 20);
            recipe4.AddIngredient(ItemID.PlatinumBow);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
    }

    public class HeavenlyBurstDummy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("a"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 0;
        }
        int ChargeTime = 0;
        int chargeLevel = 0;
        int visualTimer = 0;
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2 = ModContent.Request<Texture2D>("AwfulGarbageMod/Items/Weapons/Ranged/HeavenlyBurstArrow1").Value;

            int arrowsShot = 0;
            switch (chargeLevel)
            {
                case 1:
                    arrowsShot = 1;
                    break;
                case 2:
                    arrowsShot = 2;
                    break;
                case 3:
                    arrowsShot = 3;
                    break;
                case 4:
                    arrowsShot = 5;
                    break;
                case 5:
                    arrowsShot = 8;
                    break;
            }
            float dir = (-2.25f * (arrowsShot - 1)) / 2;

            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            int frameHeight = texture2.Height;
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture2.Width, frameHeight);

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
            Vector2 drawOrigin = origin;

            visualTimer++;
            for (var i = 0; i < arrowsShot; i++)
            {
                //spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(texture2,
                    Projectile.Center - Main.screenPosition + new Vector2(64, 0).RotatedBy(Projectile.rotation + MathHelper.ToRadians(dir)),
                    sourceRectangle, Color.White, Projectile.rotation + MathHelper.ToRadians(dir), origin, Projectile.scale, spriteEffects, 0);

                if (visualTimer < 10)
                {

                    Main.EntitySpriteDraw(texture2,
                        Projectile.Center - Main.screenPosition + new Vector2(64, 0).RotatedBy(Projectile.rotation + MathHelper.ToRadians(dir)),
                        sourceRectangle, Color.White * ((10 - visualTimer) / 10f), Projectile.rotation + MathHelper.ToRadians(dir), origin, Projectile.scale * (1 + visualTimer / 10f), spriteEffects, 0);
                }
                dir += 2.25f;
            }
            return false;
        }

        public override bool PreAI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.netUpdate = true;
            }
            Player owner = Main.player[Projectile.owner];

            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            Projectile.Center = owner.Center;
            if (!owner.channel)
            {
                return true; // Let the vanilla whip AI run.
            }
            Projectile.timeLeft = 3;
            Projectile.rotation = (Main.MouseWorld - Projectile.position).ToRotation();

            if (ChargeTime % owner.HeldItem.useTime == 0) // 1 segment per 12 ticks of charge.
            {
                if (chargeLevel < 5)
                {
                    visualTimer = 0;
                    chargeLevel++;
                    if (chargeLevel == 5)
                    {
                        if (owner.whoAmI == Main.myPlayer)
                        {
                            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                        }
                    }
                    else
                    {
                        if (owner.whoAmI == Main.myPlayer)
                        {
                            SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
                        }
                    }
                    for (var i = 0; i < 8; i++)
                    {
                        float xv = Main.rand.NextFloat(-3, 3);
                        float yv = Main.rand.NextFloat(-2, -5);
                        int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.MagicMirror, xv, yv, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                    }
                }
            }
            ChargeTime++;
            // Reset the animation and item timer while charging.
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;
            owner.itemRotation = Projectile.rotation;
            if (owner.Center.X < Main.MouseWorld.X)
            {
                owner.direction = (int)MathHelper.ToRadians(90);
            }
            else
            {
                owner.direction = (int)MathHelper.ToRadians(-90);
                owner.itemRotation += (int)MathHelper.ToRadians(180);
            }


            return false; // Prevent the vanilla whip AI from running.
        }

        public override void OnKill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];

            int arrowsShot = 0;
            switch (chargeLevel)
            {
                case 1:
                    arrowsShot = 1;
                    break;
                case 2:
                    arrowsShot = 2;
                    break;
                case 3:
                    arrowsShot = 3;
                    break;
                case 4:
                    arrowsShot = 5;
                    break;
                case 5:
                    arrowsShot = 8;
                    break;
            }
            float dir = (-2.25f * (arrowsShot - 1)) / 2;
            for (var i = 0; i < arrowsShot; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(dir) + Projectile.rotation), (int)Projectile.ai[1], Projectile.damage, Projectile.knockBack, Projectile.owner);
                dir += 2.25f;

            }
        }
    }
}