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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class SnowScepterBuff : ScepterBuff
    {
        public override int ScepterMaxManaPenalty => 5;
        public override int NonMagicDmgPenaltyPercent => 0;
        public override int ScepterProjType => ModContent.ProjectileType<SnowScepterOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.GetKnockback(DamageClass.Generic) += 0.05f;
        }
    }

    public class SnowScepter : ModItem
    {
        public static LocalizedText ScepterMax { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Snow Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital Snowballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 8;
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
            Item.damage = 23;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 5;
            Item.width = 42;
            Item.height = 46;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.knockBack = 0.1f;
            Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.crit = 2;
            Item.shoot = Mod.Find<ModProjectile>("SnowScepterOrbit").Type;
            Item.shootSpeed = 0f;
            Item.noMelee = true;
            Item.buffType = ModContent.BuffType<SnowScepterBuff>();


            Item.scepterItem().scepter = true;
            Item.scepterItem().MaxScepterProjectiles = 8;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("SnowScepterOrbit").Type] < Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("SnowScepterOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Snowball, 150);
            recipe.AddIngredient(ItemID.BorealWood, 12);
            recipe.AddIngredient(ItemID.Shiverthorn, 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }

    public class SnowScepterOrbit : ScepterOrbit
    {

        public override float ProjSpd => 14;
        public override float OrbitDistance => 65;
        public override float OrbitSpd => 1.2f;
        public override float OffsetDir => 0;
        public override int ProjType => ProjectileID.SnowBallFriendly;

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

        public class SnowScepterProj : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                // DisplayName.SetDefault("Snowball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            }

            public override void SetDefaults()
            {
                Projectile.DamageType = DamageClass.Magic;
                Projectile.width = 16;
                Projectile.height = 16;
                Projectile.aiStyle = -1;
                Projectile.friendly = true;
                Projectile.penetrate = 1;
                Projectile.timeLeft = 600;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = true;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 12;
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
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

                int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Snow, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.25f;
                Main.dust[dust].noGravity = true;
                Projectile.velocity.Y += 0.05f;
            }
        }
    }
}