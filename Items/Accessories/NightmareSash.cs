using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption;

namespace AwfulGarbageMod.Items.Accessories
{
    public class NightmareSash : ModItem
    {
        public static LocalizedText RedemptionEffect { get; private set; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "RedemptionEffect", RedemptionEffect.Format());
                tooltips.Add(tooltip);
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("24% increased critical strike damage");
            RedemptionEffect = this.GetLocalization(nameof(RedemptionEffect));

        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 4500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.96f * (1f - player.endurance));

            player.GetCritChance(DamageClass.Ranged) += 4f;
            player.GetDamage(DamageClass.Ranged) += 0.04f;
            player.GetArmorPenetration(DamageClass.Generic) += 4;
            player.GetModPlayer<GlobalPlayer>().NightmareSash = true;
            player.GetModPlayer<GlobalPlayer>().HoneyOnDamaged += 300;
            player.moveSpeed += 0.2f;

            if (ModLoader.TryGetMod("Redemption", out Mod mod))
            {
                IncreaseElementalResistance(player, ElementID.Shadow, 0.08f, mod);
                IncreaseElementalResistance(player, ElementID.Psychic, 0.08f, mod);
            }
        }
    }
    public class NightmareSashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Default;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 20;
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

            float maxDetectRadius = 800; // The maximum radius at which a projectile can detect a target

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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            projSpeed = 0.1f;
            if (Projectile.velocity.LengthSquared() < 8 * 8)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One) * 8;
            }
            Projectile.velocity = Projectile.velocity.RotatedByRandom(1);
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