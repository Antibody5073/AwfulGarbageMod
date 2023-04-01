using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace AwfulGarbageMod
{
	public class GlobalPlayer : ModPlayer
	{
		public bool spiderPendant = false;

        public override void ResetEffects()
        {
            spiderPendant = false;
        }
    }
}