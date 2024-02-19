using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class ToothpickTome : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toothpick Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons falling bone toothpicks from the sky \n\"No, no! It's called excavation!\"");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.mana = 12;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 7;
            Item.useAnimation = 28;
            Item.useStyle = 5;
            Item.knockBack = 1;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = Mod.Find<ModProjectile>("ToothpickTomeProj").Type;
            Item.shootSpeed = 18f;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
            pointPoisition.Y -= 100 * 1;
            float num90 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            float num101 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
            float ai2 = num101 + pointPoisition.Y;
            if (num101 < 0f)
            {
                num101 *= -1f;
            }
            if (num101 < 20f)
            {
                num101 = 20f;
            }
            float num112 = (float)Math.Sqrt(num90 * num90 + num101 * num101);
            num112 = Item.shootSpeed / num112;
            num90 *= num112;
            num101 *= num112;
            Vector2 vector5 = new Vector2(num90, num101) / 2f;
            Projectile.NewProjectile(source, pointPoisition.X, pointPoisition.Y, vector5.X, vector5.Y, Mod.Find<ModProjectile>("ToothpickTomeProj").Type, damage, knockback, player.whoAmI, 0f, ai2);

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
    }

    public class ToothpickTomeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bone Toothpick"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 8;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 8; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Bone, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }
    }
}