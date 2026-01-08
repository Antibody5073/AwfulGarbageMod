using AwfulGarbageMod.Configs;
using Terraria;
using Terraria.ModLoader;

namespace AwfulGarbageMod
{
	public class AwfulGarbageMod : Mod
	{
        static bool stramLoaded = ModLoader.HasMod("StramClasses");
        static bool infernusLoaded = ModLoader.HasMod("Infernus");

        public override void Load()
        {
            Terraria.On_WorldGen.SmashAltar += On_WorldGen_SmashAltar;
        }

        private void On_WorldGen_SmashAltar(On_WorldGen.orig_SmashAltar orig, int i, int j)
        {
            if (ModContent.GetInstance<Config>().HardmodeMetalRework)
            {
                bool drunkWorld = Main.drunkWorld;
                Main.drunkWorld = true;
                orig.Invoke(i, j);
                Main.drunkWorld = drunkWorld;
            }
        }
    }
}