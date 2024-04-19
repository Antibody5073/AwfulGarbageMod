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

namespace AwfulGarbageMod.Global.GlobalItems
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    public class VanillaBalancing : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.MagicPowerPotion && ModContent.GetInstance<Config>().MagicPowerMana)
            {
                tooltips[2].Text = "Increases maximum mana by 100";
            }
            if (ModContent.GetInstance<Config>().MagicArmorAdjust)
            {
                if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.ApprenticeAltShirt || item.type == ItemID.ApprenticeRobe || item.type == ItemID.CrystalNinjaLeggings || item.type == ItemID.HallowedPlateMail || item.type == ItemID.AncientHallowedPlateMail)
                {
                    TooltipLine tooltip = new TooltipLine(Mod, "ScepterStatTooltip1", "Removes the non-magic damage penalties of scepters");
                    tooltips.Add(tooltip);
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
