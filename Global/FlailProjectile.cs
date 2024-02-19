using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Printing;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using Mono.Cecil;

namespace AwfulGarbageMod.Global
{
    public class FlailProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float spinOffset = 0;
        public float shadeVortexDirection;

        public bool isAClone = false;

        public float spinSpdMultiplier = 1;
        public float rangeMultiplier = 1;
        public float distanceIncreaseMultiplier = 1;
    }
}
