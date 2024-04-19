using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Items.Armor;
using Microsoft.Xna.Framework;
using Steamworks;
using static Humanizer.In;
using AwfulGarbageMod.Buffs;
using Terraria.GameInput;
using AwfulGarbageMod.Systems;
using Mono.Cecil;
using Microsoft.CodeAnalysis;
using Terraria.Audio;
using AwfulGarbageMod.DamageClasses;
using StramClasses;
using Microsoft.Xna.Framework.Graphics;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Tiles.OresBars;
using Terraria.Localization;
using Microsoft.VisualBasic.FileIO;
using System;

namespace AwfulGarbageMod.Global
{
    public class EarthenShield : ModPlayer
    {
        public int shieldDurability; //Do not set this value.
        public int shieldMaxDurability; //Set this to a damage value
        public float shieldCooldown; //Do not set this
        public int shieldMaxCooldown; //Set this to the cooldown length
        public bool shieldsActive;
        public int shieldMaxCooldownPrevious;
        public int shieldDamageTaken;

        public override void PreUpdate()
        {
            int maxShieldDurability = (int)(shieldMaxDurability);
            MathHelper.Clamp(maxShieldDurability, 1, 9999);
            if (!Player.GetModPlayer<GlobalPlayer>().EarthenBonus && shieldMaxCooldownPrevious > 0)
            {
                shieldsActive = false;
                shieldMaxCooldownPrevious = 0;
                shieldCooldown = 0;
            }
            if (Player.GetModPlayer<GlobalPlayer>().EarthenBonus && shieldMaxCooldownPrevious == 0)
            {

                Player.AddBuff(ModContent.BuffType<EarthenShieldCooldown>(), 60);

                shieldCooldown = shieldMaxCooldown;
            }
            if (Player.GetModPlayer<GlobalPlayer>().EarthenBonus)
            {
                if (shieldCooldown <= 0 && !shieldsActive)
                {
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), Color.DarkSlateGray, "Shields Active!", dramatic: true);

                    shieldDurability = maxShieldDurability;
                    shieldsActive = true;
                    Player.AddBuff(ModContent.BuffType<EarthenShieldBuff>(), 60);
                }
                if (shieldDurability > maxShieldDurability)
                {
                    shieldDurability = maxShieldDurability;
                }
            }
            shieldCooldown -= 1;
        }
        public override void ResetEffects()
        {
            shieldMaxDurability = 0;
            shieldMaxCooldownPrevious = shieldMaxCooldown;
            shieldMaxCooldown = 0;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.GetModPlayer<GlobalPlayer>().EarthenBonus && shieldDurability > 0)
            {
                modifiers.FinalDamage *= 0.8f;
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {

            if (Player.GetModPlayer<GlobalPlayer>().EarthenBonus && shieldDurability > 0)
            {
                int shieldDmg;
                shieldDmg = (int)(Math.Ceiling(info.Damage / 4f));
                shieldDurability -= shieldDmg;

                CombatText.NewText(Player.getRect(), Color.DarkCyan, shieldDmg);

                if (shieldDurability <= 0)
                {
                    ShieldBreak();
                }
            }
        }
        public void ShieldBreak()
        {
            CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), Color.DarkSlateGray, "Shield Break!", dramatic: true);

            if (Player.HasBuff<EarthenShieldBuff>())
            {
                Player.DelBuff(Player.FindBuffIndex(ModContent.BuffType<EarthenShieldBuff>()));
            }
            Player.AddBuff(ModContent.BuffType<EarthenShieldCooldown>(), 60);

            shieldsActive = false;
            shieldDurability = 0;
            shieldCooldown = shieldMaxCooldown;

        }
    }
}