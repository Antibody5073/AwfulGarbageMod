using AwfulGarbageMod.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class GeodeGemcornStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.mana = 18;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("AcornStaffProj").Type;
			Item.shootSpeed = 12f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Main.NewText((int)(Item.mana * player.manaCost));
            int proj = Projectile.NewProjectile(source, position, velocity, Mod.Find<ModProjectile>("GeodeGemcornStaffProj").Type, damage, knockback, player.whoAmI, 0, (int)(Item.mana * player.manaCost));
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AmberGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<AmethystGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<TopazGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<SapphireGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<EmeraldGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<RubyGemcornStaff>());
            recipe.AddIngredient(ModContent.ItemType<DiamondGemcornStaff>());
            recipe.AddIngredient(ItemID.Bone, 25);
            recipe.AddIngredient(ItemID.Geode, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}

    public class GeodeGemcornStaffProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        bool regenMana = true;
        float projSpeed = 1f;
        int bounces = 0;
        int[] npcsHit = { -1, -1, -1, -1, -1, -1, -1 };


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 4; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("RubyGemcornStaffProj2").Type, Projectile.damage / 3, 0, Projectile.owner, 0);
                Main.projectile[proj].CritChance = Projectile.CritChance;
            }
            if (bounces < 4)
            {
                float maxDetectRadius = 1200f; // The maximum radius at which a projectile can detect a target
                float projSpeed = Vector2.Distance(new Vector2(0, 0), oldVelocity);

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                {
                    if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                    if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                }
                else
                {

                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                bounces++;

                return false;
            }
            else
            {
                return true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            npcsHit[Projectile.penetrate] = target.whoAmI;
            projSpeed = 1f;
            Player player = Main.LocalPlayer;
            if (regenMana)
            {
                player.ManaEffect((int)Projectile.ai[1]);
                player.statMana += (int)Projectile.ai[1];
                //Main.NewText((int)Projectile.ai[1]);
                regenMana = false;
            }
            target.AddBuff(ModContent.BuffType<EmeraldDefenseBuff>(), 180);
            int debuff = Main.rand.Next(10);
            if (debuff == 0 || debuff == 1 || debuff == 2)
            {
                target.AddBuff(BuffID.OnFire, 75);
            }
            if (debuff == 3 || debuff == 4)
            {
                target.AddBuff(BuffID.Frostburn, 75);
            }
            if (debuff == 5 || debuff == 6)
            {
                target.AddBuff(BuffID.Poisoned, 90);
            }
            if (debuff == 7)
            {
                target.AddBuff(BuffID.Confused, 45);
            }
            if (debuff == 8)
            {
                target.AddBuff(BuffID.Wet, 150);
            }
            if (debuff == 9)
            {
                target.AddBuff(BuffID.Slimed, 150);
            }
            for (int i = 0; i < 4; i++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, new Vector2(Main.rand.NextFloat(4, 7), 0).RotatedByRandom(MathHelper.ToRadians(360)), Mod.Find<ModProjectile>("RubyGemcornStaffProj2").Type, Projectile.damage / 3, 0, Projectile.owner, 0);
                Main.projectile[proj].CritChance = Projectile.CritChance;
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
            if (Projectile.timeLeft % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextFromList(DustID.GemAmethyst, DustID.GemAmber, DustID.GemTopaz, DustID.GemSapphire, DustID.GemEmerald, DustID.GemRuby, DustID.GemDiamond), 0f, 0f, 0, default(Color), 1f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(225f);

            if (Projectile.timeLeft % 10 == 5)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Mod.Find<ModProjectile>("TopazGemcornStaffProj2").Type, Projectile.damage / 3, 0, Projectile.owner, 0);
                Main.projectile[proj].CritChance = Projectile.CritChance;
            }

            float maxDetectRadius = 300; // The maximum radius at which a projectile can detect a target

            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            projSpeed += 0.1f;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 12f)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 12;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(225f);
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
                if (target.CanBeChasedBy() && !npcsHit.Contains(k))
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