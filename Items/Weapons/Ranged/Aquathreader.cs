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

    public class Aquathreader : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hydro String"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Shoots two streams of water along with an arrow");
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 29;
            Item.noMelee = true;
            Item.useAnimation = 29;
            Item.useStyle = 5;
            Item.knockBack = 3f;
            Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 9f;
            Item.crit = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int proj = Projectile.NewProjectile(source, position, velocity * 1.4f, type, damage, knockback, player.whoAmI);

            if (Main.projectile[proj].aiStyle == 1)
            {
                Main.projectile[proj].aiStyle = 0;
                Main.projectile[proj].rotation = Main.projectile[proj].velocity.ToRotation() + MathHelper.ToRadians(90);
            }

            proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("AquathreaderProj").Type, damage * 5 / 7, knockback / 2, player.whoAmI, 0, 90, -10);
            Main.projectile[proj].ArmorPenetration += 10;
            proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("AquathreaderProj").Type, damage * 5 / 7, knockback / 2, player.whoAmI, 0, -90, 10);
            Main.projectile[proj].ArmorPenetration += 10;

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("HydroString").Type);
            recipe.AddIngredient(ItemID.ShadowScale, 18);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("HydroString").Type);
            recipe2.AddIngredient(ItemID.TissueSample, 18);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
    public class AquathreaderProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Stream"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        Vector2 centerPos;
        float offsetPos = 0;
        float offsetDistance = 45;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterStream);
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            centerPos = Projectile.position;
            offsetPos = Projectile.ai[1];
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            centerPos += Projectile.velocity;
            offsetPos += Projectile.ai[2];
            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
            Projectile.position = centerPos + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * (float)Math.Sin((double)MathHelper.ToRadians(offsetPos)) * offsetDistance;

            for (var i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0, 0);
                Main.dust[dust].scale = Main.rand.NextFloat(1.25f, 1.75f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}