using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class AsteroidBeltBuff : ScepterBuff
    {

        public override int ScepterMaxManaPenalty => 10;
        public override int NonMagicDmgPenaltyPercent => 5;
        public override int ScepterProjType => ModContent.ProjectileType<AsteroidBeltOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.statDefense += 3;
        }

        public override bool DoNotReduceNonMagicDmg(Player player)
        {
            return player.GetModPlayer<GlobalPlayer>().MeteoriteVisorBonusPrev;
        }

        public override bool CancelMaxStatPenalties(Player player)
        {
            return player.GetModPlayer<GlobalPlayer>().MeteoriteVisorBonusPrev;
        }
    }

    public class AsteroidBelt : ModItem
    {

        public static LocalizedText ScepterMax { get; private set; }
        public static LocalizedText MeteoritePenalty { get; private set; }


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital slimeballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 6;
            ScepterMax = this.GetLocalization(nameof(ScepterMax));
            MeteoritePenalty = this.GetLocalization(nameof(MeteoritePenalty));
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");

            if (player.GetModPlayer<GlobalPlayer>().MeteoriteVisorBonus == false)
            {
                tooltip = new TooltipLine(Mod, "Penalty", MeteoritePenalty.Format());

                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (line != null)
                {
                    line.Text += tooltip.Text;
                }
            }
            tooltip = new TooltipLine(Mod, "ScepterMax", ScepterMax.Format(Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost));

            line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
		{
			Item.damage = 75;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 5;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 2f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("AsteroidBeltOrbit").Type;
			Item.shootSpeed = 0f;
			Item.noMelee = true;
            Item.buffType = ModContent.BuffType<AsteroidBeltBuff>();
            Item.scepterItem().scepter = true;
            Item.scepterItem().MaxScepterProjectiles = 6;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("AsteroidBeltOrbit").Type] < Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("AsteroidBeltOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MeteoriteBar, 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}

    public class AsteroidBeltOrbit : ScepterOrbit
    {

        public override float ProjSpd => 15;
        public override float OrbitDistance => 90;
        public override float OffsetDir => 45;
        public override int ProjType => ModContent.ProjectileType<AsteroidBeltProj>();


        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void Visuals()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Meteorite, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            Main.dust[dust].noGravity = true;

            if (Main.rand.NextBool(3))
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[dust2].scale = 1.25f;
                Main.dust[dust2].noGravity = true;
            }
        }
    }

    public class AsteroidBeltProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
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

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Meteorite, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            Main.dust[dust].noGravity = true;

            if (Main.rand.NextBool(3))
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[dust2].scale = 1.25f;
                Main.dust[dust2].noGravity = true;
            }
            Projectile.velocity.Y += 0.25f;

        }
    }
}