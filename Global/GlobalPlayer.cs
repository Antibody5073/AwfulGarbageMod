using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Subworlds;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Tiles.OresBars;
using gunrightsmod.Content.GlobalItems;
using gunrightsmod.Content.Items;
using JetBrains.Annotations;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using rail;
using Steamworks;
using StramClasses;
using StramClasses.Classes.Acolyte.Accessories;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using static Humanizer.In;

namespace AwfulGarbageMod.Global
{
    public class GlobalPlayer : ModPlayer
    {
        public int timer;
        public int messageTimer = 0;

        //Stats
        public int FlatMeleeCrit = 0;
        public int FlatCrit = 0;
        public float criticalStrikeDmg = 0f;
        public float rangedVelocity = 1f;
        public float knifeVelocity = 1f;
        public float ScepterMaxStatMult = 1;
        public int scepterProjectiles = 0;
        public float MaxScepterBoost = 0;
        public int BoneGloveDamage = 0;
        public float MeleeWeaponSize = 1;
        public float WhipDebuffDurationPrev = 1;
        public float WhipDebuffDuration = 1;
        public float beeDmg = 1;
        public float flailSpinSpd = 1;
        public float flailRange = 1;
        public float empowermentCooldowMultiplier = 1;
        public float wingTimeMultiplier = 1;
        public float HorizontalWingSpdMult = 1; 
        public float VerticalWingSpdMult = 1;
        public float flailRetractDmg = 1;
        public float flailExtendSpeed = 1;
        public float flailRetractSpeed = 1;
        public int extraWhipTagDamage = 0;
        public float PercentArmorIgnore = 1;
        public float PercentArmorIgnoreSummon = 1;
        public int MaxPercentArmorIgnore;
        public int MaxPercentArmorIgnoreSummon;

        public float bonusluck = 0;

        public float oldManaCost;
        public float oldManaCost2;
        public float OrbitalDir = 0;

        public float ScepterMaxManaMultPrev;

        public bool IgnoreScepterDmgPenalties = false;
        public bool DisabledUnrealBuffNerfs = false;
        public bool AccursedElixir = false;

        //Accessory effects
        public bool spiderPendant = false;
        public bool corruptedPendant = false;
        public bool crimsonPendant = false;
        public bool iceCrystalGeode = false;
        public bool meteoriteGeode = false;
        public int HoneyOnDamaged = 0;
        public bool Bees = false;
        public int necroPotence = 0;
        public float necroPotencePercent = 0;
        public bool lightningRing = false;
        public float ScaledShadeShield = 0;
        public float MeatShield = 0;
        public float MeatShieldBonus;
        public int VenomOnDamanged = 0;
        public int cactusShell = 0;
        public int cactusShellPercent = 0;
        public bool EvilWardingCharm = false;
        public bool BottledTrash = false;
        public float HarujionPetal = 0;
        public float HarujionLevel = 0;
        public bool GlacialEye = false;
        public bool IlluminantString = false;
        public bool DoubleVisionBand = false;
        public int jungleSporeFlail = 0;
        public float mechanicalArm = 1;
        public float mechanicalLens = 1;
        public float mechanicalScope = 1;
        public int poisonSigil = 0;
        public float waterSigil = 0;
        public bool fireSigil = false;
        public int shadowSigil = 0;
        public bool aridSigil = false;
        public bool frostSigil = false;
        public bool holySigil = false;
        public bool terraSigil = false;
        public bool MoltenEye = false;
        public int MoltenEyeDmg = 0;
        public int GlacialEyeDmg = 0;
        public bool AncientGadgets = false;
        public int HellShellSparks = 0;
        public int HellShellSparksPercent = 0;
        public bool PotentVenomGland = false;
        public float OverflowingVenom = 1;
        public bool EarthenAmulet = false;
        public int EarthenAmuletTimer = 0;
        public bool PumpkinRocket = false;
        public bool ThrushGliders = false;
        public float MoltenRose = 1;
        public float FrozenLotus = 1;
        public int CursedHammer = 0;
        public int CrimsonAxe = 0;
        public int EnchantedSword = 0;
        public bool FleshyAmalgam;
        public int FortifyingLink = 0;
        public bool DemonClaw = false;
        public int FieryGrip = 0;
        public int SwampyGrip = 0;
        public bool SlimyLocket = false;
        public int slimyLocketTimer = 0;
        public bool FrigidSeed = false;
        public bool MagmaSeed = false;
        public bool InfectionSeed = false;
        public bool CursedSeed = false;
        public bool IchorSeed = false;
        public bool BagOfSeeds = false;
        public bool ChloroplastCore = false;
        public bool ShockAbsorber = false;
        public bool MinionSentrySwap = false;
        public bool RangedInflictBleed = false;
        public bool NoMiniboss = false;
        public bool CelestialCurse = false;
        public bool Proboscis = false;
        public int ProboscisTimer = 0;
        public bool LifeMagnet = false;
        public bool BloodMagnet = false;
        public bool MagnosBlessing = false;
        public float SaltedTears = 0;
        public float SaltedTearsEnergy = 0;
        public bool ForbiddenFruit = false;
        public int ForbiddenFruitTimer = 0;
        public bool lightningRingPrevious;
        public bool ColdHeart = false;
        public bool GlassetteTape = false;
        public bool AntediluvianArtifact = false;
        public int AntediluvianPotionBoost = 0;
        public bool ShadowstepDodge = false;
        public int ShadowstepChance = 0;
        public int ShadowstepTimer = 0;
        public bool Ataraxia = false;
        public int AtaraxiaBoost = 0;
        public bool ShadowflameStone = false;
        public bool ImprovedShadowflameStone = false;
        public bool ShadowflameOnShieldBreak = false;
        public bool ShadowflameMelee = false;
        public int Enlightenment = 0;
        public int EnlightenmentCooldown = 0;
        public bool AccumulusChalice = false;
        public int AccumulusChaliceTimer = 0;
        public int FeatherPendant = 0;
        public int FeatherPendantArmorPen = 0;
        public bool FeatherPendantIchor = false;
        public bool FeatherPendantCursedFlame = false;
        public int FeatherPendantFlyTimer = 0;
        public int NecropotenceTimer = 0;
        public int NecropotenceDamage = 0;
        public int BoneBracelet = 0;
        public float BoneBraceletTimer = 0;
        public bool NightmareSash = false;
        public int NecroScope = 0;
        public float NecroScopeTimer = 0;
        public bool GashadokuroHeart;
        public bool GashadokuroHeartPrevious;
        public bool CursedCarapace = false;
        public int CursedCarapaceHealthLoss;
        public bool CursedToggle = false;

        public bool JunkGreaves = false;

