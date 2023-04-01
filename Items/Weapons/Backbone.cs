using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons
{

    public class Backbone : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Backbone"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Launches bone shards upwards on impact \n\"I've got a strong backbone, you know?\"");
		}

		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = 1;
			Item.knockBack = 4.5f;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 23;
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
			for (var i = 0; i < 3; i++)
			{
				Vector2 ProjVel = new Vector2(0, Main.rand.Next(-7, -5)).RotatedByRandom(MathHelper.ToRadians(15));
				Vector2 posOffset = new Vector2(target.width / 2, target.height * -1);
				Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), target.position + posOffset, ProjVel, Mod.Find<ModProjectile>("BackboneProj").Type, damage / 3 * 2, knockBack, player.whoAmI);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}