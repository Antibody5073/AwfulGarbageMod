using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons.Summon.Hardmode;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Global
{

    public class EquilibriumSentry : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool boost = false;
        public int tick = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            boost = false;
        }
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.sentry;
        }
        public override void AI(Projectile projectile)
        {
            if (Main.player[projectile.owner].GetModPlayer<GlobalPlayer>().EquilibriumSentryBonus && boost)
            {
                tick++;
                int radius = projectile.width;
                if (projectile.height > radius)
                {
                    radius = projectile.height;
                }
                radius /= 2;
                for (var j = 0; j < 2; j++)
                {
                    int dust = Dust.NewDust(projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((360 / 2) * j + tick * 2f)) + new Vector2(-4, -4), 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
                for (var j = 0; j < 2; j++)
                {
                    int dust = Dust.NewDust(projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((360 / 2) * j - tick * 2f)) + new Vector2(-4, -4), 0, 0, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
                if (tick > 60 * 10)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(5, 5);
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(projectile.GetSource_ReleaseEntity(), projectile.Center, vel.RotatedBy(i * MathHelper.TwoPi / 5), ModContent.ProjectileType<FireSentryProj>(), AGUtils.ScaleDamage(40, Main.player[projectile.owner], DamageClass.Summon), projectile.knockBack, projectile.owner);
                        Projectile.NewProjectile(projectile.GetSource_ReleaseEntity(), projectile.Center, vel.RotatedBy(i * MathHelper.TwoPi / 5 + MathHelper.Pi), ModContent.ProjectileType<FireSentryProj2>(), AGUtils.ScaleDamage(40, Main.player[projectile.owner], DamageClass.Summon), projectile.knockBack, projectile.owner);
                    }
                    projectile.Kill();
                }
            }
        }
    }
}