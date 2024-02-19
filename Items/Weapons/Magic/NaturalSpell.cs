using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class NaturalSpell : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Natural Spell"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots leaves that scatter upon enemy hit");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
		{
			Item.damage = 11;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 0;
			Item.shoot = Mod.Find<ModProjectile>("SylvanTomeProj").Type;
			Item.shootSpeed = 7f;
			Item.noMelee = true;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = 0; i < 5; i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity * Main.rand.NextFloat(0.97f, 1.03f), Mod.Find<ModProjectile>("NaturalSpellProj").Type, damage, knockback, player.whoAmI);

            }
			return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("SylvanTome").Type);
            recipe.AddIngredient(ItemID.JungleSpores, 18);
            recipe.AddIngredient(ItemID.Stinger, 12);
            recipe.AddIngredient(ItemID.Vine, 10);
            recipe.AddIngredient(ItemID.CorruptSeeds, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("SylvanTome").Type);
            recipe2.AddIngredient(ItemID.JungleSpores, 18);
            recipe2.AddIngredient(ItemID.Stinger, 12);
            recipe2.AddIngredient(ItemID.Vine, 10);
            recipe2.AddIngredient(ItemID.CrimsonSeeds, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }

    public class NaturalSpellProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leaf"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 31))) * Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void AI()
        {
            Projectile.aiStyle = 1;
            int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Grass, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

        }
    }
}