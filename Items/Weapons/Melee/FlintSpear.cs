using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public class FlintSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Spirit Pike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Shoots a homing spirit every third use");
		}

        public int counter;

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Spear);
			Item.damage = 7;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 37;
			Item.useAnimation = 37;
			Item.knockBack = 7;
			Item.value = 5000;
			Item.rare = 1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<FlintSpearProj>();
            Item.shootSpeed = 7f;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient<Flint>(19)
                .AddRecipeGroup(RecipeGroupID.Wood, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    

    public class FlintSpearProj : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 44f;
        protected virtual float HoldoutRangeMax => 114f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Spirit Pike"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        bool hitEnemy = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1.25f;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 19;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        public override void OnSpawn(IEntitySource source)
        {
            hitEnemy = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hitEnemy)
            {
                modifiers.SourceDamage *= 2;
                hitEnemy = true;
            }
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            if (Projectile.timeLeft >= halfDuration)
            {
                int dust = Dust.NewDust(Projectile.position + new Vector2(3, 3), Projectile.width - 3, Projectile.height - 3, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.25f;
                Main.dust[dust].velocity = Projectile.velocity.RotatedBy(1.5f * MathHelper.PiOver2) * 3.25f;
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(Projectile.position + new Vector2(3, 3), Projectile.width - 3, Projectile.height - 3, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.25f;
                Main.dust[dust].velocity = Projectile.velocity.RotatedBy(-1.5f * MathHelper.PiOver2) * 3.25f;
                Main.dust[dust].noGravity = true;
            }

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }



            return false; // Don't execute vanilla AI.
        }
    }
}