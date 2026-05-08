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
    public class ShieldItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool shield = false; //Whether the item is a shield or not
        public float absorption = 0f;
        public int durability = 0;
        public int cooldown = 0;
        public bool overrideShieldDamage = false;
        public bool overridePlayerDamage = false;
        public string shieldType = "";
        public float DurabilityMult;
        public float CooldownMult;
        public int shieldDT = 0;
        public int shieldDT2 = 0;
        public int shieldDT3 = 0;
        public Color shieldBreakColor = Color.White;
        public float playerDT = 0;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!item.shieldItem().shield)
            {
                return;
            }
            player.GetModPlayer<EarthenShield>().shieldEquipped = true;
            player.GetModPlayer<EarthenShield>().shieldMaxDurability = (int)(item.shieldItem().durability * player.GetModPlayer<EarthenShield>().shieldDurabilityMult);
            player.GetModPlayer<EarthenShield>().shieldMaxCooldown = item.shieldItem().cooldown;
            player.GetModPlayer<EarthenShield>().overrideShieldBreak = item.shieldItem().overrideShieldDamage;
            player.GetModPlayer<EarthenShield>().ShieldDR = 1 - item.shieldItem().absorption;
            player.GetModPlayer<EarthenShield>().shieldType = item;
            player.GetModPlayer<EarthenShield>().shieldBreakColor = shieldBreakColor;
            player.GetModPlayer<EarthenShield>().shieldDurabilityMult += item.shieldItem().DurabilityMult;
            player.GetModPlayer<EarthenShield>().shieldCooldownMult += item.shieldItem().CooldownMult;
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (!item.shieldItem().shield)
            {
                return;
            }
            damage *= player.GetModPlayer<EarthenShield>().ShieldEffectPower;
        }
    }
}
