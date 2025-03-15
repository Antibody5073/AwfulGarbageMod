using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class ShadowBookSealed : ModItem
	{
		public override void SetStaticDefaults()
		{
            
			// DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "Sealed", "\n[c/521864:(Sealed)]");
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "ItemName" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
		{
			Item.damage = 81;
			Item.mana = 14;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 300000;
            Item.rare = -1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 5;
			Item.shoot = ModContent.ProjectileType<ShadowBookProj>();
			Item.shootSpeed = 10f;
			Item.noMelee = true;
		}
	}
    public class ShadowBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "Sealed", "\n[c/521864:(Unsealed)]");
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "ItemName" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.mana = 17;
            Item.DamageType = DamageClass.Magic;
            Item.width = 42;
            Item.height = 46;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = 5;
            Item.knockBack = 0.1f;
            Item.value = 300000;
            Item.rare = -1;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.crit = 5;
            Item.shoot = ModContent.ProjectileType<ShadowBookProj>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = -1; i < 2; i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(24 * i)), type, damage, knockback, player.whoAmI);

            }
            return false;
        }

    }

    public class ShadowBookProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 840;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        float projSpeed = 0.4f;
        int bounces = 0;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bounces < 4)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                bounces++;

                return false;
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].alpha = 120;
            }
            if (Projectile.timeLeft % 10 == 0)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 vel = new Vector2((float)Math.Cos(MathHelper.TwoPi * i / 32) * 2, (float)Math.Sin(MathHelper.TwoPi * i / 16) * 4);
                    vel = vel.RotatedBy(Projectile.velocity.ToRotation());
                    int dust = Dust.NewDust(Projectile.position + new Vector2(5, 5), Projectile.width - 5, Projectile.height - 5, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
                    Main.dust[dust].velocity = vel;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].alpha = 120;
                }
            }

            float maxDetectRadius = 420; // The maximum radius at which a projectile can detect a target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            projSpeed += 0.03f;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 10f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 10;
            }
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }

}