        //Set Bonuses
        public bool AerogelBonus = false;
        public bool FrozenSpiritBonus = false;
        public bool MyceliumHoodBonus = false;
        public bool MyceliumMaskBonus = false;
        public bool RottingBonus = false;
        public bool MidnightBonus = false;
        public bool VeinBonus = false;
        public bool GoreKnightBonus = false;
        public bool StormHeadgearBonus = false;
        public bool StormHelmetBonus = false;
        public bool StormHoodBonus = false;
        public bool MeteoriteVisorBonus = false;
        public bool MeteoriteVisorBonusPrev = false;
        public bool WorthlessJunkBonus = false;
        public bool FrigidiumBonus = false;
        public int FrigidiumBonusChanneledDmg = 0;
        public float FrigidiumDmgBonus = 0;
        public bool SanguineBonus = false;
        public bool CobaltMagic = false;
        public bool EmpowermentSlot = false;
        public bool EmpowermentSlotPrevious = false;
        public bool CobaltRanged = false;
        public bool CobaltMelee = false;
        public int CobaltMeleeCooldown = 0;
        public int CobaltMeleeDefense = 0;
        public bool CandescentBonus = false;
        public bool AncientFlierBonus = false;
        public bool EarthenBonus = false;
        public bool EarthenBonusPrev = false;
        public bool UmbragelBonus = false;
        public List<Vector2> umbragelPos = [];
        public List<Vector2> umbragelVel = [];
        public int umbragelTimer = 0;
        public bool MythrilDefense = false;
        public bool MythrilOffense = false;
        public bool OrichalcumGun = false;
        public bool OrichalcumKnife = false;
        public bool OrichalcumBow = false;
        public bool AdamantiteMinion = false;
        public bool AdamantiteSentry = false;
        public bool TitaniumTank = false;
        public bool TitaniumOffense = false;
        public bool TitaniumVariety = false;
        public bool EquilibriumMinionBonus = false;
        public bool EquilibriumSentryBonus = false;
        public bool CrabBonus = false;


        public bool FrigidHelmet = false;
        public bool FrigidBreastplate = false;

        public bool FlailAtMaxLengthPrev = false;
        public bool FlailAtMaxLength = false;



        public string ImportantHoveredTile = "";

        public override void Load()
        {
            Terraria.On_Player.AddBuff_DetermineBuffTimeToAdd += ModifyBuffTime;
        }

