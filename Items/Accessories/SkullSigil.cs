using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using gunrightsmod.Content.DamageClasses;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{

    public class SkullSigil : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 1;
            Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = ItemRarityID.Green;
			Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<GlobalPlayer>().BoneBracelet += Item.damage;
        }
    }
    public class BoneBraceletProj : ModProjectile
    {
        public override string Texture => "AwfulGarbageMod/Items/Weapons/Melee/BackboneProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;

            // DisplayName.SetDefault("Bone Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y < 0)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.BoneTorch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.25f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
        }
    }
}