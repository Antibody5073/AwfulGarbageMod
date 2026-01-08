using AwfulGarbageMod;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.Vanity
{

    public class DumpsterDiverAttire : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Head", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Legs", EquipType.Legs, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Back", EquipType.Back, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Neck", EquipType.Neck, this);
            }
        }

        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.accessory = true;
            Item.value = 0;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<DumpsterDiverAttirePlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<DumpsterDiverAttirePlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Garbage>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class DumpsterDiverAttirePlayer : ModPlayer
    {
        public bool vanityEquipped = false;

        public override void ResetEffects()
        {
            vanityEquipped = false;
        }

        public override void FrameEffects()
        {
            if (vanityEquipped)
            {
                Player.back = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Back);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Head);
                Player.neck = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Neck);
            }
        }
    }
}