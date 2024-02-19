using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.DamageClasses;
using StramClasses;
using StramClasses.Classes.Rogue;
using StramClasses.Classes.Rogue.Projectiles;
using AwfulGarbageMod.Global;
using Terraria.GameContent;
using StramClasses.Classes.Rogue.Weapons;
using Terraria.Audio;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Weapons.Rogue
{
    [ExtendsFromMod("StramClasses")]

    public class FrigidJavelin : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.toRogueItem(22);
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 18;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.5f;
            Item.value = 1000;
            Item.rare = 2;
            SoundStyle soundStyle = new SoundStyle("StramClasses/Assets/Sounds/RogueKnife");
            soundStyle.Volume = 3f;
            soundStyle.PitchVariance = 0.5f;
            soundStyle.Pitch = -0.75f;
            SoundStyle value = soundStyle;
            base.Item.UseSound = value; Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("FrigidJavelinProj").Type;
            Item.shootSpeed = 7.5f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            RoguePlayer roguePlayer = player.rogue();

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[proj].rogueProjectile().critDamage = (int)Math.Round(roguePlayer.critDamage * (float)base.Item.rogueItem().baseCritDamage * base.Item.rogueItem().prefixCritDamage + (float)roguePlayer.critDamageFlat);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StramClasses.Classes.Rogue.Weapons.IceSpear>()
                .AddIngredient<FrigidiumBar>(17)
                .AddIngredient<SpiritItem>(15)
                .AddIngredient<FrostShard>(13)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class FrigidJavelinProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        int aiState = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = StramUtils.rogueDamage();
            Projectile.rogueProjectile().splittableWeapon = false;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(in SoundID.Item27, base.Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                int type = (Main.rand.NextBool(2) ? 135 : 80);
                Vector2 vector = new Vector2(0f, -2f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 46)));
                Dust.NewDust(base.Projectile.Center, 1, 1, type, vector.X, vector.Y);
            }
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC nPC = Main.npc[j];
                if (nPC.chaseable && base.Projectile.Distance(nPC.Center) <= 64f && nPC.type != 488 && !nPC.friendly && Collision.CanHit(base.Projectile.Center, 1, 1, nPC.Center, 1, 1) && !nPC.dontTakeDamage && nPC.active && nPC.lifeMax > 5)
                {
                    int num = Projectile.NewProjectile(base.Projectile.GetSource_FromThis(), nPC.Center, Vector2.Zero, ModContent.ProjectileType<IceSpearShatter>(), base.Projectile.damage / 2, 0f, base.Projectile.owner);
                    Main.projectile[num].rogueProjectile().critDamage = base.Projectile.rogueProjectile().critDamage / 2;
                    Main.projectile[num].rogueProjectile().critDamageModifier = 0.5f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                for (var i = 0; i < 3; i++)
                {
                    Vector2 positionOffset = new Vector2(800, 0).RotatedByRandom(MathHelper.ToRadians(360));
                    int num = Projectile.NewProjectile(base.Projectile.GetSource_FromThis(), target.Center + positionOffset, positionOffset.SafeNormalize(Vector2.Zero) * -7f, ModContent.ProjectileType<FrigidJavelinProj2>(), base.Projectile.damage / 2, 0f, base.Projectile.owner);
                    Main.projectile[num].rogueProjectile().critDamage = base.Projectile.rogueProjectile().critDamage / 2;
                    Main.projectile[num].rogueProjectile().critDamageModifier = 0.5f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            /*
            float offsetX = 0;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 0;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            */

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            base.Projectile.ai[0] += 1f;
            if (base.Projectile.ai[0] >= 45f)
            {
                float num = (Main.player[base.Projectile.owner].rogue().moonMagnet ? (-1f) : 1f);
                base.Projectile.velocity.Y = base.Projectile.velocity.Y + num * 0.15f;
                base.Projectile.velocity.X = base.Projectile.velocity.X * 0.97f;
            }
                
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(45);


            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1f;
            Main.dust[dust].velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.6f);
            Main.dust[dust].noGravity = true;
        }
    }
    [ExtendsFromMod("StramClasses")]

    public class FrigidJavelinProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = StramUtils.rogueDamage();
            Projectile.rogueProjectile().splittableWeapon = false;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        float projSpeed = 0.3f;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            projSpeed = 0.3f;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].noGravity = true;
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0, 0 / 2, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1.35f;
            Main.dust[dust2].noGravity = true;

            float maxDetectRadius = 500f; // The maximum radius at which a projectile can detect a target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            projSpeed += 0.02f;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 7f)
            {
                Projectile.velocity *= 0.95f;
            }
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 12f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 12;
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