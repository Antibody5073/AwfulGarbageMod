using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class DeathRipper : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slime Splatter Shark"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Normal bullets turn into splattering slime\nFires bullets extremely quickly");
		}

		int counter;

		public override void SetDefaults()
		{
			Item.damage = 194;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 4;
			Item.useAnimation = 4;
			Item.useStyle = 5;
			Item.knockBack = 0.5f;
			Item.value = 20000;
			Item.rare = 5;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 9f;
			Item.noMelee = true;
			Item.ArmorPenetration = 15;
		}
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool(2, 5);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            counter++;
            if (counter % 15 == 1)
            {
				for (int i = 0; i < Main.rand.Next(2, 5); i++)
				{
					int proj = Projectile.NewProjectile(source, position, velocity * 2 + Main.rand.NextVector2Circular(4, 4), ProjectileID.DeathSickle, damage * 2, knockback / 2, player.whoAmI);
					Main.projectile[proj].DamageType = DamageClass.Ranged;
				}
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3.5f));
        }
        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(-8, 0);
            return offset;
        }
	}
}