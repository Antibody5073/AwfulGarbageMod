using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
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
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class OcularScepterBuff : ScepterBuff
    {
        public override int ScepterMaxManaPenalty => 15;
        public override int NonMagicDmgPenaltyPercent => 8;
        public override int ScepterProjType => ModContent.ProjectileType<OcularScepterOrbit>();

        public override void ScepterEffects(Player player)
        {
            player.statDefense += 3;
        }
    }

    public class OcularScepter : ModItem
	{
        public static LocalizedText ScepterMax { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Scepter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Summons orbital slimeballs that provide 3 defense each, with a max of 6 orbitals. \nTaking damage releases them. \nReduces max mana by 5 and non-magic damage by 8% for each active orbital.");
            ModItemSets.Sets.MaxScepterProjectiles[Item.type] = 8;
            ScepterMax = this.GetLocalization(nameof(ScepterMax));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip = new TooltipLine(Mod, "ScepterMax", ScepterMax.Format(Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost));
            TooltipLine line = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text += tooltip.Text;
            }
        }

        public override void SetDefaults()
		{
            Item.damage = 23;
            Item.DamageType = ModContent.GetInstance<ScepterDamageClass>();
            Item.mana = 5;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.shoot = Mod.Find<ModProjectile>("OcularScepterOrbit").Type;
			Item.shootSpeed = 0;
			Item.noMelee = true;
            Item.buffType = ModContent.BuffType<OcularScepterBuff>();

            Item.scepterItem().scepter = true;
            Item.scepterItem().MaxScepterProjectiles = 8;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("OcularScepterOrbit").Type] < Item.scepterItem().MaxScepterProjectiles + player.GetModPlayer<GlobalPlayer>().MaxScepterBoost)
            {
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, knockback, Main.myPlayer, player.ownedProjectileCounts[Mod.Find<ModProjectile>("OcularScepterOrbit").Type]);
            }

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Lens, 3);
            recipe.AddIngredient(ItemID.DemoniteBar, 16);
            recipe.AddIngredient(ItemID.CorruptSeeds, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Lens, 3);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 16); 
            recipe2.AddIngredient(ItemID.CrimsonSeeds, 2);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
	}

    public class OcularScepterOrbit : ScepterOrbit
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Servant of Cthulhu"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            GlobalProjectiles.Sets.IsScepterProjectile[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 1;
        }

        public override float ProjSpd => 12;
        public override float OrbitDistance => 100;
        public override float OrbitSpd => 1f;
        public override float OffsetDir => 0;
        public override int ProjType => ModContent.ProjectileType<OcularScepterProj>();
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void Visuals()
        {
            int dust = Dust.NewDust(Projectile.position, 12, 12, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.1f;
            Main.dust[dust].noGravity = true;
        }
    }

    public class OcularScepterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Servant of Cthulhu"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }


        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 12; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Blood, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1.35f, 1.8f);
            }
        }



        public override void AI()
        {



            int dust = Dust.NewDust(Projectile.position, 12, 12, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.1f;
            Main.dust[dust].noGravity = true;

            float maxDetectRadius = 800f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 0.35f; // The speed at which the projectile moves towards the target

            Projectile.rotation = Projectile.velocity.ToRotation();


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
    }
}