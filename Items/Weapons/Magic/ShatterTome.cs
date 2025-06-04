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

    public class ShatterTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shatter Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots a shattering icicle \nShards have armor piercing capabilities");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
		{
			Item.damage = 17;
			Item.mana = 4;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("ShatterTomeProj").Type;
			Item.shootSpeed = 8f;
			Item.noMelee = true;
		}
        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FrostShard").Type, 14);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 3);
            recipe.AddIngredient(ItemID.BorealWood, 30);
			recipe.AddIngredient(ItemID.IceBlock, 30);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

    public class ShatterTomeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Icicle"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 5; i++)
            {
                float xv = (0f - Projectile.velocity.X) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                float yv = (0f - Projectile.velocity.Y) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + xv, Projectile.position.Y + xv), new Vector2(xv, yv), Mod.Find<ModProjectile>("ShatterTomeSplit").Type, Projectile.damage / 3, 0f, Projectile.owner);
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90f);
            int dust = Dust.NewDust(Projectile.Center, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(80, 115) * 0.013f;
            Main.dust[dust].noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 5; i++)
            {
                float xv = (0f - Projectile.velocity.X) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                float yv = (0f - Projectile.velocity.Y) * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + xv, Projectile.position.Y + xv), new Vector2(xv, yv), Mod.Find<ModProjectile>("ShatterTomeSplit").Type, Projectile.damage / 3, 0f, Projectile.owner);
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }

    public class ShatterTomeSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 12;
        }


        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
            Main.dust[dust].noGravity = true;
        }
    }
}