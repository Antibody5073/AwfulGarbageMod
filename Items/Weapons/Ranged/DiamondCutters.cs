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
using AwfulGarbageMod.Global;
using System.Collections.Generic;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class DiamondCutters : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 14;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 26;
			Item.noMelee = true;
			Item.scale = 0f;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3f;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DiamondCuttersProj>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(12)) * Main.rand.NextFloat(0.7f, 1.1f), type, damage, knockback, player.whoAmI);
            proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(5)) * Main.rand.NextFloat(0.9f, 1.3f), type, damage, knockback, player.whoAmI);

            return false;
        }
        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.Diamond, 8)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Diamond, 8)
                .AddIngredient(ItemID.TungstenBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DiamondCuttersProj : ModProjectile
    {
        public bool IsStickingToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        // Index of the current target
        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        Vector2 startingVel;
        int startingDamage;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 75;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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

        public override void OnSpawn(IEntitySource source)
        {
            startingVel = Projectile.velocity;
            startingDamage = Projectile.damage;
        }

        public override void AI()
        {
            if (IsStickingToTarget)
            {
                Projectile.alpha = 100;
                StickyAI();
            }
            else
            {
                NormalAI();
            }
        }
        private void NormalAI()
        {
            if (Projectile.timeLeft % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].noGravity = true;
            }
            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 255 / 15;
                Projectile.velocity *= 0.8f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(30);
        }

        private const int StickTime = 60 * 30; // 15 seconds

        private void StickyAI()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            StickTimer += 1f;

            // Every 30 ticks, the javelin will perform a hit effect
            bool hitEffect = StickTimer % 30f == 0f;
            int npcTarget = TargetWhoAmI;
            if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200)
            { // If the index is past its limits, kill it
                Projectile.Kill();
            }
            else if (Main.npc[npcTarget].active)
            {
                // If the target is active and can take damage
                // Set the projectile's position relative to the target's center
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                if (hitEffect)
                {
                    // Perform a hit effect here, causing the npc to react as if hit.
                    // Note that this does NOT damage the NPC, the damage is done through the debuff.
                    Main.npc[npcTarget].HitEffect(0, 1.0);
                }
            }
            else
            { // Otherwise, kill the projectile
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.npc[npcTarget].Center, startingVel.RotatedByRandom(MathHelper.PiOver2), Projectile.type, (int)(startingDamage * 1.4f), Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].GetGlobalProjectile<KnifeProjectile>().canBeEmpowered = false;
                Projectile.Kill();
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
            if (IsStickingToTarget)
            {
                int npcIndex = TargetWhoAmI;
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                    {
                        behindNPCsAndTiles.Add(index);
                    }
                    else
                    {
                        behindNPCsAndTiles.Add(index);
                    }

                    return;
                }
            }
            // Since we aren't attached, add to this list
            behindNPCsAndTiles.Add(index);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            IsStickingToTarget = true; // we are sticking to a target
            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
            Projectile.velocity = (target.Center - Projectile.Center) *
                0.75f; // Change velocity based on delta center of targets (difference between entity centers)
            Projectile.netUpdate = true; // netUpdate this javelin
            Projectile.damage = 0; // Makes sure the sticking javelins do not deal damage anymore

        }

    }
}