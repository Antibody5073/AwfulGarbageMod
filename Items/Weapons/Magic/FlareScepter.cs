using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class FlareScepterBuff : ScepterBuff
    {
        public override int ScepterMaxManaPenalty => 15;
        public override int NonMagicDmgPenaltyPercent => 5;
        public override int ScepterProjType => ModContent.ProjectileType<FlareScepterOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.maxRunSpeed += 0.05f;
            player.moveSpeed += 0.05f;
            player.runAcceleration *= 1.05f;
            player.runSlowdown *= 1.05f;
            player.statDefense += 1;
        }
    }

    public class FlareScepter : ModItem
    {
        public static LocalizedText ScepterMax { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital slimeballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 6;
            ScepterMax = this.GetLocalization(nameof(ScepterMax));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "ScepterMax", ScepterMax.Format(Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost));
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
		{
			Item.damage = 66;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 5;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("FlareScepterOrbit").Type;
			Item.noMelee = true;
            Item.buffType = ModContent.BuffType<FlareScepterBuff>();

            Item.scepterItem().scepter = true;
            Item.scepterItem().MaxScepterProjectiles = 6;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("FlareScepterOrbit").Type] < Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("FlareScepterOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
	}

    public class FlareScepterOrbit : ScepterOrbit
    {

        public override float ProjSpd => 15;
        public override float OrbitDistance => 100;
        public override float OrbitSpd => 1.3f;
        public override float OffsetDir => 45;
        public override int ProjType => ModContent.ProjectileType<FlareScepterProj>();
        public override int AssociatedBuff => ModContent.BuffType<FlareScepterBuff>();


        public override void SetStaticDefaults()
        {
            GlobalProjectiles.Sets.IsScepterProjectile[Projectile.type] = true;

            Main.projFrames[Projectile.type] = 3;
        }
        public int[] frames = { 0, 1, 2, 1 };
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 mousePos = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
            ReleaseProjectiles((mousePos - Projectile.Center).ToRotation(), ProjSpd, ProjType);

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
        public override void Visuals()
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            int frameSpeed = 4;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= frames.Length)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }

    public class FlareScepterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            Main.projFrames[Projectile.type] = 3;
        }
        public int[] frames = { 0, 1, 2, 1 };

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 450;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 720);
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

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            int frameSpeed = 4;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= frames.Length)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}