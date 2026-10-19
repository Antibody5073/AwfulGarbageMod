using Infernus.Items.Weapon.HardMode.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class ShellShardStorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shell Shard Storm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots many shell shards \nShards have armor piercing capabilities");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 4;
			Item.mana = 2;
			Item.DamageType = DamageClass.Magic;
			Item.width = 33;
			Item.height = 30;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("ShellProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
            Item.ArmorPenetration = 16;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            {
                for (var i = 0; i < Main.rand.Next(1, 4); i++)
                {
					int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(10)) * Main.rand.Next(80, 120) * 0.01f, Mod.Find<ModProjectile>("ShellProj").Type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Magic) + (int)player.GetCritChance(DamageClass.Generic) + 4;

                }
            }
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Coral, 3);
            recipe.AddIngredient(ItemID.Seashell, 6);
            recipe.AddIngredient(ItemID.Starfish, 3);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}

    public class ShellProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        public int[] frames = { 0, 1, 2, 1 };

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 3);
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
            int startY = frameHeight * frames[Projectile.frame];

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
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.friendly = true;
            Projectile.penetrate = Main.rand.Next(2) + 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
    }
}