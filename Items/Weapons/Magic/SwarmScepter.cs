using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Ranged;
using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class SwarmScepterBuff : ScepterBuff
    {
        public override int ScepterMaxLifePenalty => 10;
        public override int NonMagicDmgPenaltyPercent => 0;
        public override int ScepterProjType => ModContent.ProjectileType<SwarmScepterOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().beeDmg += 0.04f;
        }
    }

    public class SwarmScepter : ModItem
	{
        public static LocalizedText ScepterMax { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital slimeballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 12;
            ScepterMax = this.GetLocalization(nameof(ScepterMax));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "ScepterMax", ScepterMax.Format(ModItemSets.Sets.MaxScepterProjectiles[Item.type] + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost));
            tooltips.Add(tooltip);
        }

        public override void SetDefaults()
		{
            Item.damage = 17;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 0;
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
			Item.shoot = Mod.Find<ModProjectile>("SwarmScepterOrbit").Type;
			Item.shootSpeed = 0f;
			Item.noMelee = true;
            Item.buffType = ModContent.BuffType<SwarmScepterBuff>();

        }
        public override bool CanUseItem(Player player)
        {
            return (player.ownedProjectileCounts[Mod.Find<ModProjectile>("SwarmScepterOrbit").Type] < ModItemSets.Sets.MaxScepterProjectiles[Item.type] + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("SwarmScepterOrbit").Type] < ModItemSets.Sets.MaxScepterProjectiles[Item.type] + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("SwarmScepterOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
	}

    public class SwarmScepterOrbit : ScepterOrbit
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            GlobalProjectiles.Sets.IsScepterProjectile[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 4;
        }
        public override float ProjSpd => 6;
        public override float OrbitDistance => 120;
        public override float OrbitSpd => -1f;
        public override float OffsetDir => 0;
        public override int ProjType => ProjectileID.Bee;

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
        public override float DmgMult()
        {
            Player player = Main.player[Projectile.owner];

            float dmgMult = 1;
            dmgMult += player.GetDamage(DamageClass.Generic).Additive - 1;
            dmgMult += player.GetDamage(DamageClass.Magic).Additive - 1;
            dmgMult += player.GetDamage<ScepterDamageClass>().Additive - 1;


            dmgMult *= player.GetDamage(DamageClass.Generic).Multiplicative;
            dmgMult *= player.GetDamage(DamageClass.Magic).Multiplicative;
            dmgMult *= player.GetDamage<ScepterDamageClass>().Multiplicative;
            dmgMult *= player.GetModPlayer<GlobalPlayer>().beeDmg;
            return dmgMult;
        }

        public override void ReleaseProjectiles(float direction, float spd, int type)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(spd, 0).RotatedBy(direction), Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage), 0, Main.myPlayer);
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

        public override void Visuals()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bee, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].noGravity = true;

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
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
}