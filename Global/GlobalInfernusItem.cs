using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Systems;
using Infernus.Items.Weapon.HardMode.Accessories;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Humanizer.In;

namespace AwfulGarbageMod.Global
{
    [ExtendsFromMod("Infernus")]
    public class UnchainedScarabChange : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<Elemental_Lash>();
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "AGEffect", "Awful Garbage Mod:\n+1 max sentries\n+6 defense");
            tooltips.Add(tooltip);

        }

        public override void UpdateEquip(Item item, Player player)
        {
            base.UpdateEquip(item, player);
            player.maxTurrets += 1;
            player.statDefense += 6;
        }
    }
    [ExtendsFromMod("Infernus")]
    public class BloodQuiverChange : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<Blood_Quiver>();
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "AGEffect", "Awful Garbage Mod:\n+8% damage and melee speed\n+5 armor penetration\n- Ranged attacks can bleed enemies");
            tooltips.Add(tooltip);

        }

        public override void UpdateEquip(Item item, Player player)
        {
            base.UpdateEquip(item, player);
            player.GetDamage(DamageClass.Generic) += 0.08f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            player.GetModPlayer<GlobalPlayer>().RangedInflictBleed = true;
        }
    }
}