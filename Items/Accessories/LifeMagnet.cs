using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{

    public class LifeMagnet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 12500;
            Item.rare = 2;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().LifeMagnet = true;
        }
    }


    public class LifeMagnetItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == 58 || entity.type == 1734 || entity.type == 1867;
        }
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.GetModPlayer<GlobalPlayer>().LifeMagnet)
            {
                grabRange += 10 * 16;
            }
        }
        public override bool GrabStyle(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().LifeMagnet)
            {
                float speed = 10;
                int acc = 3;
                if (player.lifeMagnet)
                {
                    speed += 10;
                    acc += 3;
                }

                PullItem_Pickup(item, player, speed, acc);
                return true;
            }
            return false;
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (player.GetModPlayer<GlobalPlayer>().BloodMagnet)
            {
                player.AddBuff(ModContent.BuffType<BloodMagnetBuff>(), 60 * 5);
            }
            return base.OnPickup(item, player);
        }

        private void PullItem_Pickup(Item item, Player player, float speed, int acc)
        {
            //IL_0032: Unknown result type (might be due to invalid IL or missing references)
            //IL_003c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            float num = player.Center.X - item.Center.X;
            float num2 = player.Center.Y - item.Center.Y;
            float num3 = (float)Math.Sqrt(num * num + num2 * num2);
            num3 = speed / num3;
            num *= num3;
            num2 *= num3;
            item.velocity.X = (item.velocity.X * (float)(acc - 1) + num) / (float)acc;
            item.velocity.Y = (item.velocity.Y * (float)(acc - 1) + num2) / (float)acc;
        }
    }
}