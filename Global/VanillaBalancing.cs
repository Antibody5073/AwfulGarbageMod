using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Configs;
using System.Collections.Generic;
using AwfulGarbageMod.DamageClasses;
using Terraria.Localization;
using AwfulGarbageMod.Items.Armor;
using System.Linq;
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.Global.GlobalItems
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    public class VanillaBalancing : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            TooltipLine line;
            if (item.type == ItemID.MagicPowerPotion && ModContent.GetInstance<Config>().MagicPowerMana)
            {
                tooltips[2].Text = "Increases maximum mana by 100";
            }
            if (ModContent.GetInstance<Config>().MagicArmorAdjust)
            {
                if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.ApprenticeAltShirt || item.type == ItemID.ApprenticeRobe || item.type == ItemID.CrystalNinjaLeggings || item.type == ItemID.HallowedPlateMail || item.type == ItemID.AncientHallowedPlateMail)
                {
                    tooltip = new TooltipLine(Mod, "ScepterStatTooltip1", "\nRemoves the non-magic damage penalties of scepters");
                    line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                    if (line != null)
                    {
                        line.Text += tooltip.Text;
                    }
                }
            }
            if (ModContent.GetInstance<Config>().ScourgeOfTheCorruptorRework)
            {
                if (item.type == ItemID.ScourgeoftheCorruptor)
                {
                    tooltip = new TooltipLine(Mod, "ScepterStatTooltip1", "\nInflicts Corruptor's Curse for 6 seconds, which deals damage over time and causes enemies to emit a\nhoming mini Eater upon melee strike that deals 15 + 10% of the weapons damage");
                    line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                    if (line != null)
                    {
                        line.Text += tooltip.Text;
                    }
                }
            }
            if (item.type == ItemID.TerraBlade)
            {
                tooltip = new TooltipLine(Mod, "yago", "\n'Made in YagoTM'");
                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Knockback" && x.Mod == "Terraria");
                if (line != null)
                {
                    line.Text += tooltip.Text;
                }
            }
            if (item.type == ItemID.FlinxStaff || item.type == ItemID.ImpStaff || item.type == ItemID.SlimeStaff || item.type == ItemID.VampireFrogStaff || item.type == ItemID.OpticStaff)
            {
                tooltip = new TooltipLine(Mod, "mix", "\nUses static immunity frames; it is suggested you mix minions when using this weapon");
                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (line != null)
                {
                    line.Text += tooltip.Text;
                }
            }
            if (DifficultyModes.Difficulty > 0 && !player.GetModPlayer<GlobalPlayer>().DisabledUnrealBuffNerfs)
            {
                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");

                if (item.type == ItemID.RegenerationPotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Regeneration");
                }
                if (item.type == ItemID.IronskinPotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Ironskin");
                }
                if (item.type == ItemID.EndurancePotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Endurance");
                }
                if (item.type == ItemID.WrathPotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Wrath");

                }
                if (item.type == ItemID.RagePotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Rage");
                }
                if (item.type == ItemID.ArcheryPotion)
                {
                    line.Text += Language.GetOrRegister("Mods.AwfulGarbageMod.UnrealBuffNerfs.Archery");
                }
            }
            if (item.type == ItemID.LeadPickaxe || item.type == ItemID.SilverPickaxe)
            {
                tooltip = new TooltipLine(Mod, "Mine", "\nCan mine Frigidium");
                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "PickPower" && x.Mod == "Terraria");
                if (line != null)
                {
                    line.Text += tooltip.Text;
                }
            }
            if (item.type == ItemID.TungstenPickaxe || item.type == ItemID.GoldPickaxe || item.type == ItemID.PlatinumPickaxe)
            {
                tooltip = new TooltipLine(Mod, "Mine", "\nCan mine Frigidium and Candescite");
                line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "PickPower" && x.Mod == "Terraria");
                if (line != null)
                {
                    line.Text += tooltip.Text;
                }

            }
        }



        public override void UpdateEquip(Item item, Player player)
        {
            base.UpdateEquip(item, player);
            if (item.type == ItemID.BoneGlove)
            {
                player.GetModPlayer<GlobalPlayer>().BoneGloveDamage += 30;
            }

            if (ModContent.GetInstance<Config>().MagicArmorAdjust)
            {
                if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.ApprenticeAltShirt || item.type == ItemID.ApprenticeRobe || item.type == ItemID.CrystalNinjaLeggings || item.type == ItemID.HallowedPlateMail || item.type == ItemID.AncientHallowedPlateMail)
                {
                    player.GetModPlayer<GlobalPlayer>().IgnoreScepterDmgPenalties = true;
                }
            }
        }

        public override void SetDefaults(Item item)
        {
            //Change vanilla knives' damageClass to KnifeDamageClass

            if (item.type == ItemID.ThrowingKnife)
            {
                item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (item.type == ItemID.PoisonedKnife)
            {
                item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (item.type == 3379)
            {
                item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
            if (item.type == ItemID.FrostDaggerfish)
            {
                item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }

            //Random item balance changes
            if (ModContent.GetInstance<Config>().MiscBalancing)
            {
                if (item.type == ItemID.Boomstick)
                {
                    item.useTime += 5;
                    item.useAnimation += 5;
                    item.damage += 1;
                }
                if (item.type == ItemID.DemonScythe)
                {
                    item.damage -= 3;
                }
                if (item.type == ItemID.TheUndertaker)
                {
                    item.damage += 1;
                }
                if (item.type == ItemID.BeeGun)
                {
                    item.damage += 2;
                }
            }

            if (item.type == ItemID.Starfury && ModContent.GetInstance<Config>().StarfuryNerf)
            {
                item.StatsModifiedBy.Add(Mod);
            }

            //Scourge
            if (item.type == ItemID.ScourgeoftheCorruptor && ModContent.GetInstance<Config>().ScourgeOfTheCorruptorRework)
            {
                item.damage = (int)(item.damage * 0.8f);
                item.StatsModifiedBy.Add(Mod);
            }

            //Stardust Balance
            if (ModContent.GetInstance<Config>().StardustBalance)
            {
                if (item.type == ItemID.StardustCellStaff)
                {
                    item.damage = (int)(item.damage * 1.20f);
                }
                if (item.type == ItemID.StardustDragonStaff)
                {
                    item.damage = (int)(item.damage * 0.75f);
                }
            }

            //Moon Lord Weapon Balancing
            if (ModContent.GetInstance<Config>().MoonLordBalance)
            {
                if (item.type == ItemID.Meowmere)
                {
                    item.damage = (int)(item.damage * 1.75f);
                    item.shootSpeed *= 1.45f;
                    item.useTime = (int)(item.useTime * 1.5f);
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.StarWrath)
                {
                    item.damage = (int)(item.damage * 1.15f);
                    item.DamageType = DamageClass.Melee;
                    item.attackSpeedOnlyAffectsWeaponAnimation = false;
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.SDMG)
                {
                    item.damage = (int)(item.damage * 1.18f);
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.LastPrism)
                {
                    item.damage = (int)(item.damage * 0.6f);
                    item.mana = (int)(item.mana * 0.34);
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.Terrarian)
                {
                    item.damage = (int)(item.damage * 0.75f);
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.RainbowCrystalStaff)
                {
                    item.damage = (int)(item.damage * 1.3f);
                    item.StatsModifiedBy.Add(Mod);
                }
            }

            //Ranger Ammo nerfs
            if (ModContent.GetInstance<Config>().RangerAmmoNerf)
            {
                if ((item.ammo == AmmoID.Arrow || item.ammo == AmmoID.Bullet))
                {
                    item.damage = item.damage * 3 / 4; 
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.Minishark && ModContent.GetInstance<Config>().RangerAmmoNerf)
                {
                    item.ArmorPenetration += 3;
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.Megashark && ModContent.GetInstance<Config>().RangerAmmoNerf)
                {
                    item.damage += 1;
                    item.ArmorPenetration += 2;
                    item.StatsModifiedBy.Add(Mod);
                }
                if (item.type == ItemID.ChainGun && ModContent.GetInstance<Config>().RangerAmmoNerf)
                {
                    item.damage += 2;
                    item.ArmorPenetration += 3;
                    item.StatsModifiedBy.Add(Mod);
                }
            }
        }
    }
}
