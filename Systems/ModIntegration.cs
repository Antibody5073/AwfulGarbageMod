using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.NPCs.Town;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            Main.instance.LoadNPC(56); //Snatcher
            Main.instance.LoadNPC(61); // Vulture
            Main.instance.LoadNPC(169); // Ice Elemental
            Main.instance.LoadNPC(1); // Slime

            Main.instance.LoadProjectile(864); //Blade Staff
            Main.instance.LoadProjectile(206); //Leaf
            Main.instance.LoadProjectile(578); //Vortex Portal
            Main.instance.LoadProjectile(ProjectileID.TerraBeam); //Terra Blade
            Main.instance.LoadProjectile(ProjectileID.BoneGloveProj); //Crossbone
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
            

            internalName = "TsugumiUmatachi";
            weight = 5.0001f;
            downed = () => DownedBossSystem.downedTsugumi;
            bossType = ModContent.NPCType<NPCs.Boss.TsugumiUmatachi>();
            spawnItem = ModContent.ItemType<JarOfSpirits>();
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