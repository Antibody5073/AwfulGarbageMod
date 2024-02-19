using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AwfulGarbageMod.Configs
{
    public class ConfigClient : ModConfig
    {
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // The "$" character before a name means it should interpret the name as a translation key and use the loaded translation with the same key.
        // The things in brackets are known as "Attributes".
        [Header("Worldgen")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.

        [DefaultValue(true)] // This sets the configs default value.
        public bool NotifyMissingStuff;

        [DefaultValue(true)] // This sets the configs default value.
        public bool ShouldGenerateIcePalace;

        [DefaultValue(true)] // This sets the configs default value.
        public bool ShouldGenerateFrigidiumOre;
    }
}