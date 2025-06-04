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
using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.ItemDropRules;

namespace AwfulGarbageMod.Global;
public class GlobalLoot : GlobalNPC
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
            npcLoot.Add(ItemDropRule.ByCondition(new UnrealCondition(), ModContent.ItemType<SlimyLocket>(), 1, 1, 1));

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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PileOfFakeBones>(), 15));
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

    public override void ModifyGlobalLoot(Terraria.ModLoader.GlobalLoot globalLoot)
    {
        globalLoot.Add(ItemDropRule.ByCondition(new UnrealSoulDropCondition(), ModContent.ItemType<UnrealEssence>(), 100, 2, 7));
    }
}


