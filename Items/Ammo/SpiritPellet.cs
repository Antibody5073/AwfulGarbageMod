using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Ammo
{
    public class SpiritPellet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stone Pellet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Low damage, easy to obtain bullet");
        }

        public override void SetDefaults()
        {
            Item.damage = 2; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 2.5f;
            Item.value = 15;
            Item.rare = ItemRarityID.Green;
            Item.shoot = Mod.Find<ModProjectile>("SpiritPelletProj").Type; // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 5.5f; // The speed of the projectile.
            Item.ammo = AmmoID.Bullet; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(54);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 1);
            recipe.AddIngredient(Mod.Find<ModItem>("StonePellet").Type, 54);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe(66);
            recipe2.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 1);
            recipe2.AddIngredient(ItemID.MusketBall, 66);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
    public class SpiritPelletProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fossil Bullet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        bool isHoming = false;
        int[] npcsHit = { -1, -1, -1};

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            if (!isHoming)
            {
                Projectile.damage = (int)(Projectile.damage * 0.6f);
                isHoming = true;
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k] == target)
                    {
                        npcsHit[Projectile.penetrate] = k;
                        break;
                    }
                }
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!isHoming)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.damage = (int)(Projectile.damage * 0.75f);
                Projectile.penetrate -= 1;
                isHoming = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].noGravity = true;
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0, 0 / 2, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1.35f;
            Main.dust[dust2].noGravity = true;
            if (isHoming)
            {
                float maxDetectRadius = 300f; // The maximum radius at which a projectile can detect a target
                float projSpeed = 0.3f; // The speed at which the projectile moves towards the target

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                    return;

                // If found, change the velocity of the projectile and turn it in the direction of the target
                // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 7f)
                {
                    Projectile.velocity *= 0.95f;
                }
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
                if (!npcsHit.Contains(k))
                {
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
            }
            return closestNPC;
        }
    }
}