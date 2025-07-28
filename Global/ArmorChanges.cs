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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 372;
        }

        public override void SetDefaults(Item item) 
        {
            item.defense -= 1;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "8% increased melee damage and 6% increased melee critical strike chance";
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
            player.GetDamage(DamageClass.Melee) -= 0.07f;
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 373;
            
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 1;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.10f;
            player.GetCritChance(DamageClass.Ranged) -= 10;

            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetDamage<KnifeDamageClass>() += 0.06f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.15f;
            player.GetModPlayer<GlobalPlayer>().knifeVelocity += 0.06f;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "15% increased ranged damage and velocity";
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 371;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 3;
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 374;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "8% increased flail, knife, and scepter damage and critical strike chance and 10% reduced mana usage";
            }   
        }


        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 5f;

            player.GetDamage<FlailDamageClass>() += 0.08f;
            player.GetDamage<KnifeDamageClass>() += 0.08f;
            player.GetDamage<ScepterDamageClass>() += 0.08f;

            player.GetCritChance<FlailDamageClass>() += 8;
            player.GetCritChance<KnifeDamageClass>() += 8;
            player.GetCritChance<ScepterDamageClass>() += 8;
            
            player.manaCost *= 0.9f;
        }
    }
    public class CobaltLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == 375;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "5% increased damage and movement speed; provides knockback immunity";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.03f;
            player.moveSpeed -= 0.1f;

            player.moveSpeed += 0.05f;
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.noKnockback = true;
        }
    }

    public class PalladiumMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == 1205;
        }

        public override void SetDefaults(Item item)
        {
            item.defense -= 1;
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
                line2.Text = "10% increased flail spin speed and 6% further increased flail damage";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) -= 0.01f;
            player.GetDamage(DamageClass.Melee) -= 0.01f;
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.1f;
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 1206;
        }
        public override void SetDefaults(Item item)
        {
            item.defense += 1;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.09f;
            player.GetCritChance(DamageClass.Ranged) -= 9;

            player.GetDamage(DamageClass.Ranged) += 0.8f;
            player.GetCritChance(DamageClass.Ranged) += 8;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.12f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "8% increased ranged damage and critical strike chance";
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 1207;
        }

        public override void SetDefaults(Item item)
        {
            item.defense += 3;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.09f;
            player.GetCritChance(DamageClass.Magic) -= 9;
            player.statManaMax2 -= 60;


            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.GetCritChance(DamageClass.Magic) += 8;
            player.statManaMax2 += 40;
            player.manaCost *= 0.93f;
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 2;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "8% increased magic damage and critical strike chance and 7% reduced mana cost";
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == 1208;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "5% increased damage and critical strike chance";
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
            player.GetCritChance(DamageClass.Generic) += 5;
            player.lifeRegen += 3;
        }
    }
    public class PalladiumLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == 1209;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "3% increased damage and critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "6% reduced damage taken";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.03f;
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MoltenHelmet;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += "\n5% increased flail spin speed";
            }
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.05f;
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
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MoltenGreaves;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += "\n5% increased flail spin speed";
            }
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.05f;
        }
    }


    public class MythrilMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().HardmodeMetalRework && item.type == ItemID.MythrilHelmet;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 25;
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
                line2.Text = "+50 max mana";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Melee) -= 0.1f;
            player.GetCritChance(DamageClass.Melee) -= 8f;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.statManaMax2 += 50;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.MythrilHelmet && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
            {
                return "Mythril Melee";
            }
            return null;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Mythril Melee"))
            {
                player.setBonus = "Heal back 30% of damage taken at the cost of mana\nMana cost is equal to the damage taken\nHas no effect if you have insufficient mana";
                player.GetCritChance(DamageClass.Melee) -= 10f;
                player.GetModPlayer<GlobalPlayer>().MythrilDefense = true;
            }
        }
    }
    public class MythrilRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MythrilHat;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 13;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.12f;
            player.GetCritChance(DamageClass.Ranged) -= 7f;

            player.GetDamage(DamageClass.Magic) += 0.13f;
            player.GetCritChance(DamageClass.Magic) += 13;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "13% increased magic damage";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "13% increased magic critical strike chance";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.MythrilHat && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
            {
                return "Mythril Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Mythril Ranged"))
            {
                player.setBonus = "20% increased magic casting speed";
                player.GetAttackSpeed(DamageClass.Magic) += 0.2f;
                player.ammoCost80 = false;
            }
        }
    }
    public class MythrilMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MythrilHood;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 8;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.15f;
            player.statManaMax2 -= 60;

            player.GetCritChance(DamageClass.Magic) += 13;
            player.manaCost *= 0.83f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "13% increased magic critical strike chance";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "17% reduced mana usage";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.MythrilHood && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
            {
                return "Mythril Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Mythril Magic"))
            {
                player.manaCost += 0.17f;

                player.setBonus = "All magic weapons cost 0 mana\nWeapons lose damage equal to their mana cost\nIncreases max life by 50% of your max mana";
                player.GetModPlayer<GlobalPlayer>().MythrilOffense = true;
            }
        }

    }
    public class MythrilChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MythrilChainmail;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "13% increased magic damage and reduced mana usage";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.07f;
            player.GetDamage(DamageClass.Magic) += 0.13f;
            player.manaCost *= 0.87f;
        }
    }
    public class MythrilLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.MythrilGreaves;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "12% increased magic critical strike chance and movement speed";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 10f;
            player.GetCritChance(DamageClass.Magic) += 12f;
            player.moveSpeed += 0.12f;
        }
    }

    public class OrichalcumMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().HardmodeMetalRework && item.type == ItemID.OrichalcumMask;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 12;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "Increases gun damage by 12%";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "5% increased ranged critical strike chance";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Melee) -= 0.11f;
            player.GetAttackSpeed(DamageClass.Melee) -= 0.11f;
            player.moveSpeed -= 0.07f;
            player.bulletDamage += 0.12f;
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.OrichalcumMask && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
            {
                return "Orichalcum Melee";
            }
            return null;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Orichalcum Melee"))
            {
                player.setBonus = "Musket balls are converted to Orichalcum Bullets\nOrichalcum Bullets burst forward into flower petals on impact, dealing 25% damage";
                player.GetModPlayer<GlobalPlayer>().OrichalcumGun = true;
            }
        }
    }
    public class OrichalcumRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.OrichalcumHelmet;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 11;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Ranged) -= 15f;
            player.moveSpeed -= 0.08f;

            player.GetDamage<KnifeDamageClass>() += 0.12f;
            player.moveSpeed += 0.15f;
            player.jumpSpeedBoost += AGUtils.OnePerentJumpSpeed * 15;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "12% increased knife damage";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "15% increased movement and jump speed";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.OrichalcumHelmet && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
            {
                return "Orichalcum Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Orichalcum Ranged"))
            {
                player.setBonus = "Empowered knives summon a flower petal strike on hit enemies, dealing 75% damage\n5% faster knife empowerment cooldown";
                player.GetModPlayer<GlobalPlayer>().OrichalcumKnife = true;
                player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier *= 0.95f;
            }
        }
    }
    public class OrichalcumMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.OrichalcumHeadgear;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 14;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Magic) -= 18;
            player.statManaMax2 -= 80;

            player.arrowDamage += 0.12f;
            player.GetDamage(DamageClass.Ranged) += 0.05f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "Increases bow damage by 12%";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "5% increased ranged damage";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.OrichalcumHeadgear && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
            {
                return "Orichalcum Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Orichalcum Magic"))
            {
                player.setBonus = "Wooden Arrows are converted to Orichalcum Arrows\nOrichalcum Arrows pierce and summon a flower petal strike on hit enemies, dealing 33% damage";
                player.GetModPlayer<GlobalPlayer>().OrichalcumBow = true;
            }
        }

    }
    public class OrichalcumChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.OrichalcumBreastplate;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "11% increased ranged damage and critical strike chance";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 6f;
            player.GetDamage(DamageClass.Ranged) += 0.11f;
            player.GetCritChance(DamageClass.Ranged) += 11; 
        }
    }
    public class OrichalcumLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.OrichalcumLeggings;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "35% increased movement speed, ranged velocity, and chance not to consume ammo";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.08f;
            player.moveSpeed += 0.24f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.35f;
        }
    }

    public class AdamantiteMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().HardmodeMetalRework && item.type == ItemID.AdamantiteHelmet;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 12;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "+2 max minions";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Hide();
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Melee) -= 0.14f;
            player.GetCritChance(DamageClass.Melee) -= 7;

            player.maxMinions += 2;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.AdamantiteHelmet && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
            {
                return "Adamantite Melee";
            }
            return null;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Adamantite Melee"))
            {
                player.setBonus = "10% increased summon damage\nThe first minion summoned is powered up with adamantite energy\nPowered up minions deal double damage and benefit twice as much from whip tag effects";
                player.GetModPlayer<GlobalPlayer>().AdamantiteMinion = true;
            }
        }
    }
    public class AdamantiteRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.AdamantiteMask;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 8;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.14f;
            player.GetCritChance(DamageClass.Ranged) -= 10f;

            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 15f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "Increases all damage by 15%";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "Critical strike chance increased by 15%";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.AdamantiteMask && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
            {
                return "Adamantite Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Adamantite Ranged"))
            {
                player.setBonus = "+15% melee speed\n25% chance not to consume ammo\n+80 max mana\n+1 max minions";
                player.ammoCost75 = true;
                player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
                player.statManaMax2 += 80;
                player.maxMinions++;
            }
        }
    }
    public class AdamantiteMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.AdamantiteHeadgear;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 10;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.12f;
            player.GetCritChance(DamageClass.Magic) -= 12;
            player.statManaMax2 -= 80;

            player.maxTurrets += 2;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "+2 max sentries";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Hide();
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.AdamantiteHeadgear && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
            {
                return "Adamantite Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Adamantite Magic"))
            {
                player.setBonus = "10% increased summon damage\nPress the Armor Ability key to teleport all your sentries somewhere nearby, with a cooldown of 20 seconds";
                player.GetModPlayer<GlobalPlayer>().AdamantiteSentry = true;
            }
        }

    }
    public class AdamantiteChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.AdamantiteBreastplate;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "+1 max minions and sentries";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.08f;

            player.maxMinions++;
            player.maxTurrets++;
        }
    }
    public class AdamantiteLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.AdamantiteLeggings;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "20% increased whip and movement speed";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "15% increased whip range and summon damage";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 7f;
            player.moveSpeed -= 0.05f;

            player.moveSpeed += 0.2f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.2f;
            player.whipRangeMultiplier += 0.15f;
        }
    }

    public class TitaniumMeleeHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().HardmodeMetalRework && item.type == ItemID.TitaniumMask;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 25;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "Increases length of invincibility";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "12% reduced damage taken";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.endurance = 1f - (0.88f * (1f - player.endurance));

            player.GetDamage(DamageClass.Melee) -= 0.9f;
            player.GetCritChance(DamageClass.Melee) -= 9f;
            player.GetAttackSpeed(DamageClass.Melee) -= 0.9f;

            player.longInvince = true;

        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.TitaniumMask && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings)
            {
                return "Titanium Melee";
            }
            return null;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Titanium Melee"))
            {
                player.setBonus = "Damaging enemies creates a defensive barrier of titanium shards\n+4 defense per titanium shard";
                player.GetModPlayer<GlobalPlayer>().TitaniumTank = true;
            }
        }
    }
    public class TitaniumRangedHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.TitaniumHelmet;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 7;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Ranged) -= 0.16f;
            player.GetCritChance(DamageClass.Ranged) -= 7;

            player.GetAttackSpeed(DamageClass.Melee) += 0.27f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "27% increased melee speed";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Hide();
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.TitaniumHelmet && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings)
            {
                return "Titanium Ranged";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Titanium Ranged"))
            {
                player.setBonus = "75% reduced defense\nFor every point of defense lost, gain 2% melee damage";
                player.GetModPlayer<GlobalPlayer>().TitaniumOffense = true;
            }
        }
    }
    public class TitaniumMagicHelm : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.TitaniumHeadgear;
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 14;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Magic) -= 0.16f;
            player.GetCritChance(DamageClass.Magic) -= 7;
            player.statManaMax2 -= 100;

            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.1f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "15% increased melee damage";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "10% increased sword size";
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.TitaniumHeadgear && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings)
            {
                return "Titanium Magic";
            }
            return null;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set.Equals("Titanium Magic"))
            {
                player.setBonus = "Swords fling titanium shards, dealing 25% damage\nSpears launch a medium range shadow trident, dealing 75% damage\nIncreases yoyo speed and range";
                player.GetModPlayer<GlobalPlayer>().TitaniumVariety = true;
                if (ItemID.Sets.Yoyo[player.HeldItem.type])
                {
                    player.GetAttackSpeed(DamageClass.Melee) += 0.30f;
                }
            }
        }

    }
    public class TitaniumChest : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.TitaniumBreastplate;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 18;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "11% increased melee critical strike chance and melee speed";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) -= 3f;
            player.GetDamage(DamageClass.Generic) -= 0.04f;


            player.GetAttackSpeed(DamageClass.Melee) += 0.11f;
            player.GetCritChance(DamageClass.Melee) += 11;
        }
    }
    public class TitaniumLegs : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return GetInstance<Config>().MagicArmorAdjust && item.type == ItemID.TitaniumLeggings;
        }
        public override void SetDefaults(Item item)
        {
            item.defense = 14;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = "17% increased melee damage and movement speed";
            }
            TooltipLine line2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
            if (line2 != null)
            {
                line2.Text = "5% increased melee critical strike chance";
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) -= 0.03f;
            player.GetCritChance(DamageClass.Generic) -= 3f;
            player.moveSpeed -= 0.06f;

            player.moveSpeed += 0.17f;
            player.GetDamage(DamageClass.Melee) += 0.17f;
            player.GetCritChance(DamageClass.Melee) += 5;
            
        }
    }
}


