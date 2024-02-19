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

        public float OrbitalDir = 0;

        public float ScepterMaxManaMultPrev;

        public bool IgnoreScepterDmgPenalties = false;


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
        public bool GlacialEyePassive = false;
        public bool IlluminantString = false;
        public bool DoubleVisionBand = false;
        public int jungleSporeFlail = 0;
        public float mechanicalArm = 1;
        public int poisonSigil = 0;
        public float waterSigil = 0;
        public bool fireSigil = false;
        public int shadowSigil = 0;
        public bool aridSigil = false;

        public int fireSigilCooldown = 0;
        public int shadowSigilCooldown = 0;
        public int aridSigilCooldown = 0;


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

        public bool FrigidHelmet = false;
        public bool FrigidBreastplate = false;


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
                if (new Vector2(Structures.IcePalacePosX, Structures.IcePalacePosY) == new Vector2(0, 0) || AGUtils.GetTileCounts(ModContent.TileType<FrigidiumOre>()) <= 0)
                {
                    Main.NewTextMultiline("Your world is missing ores and/or structures\nPlease use a Worldly Scroll ([i:AwfulGarbageMod/WorldlyScroll]) to generate any missing ores and structures", c: Color.Red);
                }
            }
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.MeleeNoSpeed)
            {
                scale *= MeleeWeaponSize;
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
                if (hit.DamageType == DamageClass.Melee)
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

            //Cactus Shell
            if (cactusShell > 0 && !this.Player.HasBuff(ModContent.BuffType<CactusShellCooldown>()))
            {

                Vector2 vel;
                vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - this.Player.Center.X, (float)Main.mouseY + Main.screenPosition.Y - this.Player.Center.Y);
                vel.Normalize();
                vel *= 7.5f;
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel, Mod.Find<ModProjectile>("CactusShellProj").Type, cactusShell, 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(10)), Mod.Find<ModProjectile>("CactusShellProj").Type, cactusShell, 1, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center - new Vector2(4, 4), vel.RotatedBy(MathHelper.ToRadians(-10)), Mod.Find<ModProjectile>("CactusShellProj").Type, cactusShell, 1, Main.myPlayer);
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
                    Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldown>(), 15 * 60);
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

        public override void PostUpdateEquips()
        {
            Player player = this.Player;
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
            if (CobaltMelee )
            {
                Player.statDefense += CobaltMeleeDefense;
            }
        }

        public override void OnEnterWorld()
        {
            messageTimer = 180;
            
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
            GlacialEye = false;
            WhipDebuffDurationPrev = WhipDebuffDuration;
            WhipDebuffDuration = 1;
            IlluminantString = false;
            flailSpinSpd = 1;
            flailRange = 1;
            DoubleVisionBand = false;
            jungleSporeFlail = 0; 
            mechanicalArm = 1;
            poisonSigil = 0;
            waterSigil = 0;
            fireSigil = false;
            fireSigilCooldown -= 1;
            shadowSigil = 0;
            shadowSigilCooldown -= 1;
            aridSigil = false;
            aridSigilCooldown -= 1;
            empowermentCooldowMultiplier = 1;
            GlacialEyePassive = false;
            EmpowermentSlotPrevious = EmpowermentSlot;
            EmpowermentSlot = false;
            CobaltRanged = false;
            CobaltMelee = false;
            CobaltMeleeCooldown -= 1;
            if (CobaltMeleeCooldown < 0)
            {
                CobaltMeleeDefense = 0;
            }
            PalladiumRanged = false;
            PalladiumMelee = false;
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
    }
}