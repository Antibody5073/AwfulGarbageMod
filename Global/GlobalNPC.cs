using AwfulGarbageMod.Items;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Summon;
using Microsoft.CodeAnalysis;
using AwfulGarbageMod.Items.Tools;
using System.Linq;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.ItemDropRules;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AwfulGarbageMod.Global;
public class ExampleGlobalNPC : GlobalNPC
{

    //Crit damage boosts
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        Player player = Main.LocalPlayer;
        //Ice Crystal Geode
        if (player.GetModPlayer<GlobalPlayer>().iceCrystalGeode && projectile.CountsAsClass(DamageClass.Melee))
        {
            if (Main.rand.NextBool(10))
            {
                if (Main.hardMode)
                {
                    npc.AddBuff(BuffID.Frostburn2, 300);

                }
                else
                {
                    npc.AddBuff(BuffID.Frostburn, 300);
                }
            }
        }

        //Meteorite Geode
        if (player.GetModPlayer<GlobalPlayer>().meteoriteGeode && projectile.CountsAsClass(DamageClass.Melee) && projectile.type != Mod.Find<ModProjectile>("GeodeMeteor").Type)
        {

            if (!player.HasBuff(ModContent.BuffType<MeteorGeodeCooldown>()))
            {
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), npc.Top + new Vector2(0, -500), new Vector2(0, 10), Mod.Find<ModProjectile>("GeodeMeteor").Type, 52, 4, player.whoAmI);
                player.AddBuff(ModContent.BuffType<MeteorGeodeCooldown>(), 2 * 60);
            }
        }

        //Frozen Spirit set bonus
        if ((player.GetModPlayer<GlobalPlayer>().FrozenSpiritBonus || player.GetModPlayer<GlobalPlayer>().FrigidHelmet) && (npc.HasBuff(BuffID.Frostburn) || npc.HasBuff(BuffID.Frostburn2)) && projectile.CountsAsClass(DamageClass.Melee))
        {
            modifiers.FinalDamage *= 1.16f;
        }
        //Vein set bonus
        if (player.GetModPlayer<GlobalPlayer>().VeinBonus && !player.HasBuff(ModContent.BuffType<VeinCooldown>()))
        {
            if (!projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                for (var i = 0; i < 12; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), npc.Top + new Vector2(0, -4), new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-2, -5)), Mod.Find<ModProjectile>("VeinBlood").Type, projectile.damage / 2, projectile.knockBack / 2, player.whoAmI);
                }
                player.AddBuff(ModContent.BuffType<VeinCooldown>(), 3 * 60);
            }
        }

        //Crit damage bonuses
        modifiers.CritDamage += player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg;

        modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
        {
            if (hitInfo.Crit)
            {
                hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatCrit;

                if (projectile.CountsAsClass(DamageClass.Melee))
                {
                    hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit;
                }
            }
        };

    }

    public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
    {

        //Ice Crystal Geode
        if (player.GetModPlayer<GlobalPlayer>().iceCrystalGeode && item.DamageType == DamageClass.Melee)
        {
            if (Main.rand.NextBool(10))
            {
                if (Main.hardMode)
                {
                    npc.AddBuff(BuffID.Frostburn2, 750);

                }
                else
                {
                    npc.AddBuff(BuffID.Frostburn, 750);
                }
            }
        }

        //Frozen Spirit set bonus
        if (player.GetModPlayer<GlobalPlayer>().FrozenSpiritBonus && (npc.HasBuff(BuffID.Frostburn) || npc.HasBuff(BuffID.Frostburn2)))
        {
            modifiers.FinalDamage *= 1.16f;
        }

        if (player.GetModPlayer<GlobalPlayer>().meteoriteGeode && item.DamageType == DamageClass.Melee)
        {

            if (!player.HasBuff(ModContent.BuffType<MeteorGeodeCooldown>()))
            {
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), npc.Top + new Vector2(0, -500), new Vector2(0, 10), Mod.Find<ModProjectile>("GeodeMeteor").Type, 65, 4, player.whoAmI);
                player.AddBuff(ModContent.BuffType<MeteorGeodeCooldown>(), 2 * 60);
            }
        }

        // Crit damage bonuses
        modifiers.CritDamage += player.GetModPlayer<GlobalPlayer>().criticalStrikeDmg;

        modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
        {
            if (hitInfo.Crit)
            {
                hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatCrit;

                if (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.MeleeNoSpeed)
                {
                    hitInfo.Damage += player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit;
                }
            }
        };

    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        Player target = Main.LocalPlayer;
        if (target.GetModPlayer<GlobalPlayer>().OverflowingVenom > 1)
        {
            float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, npc.Center);

            if (sqrDistanceToTarget < 330 * 330)
            {
                modifiers.FinalDamage *= 1.15f;
            }
        }
        if (target.GetModPlayer<GlobalPlayer>().PotentVenomGland)
        {
            float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, npc.Center);

            if (sqrDistanceToTarget < 275 * 275)
            {
                modifiers.FinalDamage *= 1.1f;
            }
        }
        base.ModifyIncomingHit(npc, ref modifiers);
    }

    public override void OnKill(NPC npc)
    {
        if (npc.type == NPCID.WallofFlesh)
        {
            if (ModContent.GetInstance<ConfigClient>().ShouldGenerateHardmodeStructures)
            {
                Structures.timer = 600;
            }
        }
        base.OnKill(npc);
    }

    public int[] buffPrevious;
    public int[] buffPreviousPrevious;


    //Debuffs
    public override bool InstancePerEntity => true;

    public bool ExampleDebuff;
    public int BoneSkewerBleed;
    public int BoneSkewerTimer;
    public List<Vector2> stackableTag = [];


    public override void ResetEffects(NPC npc)
    {
        ExampleDebuff = false;
        BoneSkewerTimer -= 1;
        if (BoneSkewerTimer < 1)
        {
            BoneSkewerBleed = 0;
        }
        int count = stackableTag.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 item = stackableTag[i];
            if (item.Y <= 0)
            {
                stackableTag.Remove(item);
                count--;
                i--;
                continue;
            }
            else
            {
                stackableTag[i] = new Vector2(item.X, item.Y - 1);
            }
        }
    }

    //Enemy spawn rate
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.GetModPlayer<GlobalPlayer>().EvilWardingCharm == true)
        {
            spawnRate = (int)(spawnRate / 0.8f);
            maxSpawns = (int)(maxSpawns * 0.8f);
        }
        if (player.GetModPlayer<GlobalPlayer>().BottledTrash == true)
        {
            spawnRate = (int)(spawnRate / 2.5f);
            maxSpawns = (int)(maxSpawns * 2.5f);
        }
        if (DifficultyModes.Difficulty > 0)
        {
            spawnRate = (int)(spawnRate / 1.25f);
            maxSpawns = (int)(maxSpawns * 1.25f);
        }
    }

    public override void SetDefaults(NPC npc)
    {
        // We want our ExampleJavelin buff to follow the same immunities as BoneJavelin
        npc.buffImmune[BuffType<Buffs.BoneSkewerBleed>()] = npc.buffImmune[BuffID.BoneJavelin];

        if (npc.type == NPCID.DD2Betsy && ModContent.GetInstance<Configs.Config>().BetsyNerf)
        {
            npc.lifeMax /= 2;
        }


        if (ModContent.GetInstance<Configs.Config>().EnemyHealthMultiplier != 100)
        {
            if (npc.lifeMax > 5)
            {
                npc.lifeMax = npc.lifeMax * ModContent.GetInstance<Configs.Config>().EnemyHealthMultiplier / 100;
                npc.life = npc.lifeMax;
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {

        if (BoneSkewerBleed > 0)
        {
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen -= BoneSkewerBleed * 3 * 2;
            if (damage < BoneSkewerBleed * 2)
            {
                damage = BoneSkewerBleed * 2;
            }
        }
        if (ExampleDebuff)
        {
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen -= 16;
            if (damage < 2)
            {
                damage = 2;
            }
        }
        Player target = Main.LocalPlayer;

        if (target.GetModPlayer<GlobalPlayer>().CandescentBonus)
        {
            // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
            float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, npc.Center);

            // Check if it is within the radius
            if (sqrDistanceToTarget < 360 * 360)
            {
                if (npc.HasBuff(BuffID.OnFire))
                {
                    npc.AddBuff(BuffID.OnFire3, 5);
                }
                if (npc.HasBuff(BuffID.OnFire3))
                {
                    npc.lifeRegen -= 60;
                    if (damage < 15)
                    {
                        damage = 15;
                    }
                }
                if (npc.HasBuff(BuffID.Frostburn))
                {
                    npc.AddBuff(BuffID.Frostburn2, 5);

                }
                if (npc.HasBuff(BuffID.Frostburn2))
                {
                    npc.lifeRegen -= 100;
                    if (damage < 30)
                    {
                        damage = 30;
                    }
                }
                if (npc.HasBuff(BuffID.ShadowFlame))
                {
                    npc.lifeRegen -= 60;
                    if (damage < 15)
                    {
                        damage = 15;
                    }
                }
                if (npc.HasBuff(BuffID.CursedInferno))
                {
                    npc.lifeRegen -= 96;
                    if (damage < 30)
                    {
                        damage = 30;
                    }
                }
                if (npc.HasBuff<WaterflameBuff>())
                {
                    npc.lifeRegen -= 120;
                    if (damage < 30)
                    {
                        damage = 30;
                    }
                }
                if (npc.HasBuff<HolyFireBuff>())
                {
                    npc.lifeRegen -= 120;
                    if (damage < 30)
                    {
                        damage = 30;
                    }
                }
            }
        }
        if (target.GetModPlayer<GlobalPlayer>().OverflowingVenom > 1)
        {
            // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
            float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, npc.Center);

            // Check if it is within the radius
            if (sqrDistanceToTarget < 330 * 330)
            {
                if (npc.HasBuff(BuffID.Venom))
                {
                    npc.lifeRegen -= 120;
                    if (damage < 45)
                    {
                        damage = 45;
                    }
                }
            }
        }
    }


   

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (BoneSkewerBleed > 0)
        {
            if (Main.rand.Next(4) < 3)
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                Main.dust[dust].velocity *= 1.8f;
                Main.dust[dust].velocity.Y -= 0.5f;
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 1f;
                
            }
        }
    }
}


