using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class NaturalSpell : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Natural Spell"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots leaves that scatter upon enemy hit");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
		{
			Item.damage = 11;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("SylvanTomeProj").Type;
			Item.shootSpeed = 7f;
			Item.noMelee = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = 0; i < 5; i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity + Main.rand.NextVector2Circular(0.35f, 0.35f), Mod.Find<ModProjectile>("NaturalSpellProj").Type, damage, knockback, player.whoAmI);

            }
			return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("SylvanTome").Type);
            recipe.AddIngredient(ItemID.JungleSpores, 18);
            recipe.AddIngredient(ItemID.Stinger, 12);
            recipe.AddIngredient(ItemID.Vine, 10);
            recipe.AddIngredient(ItemID.CorruptSeeds, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("SylvanTome").Type);
            recipe2.AddIngredient(ItemID.JungleSpores, 18);
            recipe2.AddIngredient(ItemID.Stinger, 12);
            recipe2.AddIngredient(ItemID.Vine, 10);
            recipe2.AddIngredient(ItemID.CrimsonSeeds, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }

    public class NaturalSpellProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 31))) * Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = TextureAssets.Projectile[206].Value;

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
            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
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

            int dust = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 1, 1, DustID.Grass, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

            if (StateTimer % 5 == 0)
            {
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}