using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee.Hardmode;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Tiles.OresBars;
using log4net.Core;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using Steamworks;
using StramClasses;
using System;
using System.Security.Cryptography;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Humanizer.In;

namespace AwfulGarbageMod.Global
{
    public class EarthenShield : ModPlayer
    {
        public int shieldDurability; //Do not set this value.
        public int shieldMaxDurability; //Set this to a damage value
        public bool shieldEquipped = false; //Set this in all shields to have this stat
        public bool shieldEquippedPrev = false; 
        public bool overrideShieldBar = false; //Set this in all shields to have this stat
        public float shieldCooldown; //Do not set this
        public int shieldMaxCooldown; //Set this to the cooldown length
        public bool shieldsActive;
        public int shieldMaxCooldownPrevious;
        public int shieldDamageTaken;
        public float ShieldDR = 0;
        public Item shieldType;
        public float shieldDurabilityMult;
        public int flatBonusDurability;
        public float shieldCooldownMult;
        public bool overrideShieldBreak;
        public Color shieldBreakColor;
        public int attackTick;
        public float ShieldEffectPower;
        public float shieldRegenTimer = 0;
        public float shieldRegenStrength = 0;

        //Shield Accessories
        public bool reflexiveCharm = false;
        public bool verdantStimulant = false;

        public override void UpdateEquips()
        {
            if (Player.itemAnimation > 0 && Player.HeldItem.damage > 0) { }
            else
            {
                shieldCooldownMult *= 2;
            }
        }
        public override void PreUpdate()
        {
            int maxShieldDurability = (int)(shieldMaxDurability * shieldDurabilityMult) + flatBonusDurability;
            MathHelper.Clamp(maxShieldDurability, 1, 9999);
            if (shieldEquipped != shieldEquippedPrev)
            {
                shieldDurability = 0;
                shieldsActive = false;
                shieldMaxCooldownPrevious = 0;
                shieldCooldown = shieldMaxCooldown;
            }
            if (!(shieldEquipped || overrideShieldBar) && shieldMaxCooldownPrevious > 0)
            {
                shieldsActive = false;
                shieldMaxCooldownPrevious = 0;
                shieldCooldown = 0;
            }
            if ((shieldEquipped || overrideShieldBar) && shieldMaxCooldownPrevious == 0)
            {
                shieldCooldown = shieldMaxCooldown;
            }
            if ((shieldEquipped || overrideShieldBar))
            {
                if (shieldCooldown <= 0 && !shieldsActive)
                {
                    shieldDurability = maxShieldDurability;
                    shieldsActive = true;
                }
                if (shieldDurability > maxShieldDurability)
                {
                    shieldDurability = maxShieldDurability;
                }
            }
            shieldCooldown -= shieldCooldownMult;

            if (shieldsActive)
            {
                if (shieldRegenTimer > 60)
                {
                    if (shieldDurability < maxShieldDurability)
                    {
                        while (shieldRegenTimer > 60)
                        {
                            shieldRegenTimer -= 60;
                            shieldDurability++;
                        }
                    }
                }
                else
                {
                    shieldRegenTimer += shieldRegenStrength;
                }
            }
            if (shieldType != null && shieldsActive)
            {
                float distanceFromTarget;
                switch (shieldType.ModItem.Name)
                {
                    case "Nutshell":
                        if (attackTick <= 0 && Player.itemAnimation > 0)
                        {
                            attackTick = 45;

                            Vector2 mousePos = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);

                            // If found, change the velocity of the projectile and turn it in the direction of the target
                            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                            Vector2 newVel = (mousePos - Player.Center).SafeNormalize(Vector2.Zero) * AGUtils.ScaleShieldEffectPower(8f, Player, 1);
                            Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_ItemUse_WithPotentialAmmo(shieldType, AmmoID.None), Player.Center, newVel, ModContent.ProjectileType<AcornStaffProj>(), Player.GetWeaponDamage(shieldType), Player.GetTotalKnockback<SentinelDamageClass>().ApplyTo(shieldType.knockBack), Player.whoAmI, 0, 2);
                            proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                            proj.ArmorPenetration += 10;

                        }
                        break;
                    case "WaterVeil":
                        if (attackTick <= 0 && Player.itemAnimation > 0)
                        {
                            attackTick = 42;

                            Projectile proj;
                            proj = Projectile.NewProjectileDirect(Player.GetSource_ItemUse_WithPotentialAmmo(shieldType, AmmoID.None), Player.Center, new Vector2(12, 0), ProjectileID.WaterBolt, Player.GetWeaponDamage(shieldType), Player.GetTotalKnockback<SentinelDamageClass>().ApplyTo(shieldType.knockBack), Player.whoAmI, 0, 2);
                            proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                            proj.timeLeft = 42;
                            proj = Projectile.NewProjectileDirect(Player.GetSource_ItemUse_WithPotentialAmmo(shieldType, AmmoID.None), Player.Center, new Vector2(-12, 0), ProjectileID.WaterBolt, Player.GetWeaponDamage(shieldType), Player.GetTotalKnockback<SentinelDamageClass>().ApplyTo(shieldType.knockBack), Player.whoAmI, 0, 2);
                            proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                            proj.timeLeft = 42;
                        }
                        distanceFromTarget = AGUtils.ScaleShieldEffectPower(200, Player);
                        bool faster = false;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];

