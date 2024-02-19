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
using AwfulGarbageMod.Buffs;

namespace AwfulGarbageMod.Items.Weapons.Rogue
{
    [ExtendsFromMod("StramClasses")]

    public class DaggerOfEgestedPoison : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Throw 2-3 knives at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.toRogueItem(18);
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 16;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = 1000;
            Item.rare = 2;
            SoundStyle soundStyle = new SoundStyle("StramClasses/Assets/Sounds/RogueKnife");
            soundStyle.Volume = 3f;
            soundStyle.PitchVariance = 0.5f;
            SoundStyle value = soundStyle;
            base.Item.UseSound = value; Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("DaggerOfEgestedPoisonProj").Type;
            Item.shootSpeed = 7.5f;
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
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.DaggerOfIngestedPoison>()
                .AddIngredient<SpiderLeg>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class DaggerOfEgestedPoisonProj : ModProjectile
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
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 3600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
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

        public int timer;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Poisoned, 0f, 0f, 0, default(Color), 0.9f);
                Main.dust[num].noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(in SoundID.Dig, base.Projectile.Center);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Poisoned, 420);
            }
            if (hit.Crit)
            {
                target.AddBuff(ModContent.BuffType<EgestedPoisonBuff>(), 240);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Poisoned, 420);
            }
        }
        public override void AI()
        {
            base.Projectile.rotation += 0.2f * (float)base.Projectile.direction;
            base.Projectile.spriteDirection = base.Projectile.direction;
            base.Projectile.ai[0] += 1f;
            if (base.Projectile.ai[0] >= 30f)
            {
                float num = (Main.player[base.Projectile.owner].rogue().moonMagnet ? (-1f) : 1f);
                base.Projectile.velocity.Y = base.Projectile.velocity.Y + num * 0.15f;
                base.Projectile.velocity.X = base.Projectile.velocity.X * 0.97f;
            }

            if (timer == 0)
            {
                timer = 24;
                SoundEngine.PlaySound(in SoundID.Item7, base.Projectile.position);
            }

            timer--;

        }
    }
}