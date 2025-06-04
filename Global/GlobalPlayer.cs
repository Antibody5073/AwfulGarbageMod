using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Items.Armor;
using Microsoft.Xna.Framework;
using Steamworks;
using static Humanizer.In;
using AwfulGarbageMod.Buffs;
using Terraria.GameInput;
using AwfulGarbageMod.Systems;
using Mono.Cecil;
using Microsoft.CodeAnalysis;
using Terraria.Audio;
using AwfulGarbageMod.DamageClasses;
using StramClasses;
using Microsoft.Xna.Framework.Graphics;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Tiles.OresBars;
using Terraria.Localization;
using System.Collections.Generic;
using rail;
using Terraria.Graphics.Renderers;
using Microsoft.Build.Evaluation;
using System;
using AwfulGarbageMod.Projectiles;
using System.Drawing.Imaging;
using AwfulGarbageMod.Items.Accessories;

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

        public float OrbitalDir = 0;

        public float ScepterMaxManaMultPrev;

        public bool IgnoreScepterDmgPenalties = false;
        public bool DisabledUnrealBuffNerfs = false;

        //Accessory effects
        public bool spiderPendant = false;
        public bool corruptedPendant = false;
        public bool crimsonPendant = false;
        public bool iceCrystalGeode = false;
        public bool meteoriteGeode = false;
        public int HoneyOnDamaged = 0;
        public bool Bees = false;
        public int necroPotence = 0;
        public bool lightningRing = false;
        public float ScaledShadeShield = 0;
        public float MeatShield = 0;
        public float MeatShieldBonus;
        public int VenomOnDamanged = 0;
        public int cactusShell = 0;
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

        public bool lightningRingPrevious;

        public bool JunkGreaves = false;

        //Set Bonuses
        public bool AerogelBonus = false;
        public bool FrozenSpiritBonus = false;
        public bool MyceliumHoodBonus = false;
        public bool MyceliumMaskBonus = false;
        public bool RottingBonus = false;
        public bool VeinBonus = false;
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
        public bool PalladiumRanged = false;
        public bool PalladiumMelee = false;
        public bool PalladiumMagic = false;
        public int PalladiumMagicCooldown = 30*60;
        public bool CandescentBonus = false;
        public bool AncientFlierBonus = false;
        public bool EarthenBonus = false;
        public bool EarthenBonusPrev = false;
        public bool UmbragelBonus = false;
        public List<Vector2> umbragelPos = [];
        public List<Vector2> umbragelVel = [];
        public int umbragelTimer = 0;


        public bool FrigidHelmet = false;
        public bool FrigidBreastplate = false;

        public bool FlailAtMaxLengthPrev = false;
        public bool FlailAtMaxLength = false;



        public string ImportantHoveredTile = "";


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
            if (ModContent.GetInstance<ConfigClient>().NotifyMissingStuff && messageTimer == 0)
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
            NPC closestNPC = FindClosestNPC(1500);
            if (closestNPC == target)
            {
                if (PalladiumMelee && modifiers.DamageType == ModContent.GetInstance<FlailDamageClass>())
                {
                    modifiers.FinalDamage *= 1.15f;
                }
            }
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
        }

        [JITWhenModsEnabled("StramClasses")]
        public static void WorthlessJunkStramClasses(NPC.HitInfo hit, Player player)
        {
            if (hit.DamageType == StramUtils.rogueDamage())
            {
                player.AddBuff(ModContent.BuffType<NonRogueBuff1>(), 60 * 10);
            }
        }

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
            {
                for (int j = 0; j < Main.rand.Next(6, 9); j++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, Mod.Find<ModProjectile>("NecropotenceProj").Type, necroPotence, 2, Main.myPlayer);
                }
                NPC closestNPC = FindClosestNPC(1500);

                if (closestNPC == null)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, Mod.Find<ModProjectile>("NecropotenceProj").Type, necroPotence, 2, Main.myPlayer);
                }
                else
                {
                    Vector2 vel = Vector2.Normalize(closestNPC.Center - this.Player.Center) * Main.rand.NextFloat(6, 8);
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, Mod.Find<ModProjectile>("NecropotenceProj").Type, necroPotence, 2, Main.myPlayer);
                }
            }

            if (HellShellSparks > 0)
            {
                for (int j = 0; j < Main.rand.Next(6, 9); j++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(9, 11), 0).RotatedByRandom(MathHelper.ToRadians(360)) + new Vector2(0, -5);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, 402, AGUtils.ScaleDamage(HellShellSparks, Player, DamageClass.Magic), 2, Main.myPlayer);
                    Main.projectile[proj].DamageType = DamageClass.Magic;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 20;
                }
            }

            //Cactus Shell
            if (cactusShell > 0 && !this.Player.HasBuff(ModContent.BuffType<CactusShellCooldown>()))
            {

                Vector2 vel;
                vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - this.Player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - this.Player.Center.Y);
                vel.Normalize();
                vel *= 7.5f;
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel, Mod.Find<ModProjectile>("CactusShellProj").Type, AGUtils.ScaleDamage(cactusShell, Player, DamageClass.Ranged), 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(10)), Mod.Find<ModProjectile>("CactusShellProj").Type, AGUtils.ScaleDamage(cactusShell, Player, DamageClass.Ranged), 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(-10)), Mod.Find<ModProjectile>("CactusShellProj").Type, AGUtils.ScaleDamage(cactusShell, Player, DamageClass.Ranged), 1, Main.myPlayer);
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
                    projectile.timeLeft = 0;
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
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.ArmorSetAbility.JustPressed && !Player.HasBuff<ArmorAbilityCooldown>())
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
                if (PalladiumMagic)
                {
                    if (PalladiumMagicCooldown < 1)
                    {
                        Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), 1, 0, dodgeable: false, armorPenetration: 9999);
                        PalladiumMagicCooldown = 30 * 60;
                    }
                    else
                    {
                        Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.1), 0, dodgeable: false, armorPenetration: 9999);
                    }
                    Player.Hurt(PlayerDeathReason.ByCustomReason($"{Player.name} was too careless."), (int)(Player.statLifeMax2 * 0.1), 0, dodgeable: false, armorPenetration: 9999);
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 8 * 60);
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
            if (PalladiumMelee)
            {
                NPC npc = FindClosestNPCAndDistance(500, out float distance);
                if (npc != null)
                {
                    player.lifeRegen += (int)(12 * (1 - (distance / (500 * 500))));
                    
                    if (Main.rand.NextBool(3))
                    {
                        int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.HealingPlus, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                        Main.dust[dust].velocity *= 1.2f;
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= Main.rand.NextFloat(0.5f, 1f);
                    }
                }
            }
            if (PalladiumMagicCooldown < 1 && PalladiumMagic)
            {
                if (Main.rand.NextBool(3))
                {
                    int dust = Dust.NewDust(player.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.HealingPlus, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= Main.rand.NextFloat(0.5f, 1f);
                }
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
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= (ModContent.GetInstance<Config>().EnemyDamageMultiplier / 100);
        }

        public override void OnEnterWorld()
        {
            messageTimer = 180;
            umbragelPos.Clear();
            umbragelVel.Clear();
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
            if (PalladiumMagic)
            {
                PalladiumMagicCooldown -= 1;
            }
            PalladiumRanged = false;
            PalladiumMelee = false;
            PalladiumMagic = false;
            frostSigil = false;
            holySigil = false;
            terraSigil = false;
            MoltenEye = false;
            MoltenEyeDmg = 0;
            GlacialEyeDmg = 0;
            AncientGadgets = false;
            CandescentBonus = false;
            HellShellSparks = 0;
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