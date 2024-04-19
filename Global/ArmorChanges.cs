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

namespace AwfulGarbageMod.Global;



internal class ArmorChanges
{
    public class CobaltMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 372;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 2;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "12% increased melee damage and 6% increased melee critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "15% increased flail spin speed";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.moveSpeed -= 0.1f;
            player.GetCritChance(DamageClass.Melee) += 6f;
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.15f;
            player.GetDamage(DamageClass.Melee) -= 0.03f;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 372 && body.type == 374 && legs.type == 375)
            {
                return "Cobalt Melee";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Cobalt Melee"))
            {
                player.setBonus = "15% increased melee speed\nAt max range, striking enemies with flail heads grant a stacking boost to defense\nThis boost has a max of 24 defense and wears off after 10 seconds of not striking enemies with a flail";
                player.GetModPlayer<GlobalPlayer>().CobaltMelee = true;
            }
        }
    }
    public class CobaltRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 373;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 7;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.10f;
            player.GetCritChance(DamageClass.Ranged) -= 10;

            player.GetDamage(DamageClass.Ranged) += 0.18f;
            player.GetDamage<KnifeDamageClass>() += 0.06f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.18f;
            player.GetModPlayer<GlobalPlayer>().knifeVelocity += 0.06f;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "18% increased ranged damage and velocity";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "6% further increased knife damage and velocity";
            }
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 373 && body.type == 374 && legs.type == 375)
            {
                return "Cobalt Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Cobalt Ranged"))
            {
                player.setBonus = "Grants an extra accessory slot dedicated for knife empowerments\nOnly accessories which provide an empowerment and nothing else can go in this slot\nEmpowered knives deal 10% more damage\n20% chance not to consume ammo";
                player.GetModPlayer<GlobalPlayer>().EmpowermentSlot = true;
                player.GetModPlayer<GlobalPlayer>().CobaltRanged = true;

            }
        }
    }
    public class CobaltMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 371;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 8;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.10f;
            player.GetCritChance(DamageClass.Magic) -= 9;
            player.statManaMax2 -= 40;


            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.GetDamage<ScepterDamageClass>() += 0.15f;
            player.statManaMax2 += 40;

            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 3;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "15% increased magic damage";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "15% further increased scepter damage";
            }
            TooltipLine line3 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip2" && x.Mod == "Terraria");
            if (line3 != null)
            {
                line3.Text = "Increases maximum mana by 40 and maximum scepter projectiles by 3";
            }
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 371 && body.type == 374 && legs.type == 375)
            {
                return "Cobalt Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Cobalt Magic"))
            {
                player.setBonus = "Reduces mana usage by 10%\nPress ArmorSetAbility key to deal 10% of your max health as damage to yourself, triggering on-hit effects and releasing scepter projectiles\nThis effect has a cooldown of 12 seconds, during this cooldown magic damage is boosted by 15%";
                player.manaCost += 0.14f;
                player.manaCost *= 0.9f;
                player.GetModPlayer<GlobalPlayer>().CobaltMagic = true;
            }
        }
    }
    public class CobaltChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 374;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "9% increased flail, knife, and scepter damage and critical strike chance and 10% reduced mana usage";
            }   
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 2;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 5f;

            player.GetDamage<FlailDamageClass>() += 0.09f;
            player.GetDamage<KnifeDamageClass>() += 0.09f;
            player.GetDamage<ScepterDamageClass>() += 0.09f;

            player.GetCritChance<FlailDamageClass>() += 9;
            player.GetCritChance<KnifeDamageClass>() += 9;
            player.GetCritChance<ScepterDamageClass>() += 9;
            
            player.manaCost *= 0.9f;
        }
    }
    public class CobaltLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 375;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "12% increased damage and movement speed; provides knockback immunity";
            }
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 3;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.03f;
            player.moveSpeed -= 0.1f;

            player.moveSpeed += 0.12f;
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.noKnockback = true;
        }
    }

    public class PalladiumMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 1205;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 1;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "9% increased melee damage and speed";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "20% increased flail spin speed and 6% further increased flail damage";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) -= 0.01f;
            player.GetDamage(DamageClass.Melee) -= 0.01f;
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.2f;
            player.GetDamage<FlailDamageClass>() += 0.06f;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 1205 && body.type == 1208 && legs.type == 1209)
            {
                return "Palladium Melee";
            }
            return null;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Palladium Melee"))
            {
                player.setBonus = "Increases life regeneration based on the enemy closest to the player\nLife regeneration scales based on how close the enemy is and maxes out at 6 hp/sec\nFlail heads deal 15% more multiplicative damage to the closest enemy\n6% increased flail critical strike chance and 6% increased melee speed";
                player.GetModPlayer<GlobalPlayer>().PalladiumMelee = true;
                player.GetCritChance<FlailDamageClass>() += 6f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
                player.buffImmune[BuffID.RapidHealing] = true;

            }
        }
    }
    public class PalladiumRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 1206;
        }
        public override void SetDefaults(Item item)
        {
            item.defense += 5;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.09f;
            player.GetCritChance(DamageClass.Ranged) -= 9;

            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 10;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.12f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "10% increased ranged damage and critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "12% increased ranged velocity";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 1206 && body.type == 1208 && legs.type == 1209)
            {
                return "Palladium Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Palladium Ranged"))
            {
                player.setBonus = "Grants an extra accessory slot dedicated for knife empowerments\nOnly accessories which provide an empowerment and nothing else can go in this slot\nOn strike, empowered knives temporarily boost life regeneration\n5% increased knife critical strike chance and ranged damage";
                player.GetModPlayer<GlobalPlayer>().EmpowermentSlot = true;
                player.GetModPlayer<GlobalPlayer>().CobaltRanged = true;

                player.GetCritChance<KnifeDamageClass>() += 5f;
                player.GetDamage(DamageClass.Ranged) += 0.05f;
            }
        }
    }
    public class PalladiumMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 1207;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 5;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.09f;
            player.GetCritChance(DamageClass.Magic) -= 9;
            player.statManaMax2 -= 60;


            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetCritChance(DamageClass.Magic) += 10;
            player.statManaMax2 += 40;
            player.manaCost *= 0.93f;
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 2;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "10% increased magic damage and critical strike chance and 7% reduced mana cost";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "Increases maximum mana by 40 and scepter projectiles by 2";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == 1207 && body.type == 1208 && legs.type == 1209)
            {
                return "Palladium Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Palladium Magic"))
            {
                player.setBonus = "Press ArmorSetAbility key to deal 10% of your max health as damage to yourself, triggering on-hit effects and releasing scepter projectiles\nThis effect has a cooldown of 8 seconds, during this cooldown, life regeneration is increased by 3 hp/sec\nEvery 30 seconds, self-damage will be reduced to 1";
                player.buffImmune[BuffID.RapidHealing] = true;

                if (player.HasBuff<ArmorAbilityCooldown>())
                {
                    player.lifeRegen += 6;
                }
                player.GetModPlayer<GlobalPlayer>().PalladiumMagic = true;
            }
        }

    }
    public class PalladiumChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 1208;
        }

        public override void SetDefaults(Item entity)
        {
            entity.lifeRegen += 2;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "8% increased damage and critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "Increases life regeneration";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 6;
        }
    }
    public class PalladiumLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == 1209;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "4% increased damage and critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "6% reduced damage taken";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.02f;
            player.GetCritChance(DamageClass.Generic) += 3f;
            player.endurance = 1f - (0.94f * (1f - player.endurance));

        }
    }

    public class EmpowermentSlot : ModAccessorySlot
    {
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            if (checkItem.GetGlobalItem<ItemTypes>().Empowerment) // if is Wing, then can go in slot
                return true;

            return false; // Otherwise nothing in slot
        }

        // Designates our slot to be a priority for putting wings in to. NOTE: use ItemLoader.CanEquipAccessory if aiming for restricting other slots from having wings!
        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            if (item.GetGlobalItem<ItemTypes>().Empowerment) // If is Wing, then we want to prioritize it to go in to our slot.
                return true;

            return false;
        }

        public override bool IsEnabled()
        {
            if (Player.GetModPlayer<GlobalPlayer>().EmpowermentSlotPrevious) // if player is wearing a helmet, because flight safety
                return true; // Then can use Slot

            return false; // Can't use slot
        }

        // Overrides the default behavior where a disabled accessory slot will allow retrieve items if it contains items
        public override bool IsVisibleWhenNotEnabled()
        {
            return false; // We set to false to just not display if not Enabled. NOTE: this does not affect behavior when mod is unloaded!
        }

        // Icon textures. Nominal image size is 32x32. Will be centered on the slot.
        public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.CobaltMask;

        // Can be used to modify stuff while the Mouse is hovering over the slot.
        public override void OnMouseHover(AccessorySlotType context)
        {
            // We will modify the hover text while an item is not in the slot, so that it says "Wings".
            switch (context)
            {
                case AccessorySlotType.FunctionalSlot:
                case AccessorySlotType.VanitySlot:
                    Main.hoverItemName = "Empowerments";
                    break;
                case AccessorySlotType.DyeSlot:
                    Main.hoverItemName = "Dye";
                    break;
            }
        }
    }

    public class MoltenHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ItemID.MoltenHelmet;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += "\n10% increased flail spin speed";
            }
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.1f;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.MoltenHelmet && body.type == ItemID.MoltenBreastplate && legs.type == ItemID.MoltenGreaves)
            {
                return "Molten Armor";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Molten Armor"))
            {
                player.setBonus += "\n10% increased flail range";
                player.GetModPlayer<GlobalPlayer>().flailRange += 0.1f;
            }
        }
    }
    public class MoltenLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ItemID.MoltenGreaves;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += "\n6% increased flail spin speed";
            }
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.06f;
        }
    }

}


