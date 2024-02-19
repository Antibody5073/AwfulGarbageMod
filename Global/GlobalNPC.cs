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
        if (npc.type == NPCID.IlluminantBat || npc.type == NPCID.IlluminantSlime)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IlluminantString>(), 20, 1, 1));
        }
        if (npc.type == NPCID.DarkCaster)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SigilOfWater>(), 18, 1, 1));
        }
        if (npc.type == NPCID.GoblinThief)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThiefsKnife>(), 11, 1, 1));
        }
        if (npc.type == NPCID.AngryBones || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoneChalice>(), 120, 1, 1));
        }
    }


    //Crit damage boosts
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        Player player = Main.LocalPlayer;

        //Ice Crystal Geode
        if (player.GetModPlayer<GlobalPlayer>().iceCrystalGeode && (projectile.DamageType == DamageClass.Melee || projectile.DamageType == DamageClass.MeleeNoSpeed))
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
        if (player.GetModPlayer<GlobalPlayer>().meteoriteGeode && (projectile.DamageType == DamageClass.Melee || projectile.DamageType == DamageClass.MeleeNoSpeed) && projectile.type != Mod.Find<ModProjectile>("GeodeMeteor").Type)
        {

            if (!player.HasBuff(ModContent.BuffType<MeteorGeodeCooldown>()))
            {
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), npc.Top + new Vector2(0, -500), new Vector2(0, 10), Mod.Find<ModProjectile>("GeodeMeteor").Type, 52, 4, player.whoAmI);
                player.AddBuff(ModContent.BuffType<MeteorGeodeCooldown>(), 2 * 60);
            }
        }

        //Frozen Spirit set bonus
        if ((player.GetModPlayer<GlobalPlayer>().FrozenSpiritBonus || player.GetModPlayer<GlobalPlayer>().FrigidHelmet) && (npc.HasBuff(BuffID.Frostburn) || npc.HasBuff(BuffID.Frostburn2)) && (projectile.DamageType == DamageClass.Melee || projectile.DamageType == DamageClass.MeleeNoSpeed))
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

                if (projectile.DamageType == DamageClass.Melee || projectile.DamageType == DamageClass.MeleeNoSpeed)
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
    }

    public override void SetDefaults(NPC npc)
    {
        // We want our ExampleJavelin buff to follow the same immunities as BoneJavelin
        npc.buffImmune[BuffType<Buffs.BoneSkewerBleed>()] = npc.buffImmune[BuffID.BoneJavelin];

        if (npc.type == NPCID.DD2Betsy && ModContent.GetInstance<Configs.Config>().BetsyNerf)
        {
            npc.lifeMax /= 2;


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