                            if (npc.CanBeChasedBy())
                            {
                                float between = Vector2.Distance(npc.Center, this.Player.Center);
                                bool inRange = between < distanceFromTarget;

                                if (inRange)
                                {
                                    faster = true;
                                    npc.AddBuff(ModContent.BuffType<WaterVeilBuff>(), 5);
                                }
                            }
                        }
                        if (faster)
                        {
                            attackTick--;
                        }
                        break;
                    case "Microwave":
                        distanceFromTarget = AGUtils.ScaleShieldEffectPower(200, Player);
                        if (attackTick <= 0)
                        {
                            float damage = 0.5f;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];

                                if (npc.CanBeChasedBy())
                                {
                                    float between = Vector2.Distance(npc.Center, this.Player.Center);
                                    bool inRange = between < distanceFromTarget;

                                    if (inRange)
                                    {
                                        attackTick = 24;
                                        damage += 0.5f;
                                        Player.addDPS(npc.SimpleStrikeNPC(Player.GetWeaponDamage(shieldType), (Player.Center.X < npc.Center.X) ? 1 : (-1), Main.rand.NextBool(Player.GetWeaponCrit(shieldType), 100), Player.GetWeaponKnockback(shieldType)));
                                        npc.AddBuff(BuffID.OnFire, 60);
                                    }
                                }
                            }
                            if (attackTick == 24)
                            {
                                Player.HurtInfo hurtInfo = new Player.HurtInfo()
                                {
                                    SourceDamage = (int)damage,
                                    Knockback = 0,
                                    HitDirection = 0,
                                    Damage = (int)damage
                                };
                                DamageShield((int)damage, 1, hurtInfo);
                            }
                        }
                        break;
                    case "BubbleBarrier":
                        if (attackTick <= 0)
                        {
                            attackTick = 12;
                            Projectile proj;
                            proj = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), Player.Center, Vector2.One.RotatedByRandom(MathHelper.ToRadians(360)), ProjectileID.FlaironBubble, Player.GetWeaponDamage(shieldType), Player.GetWeaponKnockback(shieldType), Main.myPlayer, -10);
                            proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                        }
                        break;
                }
            }
        }
        public override void ResetEffects()
        {
            shieldEquippedPrev = shieldEquipped;
            shieldEquipped = false;
            overrideShieldBar = false;

            shieldMaxDurability = 0;
            shieldMaxCooldownPrevious = shieldMaxCooldown;
            shieldMaxCooldown = 0;

            shieldType = null;
            shieldCooldownMult = 1;
            shieldDurabilityMult = 1;
            flatBonusDurability = 0;
            shieldBreakColor = Color.LightGray;
            overrideShieldBreak = false;
            ShieldEffectPower = 1;
            attackTick--;
            shieldRegenStrength = 0;

            reflexiveCharm = false;
            verdantStimulant = false;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= ChangeHurt();

        }
        public float ChangeHurt()
        {
            float finalDamage = 1;

            if (shieldsActive)
            {
                int opd2;
                /*
                if (opd > 0)
                {
                    opd2 = (int)MathF.Abs(opd);
                    finalDamage *= opd2;
                }
                else if (opd < 0)
                {
                    opd2 = (int)MathF.Abs(opd);
                    finalDamage /= opd2;
                }
                else
                { }*/
                finalDamage *= ShieldDR;

            }
            return finalDamage;

        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if ((shieldEquipped || overrideShieldBar) && shieldDurability > 0)
            {
                int shieldDmg;
                if (!overrideShieldBreak)
                {
                    shieldDmg = (int)(info.SourceDamage * (1 - ShieldDR));
                }
                else
                {
                    shieldDmg = OverrideShieldDamage(shieldType, info);
                }

                DamageShield(shieldDmg, 1, info);
            }
        }
        public void DamageShield(int dmg, float damageModifier, Player.HurtInfo info)
        {/*
            if (osd != 0)
            {
                if (osd2 != 0 || osd3 != 0)
                {
                    shieldDurability -= osd + Main.rand.Next(osd2, osd3 + 1);
                }
                else
                {
                    shieldDurability -= osd;
                }
            }
            else
            {
            }*/
            shieldDurability -= (int)(dmg * damageModifier);

            CombatText.NewText(Player.getRect(), Color.DarkCyan, dmg);
            ShieldDamageEffects(info, (int)MathHelper.Clamp((int)(dmg * damageModifier), 0, shieldDurability));

            if (shieldDurability <= 0)
            {
                ShieldBreak(shieldType);
            }
        }
        public void ShieldDamageEffects(Player.HurtInfo info, int shieldDamageTaken)
        {
            if (reflexiveCharm)
            {
                NPC target = AGUtils.GetClosestNPC(Player.Center, 1500);
                Vector2 projVel;
                if (target == null)
                {
                    projVel = Player.Center.DirectionTo(Main.MouseWorld) * MathHelper.Clamp(0.75f + info.SourceDamage * 0.01f, 0, 16);
                }
                else
                {
                    projVel = Player.Center.DirectionTo(target.Center) * MathHelper.Clamp(0.75f + info.SourceDamage * 0.01f, 0, 16);
                }
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), Player.Center, projVel, ModContent.ProjectileType<ReflexiveProj>(), AGUtils.ScaleDamage(10, Player, ModContent.GetInstance<SentinelDamageClass>()) + shieldDamageTaken * 4, 4, Player.whoAmI);

            }
        }
        public int OverrideShieldDamage(Item type, Player.HurtInfo info)
        {
            int modifiedDmg = (int)(info.SourceDamage * (1 - ShieldDR));
            float ModifiedDR = 0f;
            /*
            switch (type)
            {
                case "SporeShield":
                    ModifiedDR = 0.125f;
                    modifiedDmg = info.SourceDamage / 8;
                    break;
                case "StoneShield":
                    ModifiedDR = 0.15f;
                    modifiedDmg = info.SourceDamage / 7;
                    break;
                case "TurtleShield":
                    modifiedDmg = info.SourceDamage;
                    break;
                case "CataShield":
                    modifiedDmg = info.SourceDamage * 2;
                    break;
            }*/
            return modifiedDmg;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f)
            {
                if (shieldType != null)
                {
                    switch (shieldType.ModItem.Name)
                    {
                        case "GoldShield":
                            if (shieldsActive)
                            {
                                AGUtils.DrawCircle(Player.Center - Main.screenPosition, AGUtils.ScaleShieldEffectPower(225, Player), Color.Gold * 0.25f, 2, 32);
                            }
                            break;
                        case "PlatinumShield":
                            if (shieldsActive)
                            {
                                AGUtils.DrawCircle(Player.Center - Main.screenPosition, AGUtils.ScaleShieldEffectPower(225, Player), Color.LightGray * 0.25f, 2, 32);
                            }
                            break;
                        case "WaterVeil":
                            if (shieldsActive)
                            {
                                AGUtils.DrawCircle(Player.Center - Main.screenPosition, AGUtils.ScaleShieldEffectPower(200, Player), Color.Blue * 0.25f, 2, 32);
                            }
                            break;
                        case "Microwave":
                            if (shieldsActive)
                            {
                                AGUtils.DrawCircle(Player.Center - Main.screenPosition, AGUtils.ScaleShieldEffectPower(200, Player), Color.OrangeRed * 0.25f, 2, 32);
                            }
                            break;
                    }
                }
            }
        }
        public void ShieldBreak(Item type)
        {
            CombatText.NewText(Player.getRect(), shieldBreakColor, "Shield Break!", dramatic: true);

            shieldsActive = false;
            shieldDurability = 0;
            shieldCooldown = shieldMaxCooldown;
            float distanceFromTarget;
            switch (type.ModItem.Name)
            {
                case "EnchantedShield":
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 newPos = Player.Center;
                        newPos += Main.rand.NextVector2Circular(150, 150) + new Vector2(0, -800);
                        Vector2 newVel = newPos.DirectionTo(Main.MouseWorld) * 14.5f;
                        newVel *= 1f - Main.rand.NextFloat(0.2f);

                        Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_ItemUse_WithPotentialAmmo(type, AmmoID.None), newPos, newVel, ProjectileID.HallowStar, Player.GetWeaponDamage(type), Player.GetTotalKnockback<SentinelDamageClass>().ApplyTo(type.knockBack), Player.whoAmI);
                        proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                        proj.light = 0;
                        proj.tileCollide = false;

                    }
                    break;
                case "GoldShield":
                    distanceFromTarget = AGUtils.ScaleShieldEffectPower(225, Player);

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
                                npc.AddBuff(BuffID.Midas, AGUtils.ScaleShieldEffectPower(750, Player));
                                Player.addDPS(npc.SimpleStrikeNPC(Player.GetWeaponDamage(type), (Player.Center.X < npc.Center.X) ? 1 : (-1), Main.rand.NextBool(Player.GetWeaponCrit(type), 100), Player.GetWeaponKnockback(type)));
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
                                    new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(npc.Hitbox) },
                                    Player.whoAmI);

                            }
                        }
                    }
                    break;
                case "PlatinumShield":
                    Player.AddBuff(ModContent.BuffType<PlatinumShieldLuck>(), AGUtils.ScaleShieldEffectPower(600, Player));

                    distanceFromTarget = AGUtils.ScaleShieldEffectPower(225, Player);

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
                                Player.addDPS(npc.SimpleStrikeNPC(Player.GetWeaponDamage(type), (Player.Center.X < npc.Center.X) ? 1 : (-1), Main.rand.NextBool(Player.GetWeaponCrit(type), 100), Player.GetWeaponKnockback(type)));
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Keybrand,
                                    new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(npc.Hitbox) },
                                    Player.whoAmI);

                            }
                        }
                    }
                    break;
                case "ArachnidAegis":
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_ItemUse_WithPotentialAmmo(type, AmmoID.None), Player.Center, new Vector2(0, -6) + Main.rand.NextVector2Circular(3, 3), ProjectileID.BabySpider, Player.GetWeaponDamage(type), Player.GetTotalKnockback<SentinelDamageClass>().ApplyTo(type.knockBack), Player.whoAmI);
                        proj.DamageType = ModContent.GetInstance<SentinelDamageClass>();
                    }
                    break;
                case "PalladiumShield":
                    Player.AddBuff(BuffID.RapidHealing, AGUtils.ScaleShieldEffectPower(720, Player));
                    break;
            }
            if (Player.GetModPlayer<GlobalPlayer>().ShadowflameOnShieldBreak)
            {

                distanceFromTarget = 330;

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
                            npc.AddBuff(BuffID.ShadowFlame, AGUtils.ScaleShieldEffectPower(60 * 10, Player));
                        }
                    }
                }
            }
            if (Player.GetModPlayer<GlobalPlayer>().CrabBonus)
            {

                NPC closestNPC = FindClosestNPC(300);

                if (closestNPC != null)
                {
                    Vector2 vel = (closestNPC.Center - this.Player.Center).SafeNormalize(Vector2.One);
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), this.Player.Center, vel, ModContent.ProjectileType<CrabClaw>(), AGUtils.ScaleDamage(120, Player, ModContent.GetInstance<SentinelDamageClass>()), 20, Player.whoAmI);
                }
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
    }
}