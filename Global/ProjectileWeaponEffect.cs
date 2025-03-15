using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Printing;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using Mono.Cecil;
using Terraria.Audio;
using Terraria.Localization;
using AwfulGarbageMod.Items.Accessories;
using Microsoft.CodeAnalysis;
using AwfulGarbageMod.Items.Weapons.Ranged;
using System.Security.Policy;

namespace AwfulGarbageMod.Global
{

    public class ProjectileWeaponEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int frames;
        public bool DeathStrings = false;
        public bool HotGlock = false;
        public bool FireFirearm = false;
        public bool noDaybreak = false;
        public bool Shadowflame = false;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.friendly;
        }

        public override void AI(Projectile projectile)
        {
            frames++;
            if (DeathStrings)
            {
                if (frames % 40 == 20)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, ModContent.ProjectileType<DeathStringsProj>(), projectile.damage / 4, projectile.knockBack / 5, projectile.owner, 0, 1.2f);
                }
            }
            if (Shadowflame)
            {
                for (int i = 0; i < 2; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0.1f;
                    Main.dust[dust].noGravity = true;
                }
            }
            base.AI(projectile);
        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DeathStrings)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<DeathStringsProj>(), projectile.damage / 2, projectile.knockBack / 5, projectile.owner, 60, 0.9f);
                }
            }
            if (hit.Crit)
            {
                if (HotGlock)
                {
                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Main.rand.NextVector2CircularEdge(3.5f, 3.5f), ModContent.ProjectileType<HotGlockProj>(), hit.Damage / 2, projectile.knockBack / 5, projectile.owner);
                    Main.projectile[proj].ArmorPenetration += 999;
                    Main.projectile[proj].CritChance = 0;
                }
                if (FireFirearm)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Main.rand.NextVector2CircularEdge(3.5f, 3.5f), ModContent.ProjectileType<HotGlockProj>(), hit.Damage * 2 / 5, projectile.knockBack / 5, projectile.owner);
                        Main.projectile[proj].timeLeft *= 2;
                        Main.projectile[proj].ArmorPenetration += 999;
                        Main.projectile[proj].CritChance = 0;
                    }
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Main.rand.NextVector2CircularEdge(6.5f, 3f) + new Vector2(0, 2), ProjectileID.MolotovFire3, hit.Damage / 3, projectile.knockBack / 5, projectile.owner);
                        Main.projectile[proj].ArmorPenetration += 999;
                        Main.projectile[proj].CritChance = 0;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 24;
                        Main.projectile[proj].penetrate += 3;
                    }
                }
            }

            if (noDaybreak)
            {
                if (target.HasBuff(BuffID.Daybreak))
                {
                    target.DelBuff(target.FindBuffIndex(BuffID.Daybreak));
                }
            }
            if (Shadowflame)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(4, 4);
                    float num102 = velocity.X;
                    float num113 = velocity.Y;
                    Vector2 vector2 = new Vector2(num102, num113);
                    vector2.Normalize();
                    vector2 *= 4f;
                    Vector2 vector3 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    vector3.Normalize();
                    vector2 += vector3;
                    vector2.Normalize();
                    vector2 *= 12;
                    float num77 = (float)Main.rand.Next(10, 80) * 0.001f;
                    if (Main.rand.Next(2) == 0)
                    {
                        num77 *= -1f;
                    }
                    float num87 = (float)Main.rand.Next(10, 80) * 0.001f;
                    if (Main.rand.Next(2) == 0)
                    {
                        num87 *= -1f;
                    }
                    int proj = Projectile.NewProjectile(projectile.GetSource_FromAI(), target.Center, vector2, 496, projectile.damage / 2, 2, projectile.owner, num87, num77);
                    Main.projectile[proj].DamageType = DamageClass.Ranged;
                }
            }
        }
    }
}