        private static int ModifyBuffTime(On_Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player self, int type, int time1)
        {
            int buffTime = orig(self, type, time1);

            if (!Main.debuff[type] && !BuffID.Sets.IsAFlaskBuff[type] && !Main.buffNoTimeDisplay[type] && time1 != 2)
            {
                if (self.GetModPlayer<GlobalPlayer>().GashadokuroHeart)
                {
                    return buffTime * 5 / 4;
                }
            }

            return buffTime;
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return [
                new Item(ModContent.ItemType<EyeOfSenri>())
            ];
        }
        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            float sqrMaxDetectDistance = 480 * 480;
            float fishingLevelDecrease = 0;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile target = Main.projectile[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.active && target.type == ModContent.ProjectileType<BucketOfTrashProj>() && target.wet)
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, this.Player.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        fishingLevelDecrease += 0.05f;
                    }
                }
            }
            if (fishingLevelDecrease > 0.3f)
            {
                fishingLevelDecrease = 0.3f;
            }
            fishingLevel -= fishingLevelDecrease;
        }
        
        public override void OnExtraJumpStarted(ExtraJump jump, ref bool playSound)
        {
            FeatherPendantFlyTimer = 0;
            if (FeatherPendant > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<FeatherPendant>().Item), Player.Center, Main.rand.NextVector2CircularEdge(2, 0.5f) + new Vector2(0, 3), ModContent.ProjectileType<FeatherPendantProj>(), FeatherPendant, 0, Player.whoAmI);
                    proj.originalDamage = proj.damage;
                    proj.OriginalArmorPenetration += FeatherPendantArmorPen;
                    proj.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                }
            }

        }
        public override void PreUpdate()
        {
            timer++;
            if (timer >= 60)
            {
                timer = 0;
                HarujionLevel += HarujionPetal / 30;
            }
            if (HarujionLevel > HarujionPetal)
            {
                HarujionLevel = HarujionPetal;
            }
            if (HarujionLevel > 0)
            {
                this.Player.AddBuff(ModContent.BuffType<HarujionPetalBuff>(), 3);
            }
            messageTimer--;
            if (ModContent.GetInstance<ConfigClient>().NotifyMissingStuff && messageTimer == 0 && Main.netMode == NetmodeID.SinglePlayer && !SubworldSystem.AnyActive())
            {
                if (new Vector2(Structures.IcePalacePosX, Structures.IcePalacePosY) == new Vector2(0, 0) || AGUtils.GetTileCounts(ModContent.TileType<CandesciteOre>()) <= 0 || AGUtils.GetTileCounts(ModContent.TileType<FrigidiumOre>()) <= 0 || AGUtils.GetTileCounts(ModContent.TileType<FlintDirt>()) <= 0)
                {
                    Main.NewTextMultiline("Your world is missing ores and/or structures\nPlease use a Worldly Scroll ([i:AwfulGarbageMod/WorldlyScroll]) to generate any missing ores and structures", c: Color.Red);
                }
            }

            if (CandescentBonus)
            {
                for (var j = 0; j < 3; j++)
                {
                    int dust = Dust.NewDust(this.Player.Center + new Vector2(360, 0).RotatedBy(MathHelper.ToRadians((360 / 3) * j - OrbitalDir * 2f)), 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
            }

            if (ShadowflameStone || ImprovedShadowflameStone)
            {
                for (var j = 0; j < 3; j++)
                {
                    int dust = Dust.NewDust(this.Player.Center + new Vector2(330, 0).RotatedBy(MathHelper.ToRadians((360 / 3) * j - OrbitalDir * 2f)), 0, 0, DustID.ShadowbeamStaff, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].alpha = 150;
                }
            }
            
            if (OverflowingVenom > 1)
            {
                for (var j = 0; j < 3; j++)
                {
                    int dust = Dust.NewDust(this.Player.Center + new Vector2(330, 0).RotatedBy(MathHelper.ToRadians((360 / 3) * j - OrbitalDir * 1.5f)), 0, 0, DustID.Venom, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.75f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
                for (var j = 0; j < 3; j++)
                {
                    int dust = Dust.NewDust(this.Player.Center + new Vector2(275, 0).RotatedBy(MathHelper.ToRadians((360 / 3) * j + OrbitalDir * 1.5f)), 0, 0, DustID.Venom, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.75f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    float distanceFromTarget = 330;

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, this.Player.Center);
                        bool inRange = between < distanceFromTarget;

                        if (inRange)
                        {
                            npc.AddBuff(BuffID.Venom, 60);
                        }
                    }
                }
            }
            else if (PotentVenomGland)
            {
                for (var j = 0; j < 3; j++)
                {
                    int dust = Dust.NewDust(this.Player.Center + new Vector2(275, 0).RotatedBy(MathHelper.ToRadians((360 / 3) * j + OrbitalDir * 1.5f)), 0, 0, DustID.Venom, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.75f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    float distanceFromTarget = 275;

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, this.Player.Center);
                        bool inRange = between < distanceFromTarget;

                        if (inRange)
                        {
                            npc.AddBuff(BuffID.Venom, 60);
                        }
                    }
                }
            }
            if (MoltenRose > 1)
            {
                NPC target = FindLowestHealthNPC();
                if (target != null)
                {
                    int radius = target.width;
                    if (target.height > radius)
                    {
                        radius = target.height;
                    }
                    radius /= 2;
                    for (var j = 0; j < 2; j++)
                    {
                        int dust = Dust.NewDust(target.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((360 / 2) * j + OrbitalDir * 2f)) + new Vector2(-4, -4), 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = 2f;
                        Main.dust[dust].velocity *= 0;
                        Main.dust[dust].noGravity = true;
                    }
                }
            }
            if (FrozenLotus > 1)
            {
                NPC target = FindHighestHealthNPC();
                if (target != null)
                {
                    int radius = target.width;
                    if (target.height > radius)
                    {
                        radius = target.height;
                    }
                    radius /= 2;
                    for (var j = 0; j < 2; j++)
                    {
                        int dust = Dust.NewDust(target.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((360 / 2) * j - OrbitalDir * 2f)) + new Vector2(-4, -4), 0, 0, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = 2f;
                        Main.dust[dust].velocity *= 0;
                        Main.dust[dust].noGravity = true;
                    }
                }
            }

            umbragelPos.Add(Player.position);
            umbragelVel.Add(Player.velocity);
            if (umbragelPos.Count > 30)
            {
                umbragelPos.RemoveAt(0);
            }
            if (umbragelVel.Count > 30)
            {
                umbragelVel.RemoveAt(0);
            }

            if (slimyLocketTimer > 0)
            {
                slimyLocketTimer--;
            }

            if (UmbragelBonus) {
                if (umbragelTimer > 0)
                {
                    umbragelTimer--;
                }
                else
                {

                    float maxDetectRadius = 720; // The maximum radius at which a projectile can detect a target
                                                 // Trying to find NPC closest to the projectile
                    NPC closestNPC = AGUtils.GetClosestNPC(umbragelPos[0] + new Vector2(Player.width / 2, Player.height / 2), maxDetectRadius);
                    if (closestNPC == null)
                        return;
                    if (umbragelVel[0].Length() > 2f)
                    {
                        umbragelTimer = 24;

                        Vector2 direction;

                        direction = closestNPC.Center - (umbragelPos[0] + new Vector2(Player.width / 2, Player.height / 2));
                        int proj = Projectile.NewProjectile(Player.GetSource_Accessory(ModContent.GetInstance<UmbragelHelmet>().Item), umbragelPos[0] + new Vector2(Player.width / 2, Player.height / 2), direction.SafeNormalize(Vector2.Zero) * 20, ProjectileID.Shuriken, 13, 0, Player.whoAmI);
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 13;
                    }
                }
            }

            if (AccumulusChalice) {
                if (AccumulusChaliceTimer > 0)
                {
                    AccumulusChaliceTimer--;
                }
                else
                {

                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<AccumulusChalice>().Item), Player.Center, Main.rand.NextVector2CircularEdge(3, 3), ModContent.ProjectileType<AccumulusChaliceProj>(), 25, 0, Player.whoAmI);
                    proj.originalDamage = proj.damage;
                    AccumulusChaliceTimer = 26;
                }
            }
            if (FeatherPendant > 0)
            {
                if (Player.justJumped)
                {
                    FeatherPendantFlyTimer = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<FeatherPendant>().Item), Player.Center, Main.rand.NextVector2CircularEdge(2, 0.5f) + new Vector2(0, 3), ModContent.ProjectileType<FeatherPendantProj>(), FeatherPendant, 0, Player.whoAmI);
                        proj.originalDamage = proj.damage;
                        proj.OriginalArmorPenetration += FeatherPendantArmorPen;
                        proj.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    }
                }
            }
            if (necroPotence > 0 && NecropotenceTimer > 0)
            {
                NecropotenceTimer--;
                if (NecropotenceTimer % 12 == 0)
                {

                    Vector2 pointPoisition = new Vector2(Player.position.X + (float)Player.width * 0.5f + (float)(Main.rand.Next(201) * -Player.direction) + ((float)Main.mouseX + Main.screenPosition.X - Player.position.X), Player.MountedCenter.Y - 600f);
                    pointPoisition.X = (pointPoisition.X + Player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                    pointPoisition.Y -= 100 * 1;
                    float num90 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                    float num101 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                    float ai2 = num101 + pointPoisition.Y;
                    if (num101 < 0f)
                    {
                        num101 *= -1f;
                    }
                    if (num101 < 20f)
                    {
                        num101 = 20f;
                    }
                    float num112 = (float)Math.Sqrt(num90 * num90 + num101 * num101);
                    num112 = 15 / num112;
                    num90 *= num112;
                    num101 *= num112;
                    Vector2 vector5 = new Vector2(num90, num101) / 2f;
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<Necropotence>().Item), pointPoisition, vector5, ModContent.ProjectileType<NecropotenceProj>(), (int)(NecropotenceDamage * necroPotencePercent), 2, Player.whoAmI, 0f, ai2);
                    proj.originalDamage = proj.damage;
                }
            }
            if (ForbiddenFruit)
            {
                Player.AddBuff(BuffID.MoonLeech, 3);
                if (Player.potionDelay > 0)
                {
                    Player.potionDelay++;
                    if (Player.HasBuff(BuffID.PotionSickness))
                    {
                        Player.buffTime[Player.FindBuffIndex(BuffID.PotionSickness)]++;
                    }
                }
                if (ForbiddenFruitTimer-- <= 0)
                {
                    ForbiddenFruitTimer = 60;
                    Player.Heal(6);
                }   
            }
            if (ShadowstepDodge && !Player.HasBuff<ShadowstepCooldown>())
            {
                if (ShadowstepTimer++ >= 60)
                {
                    ShadowstepTimer -= 60;
                    ShadowstepChance++;
                    if (ShadowstepChance > 60)
                    {
                        ShadowstepChance = 60;
                    }
                }
            }
            if (AccursedElixir)
            {
                Player.manaRegenDelay = 30;
            }
        }

       
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (UmbragelBonus)
            {

                Item item = new(ItemID.ShadowDye);

                int owner = Player.whoAmI;
                Player other = Main.player[owner];
                if (Main.playerVisualClone[owner] == null)
                {
                    Main.playerVisualClone[owner] = new Player();
                }
                Player player = Main.playerVisualClone[owner];
                player.CopyVisuals(other);
                //player.isFirstFractalAfterImage = true;
                //player.firstFractalAfterImageOpacity = 50 * 1f;
                player.ResetEffects();
                player.ResetVisibleAccessories();

                for (int i = 0; i < player.dye.Length; i++)
                {
                    player.dye[i] = item;
                }
                player.UpdateDyes();
                player.DisplayDollUpdate();
                player.UpdateSocialShadow();
                player.itemAnimationMax = 60;
                player.itemAnimation = 0;
                player.skinColor = Color.Black;
                player.itemRotation = umbragelVel[0].ToRotation();
                player.position = umbragelPos[0];
                player.direction = ((umbragelVel[0].X > 0f) ? 1 : (-1));
                player.velocity.Y = 0.01f;
                player.wingFrame = Player.wingFrame;
                player.PlayerFrame();
                player.socialIgnoreLight = true;
                Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, 0f, player.fullRotationOrigin, 0.4f);
            }
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.MeleeNoSpeed)
            {
                scale *= MeleeWeaponSize;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (modifiers.DamageType.CountsAsClass(DamageClass.Magic))
            {
                if (MoltenRose > 1)
                {
                    NPC lowestNPC = FindLowestHealthNPC();
                    if (lowestNPC == target)
                    {
                        modifiers.SourceDamage *= MoltenRose;
                        target.AddBuff(BuffID.OnFire3, 180);
                    }
                }
                if (FrozenLotus > 1)
                {
                    NPC highestNPC = FindHighestHealthNPC();
                    if (highestNPC == target)
                    {
                        modifiers.SourceDamage *= FrozenLotus;
                        target.AddBuff(BuffID.Frostburn2, 180);
                    }
                }
            }
            if (SlimyLocket && slimyLocketTimer <= 0)
            {
                slimyLocketTimer = 180;
                modifiers.Knockback += 6;

                SoundEngine.PlaySound(SoundID.NPCDeath1, target.Center);


                for (var j = 0; j < 20; j++)
                {
                    int dust = Dust.NewDust(target.Center + new Vector2(15, 0).RotatedBy(MathHelper.ToRadians((360 / 20) * j)) + new Vector2(-4, -4), 0, 0, DustID.t_Slime, 0f, 0f, 0, Color.SkyBlue, 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].velocity = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(360 / 20) * j);
                    Main.dust[dust].noGravity = true;
                }
                if (Player.statLife <= Player.statLifeMax2 / 3)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(ModContent.GetModItem(ModContent.ItemType<SlimyLocket>()).Item), target.Center, Vector2.Zero, ModContent.ProjectileType<SlimeExplosion>(), 60, 6, Main.myPlayer);
                }
            }
            int defense = target.defense;
            if (modifiers.DamageType.CountsAsClass(DamageClass.Summon))
            {
                modifiers.ArmorPenetration += MathHelper.Clamp(defense * (1-PercentArmorIgnoreSummon), 0, MaxPercentArmorIgnoreSummon);
                defense -= (int)MathHelper.Clamp(defense * (1 - PercentArmorIgnoreSummon), 0, MaxPercentArmorIgnoreSummon);
            }

            modifiers.ArmorPenetration += MathHelper.Clamp(defense * (1 - PercentArmorIgnore), 0, MaxPercentArmorIgnore);
            defense -= (int)MathHelper.Clamp(defense * (1 - PercentArmorIgnore), 0, MaxPercentArmorIgnore);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (WorthlessJunkBonus)
            {
                if (AGUtils.AnyMeleeDmg(hit.DamageType))
                {
                    this.Player.AddBuff(ModContent.BuffType<NonMeleeBuff1>(), 60 * 10);
                }
                if (AGUtils.AnyRangedDmg(hit.DamageType))
                {
                    this.Player.AddBuff(ModContent.BuffType<NonRangedBuff1>(), 60 * 10);
                }
                if (AGUtils.AnyMagicDmg(hit.DamageType))
                {
                    this.Player.AddBuff(ModContent.BuffType<NonMagicBuff1>(), 60 * 10);
                }
                if (AGUtils.AnySummonDmg(hit.DamageType))
                {
                    this.Player.AddBuff(ModContent.BuffType<NonSummonBuff1>(), 60 * 10);
                }
                if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
                {
                    WorthlessJunkStramClasses(hit, this.Player);
                }
            }
            if (FrigidiumBonus && !Player.HasBuff<ArmorAbilityCooldown>())
            {
                if (AGUtils.AnyMeleeDmg(hit.DamageType))
                {
                    FrigidiumBonusChanneledDmg += hit.Damage;
                    if (FrigidiumBonusChanneledDmg > 350)
                    {
                        FrigidiumBonusChanneledDmg = 0;
                        this.Player.AddBuff(ModContent.BuffType<FrigidArmorBuff1>(), 2);

                        // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
                        if (this.Player.ownedProjectileCounts[Mod.Find<ModProjectile>("FrigidiumArmorProj").Type] < 5)
                        {
                            var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, Mod.Find<ModProjectile>("FrigidiumArmorProj").Type, 0, 0, Main.myPlayer, Player.ownedProjectileCounts[Mod.Find<ModProjectile>("FrigidiumArmorProj").Type], 0);
                        }
                    }
                }
            }
            if (FrigidSeed && (hit.DamageType.CountsAsClass(DamageClass.Magic) || hit.DamageType.CountsAsClass(DamageClass.Summon)))
            {
                target.AddBuff(BuffID.Frostburn, 60);
            }
            if (MagmaSeed && (hit.DamageType.CountsAsClass(DamageClass.Magic) || hit.DamageType.CountsAsClass(DamageClass.Summon)))
            {
                target.AddBuff(BuffID.OnFire3, 60);
            }
            if (RangedInflictBleed && hit.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                target.AddBuff(ModContent.BuffType<AGBleed>(), 240);
            }
            if (Proboscis && ProboscisTimer < 0)
            {
                ProboscisTimer = 15 * 60;
                Item.NewItem(target.GetSource_ReleaseEntity(), target.position, target.Size, ItemID.Heart);
                if (MagnosBlessing)
                {
                    Item.NewItem(target.GetSource_ReleaseEntity(), target.position, target.Size, ItemID.Star);
                }
            }
            if (ShadowflameMelee && hit.DamageType.CountsAsClass(DamageClass.Melee))
            {
                target.AddBuff(BuffID.ShadowFlame, 240);
            }
            if (BoneBracelet > 0 && BoneBraceletTimer <= 0)
            {
                Vector2 ProjVel = new Vector2(0, Main.rand.NextFloat(-7, -4)).RotatedByRandom(MathHelper.ToRadians(13));
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<SkullSigil>().Item), target.Top + ProjVel * 2, ProjVel, ModContent.ProjectileType<BoneBraceletProj>(), BoneBracelet, 1, Player.whoAmI);
                proj.originalDamage = proj.damage;
                proj.OriginalArmorPenetration += 20;
                BoneBraceletTimer += 5 + (10 / Player.GetTotalAttackSpeed(DamageClass.Melee));
            }
            if (NecroScope > 0 && NecroScopeTimer <= 0)
            {
                Vector2 ProjVel = new Vector2(0, Main.rand.NextFloat(-7, -4)).RotatedByRandom(MathHelper.ToRadians(13));
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(ModContent.GetInstance<SkullSigil>().Item), target.Top + ProjVel * 2, ProjVel, ModContent.ProjectileType<BoneBraceletProj>(), BoneBracelet, 1, Player.whoAmI);
                proj.originalDamage = proj.damage;
                proj.OriginalArmorPenetration += 20;
                proj.DamageType = DamageClass.Ranged;
                NecroScopeTimer += 5 + (10 / rangedVelocity);
            }
        }
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (ShadowflameMelee && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Shadowflame);
                    dust.velocity *= 0.5f;
                }
            }
        }
        public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
        {
            if (ShadowflameMelee && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Shadowflame);
                    dust.velocity *= 0.5f;
                }
            }
        }
        [JITWhenModsEnabled("StramClasses")]
        public static void WorthlessJunkStramClasses(NPC.HitInfo hit, Player player)
        {
            if (hit.DamageType == StramUtils.rogueDamage())
            {
                player.AddBuff(ModContent.BuffType<NonRogueBuff1>(), 60 * 10);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (MagnosBlessing)
            {
                if (npc.GetGlobalNPC<ExampleGlobalNPC>().MagnoTimer > 0 && npc.GetGlobalNPC<ExampleGlobalNPC>().MagnoPlayer == Player)
                {
                    modifiers.Cancel();
                }
                if (!Player.HasBuff<MagnosBlessingCooldown>())
                {
                    modifiers.Cancel();
                    Player.AddBuff(ModContent.BuffType<MagnosBlessingCooldown>(), 60 * 20);
                    npc.GetGlobalNPC<ExampleGlobalNPC>().MagnoTimer = 60;
                    npc.GetGlobalNPC<ExampleGlobalNPC>().MagnoPlayer = Player;
                    Player.velocity += (Player.Center - npc.Center).SafeNormalize(Vector2.One) * 8f;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {

            if (MagnosBlessing)
            {
                if (proj.GetGlobalProjectile<GlobalProjectiles>().MagnoTimer > 0 && proj.GetGlobalProjectile<GlobalProjectiles>().MagnoPlayer == Player)
                {
                    modifiers.Cancel();
                }
                if (!Player.HasBuff<MagnosBlessingCooldown>())
                {
                    modifiers.Cancel();
                    Player.AddBuff(ModContent.BuffType<MagnosBlessingCooldown>(), 60 * 20);
                    proj.GetGlobalProjectile<GlobalProjectiles>().MagnoTimer = 60;
                    proj.GetGlobalProjectile<GlobalProjectiles>().MagnoPlayer = Player;
                    Player.velocity += (Player.Center - proj.Center).SafeNormalize(Vector2.One) * 8f;
                }
            }
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (ShadowstepDodge && Main.rand.NextBool(ShadowstepChance, 100))
            {
                ExampleDodgeEffects();
                return true;
            }

            return false;
        }

        // ExampleDodgeEffects() will be called from ConsumableDodge and HandleExampleDodgeMessage to sync the effect.
        public void ExampleDodgeEffects()
        {
            Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

            // Some sound and visual effects
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.Wraith, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCHit54 with { Pitch = 0.5f });

            // The visual and sound effects happen on all clients, but the code below only runs for the dodging player
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            Player.AddBuff(ModContent.BuffType<ShadowstepCooldown>(), 900);
            ShadowstepChance = 0;
            ShadowstepTimer = 0;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                //SendExampleDodgeMessage(Player.whoAmI);
            }
        }
        /*
         public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

            Main.player[player].GetModPlayer<ExampleDamageModificationPlayer>().ExampleDodgeEffects();

            if (Main.netMode == NetmodeID.Server)
            {
                // If the server receives this message, it sends it to all other clients to sync the effects.
                SendExampleDodgeMessage(player);
            }
        }

        public static void SendExampleDodgeMessage(int whoAmI)
        {
            // This code is called by both the initial
            ModPacket packet = ModContent.GetInstance<AwfulGarbageMod>().GetPacket();
            packet.Write((byte)AwfulGarbageMod.MessageType.ExampleDodge);
            packet.Write((byte)whoAmI);
            packet.Send(ignoreClient: whoAmI);
        }
        */
        public override void OnHurt(Player.HurtInfo info)
        {

            if (HoneyOnDamaged > 0)
            {
                this.Player.AddBuff(48, HoneyOnDamaged);
            }

            //Bees
            if (Bees)
            {
                int num11 = 1;
                if (Main.rand.NextBool(3))
                {
                    num11++;
                }
                if (Main.rand.NextBool(3))
                {
                    num11++;
                }
                if (this.Player.strongBees && Main.rand.NextBool(3))
                {
                    num11++;
                }
                float num12 = 13f;
                if (this.Player.strongBees)
                {
                    num12 = 18f;
                }
                if (Main.masterMode)
                {
                    num12 *= 2f;
                }
                else if (Main.expertMode)
                {
                    num12 *= 1.5f;
                }
                for (int num13 = 0; num13 < num11; num13++)
                {
                    float speedX = (float)Main.rand.Next(-35, 36) * 0.02f;
                    float speedY = (float)Main.rand.Next(-35, 36) * 0.02f;
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.position.X, this.Player.position.Y, speedX, speedY, this.Player.beeType(), this.Player.beeDamage((int)num12), this.Player.beeKB(0f), Main.myPlayer);
                }
            }

            //Necropotence
            if (necroPotence > 0)
            {/*
                for (int j = 0; j < Main.rand.Next(6, 9); j++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, ModContent.ProjectileType<NecropotenceProj>(), AGUtils.ScaleDamage(necroPotence + (int)(info.SourceDamage * (necroPotencePercent / 100f)), Player, DamageClass.Summon), 2, Main.myPlayer);
                }
                NPC closestNPC = FindClosestNPC(1500);

                if (closestNPC == null)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, ModContent.ProjectileType<NecropotenceProj>(), AGUtils.ScaleDamage(necroPotence + (int)(info.SourceDamage * (necroPotencePercent / 100f)), Player, DamageClass.Summon), 2, Main.myPlayer);
                }
                else
                {
                    Vector2 vel = Vector2.Normalize(closestNPC.Center - this.Player.Center) * Main.rand.NextFloat(6, 8);
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, ModContent.ProjectileType<NecropotenceProj>(), AGUtils.ScaleDamage(necroPotence + (int)(info.SourceDamage * (necroPotencePercent / 100f)), Player, DamageClass.Summon), 2, Main.myPlayer);
                }*/
            }

            if (HellShellSparks > 0)
            {
                for (int j = 0; j < Main.rand.Next(6, 9); j++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(9, 11), 0).RotatedByRandom(MathHelper.ToRadians(360)) + new Vector2(0, -5);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, 402, AGUtils.ScaleDamage(HellShellSparks + (int)(info.SourceDamage * (HellShellSparksPercent / 100f)), Player, DamageClass.Magic), 2, Main.myPlayer);
                    Main.projectile[proj].DamageType = DamageClass.Magic;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 20;
                }
            }
            if (NightmareSash)
            {

                Vector2 vel;
                vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - this.Player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - this.Player.Center.Y);
                vel.Normalize();
                vel *= 7.5f;

                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, ModContent.ProjectileType<NightmareSashProj>(), 25, 1, Main.myPlayer);


            }
            //Cactus Shell
            if (cactusShell > 0 && !this.Player.HasBuff(ModContent.BuffType<CactusShellCooldown>()))
            {

                Vector2 vel;
                vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - this.Player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - this.Player.Center.Y);
                vel.Normalize();
                vel *= 7.5f;
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel, ModContent.ProjectileType<CactusShellProj>(), AGUtils.ScaleDamage(cactusShell + (int)(info.SourceDamage * (cactusShellPercent/100f)), Player, DamageClass.Ranged), 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(10)), ModContent.ProjectileType<CactusShellProj>(), AGUtils.ScaleDamage(cactusShell + (int)(info.SourceDamage * (cactusShellPercent / 100f)), Player, DamageClass.Ranged), 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(-10)), ModContent.ProjectileType<CactusShellProj>(), AGUtils.ScaleDamage(cactusShell + (int)(info.SourceDamage * (cactusShellPercent / 100f)), Player, DamageClass.Ranged), 1, Main.myPlayer);
                this.Player.AddBuff(ModContent.BuffType<CactusShellCooldown>(), 120);
            }

            //Frozen Spirit Bonus
            if (FrozenSpiritBonus || FrigidBreastplate)
            {
                float distanceFromTarget = 320f;

                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, this.Player.Center);
                        bool inRange = between < distanceFromTarget;

                        if (inRange)
                        {
                            npc.AddBuff(BuffID.Frostburn, 150);
                        }
                    }
                }
            }

            //Frozen Spirit Bonus
            if (VenomOnDamanged > 0)
            {
                float distanceFromTarget = 275;

                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, this.Player.Center);
                        bool inRange = between < distanceFromTarget;

                        if (inRange)
                        {
                            npc.AddBuff(BuffID.Venom, VenomOnDamanged);
                        }
                    }
                }
            }

            //Orbital releases and Frigidium bonus
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (GlobalProjectiles.Sets.IsScepterProjectile[projectile.type] && projectile.owner == this.Player.whoAmI)
                {
                    projectile.Kill();
                }
                
            }
            if (FrigidiumBonus)
            {
                if (this.Player.ownedProjectileCounts[Mod.Find<ModProjectile>("FrigidiumArmorProj").Type] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 18 * 60);
                    SoundEngine.PlaySound(SoundID.Item27);
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.type == Mod.Find<ModProjectile>("FrigidiumArmorProj").Type && projectile.owner == this.Player.whoAmI)
                        {
                            if (projectile.ai[1] == 0)
                            {
                                projectile.timeLeft = 0;
                                projectile.ai[1] = -1;
                            }
                        }
                    }
                }
            }
            if (ShockAbsorber)
            {
                Player.AddBuff(ModContent.BuffType<ShockAbsorberBuff>(), 480);
            }
            if (MythrilDefense)
            {
                if (Player.CheckMana(info.Damage, true, true))
                {
                    Player.Heal((int)(info.Damage * 0.3f));
                    Player.manaRegenDelay = 60;
                }
            }
            if (GlassetteTape)
            {
                Player.Heal(Player.statLifeMax2 / -10);
            }
            if (CursedCarapace)
            {
                CursedCarapaceHealthLoss += info.Damage / 3;
                CursedToggle = !CursedToggle;
            }
            SaltedTearsEnergy += info.Damage * SaltedTears;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.ArmorSetAbility.JustPressed && !Player.HasBuff<ArmorAbilityCooldown>() && !Player.HasBuff<ArmorAbilityCooldownWhip>())
            {
                if (MyceliumMaskBonus)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.1), 0, dodgeable: false, armorPenetration: 9999);
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 2 * 60);
                }
                if (CobaltMagic)
                {
                    Player.GetDamage(DamageClass.Magic) += 0.15f;

                    Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.1), 0, dodgeable: false, armorPenetration: 9999);
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 12 * 60);
                }
                if (MeteoriteVisorBonus)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.08), 0, dodgeable: false, armorPenetration: 9999);
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 12 * 60);
                }
                if (SanguineBonus)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.2), 0, dodgeable: false, armorPenetration: 9999);
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 6 * 60);
                    Player.AddBuff(ModContent.BuffType<SanguineRegenerationBuff>(), 15 * 60);

                }
                if (FrigidiumBonus)
                {
                    if (this.Player.ownedProjectileCounts[Mod.Find<ModProjectile>("FrigidiumArmorProj").Type] > 0)
                    {
                        Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 18 * 60);
                        int projectileCount = 0;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (projectile.type == Mod.Find<ModProjectile>("FrigidiumArmorProj").Type && projectile.owner == this.Player.whoAmI)
                            {
                                if (projectile.ai[1] == 0)
                                {
                                    projectileCount++;
                                    projectile.ai[1] = 1;
                                }
                            }
                        }
                        Player.AddBuff(ModContent.BuffType<FrigidArmorBuff2>(), 18 * 60);
                        FrigidiumDmgBonus = projectileCount;
                    }
                }
                if (AdamantiteSentry)
                {
                    foreach (var projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.owner == Player.whoAmI && projectile.sentry)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                projectile.Center = Player.Center + Main.rand.NextVector2Circular(200, 200);
                                Tile tile = Framing.GetTileSafely(projectile.Center);
                                if (!tile.HasTile || !AGUtils.IsNotAmbientObject(tile.TileType))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 20 * 60);
                }
                if (EquilibriumSentryBonus)
                {
                    float maxDist = 600 * 600;
                    Projectile target = null;
                    foreach (var projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.owner == Player.whoAmI && projectile.sentry)
                        {
                            if (!projectile.GetGlobalProjectile<EquilibriumSentry>().boost)
                            {
                                float distSq = Vector2.DistanceSquared(projectile.Center, Player.Center);
                                if (distSq < maxDist)
                                {
                                    maxDist = distSq;
                                    target = projectile;
                                }
                            }
                        }
                    }

                    if (target != null)
                    {
                        target.GetGlobalProjectile<EquilibriumSentry>().boost = true;
                        target.originalDamage *= 2;
                        Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldownWhip>(), 20 * 60);
                    }
                }
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if (ThrushGliders)
            {
                Player.accRunSpeed *= 1.12f;
                Player.maxRunSpeed *= 1.12f;
            }
            if (PumpkinRocket && Player.itemAnimation == 0 && !Player.channel)
            {
                Player.accRunSpeed *= 1.5f;
                Player.maxRunSpeed *= 1.5f;
            }
            if (UmbragelBonus)
            {
                Player.accRunSpeed *= 1.22f;
                Player.maxRunSpeed *= 1.22f;
            }
            if (Player.HasBuff(ModContent.BuffType<SprintBuff>()))
            {
                Player.accRunSpeed *= 1.08f;
                Player.maxRunSpeed *= 1.08f;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (Player.HasBuff<ShockAbsorberBuff>())
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
            }
            if (ForbiddenFruit)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.buffImmune[BuffID.Honey] = true;
            }
        }
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (MythrilOffense && item.mana > 0)
            {
                damage.Flat -= item.mana * oldManaCost2;
            }
        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (MythrilOffense)
            {
                mult = 0;
            }
        }
        public override void UpdateEquips()
        {
            if (Ataraxia)
            {
                int accessorySlots = 8;
                if (Main.masterMode)
                {
                    accessorySlots++;
                }
                if (Player.extraAccessory)
                {
                    accessorySlots++;
                }
                
                for (int i = 3; i < accessorySlots; i++)
                {
                    if (Player.armor[i].type == ItemID.None)
                    {
                        AtaraxiaBoost++;
                    }
                }
                for (int i = 0; i < Player.GetModPlayer<ModAccessorySlotPlayer>().LoadedSlotCount; i++)
                {
                    ModAccessorySlot acc = LoaderManager.Get<AccessorySlotLoader>().Get(i, Player);
                    
                    if (acc.IsEnabled() && acc.FunctionalItem.type == ItemID.None)
                    {
                        AtaraxiaBoost++;
                    }
                }
                //Player.GetModPlayer<ModAccessorySlotPlayer>().

                Player.GetDamage(DamageClass.Generic) += AtaraxiaBoost * 0.03f;
                Player.GetCritChance(DamageClass.Generic) += AtaraxiaBoost * 3f;
                Player.statDefense += AtaraxiaBoost * 3;
                Player.lifeRegen += AtaraxiaBoost;
                Player.moveSpeed += AtaraxiaBoost * 0.05f;
                Player.GetAttackSpeed(DamageClass.Melee) += AtaraxiaBoost * 0.03f;
                rangedVelocity += AtaraxiaBoost * 0.03f;
                Player.statManaMax2 += AtaraxiaBoost * 40;

                if (AtaraxiaBoost >= 3)
                {
                    Player.noKnockback = true;
                    Player.longInvince = true;
                }
                if (AtaraxiaBoost >= 5)
                {
                    Player.maxMinions += 3;
                }
                if (AtaraxiaBoost >= 6 && Player.armor[0].type != ItemID.None)
                {
                    Player.GrantArmorBenefits(Player.armor[0]);
                }
            }

        }
        public override void PostUpdateEquips()
        {
            Player player = this.Player;
            player.wingTimeMax = (int)(player.wingTimeMax * wingTimeMultiplier);
            if (ScaledShadeShield > 0)
            {
                if (player.statLife < (player.statLifeMax2) * 0.30f)
                {
                    player.statDefense += (int)ScaledShadeShield;
                }
                else
                {
                    player.statDefense = player.statDefense + (int)(ScaledShadeShield * ((player.statLifeMax2 - player.statLife) / (player.statLifeMax2 * 0.7)));

                }
            }
            if (MeatShield > 0)
            {
                if (player.statLife < (player.statLifeMax2) * 0.15f)
                {
                    MeatShieldBonus += MeatShield;
                }
                else
                {
                    MeatShieldBonus += (MeatShield * ((player.statLifeMax2 - player.statLife) / (player.statLifeMax2 * 0.85f)));
                }
            }
            if (player.HasBuff<FrigidArmorBuff2>())
            {
                player.GetDamage(DamageClass.Melee) *= 1 + (0.06f * FrigidiumDmgBonus);
            }
            if (player.HasBuff<ArmorAbilityCooldown>() && CobaltMagic == true)
            {
                player.GetDamage(DamageClass.Magic) += 0.15f;
            }
            if (CobaltMelee)
            {
                Player.statDefense += CobaltMeleeDefense;
            }
            if (FlailAtMaxLengthPrev)
            {
                Player.statDefense += FortifyingLink;
            }
            if (ChloroplastCore)
            {
                NPC npc = FindClosestNPCAndDistance(800, out float distance);
                if (npc != null)
                {
                    if (player.velocity.X == 0)
                    {
                        npc.AddBuff(ModContent.BuffType<ChloroplastCoreBuff2>(), 2);
                    }
                    else
                    {
                        npc.AddBuff(ModContent.BuffType<ChloroplastCoreBuff>(), 2);
                    }
                }
            }
            if (ShockAbsorber)
            {
                if (Player.statDefense * 0.12f < 3)
                {
                    Player.statDefense += 3;
                }
                else
                {
                    Player.statDefense += Player.statDefense * 0.12f;
                }
            }
            if (GlassetteTape)
            {
                Player.statDefense += Player.statDefense * 0.1f;
            }
            if (MinionSentrySwap)
            {
                int oldMinions = player.maxMinions;
                int oldSentries = player.maxTurrets;
                player.maxMinions = oldSentries;
                player.maxTurrets = oldMinions;
            }
            if (MythrilOffense)
            {
                Player.statLifeMax2 += Player.statManaMax2 / 2;
            }
            if (OrichalcumBow || OrichalcumGun || OrichalcumKnife)
            {
                Player.onHitPetal = false;
            }
            if (TitaniumTank)
            {
                Player.statDefense += Player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] * 4;
            }
            
            if (TitaniumOffense)
            {
                Player.onHitTitaniumStorm = false;
                int def = Player.statDefense / 4;
                int defLost = Player.statDefense - def;
                Player.GetDamage(DamageClass.Melee) += 0.01f * defLost;
                Player.statDefense -= defLost;
            }
            if (AntediluvianArtifact)
            {
                Player.statLifeMax2 += Player.statDefense * 4;
                Player.statManaMax2 += Player.statDefense * 4;
                Player.maxTurrets += Player.statDefense / 40;
                AntediluvianPotionBoost = Player.statDefense;
                Player.statDefense *= 0;
            }

            if (EquilibriumSentryBonus && Player.HasBuff<ArmorAbilityCooldownWhip>())
            {
                Player.whipRangeMultiplier *= 0.7f;
            }
            oldManaCost2 = Player.manaCost;

            if (CelestialCurse)
            {
                if (Player.maxMinions > Player.maxTurrets)
                {
                    Player.maxMinions += 2;
                    Player.maxTurrets -= 1;
                }
                else if (Player.maxMinions < Player.maxTurrets)
                {
                    Player.maxMinions -= 1;
                    Player.maxTurrets += 2;
                }
                else
                {
                    Player.maxMinions += 1;
                    Player.maxTurrets += 1;
                    Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) -= 0.2f;
                }
            }
            if (Player.GetModPlayer<EarthenShield>().verdantStimulant)
            {
                Player.GetModPlayer<EarthenShield>().flatBonusDurability += player.statLifeMax2 / 3;
                player.statLifeMax2 -= player.statLifeMax2 / 3;
            }
            if (Player.statLife <= MathF.Ceiling(Player.statLifeMax2 / 2))
            {
                if (SaltedTears > 0)
                {
                    Player.GetDamage(DamageClass.Generic) += 0.1f;
                }
            }
            if (CursedCarapace)
            {
                int originalMaxLife = Player.statLifeMax2;
                Player.statLifeMax2 -= CursedCarapaceHealthLoss;
                if (Player.statLifeMax2 < (int)(originalMaxLife * 0.3f))
                {
                    Player.statLifeMax2 = (int)(originalMaxLife * 0.3f);
                }
                if (CursedToggle)
                {
                    Player.AddBuff(BuffID.CursedInferno, 5);
                    if (!Player.buffImmune[BuffID.CursedInferno] && timer % 12 == 0)
                    {
                        if (Player.statLifeMax2 > (int)(originalMaxLife * 0.3f))
                        {
                            CursedCarapaceHealthLoss++;
                        }
                    }
                }
                else 
                {
                    if (Player.lifeRegen > 0 && timer % 30 == 0)
                    {
                        {
                            CursedCarapaceHealthLoss--;
                            if (CursedCarapaceHealthLoss < 0)
                            {
                                CursedCarapaceHealthLoss = 0;
                            }
                        }
                    }
                }
            }
        }
        
        public override void PostUpdate()
        {
            if (ColdHeart)
            {
                if (Player.statLife > MathF.Ceiling(Player.statLifeMax2 / 2))
                {
                    Player.statLife = (int)MathF.Ceiling(Player.statLifeMax2 / 2);
                }
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (SubworldSystem.IsActive<EbonwoodBossSubworld>())
            {
                Player.noBuilding = true;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= (ModContent.GetInstance<Configs.Config>().EnemyDamageMultiplier / 100);
            if (DifficultyModes.Difficulty == 2)
            {
                modifiers.SourceDamage *= 1.5f;
            }
            if (Player.HasBuff<FrosshroomPotionBuff>())
            {
                modifiers.FinalDamage *= 0.6f;
                Player.ClearBuff(ModContent.BuffType<FrosshroomPotionBuff>());
            }
        }

        public override void OnEnterWorld()
        {
            messageTimer = 180;
            umbragelPos.Clear();
            umbragelVel.Clear();


            Main.instance.LoadNPC(56); //Snatcher
            Main.instance.LoadNPC(NPCID.Clinger); //Snatcher
            Main.instance.LoadNPC(61); // Vulture
            Main.instance.LoadNPC(169); // Ice Elemental
            Main.instance.LoadNPC(1); // Slime

            Main.instance.LoadProjectile(864); //Blade Staff
            Main.instance.LoadProjectile(206); //Leaf
            Main.instance.LoadProjectile(578); //Vortex Portal
            Main.instance.LoadProjectile(15); //Fireball
            Main.instance.LoadProjectile(253); //Iceball
            Main.instance.LoadProjectile(95); //Cursed Fireball
            Main.instance.LoadProjectile(ProjectileID.TerraBeam); //Terra Blade
            Main.instance.LoadProjectile(ProjectileID.TitaniumStormShard); //Titanium Storm
            Main.instance.LoadProjectile(ProjectileID.BoneGloveProj); //Crossbone
            Main.instance.LoadProjectile(932);

        }

        public override void ResetEffects()
        {
            spiderPendant = false;
            corruptedPendant = false;
            crimsonPendant = false;
            iceCrystalGeode = false;
            meteoriteGeode = false;
            FlatMeleeCrit = 0;
            FlatCrit = 0;
            HoneyOnDamaged = 0;
            Bees = false;
            criticalStrikeDmg = 0f;
            rangedVelocity = 1f;
            knifeVelocity = 1f;
            FrozenSpiritBonus = false;
            OrbitalDir += 1;
            necroPotence = 0;
            necroPotencePercent = 0;
            MyceliumHoodBonus = false;
            MyceliumMaskBonus = false;
            RottingBonus = false;
            VeinBonus = false;
            StormHeadgearBonus = false;
            StormHelmetBonus = false;
            StormHoodBonus = false;
            lightningRingPrevious = lightningRing;
            lightningRing = false;
            ScaledShadeShield = 0f;
            MeatShield = 0;
            MeatShieldBonus = 1f;
            AerogelBonus = false;
            ScepterMaxManaMultPrev = ScepterMaxStatMult;
            ScepterMaxStatMult = 1f;
            scepterProjectiles = 0;
            MaxScepterBoost = 0;
            VenomOnDamanged = 0;
            cactusShell = 0;
            cactusShellPercent = 0;
            BoneGloveDamage = 0;
            EvilWardingCharm = false;
            BottledTrash = false;
            MeteoriteVisorBonusPrev = MeteoriteVisorBonus;
            MeteoriteVisorBonus = false;
            HarujionPetal = 0;
            MeleeWeaponSize = 1;
            JunkGreaves = false;
            IgnoreScepterDmgPenalties = false;
            ImportantHoveredTile = "";
            WorthlessJunkBonus = false;
            if (!FrigidiumBonus)
            {
                FrigidiumBonusChanneledDmg = 0;
            }
            FrigidiumBonus = false;
            FrigidHelmet = false;
            FrigidBreastplate = false;
            beeDmg = 1;
            SanguineBonus = false;
            CobaltMagic = false;
            WhipDebuffDurationPrev = WhipDebuffDuration;
            WhipDebuffDuration = 1;
            IlluminantString = false;
            flailSpinSpd = 1;
            flailRange = 1;
            DoubleVisionBand = false;
            jungleSporeFlail = 0;
            mechanicalArm = 1;
            mechanicalLens = 1;
            mechanicalScope = 1;
            poisonSigil = 0;
            waterSigil = 0;
            fireSigil = false;
            shadowSigil = 0;
            aridSigil = false;
            empowermentCooldowMultiplier = 1;
            GlacialEye = false;
            EmpowermentSlotPrevious = EmpowermentSlot;
            EmpowermentSlot = false;
            CobaltRanged = false;
            CobaltMelee = false;
            CobaltMeleeCooldown -= 1;
            if (CobaltMeleeCooldown < 0)
            {
                CobaltMeleeDefense = 0;
            }
            frostSigil = false;
            holySigil = false;
            terraSigil = false;
            MoltenEye = false;
            MoltenEyeDmg = 0;
            GlacialEyeDmg = 0;
            AncientGadgets = false;
            CandescentBonus = false;
            HellShellSparks = 0;
            HellShellSparksPercent = 0;
            PotentVenomGland = false;
            OverflowingVenom = 1;
            wingTimeMultiplier = 1;
            AncientFlierBonus = false;
            HorizontalWingSpdMult = 1;
            VerticalWingSpdMult = 1;
            EarthenBonusPrev = EarthenBonus;
            EarthenBonus = false;
            ThrushGliders = false;
            EarthenAmulet = false;
            EarthenAmuletTimer--;
            PumpkinRocket = false;
            MoltenRose = 1;
            FrozenLotus = 1;
            CursedHammer = 0;
            CrimsonAxe = 0;
            EnchantedSword = 0;
            FleshyAmalgam = false;
            flailRetractDmg = 1;
            FortifyingLink = 0;
            DemonClaw = false;
            FlailAtMaxLengthPrev = FlailAtMaxLength;
            FlailAtMaxLength = false;
            FieryGrip = 0;
            SwampyGrip = 0;
            flailExtendSpeed = 1;
            flailRetractSpeed = 1;
            UmbragelBonus = false;
            SlimyLocket = false;
            extraWhipTagDamage = 0;
            FrigidSeed = false;
            MagmaSeed = false;
            InfectionSeed = false;
            IchorSeed = false;
            CursedSeed = false;
            BagOfSeeds = false;
            ChloroplastCore = false;
            ShockAbsorber = false;
            DisabledUnrealBuffNerfs = false;
            MinionSentrySwap = false;
            MythrilDefense = false;
            MythrilOffense = false;
            OrichalcumBow = false;
            OrichalcumGun = false;
            OrichalcumKnife = false;
            AdamantiteMinion = false;
            AdamantiteSentry = false;
            TitaniumTank = false;
            TitaniumOffense = false;
            TitaniumVariety = false;
            RangedInflictBleed = false;
            EquilibriumMinionBonus = false;
            EquilibriumSentryBonus = false;
            NoMiniboss = false;
            CelestialCurse = false;
            Proboscis = false;
            PercentArmorIgnore = 1;
            PercentArmorIgnoreSummon = 1;
            bonusluck = 0;
            ProboscisTimer--;
            LifeMagnet = false;
            BloodMagnet = false;
            MagnosBlessing = false;
            MaxPercentArmorIgnore = 0;
            MaxPercentArmorIgnoreSummon = 0;
            if (SaltedTears == 0)
            {
                SaltedTearsEnergy = 0;
            }
            SaltedTears = 0;
            ForbiddenFruit = false;
            ColdHeart = false;
            MidnightBonus = false;
            GoreKnightBonus = false;
            GlassetteTape = false;
            AntediluvianArtifact = false;
            AntediluvianPotionBoost = 0;
            if (!ShadowstepDodge)
            {
                ShadowstepChance = 0;
                ShadowstepTimer = 0;
            }
            ShadowstepDodge = false;
            Ataraxia = false;
            AtaraxiaBoost = 0;
            ShadowflameStone = false;
            ImprovedShadowflameStone = false;
            ShadowflameMelee = false;
            ShadowflameOnShieldBreak = false;
            if (!Player.HasBuff(BuffID.PotionSickness))
            {
                AccursedElixir = false;
            }
            Enlightenment = 0;
            if (EnlightenmentCooldown > 0)
            {
                EnlightenmentCooldown--;
            }
            AccumulusChalice = false;
            FeatherPendant = 0;
            FeatherPendantIchor = false;
            FeatherPendantCursedFlame = false;
            FeatherPendantArmorPen = 0;
            BoneBracelet = 0;
            if (BoneBraceletTimer > 0)
            {
                BoneBraceletTimer--;
            }
            NightmareSash = false;
            NecroScope = 0;
            if (NecroScopeTimer > 0)
            {
                NecroScopeTimer--;
            }
            GashadokuroHeartPrevious = GashadokuroHeart;
            GashadokuroHeart = false;
            CursedCarapace = false;
            CrabBonus = false;
        }
        public override void ModifyLuck(ref float luck)
        {
            luck += bonusluck;
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
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, this.Player.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
        public NPC FindClosestNPCAndDistance(float maxDetectDistance, out float distance)
        {
            NPC closestNPC = null;
            distance = 0;

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
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, this.Player.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }
            distance = sqrMaxDetectDistance;
            return closestNPC;
        }
        public NPC FindLowestHealthNPC()
        {
            NPC lowestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float lowestHP = -1;

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
                    float targetHP = target.life;
                    // Check if it is within the radius
                    if (targetHP < lowestHP || lowestHP == -1)
                    {
                        lowestHP = targetHP;
                        lowestNPC = target;
                    }
                }
            }

            return lowestNPC;
        }
        public NPC FindHighestHealthNPC()
        {
            NPC highestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float highestHP = -1;

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
                    float targetHP = target.life;
                    // Check if it is within the radius
                    if (targetHP > highestHP || highestHP == -1)
                    {
                        highestHP = targetHP;
                        highestNPC = target;
                    }
                }
            }

            return highestNPC;
        }
    }
}