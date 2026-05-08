using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Items.Ammo;
using AwfulGarbageMod.Items.Ammo.Hardmode;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Armor.Hardmode;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace AwfulGarbageMod.Global
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    public class ItemAccessoryEffects : GlobalItem
    {
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            speed *= player.GetModPlayer<GlobalPlayer>().HorizontalWingSpdMult;
        }
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            maxAscentMultiplier *= player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult;
            ascentWhenRising *= player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult;
            ascentWhenFalling *= player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult;

        }

        public override bool InstancePerEntity => true;
        int shotNumber;
        public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue)
        {
            if (player.GetModPlayer<GlobalPlayer>().lightningRing == true && item.healMana > 0)
            {
                if (quickHeal)

                {
                    player.AddBuff(BuffID.Electrified, 2 * 60);
                }
                healValue = (int)(healValue * 1.2f);
            }
            if (player.GetModPlayer<GlobalPlayer>().MyceliumHoodBonus == true && !player.HasBuff(BuffID.ManaSickness))
            {
                if (quickHeal)
                {
                    for (int j = 0; j < Main.rand.Next(1, 3); j++)
                    {
                        Vector2 vel = new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), player.Center, vel, Mod.Find<ModProjectile>("MyceliumProj").Type, 35, 2, Main.myPlayer, player.manaCost);
                    }
                }
            }
            if (player.GetModPlayer<GlobalPlayer>().HarujionPetal > 0)
            {
                if (quickHeal)
                {
                    if (player.GetModPlayer<GlobalPlayer>().HarujionLevel > 5)
                    {
                        for (var i = 0; i < 6; i++)
                        {
                            int proj = Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 7).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("IceSpiritPikeSpiritProj").Type, (int)player.GetModPlayer<GlobalPlayer>().HarujionLevel, 2f, Main.myPlayer, 2);
                            Main.projectile[proj].DamageType = DamageClass.Magic;
                            Main.projectile[proj].penetrate = 1;
                        }
                        player.GetModPlayer<GlobalPlayer>().HarujionLevel = 0;
                    }
                }
                healValue = (int)(healValue * 1.2f);

            }
            healValue += player.GetModPlayer<GlobalPlayer>().AntediluvianPotionBoost;
            base.GetHealMana(item, player, quickHeal, ref healValue);
        }

        public override bool OnPickup(Item item, Player player)
        {
            if ((item.type == 58 || item.type == 1734 || item.type == 1867) && player.GetModPlayer<GlobalPlayer>().ForbiddenFruit)
            {
                player.statLife -= 20;
            }
            return base.OnPickup(item, player);
        }
        public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
        {

            if (item.healLife > 0)
            {
                if (player.GetModPlayer<GlobalPlayer>().MeatShield > 0)
                {
                    healValue = (int)(healValue * player.GetModPlayer<GlobalPlayer>().MeatShieldBonus);
                }
            }

            healValue += (int)MathHelper.Clamp(player.GetModPlayer<GlobalPlayer>().SaltedTearsEnergy, 0, healValue * 0.5f);
            healValue += player.GetModPlayer<GlobalPlayer>().AntediluvianPotionBoost;
            base.GetHealLife(item, player, quickHeal, ref healValue);
        }
        public override void OnConsumeItem(Item item, Player player)
        {
            if (item.healLife > 0)
            {
                player.GetModPlayer<GlobalPlayer>().SaltedTearsEnergy = 0;
            }

        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().ForbiddenFruit && item.healLife > 0)
            {
                return false;
            }
            return base.CanUseItem(item, player);
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().JunkGreaves && Main.rand.NextBool(4))
            {
                return false;
            }
            if (player.body == ModContent.ItemType<AncientFlierBreastplate>() && Main.rand.NextBool(5))
            {
                return false;
            }
            if (player.body == ModContent.ItemType<MidnightBreastplate>() && Main.rand.NextBool(10))
            {
                return false;
            }
            if (player.legs == ModContent.ItemType<GoreKnightLeggings>() && Main.rand.NextBool(15, 100))
            {
                return false;
            }
            if ((player.head == ModContent.ItemType<StormHeadgear>() || player.head == ModContent.ItemType<StormHelmet>() || player.head == ModContent.ItemType<StormHood>()) && Main.rand.NextBool(3, 20))
            {
                return false;
            }
            if (ModContent.GetInstance<Config>().HardmodeMetalRework && player.legs == ItemID.OrichalcumLeggings && Main.rand.NextBool(7, 20))
            {
                return false;
            }
            if (player.GetModPlayer<GlobalPlayer>().AncientGadgets && Main.rand.NextBool(5))
            {
                return false;
            }

            return true;

        }

        public override bool? UseItem(Item item, Player player)
        {
            shotNumber++;

            return base.UseItem(item, player);
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            List<float> mechanicalAccessoryStats = [player.GetModPlayer<GlobalPlayer>().mechanicalArm, player.GetModPlayer<GlobalPlayer>().mechanicalLens, player.GetModPlayer<GlobalPlayer>().mechanicalScope];
            if (shotNumber % 3 == 0)
            {
                //Mechanical accessories
                if (item.DamageType == ModContent.GetInstance<KnifeDamageClass>())
                {
                    if (player.GetModPlayer<GlobalPlayer>().mechanicalArm > 1)
                    {
                        NPC target = FindClosestNPC(1600, player);
                        if (target != null)
                        {
                            float magnitude = Vector2.Distance(Vector2.Zero, velocity);
                            velocity = (target.Center - position).SafeNormalize(Vector2.Zero);
                            velocity *= magnitude;

                        }
                        damage = (int)(damage * player.GetModPlayer<GlobalPlayer>().mechanicalArm);
                    }
                }
                else if (item.useAmmo == AmmoID.Arrow)
                {
                    if (player.GetModPlayer<GlobalPlayer>().mechanicalLens > 1)
                    {
                        NPC target = FindClosestNPC(1600, player);
                        if (target != null)
                        {
                            float magnitude = Vector2.Distance(Vector2.Zero, velocity);
                            velocity = (target.Center - position).SafeNormalize(Vector2.Zero);
                            velocity *= magnitude;

                        }
                        damage = (int)(damage * player.GetModPlayer<GlobalPlayer>().mechanicalLens);
                    }
                }
                else if (item.useAmmo == AmmoID.Bullet)
                {
                    if (player.GetModPlayer<GlobalPlayer>().mechanicalScope > 1)
                    {
                        NPC target = FindClosestNPC(1600, player);
                        if (target != null)
                        {
                            float magnitude = Vector2.Distance(Vector2.Zero, velocity);
                            velocity = (target.Center - position).SafeNormalize(Vector2.Zero);
                            velocity *= magnitude;

                        }
                        damage = (int)(damage * player.GetModPlayer<GlobalPlayer>().mechanicalScope);
                    }
                }
                else if (item.DamageType == DamageClass.Ranged)
                {
                    if (player.GetModPlayer<GlobalPlayer>().mechanicalArm > 1 && player.GetModPlayer<GlobalPlayer>().mechanicalScope > 1 && player.GetModPlayer<GlobalPlayer>().mechanicalLens > 1)
                    {
                        NPC target = FindClosestNPC(1600, player);
                        if (target != null)
                        {
                            float magnitude = Vector2.Distance(Vector2.Zero, velocity);
                            velocity = (target.Center - position).SafeNormalize(Vector2.Zero);
                            velocity *= magnitude;
                        }
                        mechanicalAccessoryStats.Sort();
                        damage = (int)(damage * mechanicalAccessoryStats.First());
                    }
                }               
            }

            if (type == ProjectileID.Bullet)
            {
                if (player.GetModPlayer<GlobalPlayer>().OrichalcumGun)
                {
                    type = ModContent.ProjectileType<OrichalcumBullet>();
                }
            }
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                if (player.GetModPlayer<GlobalPlayer>().OrichalcumBow)
                {
                    type = ModContent.ProjectileType<OrichalcumArrow>();
                }
            }
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            //Fleshy Amalgam
            if (player.GetModPlayer<GlobalPlayer>().FleshyAmalgam)
            {
                if (shotNumber % 3 == 0)
                {
                    crit = 100;
                }
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.DamageType == ModContent.GetInstance<KnifeDamageClass>())
            {
                if (player.GetModPlayer<GlobalPlayer>().waterSigil > 0)
                {
                    if (shotNumber % 4 == 0)
                    {
                        Vector2 vel;
                        vel = velocity.SafeNormalize(Vector2.One) * 12;

                        float distanceFromTarget = Vector2.Distance(position, Main.MouseWorld);
                        vel.Y -= (distanceFromTarget / 24) * 0.15f;
                        int proj = Projectile.NewProjectile(source, position, vel, ModContent.ProjectileType<WaterStreamRanged>(), (int)(damage * player.GetModPlayer<GlobalPlayer>().waterSigil), 2, player.whoAmI);
                    }
                }
                if (player.GetModPlayer<GlobalPlayer>().terraSigil)
                {
                    if (shotNumber % 6 == 0)
                    {
                        List<NPCandValue> npcDistances = new List<NPCandValue> { };


                        // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
                        float sqrMaxDetectDistance = 1280 * 1280;

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
                                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, player.Center);

                                // Check if it is within the radius
                                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                                {
                                    npcDistances.Add(
                                        new NPCandValue { npc = target, value = sqrDistanceToTarget }
                                        );
                                }
                            }
                        }

                        npcDistances.Sort();

                        int npcs = npcDistances.Count;
                        if (npcs > 4)
                        {
                            npcs = 4;
                        }
                        if (npcs > 0)
                        {
                            for (int i = 0; i < npcs; i++)
                            {
                                Projectile.NewProjectile(source, npcDistances[i].npc.Bottom + new Vector2(0, 32), Vector2.Zero, ModContent.ProjectileType<TerraWristSwordProj>(), damage * 4 / 5, 5, player.whoAmI, npcDistances[i].npc.whoAmI);
                            }
                        }
                    }
                }
                if (player.GetModPlayer<GlobalPlayer>().shadowSigil > 0)
                {
                    if (shotNumber % 7 == 0)
                    {
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
                        int proj = Projectile.NewProjectile(source, position, vector2, 496, damage, 2, player.whoAmI, num87, num77);
                        Main.projectile[proj].DamageType = DamageClass.Ranged;
                    }
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public override void UseAnimation(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().TitaniumVariety)
            {
                if (item.CountsAsClass(DamageClass.Melee))
                {
                        Projectile proj = Projectile.NewProjectileDirect(item.GetSource_FromThis(), player.Center, new Vector2(Main.mouseX + Main.screenPosition.X - player.Center.X, Main.mouseY + Main.screenPosition.Y - player.Center.Y).SafeNormalize(Vector2.Zero) * 7 + Main.rand.NextVector2CircularEdge(1, 1), ModContent.ProjectileType<TitaniumShard>(), AGUtils.ScaleDamage(item.damage, player, DamageClass.Melee) * 3 / 10, 2);
                        proj.frame = Main.rand.Next(12);
                }
            }
            base.UseAnimation(item, player);
        }
        public NPC FindClosestNPC(float maxDetectDistance, Player player)
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
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, player.Center);

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
