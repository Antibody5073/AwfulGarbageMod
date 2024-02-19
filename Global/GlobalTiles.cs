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
using AwfulGarbageMod.Tiles;

namespace AwfulGarbageMod.Global
{
    public class GT : GlobalTile
    {

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (type == TileID.PalmTree && Vector2.Distance(new Vector2(i, j), new Vector2(Structures.HarujionPosX, Structures.HarujionPosY)) < 50)
            {
                return false;
            }
            if (type == ModContent.TileType<LifelessBlock>())
            {
                return false;
            }
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }
    }
}