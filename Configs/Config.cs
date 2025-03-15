using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AwfulGarbageMod.Configs
{
    public class Config : ModConfig
    {
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // The "$" character before a name means it should interpret the name as a translation key and use the loaded translation with the same key.
        // The things in brackets are known as "Attributes".
        [Header("VanillaTweaksAndBalancing")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.

        [DefaultValue(true)]
        [ReloadRequired]
        public bool MagicArmorAdjust;

        [Tooltip("Ranged ammo damage is reduced by half. Balances a few vanilla weapons that are negatively affected by this a lot, like the Minishark.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(true)] // This sets the configs default value.
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool RangerAmmoNerf; // To see the implementation of this option, see ExampleWings.cs

        [Label("Chlorophyte Bullet Nerf")]
        [Tooltip("")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool ChlorophyteBulletNerf;

        [Label("Starfury Nerf")]
        [Tooltip("Starfury's projectile deals 20% less damage.")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool StarfuryNerf;

        [Label("Demon Scythe Nerf")]
        [Tooltip("Starfury's projectile deals 20% less damage.")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DemonScytheNerf;

        [Label("Stardust Balance")]
        [Tooltip("Stardust Dragon Staff deals 25% less damage and Stardust Cell Staff deals 20% more damage.")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool StardustBalance;

        [Label("Moon Lord Weapon Balancing")]
        [Tooltip("")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool MoonLordBalance;

        [Label("Magic Power Potion Change")]
        [Tooltip("Magic Power Potion provides 100 max mana instead of 20% magic damage.")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool MagicPowerMana;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ScourgeOfTheCorruptorRework;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool BetsyNerf;

        [Label("Misc Balancing")]
        [Tooltip("Magic Power Potion provides 100 max mana instead of 20% magic damage.")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool MiscBalancing;

        [Label("Remix World / GetFixedBoi recipes")]
        [Tooltip("Adds recipes for difficult to obtain items in Remix or GetFixedBoi worlds that are needed for many recipes in this mod")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool RemixRecipes;

        [Label("Cheaty Vanilla QOL Recipes")]
        [Tooltip("Adds recipes for some difficult to obtain vanilla items for those who are lazy")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool AreYouLazy;

        [Header("ModAdjustments")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.


        [DefaultValue(100)]
        [Range(20, 1000)]
        public int BossHealthMultiplier;


        [DefaultValue(100)]
        [Range(20, 1000)]
        public int EnemyDamageMultiplier;


        [DefaultValue(100)]
        [Range(20, 1000)]
        public int EnemyHealthMultiplier;
    }
}