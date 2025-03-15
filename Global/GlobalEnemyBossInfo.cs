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
    public class GlobalEnemyBossInfo : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        
        public float OrbitDistance;
        public float OrbitDirection;
        public bool killOrbitals;
        public bool orbitalsDealDamage;

        public bool finishedAtk;

        public float attack;
    }
}
