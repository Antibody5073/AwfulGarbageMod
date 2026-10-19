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
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttireHalf_Head", EquipType.Head, name: "Half");
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttireClose_Head", EquipType.Head, name: "Close");
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Legs", EquipType.Legs, this);
                EquipLoader.AddEquipTexture(Mod, "AwfulGarbageMod/Items/Accessories/Vanity/DumpsterDiverAttire_Back", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            int equipSlotHeadHalf = EquipLoader.GetEquipSlot(Mod, "Half", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHeadHalf] = false;
            int equipSlotHeadClose = EquipLoader.GetEquipSlot(Mod, "Close", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHeadClose] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.DisableHandOnAndOffAccDraw[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            ArmorIDs.Legs.Sets.OverridesLegs[equipSlotLegs] = true;
            ArmorIDs.Legs.Sets.IncompatibleWithFrogLeg[equipSlotLegs] = true;
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
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            // By changing the equipSlot to the leg equip texture slot, the leg texture will now be drawn on the player
            // We're changing the leg slot so we set this to true
            robes = true;
            // Here we can get the equip slot by name since we referenced the item when adding the texture
            // You can also cache the equip slot in a variable when you add it so this way you don't have to call GetEquipSlot
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            /* Here is example code for supporting a female-specifig legs equip texture. See Load as well.
			if (!male) {
				equipSlot = EquipLoader.GetEquipSlot(Mod, Name + "_Female", EquipType.Legs);
			}
			*/
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
                if (Player.eyeHelper.CurrentEyeFrame == 0)
                {
                    Player.head = EquipLoader.GetEquipSlot(Mod, "DumpsterDiverAttire", EquipType.Head);
                }

                else if (Player.eyeHelper.CurrentEyeFrame.ToString() == "EyeHalfClosed")
                {
                    Player.head = EquipLoader.GetEquipSlot(Mod, "Half", EquipType.Head);
                }
                else
                {
                    Player.head = EquipLoader.GetEquipSlot(Mod, "Close", EquipType.Head);
                }
            }
        }
    }
}