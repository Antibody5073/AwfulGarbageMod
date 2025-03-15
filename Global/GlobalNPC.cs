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
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.Global;
public class ExampleGlobalNPC : GlobalNPC
{

    // Loot
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.WallCreeper || npc.type == NPCID.WallCreeperWall || npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderLeg>(), 1, 1, 3));
        }
        if (npc.type == NPCID.WallCreeper || npc.type == NPCID.WallCreeperWall)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VenomGland>(), 25, 1, 1));
        }
        if (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VenomGland>(), 22, 1, 1));
        }
        if (npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.IceBat)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostShard>(), 2, 1, 2));
        }
        if (npc.type == NPCID.EyeofCthulhu)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ContactLens>(), 2, 1, 1));
        }
        if (npc.type == NPCID.KingSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimyKnives>(), 2, 1, 1));
        }
        if (npc.type == NPCID.SkeletronHead)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Toothpick>(), 1, 1, 1));
        }
        if (npc.type == NPCID.CaveBat)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodBolt>(), 60, 1, 1));
        }
        if (npc.type == NPCID.Drippler || npc.type == NPCID.BloodZombie)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeinJuice>(), 2, 1, 2));
        }
        if (npc.type == NPCID.QueenBee)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwarmScepter>(), 2, 1, 1));
        }
        if (npc.type == NPCID.Golem)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LihzahrdsAncientBattery>(), 2, 1, 1));
        }
        if (npc.type == NPCID.QueenSlimeBoss)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalKnives>(), 2, 1, 1));
        }
        if (npc.type == NPCID.SkeletronPrime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalArm>(), 2, 1, 1));
        }
        if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalLens>(), 4, 1, 1));
        }
        if (npc.type == NPCID.TheDestroyer)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MechanicalScope>(), 2, 1, 1));
        }
        if (npc.type == NPCID.WallofFlesh)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FleshyAmalgam>(), 2, 1, 1));
        }
        if (npc.type == NPCID.DukeFishron)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BubbleSlicers>(), 2, 1, 1));
        }
        if (npc.type == NPCID.IlluminantBat || npc.type == NPCID.IlluminantSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IlluminantString>(), 20, 1, 1));
        }
        if (npc.type == NPCID.DarkCaster)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SigilOfWater>(), 20, 1, 1));
        }
        if (npc.type == NPCID.GoblinThief)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThiefsKnife>(), 11, 1, 1));
        }
        if (npc.type == NPCID.Mothron)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EclipseKnives>(), 3, 1, 1));
        }
        if (npc.type == NPCID.AngryBones || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoneChalice>(), 120, 1, 1));
        }
        if (npc.type == NPCID.DungeonSpirit || npc.type == NPCID.GiantCursedSkull)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SigilOfSpirits>(), 17, 1, 1));
        }
        if (npc.type == NPCID.Hellbat || npc.type == NPCID.Lavabat)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LavaLamp>(), 30, 1, 1));
        }
        if (npc.type == NPCID.Demon)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonClaw>(), 15, 1, 1));
        }
        if (npc.type == NPCID.JungleSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwampyGrip>(), 100, 1, 1));
        }
        if (npc.type == NPCID.SpikedJungleSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwampyGrip>(), 30, 1, 1));
        }
        if (npc.type == NPCID.ManEater)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwampyGrip>(), 20, 1, 1));
        }
        if (npc.type == NPCID.LavaSlime || npc.type == NPCID.FireImp)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FieryGrip>(), 30, 1, 1));
        }
        if (npc.type == NPCID.Lavabat)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Pyrogem>(), 1, 1, 5));
        }
        if (npc.type == NPCID.Hellbat || npc.type == NPCID.LavaSlime)
        {
            LeadingConditionRule hardmodeRule = new LeadingConditionRule(new Conditions.IsHardmode());

            hardmodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Pyrogem>(), 1, 1, 3));
            npcLoot.Add(hardmodeRule);

        }
        if (npc.type == NPCID.RedDevil)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Pyrogem>(), 1, 4, 6));
        }
        if (npc.type == NPCID.IceElemental || npc.type == NPCID.IcyMerman || npc.type == NPCID.IceTortoise)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cryogem>(), 1, 1, 4));
        }
        if (npc.type == NPCID.SandShark || npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.DesertBeast || npc.type == NPCID.DesertScorpionWalk || npc.type == NPCID.DesertScorpionWall)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DesertScale>(), 2, 3, 4));
        }
        if (npc.type == NPCID.CursedHammer)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorruptedMetalChunk>(), 2));
        }
        if (npc.type == NPCID.CrimsonAxe)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrimsonMetalChunk>(), 2));
        }
        if (npc.type == NPCID.EnchantedSword)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HallowedMetalChunk>(), 2));
        }
        if (npc.type == NPCID.BoneLee)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LeeNunchucks>(), 5));
        }
        if (NPCID.Sets.Skeletons[npc.type])
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PileOfActualBones>(), 20));
        }
        if (npc.type == NPCID.Pumpking)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PumpkingHarvester>(), 15, 10));
        }
        if (npc.type == NPCID.IceBat || npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking || npc.type == NPCID.IceTortoise || npc.type == NPCID.IcyMerman)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidSeed>(), 50));
        }
        if (npc.type == NPCID.FireImp || npc.type == NPCID.LavaSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagmaSeed>(), 40));
        }
        if (npc.type == NPCID.Clinger || npc.type == NPCID.Corruptor || npc.type == NPCID.DesertGhoulCorruption)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulflowerSeed>(), 75));
        }
        if (npc.type == NPCID.IchorSticker || npc.type == NPCID.FloatyGross || npc.type == NPCID.DesertGhoulCrimson)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodflowerSeed>(), 75));
        }
        if (npc.type == NPCID.JungleCreeper || npc.type == NPCID.JungleCreeperWall|| npc.type == NPCID.MossHornet)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SeedOfInfection>(), 60));
        }
    }

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
                Structures.timer = 300;
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


    public override void ResetEffects(NPC npc)
    {
        ExampleDebuff = false;
        BoneSkewerTimer -= 1;
        if (BoneSkewerTimer < 1)
        {
            BoneSkewerBleed = 0;
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


