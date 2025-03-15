using AwfulGarbageMod.DamageClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class AssortedKnifeBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Throw 2-3 knives at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 22;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.value = 30000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("SlimyKnivesProj").Type;
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int seed = Main.rand.Next(10);
            int proj;
            switch (seed)
            {
                case 0:
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)) * 0.8f, ProjectileID.ThrowingKnife, (int)(damage * 0.7f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 15;
                    proj = Projectile.NewProjectile(source, position, velocity, ProjectileID.ThrowingKnife, (int)(damage * 0.7f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 15;
                    proj = Projectile.NewProjectile(source, position, velocity, ProjectileID.ThrowingKnife, (int)(damage * 0.7f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 15;
                    proj = Projectile.NewProjectile(source, position, velocity * 0.7f, ModContent.ProjectileType<FractureKnifeProj>(), (int)(damage * 1.2f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    break;
                case 1:
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(15)) * 0.9f, ProjectileID.ThrowingKnife, (int)(damage * 0.7f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 15;
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * 0.8f, ModContent.ProjectileType<SlimyKnivesProj>(), (int)(damage * 0.9f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(3)) * 0.7f, ModContent.ProjectileType<SunbladesProj>(), (int)(damage * 1.3f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    break;
                case 2:
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3)) * 0.7f, Mod.Find<ModProjectile>("SunbladesProj").Type, (int)(damage * 1.3f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-3)) * 0.7f, Mod.Find<ModProjectile>("SunbladesProj").Type, (int)(damage * 1.3f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<SlimyKnivesProj>(), (int)(damage * 0.9f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * Main.rand.NextFloat(0.6f, 0.9f), ModContent.ProjectileType<ImperceptibleProj>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    break;
                case 3:
                    for (var i = 0; i < Main.rand.Next(3, 5); i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<SlimyKnivesProj>(), damage, knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;
                case 4:
                    if (Main.rand.NextBool())
                    {
                        proj = Projectile.NewProjectile(source, position, velocity * 1.2f, ProjectileID.ThrowingKnife, (int)(damage * 0.8f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 15;
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(16)) * Main.rand.NextFloat(0.8f, 1f), ModContent.ProjectileType<BagOfFrozenKnivesProj>(), (int)(damage * 0.7f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-16)) * Main.rand.NextFloat(0.8f, 1f), ModContent.ProjectileType<BagOfPoisonedKnivesProj>(), (int)(damage * 0.7f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    else
                    {
                        proj = Projectile.NewProjectile(source, position, velocity * 1.2f, ProjectileID.ThrowingKnife, (int)(damage), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 15;
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-12)) * Main.rand.NextFloat(0.8f, 1f), ModContent.ProjectileType<BagOfFrozenKnivesProj>(), (int)(damage * 0.9f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(12)) * Main.rand.NextFloat(0.8f, 1f), ModContent.ProjectileType<BagOfPoisonedKnivesProj>(), (int)(damage * 0.9f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;
                case 5:
                    for (var i = 0; i < 3; i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(3)) * (0.8f + 0.1f * i), ModContent.ProjectileType<BagOfFrozenPoisonedKnivesProj>(), (int)(damage * 0.9f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;
                case 6:
                    for (var i = -1; i < 2; i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(8 * i)) * 0.7f, ModContent.ProjectileType<FractureKnifeProj>(), (int)(damage * 1.2f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;
                case 7:
                    for (var i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<SlimyKnivesProj>(), (int)(damage * 0.5f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    for (var i = 0; i < Main.rand.Next(2, 3); i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(16)) * Main.rand.NextFloat(0.8f, 1.2f), ProjectileID.ThrowingKnife, (int)(damage * 0.6f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;
                case 8:
                    for (var i = -3; i < 4; i++)
                    {
                        if (Main.rand.NextBool())
                        {
                            proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3 * i)) * 0.5f, ModContent.ProjectileType<BagOfFrozenKnivesProj>(), (int)(damage * 0.7f), knockback, player.whoAmI);
                        }
                        else
                        {
                            proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3 * i)) * 0.5f, ModContent.ProjectileType<BagOfPoisonedKnivesProj>(), (int)(damage * 0.7f), knockback, player.whoAmI);
                        }
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    proj = Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<ImperceptibleProj>(), (int)(damage * 0.75f), knockback, player.whoAmI);
                    Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    break;
                case 9:
                    for (var i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(12)) * Main.rand.NextFloat(0.6f, 0.9f), ModContent.ProjectileType<ImperceptibleProj>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                        Main.projectile[proj].CritChance = Item.crit + (int)player.GetCritChance(DamageClass.Ranged) + (int)player.GetCritChance(DamageClass.Generic) + 4;
                    }
                    break;

            }

                    return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("BagOfFrozenPoisonedKnives").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("SlimyKnives").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Imperceptible").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("FractureKnife").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Sunblades").Type);
            recipe.AddIngredient(ItemID.ThrowingKnife, 50);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}