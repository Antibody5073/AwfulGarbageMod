using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AwfulGarbageMod.Global
{
    public class ScepterItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool scepter = false; //Whether the item is a scepter or not
        public int MaxScepterProjectiles = 0;
    }
}
