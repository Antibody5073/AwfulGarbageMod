using AwfulGarbageMod.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class EmeraldGemcornStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 29;
			Item.mana = 8;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 33;
			Item.useAnimation = 33;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
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
            int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("EmeraldGemcornStaffProj").Type, damage, knockback, player.whoAmI, 0, (int)(Item.mana * player.manaCost));
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AcornStaff>());
            recipe.AddIngredient(ItemID.GemTreeEmeraldSeed, 6);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<AcornStaff>());
            recipe2.AddIngredient(ItemID.GemTreeEmeraldSeed, 2);
            recipe2.AddIngredient(ItemID.EmeraldStaff);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
            CreateRecipe()
                .AddIngredient<AcornStaff>()
                .AddIngredient(ItemID.GemTreeEmeraldSeed, 6)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
	}

    public class EmeraldGemcornStaffProj : ModProjectile
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
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
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
            }

            target.AddBuff(ModContent.BuffType<EmeraldDefenseBuff>(), 180);
            int debuff = Main.rand.Next(10);
            if (debuff == 0 || debuff == 1 || debuff == 2)
            {
                target.AddBuff(BuffID.OnFire, 75);
            }
            if (debuff == 3 || debuff == 4)
            {
                target.AddBuff(BuffID.Frostburn, 75);
            }
            if (debuff == 5 || debuff == 6)
            {
                target.AddBuff(BuffID.Poisoned, 90);
            }
            if (debuff == 7)
            {
                target.AddBuff(BuffID.Confused, 45);
            }
            if (debuff == 8)
            {
                target.AddBuff(BuffID.Wet, 150);
            }
            if (debuff == 9)
            {
                target.AddBuff(BuffID.Slimed, 150);
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
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald, 0f, 0f, 0, default(Color), 1f);
            }
        }
    }
}