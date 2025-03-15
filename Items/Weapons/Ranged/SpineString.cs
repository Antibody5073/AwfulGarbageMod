using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class SpineString : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spine String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Arrows fired gain 1 penetration and move incredibly fast\nRegular arrows are not affected by gravity\n\"We're gonna exterminate youuu!\"");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 18;
            Item.noMelee = true;
            Item.useAnimation = 18;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 8f;
            Item.crit = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity * 1.5f, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].extraUpdates += 2;

            if (Main.projectile[proj].penetrate != -1)
            {
                Main.projectile[proj].penetrate += 1;
            }

            if (Main.projectile[proj].type == ProjectileID.HellfireArrow)
            {
                Main.projectile[proj].aiStyle = 0;
                Main.projectile[proj].rotation = Main.projectile[proj].velocity.ToRotation() + MathHelper.ToRadians(90);
                int proj2 = Projectile.NewProjectile(source, position, velocity * 1.5f, type, damage, knockback, player.whoAmI);
                Main.projectile[proj2].extraUpdates += 2;
                Main.projectile[proj2].aiStyle = 0;
                Main.projectile[proj2].rotation = Main.projectile[proj].velocity.ToRotation() + MathHelper.ToRadians(90);
                return false;
            }
            if (Main.projectile[proj].aiStyle == 1)
            {
                Main.projectile[proj].aiStyle = 0;
                Main.projectile[proj].rotation = Main.projectile[proj].velocity.ToRotation() + MathHelper.ToRadians(90);
                Main.projectile[proj].usesLocalNPCImmunity = true;
                Main.projectile[proj].localNPCHitCooldown = 10;

            }


            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }
    }
}