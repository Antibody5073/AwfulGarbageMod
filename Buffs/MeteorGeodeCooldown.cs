using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Buffs
{
    public class MeteorGeodeCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meteor Geode Cooldown"); // Buff display name
            // Description.SetDefault("Can't summon meteors from the sky"); // Buff description
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}