using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.demo
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class SnowfallCloak : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Frozen Spirit Breastplate");
            // Tooltip.SetDefault("Increases melee armor penetration by 4");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = 1; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
        {
            player.ammoCost80 = true;
            player.moveSpeed += 0.2f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<SnowfallHood>() && legs.type == ModContent.ItemType<SnowfallLeggings>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Ice prisms orbit you and shatter with varying effects\nHitting prisms with an arrow will duplicate the arrow\nHitting prisms with bullets or other ranged projectile types will drop a healing heart"; // This is the setbonus tooltip
            {
                player.GetModPlayer<SnowfallSetBonus>().HasArmorSet = true;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SnowfallPrism>()] < 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<SnowfallPrism>(), 0, 0, Main.myPlayer, i);
                    }
                }
            }
        }
    }
    public class SnowfallSetBonus : ModPlayer
    {
        public bool HasArmorSet;

        public override void ResetEffects()
        {
            HasArmorSet = false;
        }
    }
    public class SnowfallPrism : ModProjectile
    {

        public float dir;

        public float OrbitDistance => 200;
        public float OrbitSpd => 1;
        public float OffsetDir => 0;

        float distance = 90;

        public float cooldown = 300;

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }


        public override void OnKill(int timeLeft)
        {
                for (var i = 0; i < 10; i++)
                {
                    float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, xv, yv, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1f;
                    Main.dust[dust].noGravity = true;
                }
        }


        public virtual void GetOrbitDirection(out float tempdir, out Vector2 targetPos, float spd = 1, float distance = 120)
        {

            Player player = Main.player[Projectile.owner];
            tempdir = MathHelper.ToRadians((player.GetModPlayer<SnowflakeTimer>().snowflakeTimer * spd) + Projectile.ai[0] * 360 / player.ownedProjectileCounts[this.Type]);
            targetPos = player.MountedCenter + new Vector2(distance, 0).RotatedBy(tempdir) - new Vector2(Projectile.width / 2, Projectile.height / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (cooldown <= 0)
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

                if (cooldown > -10)
                {

                    Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                        sourceRectangle, Color.White * ((10 + cooldown) / 10f), Projectile.rotation, origin, Projectile.scale * (1 - cooldown / 10f), spriteEffects, 0);
                }
            }
            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public virtual void SetRotation(Player player, float direction, float offsetDir = 0)
        {
            Projectile.rotation = 0;
            dir = direction;

        }

        public virtual void SetPositionBasedOnDirection(float direction, Vector2 targetPos)
        {
            Projectile.position = targetPos;
        }

        public override void AI()
        {
            cooldown--;
            Player player = Main.player[Projectile.owner];

            GetOrbitDirection(out float tempDir, out Vector2 targetPos, OrbitSpd, OrbitDistance);
            SetPositionBasedOnDirection(tempDir, targetPos);

            SetRotation(player, tempDir, OffsetDir);

            if (player.GetModPlayer<SnowfallSetBonus>().HasArmorSet)
            {
                Projectile.timeLeft = 3;
            }
            distance = 210;
            if (cooldown <= 0)
            {
                foreach (var item in Main.projectile)
                {
                    if (item.active && item.CountsAsClass(DamageClass.Ranged))
                    {

                        if (item.Hitbox.Intersects(Projectile.Hitbox))
                        {
                            for (var i = 0; i < 10; i++)
                            {
                                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, xv, yv, 0, default(Color), 1f);
                                Main.dust[dust].scale = 1f;
                                Main.dust[dust].noGravity = true;
                            }
                            SoundEngine.PlaySound(SoundID.Shatter); 

                            if (item.arrow)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), item.Center, item.velocity.RotatedBy(MathHelper.ToRadians(10)), item.type, item.damage, item.knockBack, item.owner, item.ai[0], item.ai[1], item.ai[2]);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), item.Center, item.velocity.RotatedBy(MathHelper.ToRadians(-10)), item.type, item.damage, item.knockBack, item.owner, item.ai[0], item.ai[1], item.ai[2]);
                                cooldown = 300;
                            }
                            else
                            {
                                Item.NewItem(Projectile.GetSource_ReleaseEntity(), Projectile.position, Projectile.Size, ItemID.Heart);
                                cooldown = 1200;

                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
