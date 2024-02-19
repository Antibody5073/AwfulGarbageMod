using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using StramClasses;
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

    public abstract class ScepterBuff : ModBuff
    {

        public virtual int ScepterMaxManaPenalty => 0;
        public virtual int ScepterMaxLifePenalty => 0;
        public virtual int NonMagicDmgPenaltyPercent => 0;
        public virtual int ScepterProjType => -1;


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeballs"); // Buff display name
            // Description.SetDefault("Orbiting slimeballs");
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ScepterProjType] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                int maxManaLost = 0;
                int maxLifeLost = 0;
                for (int i = 0; i < player.ownedProjectileCounts[ScepterProjType]; i++)
                {
                    if (!DoNotReduceNonMagicDmg(player) && !player.GetModPlayer<GlobalPlayer>().IgnoreScepterDmgPenalties) {
                        player.GetDamage(DamageClass.Melee) -= NonMagicDmgPenaltyPercent / 100f;
                        player.GetDamage(DamageClass.Ranged) -= NonMagicDmgPenaltyPercent / 100f;
                        player.GetDamage(DamageClass.Summon) -= NonMagicDmgPenaltyPercent / 100f;

                        if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
                        {
                            StramClassesRogueDmg(player, NonMagicDmgPenaltyPercent / 100);
                        }
                    }
                    maxManaLost += ScepterMaxManaPenalty;
                    maxLifeLost += ScepterMaxLifePenalty;

                    ScepterEffects(player);

                    player.GetModPlayer<GlobalPlayer>().scepterProjectiles += 1;
                }

                maxManaLost = (int)(maxManaLost * player.GetModPlayer<GlobalPlayer>().ScepterMaxManaMultPrev);
                maxLifeLost = (int)(maxLifeLost * player.GetModPlayer<GlobalPlayer>().ScepterMaxManaMultPrev);

                if (!CancelMaxStatPenalties(player))
                {
                    player.statManaMax2 -= maxManaLost;
                    player.statLifeMax2 -= maxLifeLost;
                }
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
        public virtual bool DoNotReduceNonMagicDmg(Player player)
        {
            return false;
        }
        public virtual bool CancelMaxStatPenalties(Player player)
        {
            return false;
        }
        public virtual void ScepterEffects(Player player)
        {

        }

        [JITWhenModsEnabled("StramClasses")]
        public static void StramClassesRogueDmg(Player player, int percentDmg)
        {
            player.rogue().critDamage -= percentDmg;
            player.GetDamage(StramUtils.rogueDamage()) -= percentDmg;

        }

    }

   

    public abstract class ScepterOrbit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeball"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            GlobalProjectiles.Sets.IsScepterProjectile[Projectile.type] = true;
        }

        public float dir;

        public virtual float ProjSpd => 0;
        public virtual int ProjType => -1;
        public virtual float OrbitDistance => 100;
        public virtual float OrbitSpd => 1;
        public virtual float OffsetDir => 0;




        public virtual float DmgMult()
        {
            Player player = Main.player[Projectile.owner];

            float dmgMult = 1;
            dmgMult += player.GetDamage(DamageClass.Generic).Additive - 1;
            dmgMult += player.GetDamage(DamageClass.Magic).Additive - 1;
            dmgMult += player.GetDamage<ScepterDamageClass>().Additive - 1;


            dmgMult *= player.GetDamage(DamageClass.Generic).Multiplicative;
            dmgMult *= player.GetDamage(DamageClass.Magic).Multiplicative;
            dmgMult *= player.GetDamage<ScepterDamageClass>().Multiplicative;
            return dmgMult;
        }

        public override void OnKill(int timeLeft)
        {
            ReleaseProjectiles(dir, ProjSpd, ProjType);
        }

        public virtual void ReleaseProjectiles(float direction, float spd, int type)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(spd, 0).RotatedBy(direction), ProjType, (int)(Projectile.damage * DmgMult()), Projectile.knockBack, Projectile.owner);

        }

        public virtual void GetOrbitDirection(out float tempdir, out Vector2 targetPos, float spd = 1, float distance = 120)
        {

            Player player = Main.player[Projectile.owner];
            tempdir = MathHelper.ToRadians((player.GetModPlayer<GlobalPlayer>().OrbitalDir * spd) + Projectile.ai[0] * 360 / player.ownedProjectileCounts[this.Type]);
            targetPos = player.MountedCenter + new Vector2(distance, 0).RotatedBy(tempdir) - new Vector2(Projectile.width / 2, Projectile.height / 2);
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

        public virtual void SetRotation(Player player, float direction, float offsetDir = 0)
        {
            Projectile.rotation = direction + MathHelper.ToRadians(offsetDir);
            dir = direction;

        }

        public virtual void SetPositionBasedOnDirection(float direction, Vector2 targetPos)
        {
            Projectile.position = targetPos;
        }

        public override void AI()
        {
            GetOrbitDirection(out float tempDir, out Vector2 targetPos, OrbitSpd, OrbitDistance);
            SetPositionBasedOnDirection(tempDir, targetPos);

            Player player = Main.player[Projectile.owner];

            SetRotation(player, tempDir, OffsetDir);

            Projectile.timeLeft += 1;


            Visuals();
        }

        public virtual void Visuals()
        {

        }
    }
}