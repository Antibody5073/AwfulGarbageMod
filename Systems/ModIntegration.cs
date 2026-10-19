using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Ammo;
using AwfulGarbageMod.Items.Ammo.Hardmode;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Magic.Hardmode;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Melee.Hardmode;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Ranged.Hardmode;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Items.Weapons.Summon.Hardmode;
using AwfulGarbageMod.NPCs;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.NPCs.Town;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using gunrightsmod.Content.DamageClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Bson;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.ModIntegration

{
    // Showcases using Mod.Call of other mods to facilitate mod integration/compatibility/support
    // Mod.Call is explained here https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate
    // This only showcases one way to implement such integrations, you are free to explore your own options and other mods examples

    // You need to look for resources the mod developers provide regarding how they want you to add mod compatibility
    // This can be their homepage, workshop page, wiki, github, discord, other contacts etc.
    // If the mod is open source, you can visit its code distribution platform (usually GitHub) and look for "Call" in its Mod class
    public class ModIntegrationsSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

            // Census Mod allows us to add spawn information to the town NPCs UI:
            // https://forums.terraria.org/index.php?threads/.74786/
            DoCensusIntegration();

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();

            // We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
            // DoMusicDisplayIntegration();
            DoRecipeBrowserIntegration();

            DoRedemptionIntegration();
        }
        public void DoRedemptionIntegration()
        {
            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                RedemptionMelee(mod);
                RedemptionRanged(mod);
                RedemptionMagic(mod);
                RedemptionSummon(mod);
                RedemptionAmmo(mod);
                RedemptionNPC(mod);
                

            }

        }
        private void RedemptionMelee(Mod mod)
        {

            Redemption.AddElementToItem(ModContent.ItemType<Cirrus>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CirrusProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CloudCleaver>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudCleaverProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Constellate>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ConstellateProj>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ConstellateProj2>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Constellate>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ConstellateProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ConstellateProj2>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<DarkStar>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<DarkStarProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FlintSpear>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlintSpearProj>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Foliaggregate>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FoliaggregateProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FoliaggregateProj2>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrigidiumFlails>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidiumFlailsProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrigidiumSpear>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToItem(ModContent.ItemType<FrigidiumSpear>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidiumSpearProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidiumSpearProj2>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrostFlareBoomerang>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToItem(ModContent.ItemType<FrostFlareBoomerang>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrostFlareBoomerangProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrostFlareBoomerangProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrostFlareBoomerangProjIcicle>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrostFlareBoomerangProjFlame>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Funguerilla>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FunguerillaProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<GrassBasher>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GrassBasherProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<GrassBasher>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GrassBasherProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BeheMoth>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BeheMothProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<BeheMoth>(), Redemption.ElementID.Explosive, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BeheMothProj>(), Redemption.ElementID.Explosive, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CryoCleaver>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CryoCleaverProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CryoCleaverProj2>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CursedFlametongue>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<CursedFlametongue>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedFlametongueFlame>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedFlametongueFlame>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<LeeNunchucks>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeeNunchucksProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeeNunchucksProj2>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<PhantomFlail>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PhantomFlailProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<PumpkingHarvester>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PumpkingHarvesterProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Pyrochain>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PyrochainProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PyrochainProjHitbox>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<RottenChainfork>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<RottenChainforkProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SoulSpear>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SoulSpearProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SoulSpearProjSoul>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TheSandstorm>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TheSandstorm>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheSandstormProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheSandstormProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TheTumbler>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TheTumbler>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheTumblerProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheTumblerProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheTumblerProj2>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheTumblerProj2>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VortexStorm>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToItem(ModContent.ItemType<VortexStorm>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VortexStormProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VortexStormProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Hellslash>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HellslashProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HellslashExplosion>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Icebreaker>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IcebreakerProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<IceSpiritPike>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToItem(ModContent.ItemType<IceSpiritPike>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritPikeProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritPikeProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritPikeSpiritProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritPikeSpiritProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<InfernalBlade>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<InfernalBladeExplosion>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<InfernalBladeProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<LavaLance>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LavaLanceProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LavaLanceProj2>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<LeafBlade>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeafBladeProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Leafstorm>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeafstormProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<LeafyDisc>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeafyDiscProj>(), Redemption.ElementID.Poison, mod);

            Redemption.AddElementToItem(ModContent.ItemType<OrganicGreatsword>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShadeVortex>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadeVortexProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadeVortexProj2>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShadowtrickSpinner>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToItem(ModContent.ItemType<ShadowtrickSpinner>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowtrickSparkProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowtrickSparkProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowtrickSpinnerProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowtrickSpinnerProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShootingStar>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToItem(ModContent.ItemType<ShootingStar>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShootingStarProj>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShootingStarProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SkysBane>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<SkysBane>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SkysBaneProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SkysBaneProj>(), Redemption.ElementID.Thunder, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SweetBeatdown>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<SweetBeatdown>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SweetBeatdownProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SweetBeatdownProj>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SweetBeatdownProj2>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SweetBeatdownProj2>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TetheredEye>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TetheredEyeProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TheEyeOfTheStorm>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheEyeOfTheStormProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheEyeOfTheStormProjLightning>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TheEyeOfTheStorm>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheEyeOfTheStormProj>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TheEyeOfTheStormProjLightning>(), Redemption.ElementID.Thunder, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TidalSlicer>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<WaterStreamMelee>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TidesTurn>(), Redemption.ElementID.Water, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<TrashSlash>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TrashSlashProj>(), Redemption.ElementID.Poison, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<TriElement>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TriElement>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TriElement>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TrueDeathSickle>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TrueDeathSickleProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TsunamiThrow>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TsunamiThrowProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<TyphoonThrow>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TyphoonThrowProj>(), Redemption.ElementID.Water, mod);

        }
        private void RedemptionRanged(Mod mod)
        {
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FeatherPendantProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Aquathreader>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<AquathreaderProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BagOfFrozenKnives>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BagOfFrozenKnivesProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BagOfPoisonedKnives>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BagOfPoisonedKnivesProj>(), Redemption.ElementID.Poison, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BagOfFrozenPoisonedKnives>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BagOfFrozenPoisonedKnivesProj>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToItem(ModContent.ItemType<BagOfFrozenPoisonedKnives>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BagOfFrozenPoisonedKnivesProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BlinkrootBow>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BlinkrootBowProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<BlinkrootBow>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BlinkrootBowProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodstainedKnife>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodstainedKnifeProj>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodstainedKnifeProj2>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CactusBow>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CactusBowProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<DeathRipper>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<DeathStringsSealed>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<DeathStrings>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<DeathStringsProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Eruption>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EruptionProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Eruption>(), Redemption.ElementID.Explosive, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EruptionProj>(), Redemption.ElementID.Explosive, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FlintKnives>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlintKnivesProj>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FossilFirearm>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FossilFirearmProj>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrigidFury>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrigidiumKnives>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidiumKnivesProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrigidiumShotgun>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FrostBow>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrostBowShard>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<GlacierBow>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GlacialEcho>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BubbleSlicers>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BubbleSlicersProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ChaosFlame>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ChaosFlameProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ChlorophyteKnives>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ChlorophyteKnivesProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<EclipseKnives>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EclipseKnivesProjSun>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EclipseKnivesProjMoon>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToItem(ModContent.ItemType<EclipseKnives>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EclipseKnivesProjSun>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<EclipseKnives>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EclipseKnivesProjMoon>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<EctoDaggers>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EctoDaggersProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FireFirearm>(), Redemption.ElementID.Fire, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<GlacialTrail>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GlacialEcho2>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToItem(ModContent.ItemType<GlacialTrail>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GlacialEcho2>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SandShark>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Starfall>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StarfallProj>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Toxichorp>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToxichorpProj>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Toxichorp>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToxichorpProj>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Toxichorp>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToxichorpProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Toxichorp>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToxichorpProj>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Toxichorp>(), Redemption.ElementID.Explosive, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToxichorpProj>(), Redemption.ElementID.Explosive, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HoneycombKnives>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HoneycombKnivesProj>(), Redemption.ElementID.Poison, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HotGlock>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HotGlockProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HydroString>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<WaterStreamRanged>(), Redemption.ElementID.Water, mod);
          
            Redemption.AddElementToItem(ModContent.ItemType<IlluminantRifle>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Imperceptible>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ImperceptibleProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<LightningBurst>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LightningBurstProj>(), Redemption.ElementID.Thunder, mod);

            Redemption.AddElementToItem(ModContent.ItemType<NeuralMessenger>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<NeuralMessengerProj>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToItem(ModContent.ItemType<NeuralMessenger>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<NeuralMessengerProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Rudhira>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<RudhiraProj>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Rudhira>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<RudhiraProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<OceanicPistol>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Phloem>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PhloemProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SaltwaterSpray>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SporeBow>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SporeBowProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SporeBowSpore>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Sunblades>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SunbladesProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VilespitCannon>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VilespitCannonProj>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToItem(ModContent.ItemType<VilespitCannon>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VilespitCannonProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<WingedKnife>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<WingedKnifeProj>(), Redemption.ElementID.Wind, mod);
        }
        private void RedemptionMagic(Mod mod)
        {
            Redemption.AddElementToItem(ModContent.ItemType<AcornStaff>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<AcornStaffProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Anatomy101>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Anatomy101Proj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodBolt>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodBoltProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodVolley>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodyJaws>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodyJawsProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CloudBlaster>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBlasterProj>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBlasterRain>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToItem(ModContent.ItemType<CloudBlaster>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBlasterProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBlasterRain>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Cloudstrike>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudstrikeProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Cloudstrike>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudstrikeProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<DevilSpell>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<DevilSpellProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FlareScepter>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlareScepterOrbit>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlareScepterProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FlintStaff>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlintStaffProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<FlintStaff>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlintStaffProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Items.Weapons.Magic.ForestBeam>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ForestBeamProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Items.Weapons.Magic.ForestBeam>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ForestBeamProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<GreatBite>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<GreatBiteProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Mothstorm>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<MothstormProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Mothstorm>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<MothstormProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HeatTippedToothpickTome>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HeatTippedToothpickTomeProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Items.Weapons.Magic.IceSpiritBeam>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritBeamProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Items.Weapons.Magic.IceSpiritBeam>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritBeamProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Icewave>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IcewaveProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IcewaveProj2>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<LeafScepter>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeafScepterOrbit>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<LeafScepterProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<NaturalSpell>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<NaturalSpellProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<NaturalSpell>(), Redemption.ElementID.Explosive, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<NaturalSpellProj>(), Redemption.ElementID.Explosive, mod);

            Redemption.AddElementToItem(ModContent.ItemType<OcularScepter>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<OcularScepterOrbit>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<OcularScepterProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<PerfectFreeze>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PerfectFreezeProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Sandkicker>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SandkickerProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Sandkicker>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SandkickerProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ScaleShardStorm>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ScaleShardStormProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToItem(ModContent.ItemType<ScaleShardStorm>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ScaleShardStormProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShatterTome>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShatterTomeProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShatterTomeSplit>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShellShardStorm>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShellProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SnowScepter>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SnowScepterOrbit>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SnowScepterProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Sprinkler>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SprinklerProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Starkicker>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StarkickerProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<Starkicker>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StarkickerProj>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToItem(ModContent.ItemType<StoneScepter>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StoneScepterProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StoneScepterOrbit>(), Redemption.ElementID.Earth, mod);
        
            Redemption.AddElementToItem(ModContent.ItemType<StormForecast>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StormForecastProj>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToItem(ModContent.ItemType<StormForecast>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StormForecastProj>(), Redemption.ElementID.Arcane, mod);
        
            Redemption.AddElementToItem(ModContent.ItemType<StormSpell>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<StormSpellProj>(), Redemption.ElementID.Thunder, mod);
        
            Redemption.AddElementToItem(ModContent.ItemType<SwarmScepter>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SwarmScepterOrbit>(), Redemption.ElementID.Poison, mod);
        
            Redemption.AddElementToItem(ModContent.ItemType<SylvanTome>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SylvanTomeProj>(), Redemption.ElementID.Nature, mod);
        
            Redemption.AddElementToItem(ModContent.ItemType<TissueStorm>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TissueStormProj>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToItem(ModContent.ItemType<TissueStorm>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<TissueStormProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ViciousScepter>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ViciousScepterProj>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ViciousScepterOrbit>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VileScepter>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VileScepterProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VileScepterOrbit>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VitalScepter>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VitalScepterOrbit>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VitalScepterProj>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToItem(ModContent.ItemType<VitalScepter>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VitalScepterOrbit>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VitalScepterProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<WormBite>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<WormBiteProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<FireSentry>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FireSentryProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FireSentryProj2>(), Redemption.ElementID.Fire, mod);
        }
        private void RedemptionSummon(Mod mod)
        {
            Redemption.AddElementToItem(ModContent.ItemType<IceSpiritStaff>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritStaffMinion>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritStaffProjectile>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToItem(ModContent.ItemType<IceSpiritStaff>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritStaffMinion>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceSpiritStaffProjectile>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CursedCandleStaff>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedCandleStaffMinion>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedCandleStaffProj>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToItem(ModContent.ItemType<CursedCandleStaff>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedCandleStaffMinion>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedCandleStaffProj>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VerdantStaff>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VerdantStaffMinion>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VerdantStaffProjectile>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CloudBangasa>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBangasaMinion>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBangasaProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<CloudBangasa>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBangasaMinion>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudBangasaProj>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SatelliteParasol>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SatelliteParasolMinion>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SatelliteParasolProj>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<SatelliteParasol>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SatelliteParasolMinion>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SatelliteParasolProj>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HammerOfClay>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HammerOfClayMinion>(), Redemption.ElementID.Earth, mod);

            Redemption.AddElementToItem(ModContent.ItemType<UnholyStaff>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<UnholyStaffMinion>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<UnholyStaffProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToItem(ModContent.ItemType<VultureStaff>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VultureStaffMinion>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToItem(ModContent.ItemType<VultureStaff>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<VultureStaffMinion>(), Redemption.ElementID.Earth, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<IceElementalStaff>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceElementalStaffMinion>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IceElementalStaffProjectile>(), Redemption.ElementID.Ice, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<EyeLasher>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<EyeLasherProj>(), Redemption.ElementID.Blood, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<Wirespool>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<WirespoolProj>(), Redemption.ElementID.Earth, mod);
            
            Redemption.AddElementToItem(ModContent.ItemType<ToadsTongue>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToadsTongueProj>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToItem(ModContent.ItemType<ToadsTongue>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ToadsTongueProj>(), Redemption.ElementID.Psychic, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodChain>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodChainProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CloudChain>(), Redemption.ElementID.Wind, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CloudChainProj>(), Redemption.ElementID.Wind, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Iciclash>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IciclashProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<IciclashProj2>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToItem(ModContent.ItemType<ShadowTap>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowTapProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToItem(ModContent.ItemType<ShadowTap>(), Redemption.ElementID.Psychic, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ShadowTapProj>(), Redemption.ElementID.Psychic, mod);

            Redemption.AddElementToItem(ModContent.ItemType<Thundercrack>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ThundercrackProj>(), Redemption.ElementID.Thunder, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<ThundercrackLightning>(), Redemption.ElementID.Thunder, mod);

            Redemption.AddElementToItem(ModContent.ItemType<HorseSnapper>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<HorseSnapperProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BlightedBranch>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BlightedBranchProj>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToItem(ModContent.ItemType<BlightedBranch>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BlightedBranchProj>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToItem(ModContent.ItemType<BlightedBranch>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BlightedBranchProj>(), Redemption.ElementID.Poison, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SaplingCane>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SaplingCaneMinion>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SaplingCaneProj>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<PileOfGarbage>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PileOfGarbageMinion>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PileOfGarbageProj>(), Redemption.ElementID.Poison, mod);

            Redemption.AddElementToItem(ModContent.ItemType<SnatcherStaff>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SnatcherStaffMinion>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToItem(ModContent.ItemType<BloodFountain>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodFountainMinion>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<BloodFountainProj>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CelesteelWrench>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CelesteelWrenchMinion>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CelesteelWrenchProj>(), Redemption.ElementID.Celestial, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CelesteelWrenchProj2>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToItem(ModContent.ItemType<CursedChandelierStaff>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedChandelierStaffMinion>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CursedChandelierStaffProj>(), Redemption.ElementID.Arcane, mod);
        }
        private void RedemptionAmmo(Mod mod)
        {
            Redemption.AddElementToItem(ModContent.ItemType<FlintArrow>(), Redemption.ElementID.Earth, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FlintArrowProj>(), Redemption.ElementID.Earth, mod);
      
            Redemption.AddElementToItem(ModContent.ItemType<FrigidArrow>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidArrowProj>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<FrigidArrowProj2>(), Redemption.ElementID.Ice, mod);
      
            Redemption.AddElementToItem(ModContent.ItemType<CandescentArrow>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CandescentArrowProj>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<CandescentArrowProj2>(), Redemption.ElementID.Fire, mod);
      
            Redemption.AddElementToItem(ModContent.ItemType<PoisonPellet>(), Redemption.ElementID.Poison, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<PoisonPelletProj>(), Redemption.ElementID.Poison, mod);
      
            Redemption.AddElementToItem(ModContent.ItemType<SpiritPellet>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<SpiritPelletProj>(), Redemption.ElementID.Arcane, mod);
        }
        private void RedemptionNPC(Mod mod)
        {
            Redemption.AddNPCToElementList(ModContent.NPCType<CosmicObserver>(), Redemption.NPCType.Inorganic, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<CursedChandelier>(), Redemption.NPCType.Inorganic, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<CursedChandelier>(), Redemption.NPCType.Hot, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<DecayingBloodflower>(), Redemption.NPCType.Plantlike, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<DecayingBloodflower>(), Redemption.NPCType.Blood, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<FrosshroomStalk>(), Redemption.NPCType.Plantlike, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<FrosshroomStalk>(), Redemption.NPCType.Cold, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<IceChunk>(), Redemption.NPCType.Cold, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<IceSpirit>(), Redemption.NPCType.Cold, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<IceSpirit>(), Redemption.NPCType.Spirit, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<Mosquito>(), Redemption.NPCType.Blood, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<RottingSoulflower>(), Redemption.NPCType.Plantlike, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<RottingSoulflower>(), Redemption.NPCType.Dark, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<SentientSapling>(), Redemption.NPCType.Plantlike, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<StarFighterArm>(), Redemption.NPCType.Robotic, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<StarFighterBody>(), Redemption.NPCType.Robotic, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<StarFighterHead>(), Redemption.NPCType.Robotic, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<StarFighterTail>(), Redemption.NPCType.Robotic, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<SuspiciousLookingBush>(), Redemption.NPCType.Plantlike, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<TrashSlime>(), Redemption.NPCType.Slime, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<TreeToad>(), Redemption.NPCType.Plantlike, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<SeseKitsugai>(), Redemption.NPCType.SkeletonHumanoid, mod);
            
            Redemption.AddNPCToElementList(ModContent.NPCType<EyeOfTheStorm>(), Redemption.NPCType.Wet, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<FrigidiusHead>(), Redemption.NPCType.Cold, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<FrigidiusBody>(), Redemption.NPCType.Cold, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<FrigidiusTail>(), Redemption.NPCType.Cold, mod);

            Redemption.AddNPCToElementList(ModContent.NPCType<Blightdrasil>(), Redemption.NPCType.Blood, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<Blightdrasil>(), Redemption.NPCType.Dark, mod);
            Redemption.AddNPCToElementList(ModContent.NPCType<Blightdrasil>(), Redemption.NPCType.Plantlike, mod);


        }
        private void RedemptionProjectile(Mod mod)
        {
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.ForestBeam>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.IceSpiritBeam>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.IceSpiritBeam>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.HostileBoHProj>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.VilethornSpawner>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.VilethornHostileBase>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.VilethornHostileTip>(), Redemption.ElementID.Shadow, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.HostileForkProj>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CrimsonRodHostileCloud>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CrimsonRodHostileProjectile>(), Redemption.ElementID.Blood, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CursedChandelierProjRise>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CursedChandelierProjHoming>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CursedChandelierProjWater>(), Redemption.ElementID.Water, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.StarFighterProjLaser>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.StarFighterProjStar>(), Redemption.ElementID.Celestial, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.TreeToadLeaf>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.TreeToadLeafExpert>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.TreeToadLeafGrav>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.TreeToadOrb>(), Redemption.ElementID.Nature, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.SeseBulletProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.SeseNonGravProj>(), Redemption.ElementID.Arcane, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.SeseSpinProj>(), Redemption.ElementID.Arcane, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.EoTSWater1>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.EoTSWater2>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.EoTSSnow>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.EoTSLightningStrike>(), Redemption.ElementID.Thunder, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.IceShardFrigidius>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.IceShardFrigidiusAccelerate>(), Redemption.ElementID.Ice, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.IceShardFrigidiusSpin>(), Redemption.ElementID.Ice, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjFireball>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjFireballSlow>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjFireBlast>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjFireBlastUnreal>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjHomingFireball>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjLavaOrb>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjLavaOrb>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjLavaOrb2>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjLavaOrb2>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjTwinFire>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.CandescenceProjTwinFireShot>(), Redemption.ElementID.Fire, mod);

            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilDonut>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilHeart>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilIchor>(), Redemption.ElementID.Water, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilIchor>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilOrb>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilProjClinger>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilProjFireball>(), Redemption.ElementID.Fire, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilProjFireball>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilProjLeaf>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilRottedFork>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilRottedFork2>(), Redemption.ElementID.Blood, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilVilethornBase>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilVilethornBase>(), Redemption.ElementID.Nature, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilVilethornTip>(), Redemption.ElementID.Shadow, mod);
            Redemption.AddElementToProjectile(ModContent.ProjectileType<Projectiles.BlightdrasilVilethornTip>(), Redemption.ElementID.Nature, mod);

        }

        private void DoRecipeBrowserIntegration()
        {

            if (ModLoader.TryGetMod("RecipeBrowser", out Mod mod) && !Main.dedServ)
            {
                mod.Call(
                [
                    "AddItemCategory",
                    "Shields",
                    "Accessories",
                    Mod.Assets.Request<Texture2D>("Items/Accessories/Shield/StoneShield", AssetRequestMode.AsyncLoad), // 24x24 icon
			        (Predicate<Item>)((Item item) =>
                    {
                            return item.shieldItem().shield;
                     })
                ]);
            }
        }
        private void DoCensusIntegration()
        {
            // We figured out how to add support by looking at it's Call method: https://github.com/JavidPack/Census/blob/1.4/Census.cs
            // Census also has a wiki, where the Call methods are better explained: https://github.com/JavidPack/Census/wiki/Support-using-Mod-Call

            if (!ModLoader.TryGetMod("Census", out Mod censusMod))
            {
                // TryGetMod returns false if the mod is not currently loaded, so if this is the case, we just return early
                return;
            }

            censusMod.Call("TownNPCCondition", ModContent.NPCType<HaijiSenri>(), ModContent.GetInstance<HaijiSenri>().GetLocalization("Census.SpawnCondition").WithFormatArgs());
            // Additional calls can be made here for other Town NPCs in our mod
        }

        private void DoMusicDisplayIntegration()
        {
            if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
            {
                return;
            }

            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/TreeToadTheme2"), "Antibody - Nightmare of Insects", "Awful Garbage Mod");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/SeseTheme"), "Antibody - Carcass and Cadaver", "Awful Garbage Mod");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/EotSTheme"), "Antibody - Peering Through Clouds", "Awful Garbage Mod");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrigidiusTheme"), "Antibody - Cold Blooded", "Awful Garbage Mod");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/TsugumiTheme"), "Antibody - Perpetual Motion Machine Vegetable", "Awful Garbage Mod");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, "Assets/Music/CandescenceTheme"), "Antibody - Warm Blooded", "Awful Garbage Mod");
        }

        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/Support-using-Mod-Call
            // If we navigate the wiki, we can find the "AddBoss" method, which we want in this case

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.3.1
            // Usually mods either provide that information themselves in some way, or it's found on the github through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }

            // The "AddBoss" method requires many parameters, defined separately below:

            // The name used for the title of the page
            string internalName = "TreeToad";

            // Value inferred from boss progression, see the wiki for details
            float weight = -1f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedTreeToad;

            // The NPC type of the boss
            int bossType = ModContent.NPCType<NPCs.Boss.TreeToad>();

            // The item used to summon the boss with (if available)
            int spawnItem = ModContent.ItemType<InsectOnAStick>();

            // "collectibles" like relic, trophy, mask, pet
            List<int> collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.TreeToadRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.TreeToadTrophy>()
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
           

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );


            internalName = "RottingSoulflower";

            // Value inferred from boss progression, see the wiki for details
            weight = 1.001f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedEvilFlowerMiniboss;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.RottingSoulflower>();

            // The item used to summon the boss with (if available)
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                }
            );

            internalName = "DecayingBloodflower";

            // Value inferred from boss progression, see the wiki for details
            weight = 1.001f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedEvilFlowerMiniboss;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.DecayingBloodflower>();

            // The item used to summon the boss with (if available)
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                }
            );



            internalName = "SeseKitsugai";

            // Value inferred from boss progression, see the wiki for details
            weight = 2.001f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedSeseKitsugai;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.Boss.SeseKitsugai>();

            // The item used to summon the boss with (if available)
            spawnItem = ModContent.ItemType<PileOfFakeBones>();

            // "collectibles" like relic, trophy, mask, pet
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiTrophy>()
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );



            internalName = "EyeOfTheStorm";

            // Value inferred from boss progression, see the wiki for details
            weight = 3.3301f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedEyeOfTheStorm;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.Boss.EyeOfTheStorm>();

            // The item used to summon the boss with (if available)
            spawnItem = ModContent.ItemType<FoggyLens>();

            // "collectibles" like relic, trophy, mask, pet
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>()
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            internalName = "StarFighter";

            // Value inferred from boss progression, see the wiki for details
            weight = 3.3302f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedStarFighter;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.StarFighterHead>();

            // The item used to summon the boss with (if available)
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                }
            );

            internalName = "DecayingBloodflower";

            // Value inferred from boss progression, see the wiki for details
            weight = 1.001f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedEvilFlowerMiniboss;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.DecayingBloodflower>();

            // The item used to summon the boss with (if available)
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                }
            );

            internalName = "Frigidius";

            // Value inferred from boss progression, see the wiki for details
            weight = 3.9999f;

            // Used for tracking checklist progress
            downed = () => DownedBossSystem.downedFrigidius;

            // The NPC type of the boss
            bossType = ModContent.NPCType<NPCs.Boss.FrigidiusHead>();

            // The item used to summon the boss with (if available)
            spawnItem = ModContent.ItemType<Items.FrigidPointer>();

            // "collectibles" like relic, trophy, mask, pet
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.FrigidiusRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.FrigidiusTrophy>()
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location


            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            internalName = "CursedChandelier";
            weight = 5.0001f;
            downed = () => DownedBossSystem.downedCursedChandelier;
            bossType = ModContent.NPCType<NPCs.CursedChandelier>();
            spawnItem = 0;
            collectibles = new List<int>()
            {
            };
            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            internalName = "TsugumiUmatachi";
            weight = 5.0002f;
            downed = () => DownedBossSystem.downedTsugumi;
            bossType = ModContent.NPCType<NPCs.Boss.TsugumiUmatachi>();
            spawnItem = ModContent.ItemType<JarOfSpirits>();
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.TsugumiRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.TsugumiTrophy>()
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );


            
            internalName = "Candescence";
            weight = 7.0001f;
            downed = () => DownedBossSystem.downedFireMoth;
            bossType = ModContent.NPCType<NPCs.Boss.Candescence>();
            spawnItem = ModContent.ItemType<LavaLamp>();
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>()
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            ); 
            
            internalName = "Candescence";
            weight = 10.0001f;
            downed = () => DownedBossSystem.downedIyozane;
            bossType = ModContent.NPCType<NPCs.Boss.FujiwaraNoIyozane>();
            spawnItem = ModContent.ItemType<LavaLamp>();
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>()
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            ); 
            

            internalName = "Blightdrasil";
            weight = 12.0001f;
            downed = () => DownedBossSystem.downedEvilTree;
            bossType = ModContent.NPCType<NPCs.Boss.Blightdrasil>();
            spawnItem = ModContent.ItemType<RootOfEvil>();
            collectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>(),
                ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>()
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            /*
            internalName = "AwfulGarabage";
            weight = 19f;
            downed = () => DownedBossSystem.downedAwfulGarbage;
            bossType = ModContent.NPCType<NPCs.Boss.AwfulGarbageFake>();
            spawnItem = ModContent.ItemType<Items.Consumables.Trash>();
            collectibles = new List<int>()
            {
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles,
                    // Other optional arguments as needed are inferred from the wiki
                }
            );*/
            // Other bosses or additional Mod.Call can be made here.
        }
    }
}