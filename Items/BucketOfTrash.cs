using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework.Input;
using AwfulGarbageMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AwfulGarbageMod.Items
{
    public class BucketOfTrash : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("\"Doesn't seem to melt\""); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of Projectile item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 19;
            Item.useTime = 19;
            Item.autoReuse = true;
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 9999; // The item's max stack value
            Item.value = 0; // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.consumable = true;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<BucketOfTrashProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Garbage>(2)
                .AddIngredient(ItemID.EmptyBucket)
                .Register();
        }
    }

    public class BucketOfTrashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            float num = 0.05f;
            float num2 = Projectile.width / 2;
            for (int i = 0; i < 1000; i++)
            {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[i].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < num2)
                {
                    if (Projectile.position.X < Main.projectile[i].position.X)
                    {
                        Projectile.velocity.X -= num;
                    }
                    else
                    {
                        Projectile.velocity.X += num;
                    }
                    if (Projectile.position.Y < Main.projectile[i].position.Y)
                    {
                        Projectile.velocity.Y -= num;
                    }
                    else
                    {
                        Projectile.velocity.Y += num;
                    }
                }
            }
            if (Projectile.wet)
            {
                Projectile.velocity.X *= 0.9f;
                int num3 = (int)(Projectile.Center.X + (float)((Projectile.width / 2 + 8) * Projectile.direction)) / 16;
                int num4 = (int)(Projectile.Center.Y / 16f);
                _ = Projectile.position.Y / 16f;
                int num5 = (int)((Projectile.position.Y + (float)Projectile.height) / 16f);
                Tile tile1 = Main.tile[num3, num4];
                Tile tile2 = Main.tile[num3, num5];

                if (tile1 == null)
                {
                    tile1 = default(Tile);
                }
                if (tile2 == null)
                {
                    tile2 = default(Tile);
                }
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y *= 0.5f;
                }
                num3 = (int)(Projectile.Center.X / 16f);
                num4 = (int)(Projectile.Center.Y / 16f);
                float num6 = AI_061_FishingBobber_GetWaterLine(num3, num4);
                if (Projectile.Center.Y > num6)
                {
                    Projectile.velocity.Y -= 0.1f;
                    if (Projectile.velocity.Y < -8f)
                    {
                        Projectile.velocity.Y = -8f;
                    }
                    if (Projectile.Center.Y + Projectile.velocity.Y < num6)
                    {
                        Projectile.velocity.Y = num6 - Projectile.Center.Y;
                    }
                }
                else
                {
                    Projectile.velocity.Y = num6 - Projectile.Center.Y;
                }
            }
            else
            {
                if (Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X *= 0.95f;
                }
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y += 0.3f;
                if (Projectile.velocity.Y > 15.9f)
                {
                    Projectile.velocity.Y = 15.9f;
                }
            }
            if (Projectile.frameCounter == 0)
            {
                Projectile.frameCounter = 1;
                Projectile.frame = Main.rand.Next(3);
            }
            if (Projectile.frameCounter < 10 && Projectile.wet)
            {
                Projectile.frameCounter++;
                for (float num7 = 0f; num7 < 1f; num7 += 0.5f)
                {
                    int goreType;
                    if (Main.rand.NextBool(3))
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore1").Type;
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore2").Type;

                    }
                    else
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore3").Type;
                    }


                    Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position + Vector2.UnitY * 6f, Vector2.Zero, goreType, Projectile.scale);
                    gore.velocity = Main.rand.NextVector2CircularEdge(10f, 10f);
                    if (gore.velocity.Y > 0f)
                    {
                        gore.velocity.Y *= -1f;
                    }
                }
                for (float num8 = 0f; num8 < 2f; num8 += 1f)
                {
                    int goreType;
                    if (Main.rand.NextBool(3))
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore1").Type;
                    }
                    else if (Main.rand.NextBool(2))
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore2").Type;

                    }
                    else
                    {
                        goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore3").Type;
                    }
                    Gore gore2 = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position + Vector2.UnitY * 6f, Vector2.Zero, goreType, Projectile.scale * 0.85f + Main.rand.NextFloat() * 0.15f);
                    gore2.velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                    if (gore2.velocity.Y > 0f)
                    {
                        gore2.velocity.Y *= -1f;
                    }
                }
            }
            Projectile.scale = Utils.GetLerpValue(0f, 60f, Projectile.timeLeft, clamped: true);
            Projectile.rotation += Projectile.velocity.X * 0.14f;
            bool flag = !Projectile.wet && Projectile.velocity.Length() < 0.8f;
            int maxValue = (Projectile.wet ? 90 : 5);
            if (Main.rand.Next(maxValue) == 0 && !flag)
            {
                int goreType;
                if (Main.rand.NextBool(3))
                {
                    goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore1").Type;
                }
                else if (Main.rand.NextBool(2))
                {
                    goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore2").Type;

                }
                else
                {
                    goreType = Mod.Find<ModGore>($"BucketOfTrash_Gore3").Type;
                }
                Gore gore3 = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position + Vector2.UnitY * 6f, Vector2.Zero, goreType, Projectile.scale);
                if (Projectile.wet)
                {
                    gore3.velocity = Vector2.UnitX * Main.rand.NextFloatDirection() * 0.75f + Vector2.UnitY * Main.rand.NextFloat();
                }
                else if (gore3.velocity.Y < 0f)
                {
                    gore3.velocity.Y = 0f - gore3.velocity.Y;
                }
            }
        }
        private float AI_061_FishingBobber_GetWaterLine(int X, int Y)
        {
            float result = Projectile.position.Y + (float)Projectile.height;
            Tile tileAbove = Main.tile[X, Y - 1];
            Tile tileThis = Main.tile[X, Y];
            Tile tileBelow = Main.tile[X, Y + 1];


            if (tileAbove == null)
            {
                tileAbove = default(Tile);
            }
            if (tileThis == null)
            {
                tileThis = default(Tile);
            }
            if (tileBelow == null)
            {
                tileBelow = default(Tile);
            }
            if (tileAbove.LiquidAmount > 0)
            {
                result = Y * 16;
                result -= (float)((int)tileAbove.LiquidAmount / 16);
            }
            else if (tileThis.LiquidAmount > 0)
            {
                result = (Y + 1) * 16;
                result -= (float)((int)tileThis.LiquidAmount / 16);
            }
            else if (tileBelow.LiquidAmount > 0)
            {
                result = (Y + 2) * 16;
                result -= (float)((int)tileThis.LiquidAmount / 16);
            }
            return result;
        }
    }
}