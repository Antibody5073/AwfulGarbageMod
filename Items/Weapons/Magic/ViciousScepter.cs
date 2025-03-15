using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Ranged;
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

    public class ViciousScepterBuff : ScepterBuff
    {
        public override int ScepterMaxManaPenalty => 25;
        public override int NonMagicDmgPenaltyPercent => 10;
        public override int ScepterProjType => ModContent.ProjectileType<ViciousScepterOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.02f;
            player.statDefense += 4;
            player.aggro += 50;
        }
    }

    public class ViciousScepter : ModItem
	{
        public static LocalizedText ScepterMax { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital slimeballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 3;
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
            Item.damage = 54;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 10;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 7f;
			Item.value = 50000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("ViciousScepterOrbit").Type;
			Item.shootSpeed = 0f;
			Item.noMelee = true;
            Item.buffType = ModContent.BuffType<ViciousScepterBuff>();
            Item.scepterItem().scepter = true;
            Item.scepterItem().MaxScepterProjectiles = 3;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("ViciousScepterOrbit").Type] < Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("ViciousScepterOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.CrimtaneBar, 15)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddIngredient(ItemID.ViciousPowder, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}

    public class ViciousScepterOrbit : ScepterOrbit
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            GlobalProjectiles.Sets.IsScepterProjectile[Projectile.type] = true;
        }
        public override float ProjSpd => 9;
        public override float OrbitDistance => 150;
        public override float OrbitSpd => -0.7f;
        public override float OffsetDir => 0;
        public override int ProjType => ModContent.ProjectileType<ViciousScepterProj>();

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


        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = ((new Vector2(1, 0).RotatedBy(Projectile.rotation).X <= 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);

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

        public override void SetRotation(Player player, float direction, float offsetDir = 0)
        {
            float maxDetectRadius = 700f;
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
            {
                base.SetRotation(player, direction, offsetDir);

                return;
            }
            dir = (closestNPC.Center - Projectile.Center).ToRotation();
            Projectile.rotation = dir;

        }

        public override void Visuals()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].noGravity = true;

        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance && lineOfSight)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }

    public class ViciousScepterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Crystal"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }


        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 15; i++)
            {
                float xv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(2, 5);
                float yv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(2, 5);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.CorruptGibs, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.3f);
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = ((new Vector2(1, 0).RotatedBy(Projectile.rotation).X <= 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);

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
            Projectile.rotation = Projectile.velocity.ToRotation();


            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].noGravity = true;
            
        }
    }
}