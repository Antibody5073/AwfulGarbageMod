using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class StormSpell : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Spell"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Casts a lightning bolt that jumps between up to 6 nearby enemies\nDirect hits ignore some defense");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.mana = 7;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = 5;
            Item.knockBack = 3;
            Item.value = 15000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item94;
            Item.autoReuse = true;
            Item.crit = 0;
            Item.shoot = Mod.Find<ModProjectile>("StormSpellProj").Type;
            Item.shootSpeed = 12f;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, velocity.X, velocity.Y);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(40, 40);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("Cloudstrike").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 6);
            recipe.AddIngredient(ItemID.RainCloud, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class StormSpellProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 200;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 200;
            Projectile.ArmorPenetration = 16;
        }

        float oldOffset = 0;
        float newOffset = Main.rand.NextFloat(-12, 12);
        float currentOffset = 0;
        float counter = -1;


        int[] npcsHit = { -1, -1, -1, -1, -1, -1, -1 };


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Projectile.ArmorPenetration = 0;
            Projectile.damage = (int)(Projectile.damage * 0.95);

            for (int k = 0; k < Main.maxNPCs; k++)
            { 
                if (Main.npc[k] == target)
                {
                    npcsHit[Projectile.penetrate] = k;
                    break;
                }
            }

            float maxDetectRadius = 800f; // The maximum radius at which a projectile can detect a target
            float projSpeed = Vector2.Distance(new Vector2(0, 0), Projectile.velocity); // The speed at which the projectile moves towards the target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
            {
                Projectile.timeLeft = 1;
                return;
            }
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            Projectile.aiStyle = -1;
            counter++;
            currentOffset += (newOffset - oldOffset) / 5;
            if (counter == 5)
            {
                oldOffset = newOffset;
                newOffset = Main.rand.NextFloat(-12, 12);
                counter = -1;
            }

            Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);

            int dust = Dust.NewDust(Projectile.Center + normalizedVel.RotatedBy(MathHelper.ToRadians(90)) * currentOffset, 0, 0, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;


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