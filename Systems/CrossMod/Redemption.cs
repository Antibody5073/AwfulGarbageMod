using Terraria;
using Terraria.ModLoader;

public class Redemption
{
    public static class ElementID
    {
        public const short Arcane = 1;
        public const short Fire = 2;
        public const short Water = 3;
        public const short Ice = 4;
        public const short Earth = 5;
        public const short Wind = 6;
        public const short Thunder = 7;
        public const short Holy = 8;
        public const short Shadow = 9;
        public const short Nature = 10;
        public const short Poison = 11;
        public const short Blood = 12;
        public const short Psychic = 13;
        public const short Celestial = 14;
        public const short Explosive = 15;
    }

    public static class NPCType
    {
        public const string Skeleton = "Skeleton";
        public const string SkeletonHumanoid = "SkeletonHumanoid";
        public const string Humanoid = "Humanoid";
        public const string Undead = "Undead";
        public const string Spirit = "Spirit";
        public const string Plantlike = "Plantlike";
        public const string Demon = "Demon";
        public const string Cold = "Cold";
        public const string Hot = "Hot";
        public const string Wet = "Wet";
        public const string Dragonlike = "Dragonlike";
        public const string Inorganic = "Inorganic";
        public const string Robotic = "Robotic";
        public const string Armed = "Armed";
        public const string Hallowed = "Hallowed";
        public const string Dark = "Dark";
        public const string Blood = "Blood";
        public const string Slime = "Slime";
    }

    // ------------------------------------------------------------------------------------------------------
    // These go in SetStaticDefaults()
    public static void AddItemToBluntSwing(int itemType, Mod Mod)
    {
        Mod.Call("addItemToBluntSwing", itemType);
    }

    public static void AddElement(Entity entity, int elementID, Mod Mod, bool projsInheritElements = false)
    {
        if (entity is Item item)
            Mod.Call("addElementItem", elementID, item.type, projsInheritElements);
        else if (entity is NPC npc)
            Mod.Call("addElementNPC", elementID, npc.type);
        else if (entity is Projectile proj)
            Mod.Call("addElementProj", elementID, proj.type, projsInheritElements);
    }

    public static void AddElementToItem(int type, int elementID, Mod Mod, bool projsInheritElements = false)
    {
        Mod.Call("addElementItem", elementID, type, projsInheritElements);
    }

    public static void AddElementToNPC(int type, int elementID, Mod Mod)
    {
        Mod.Call("addElementNPC", elementID, type);
    }

    public static void AddElementToProjectile(int type, int elementID, Mod Mod, bool projsInheritElements = false)
    {
        Mod.Call("addElementProj", elementID, type, projsInheritElements);
    }

    public static void AddNPCToElementList(int npcType, string typeString, Mod Mod)
    {
        Mod.Call("addNPCToElementTypeList", typeString, npcType);
    }


    public static void IncreaseElementalResistance(Player player, int elementID, float increase, Mod Mod)
    {
        Mod.Call("increaseElementalResistance", player, elementID, increase);
    }

    public static void IncreaseElementalDamage(Player player, int elementID, float increase, Mod Mod)
    {
        Mod.Call("increaseElementalDamage", player, elementID, increase);
    }



    // ------------------------------------------------------------------------------------------------------
    // These are dynamic, so they can go in SetDefaults or wherever you want to update them
    // Keep in mind they don't get reset, so not required to put in an Update method that happens every frame
    public static void OverrideElement(Entity entity, int elementID, Mod Mod, int overrideID = 1)
    {
        if (entity is Item item)
            Mod.Call("elementOverrideItem", item, elementID, overrideID);
        else if (entity is NPC npc)
            Mod.Call("elementOverrideNPC", npc, elementID, overrideID);
        else if (entity is Projectile proj)
            Mod.Call("elementOverrideProj", proj, elementID, overrideID);
    }

    public static void OverrideElementMultiplier(NPC npc, int elementID, Mod Mod, float value = 1, bool dontSetMultipliers = false)
    {
        Mod.Call("elementMultiplier", npc, elementID, value, dontSetMultipliers);
    }

    public static void NoBossMultiplierCap(NPC npc, Mod Mod, bool uncap = true)
    {
        Mod.Call("uncapBossElementMultiplier", npc, uncap);
    }

    public static void HideElementIcon(Item item, int elementID, Mod Mod, bool hidden = true)
    {
        Mod.Call("hideElementIcon", item, elementID, hidden);
    }

    public static bool Decapitation(NPC target, ref int damageDone, ref bool crit, Mod Mod, int chance = 200)
    {
        return (bool)Mod.Call("decapitation", target, damageDone, crit, chance);
    }

    public static bool SetSlashBonus(Item item, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setSlashBonus", item, setBonus);
    }

    public static bool SetAxeBonus(Item item, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setAxeBonus", item, setBonus);
    }

    public static bool SetAxeBonus(Projectile proj, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setAxeProj", proj, setBonus);
    }

    public static bool SetHammerBonus(Item item, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setHammerBonus", item, setBonus);
    }

    public static bool SetHammerBonus(Projectile proj, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setHammerProj", proj, setBonus);
    }

    // Items already have "ItemID.Sets.Spears[Item.type]" to set them as spears
    public static bool SetSpearBonus(Projectile proj, Mod Mod, bool setBonus = true)
    {
        return (bool)Mod.Call("setSpearProj", proj, setBonus);
    }

    // ------------------------------------------------------------------------------------------------------
    public static bool HasElement(Entity entity, int elementID, Mod Mod)
    {
        if (entity is Item item)
            return (bool)Mod.Call("hasElementItem", item, elementID);
        else if (entity is NPC npc)
            return (bool)Mod.Call("elementOverrideNPC", npc, elementID);
        else if (entity is Projectile proj)
            return (bool)Mod.Call("elementOverrideProj", proj, elementID);
        return false;
    }
    public static int GetFirstElement(Entity entity, Mod Mod, bool ignoreExplosive = false)
    {
        if (entity is Item item)
            return (int)Mod.Call("getFirstElementItem", item, ignoreExplosive);
        else if (entity is NPC npc)
            return (int)Mod.Call("getFirstElementNPC", npc, ignoreExplosive);
        else if (entity is Projectile proj)
            return (int)Mod.Call("getFirstElementProj", proj, ignoreExplosive);
        return 0;
    }
}