using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AwfulGarbageMod.Global;


namespace AwfulGarbageMod.Projectiles
{
    public class UmbragelShadow : ModProjectile
    {
        int timer = 0;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Item item = new(ItemID.ShadowDye);

            int owner = Projectile.owner;
            Player other = Main.player[owner];
            if (Main.playerVisualClone[owner] == null)
            {
                Main.playerVisualClone[owner] = new Player();
            }
            Player player = Main.playerVisualClone[owner];
            player.CopyVisuals(other);
            //player.isFirstFractalAfterImage = true;
            //player.firstFractalAfterImageOpacity = 50 * 1f;
            player.ResetEffects();
            player.ResetVisibleAccessories();

            for (int i = 0; i < player.dye.Length; i++)
            {
                player.dye[i] = item;
            }
            player.UpdateDyes();
            player.DisplayDollUpdate();
            player.UpdateSocialShadow();
            player.itemAnimationMax = 60;
            player.itemAnimation = 0;
            player.skinColor = Color.Black;
            player.itemRotation = player.GetModPlayer<GlobalPlayer>().umbragelVel[0].ToRotation();
            player.position = player.GetModPlayer<GlobalPlayer>().umbragelPos[0];
            player.direction = ((player.GetModPlayer<GlobalPlayer>().umbragelVel[0].X > 0f) ? 1 : (-1));
            player.velocity.Y = 0.01f;
            player.wingFrame = other.wingFrame;
            player.PlayerFrame();
            player.socialIgnoreLight = true;
            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, 0f, player.fullRotationOrigin, 0.4f);
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.GetModPlayer<GlobalPlayer>().umbragelPos[0];

            if (!CheckActive(player))
            {
                return;
            }

            Shoot();
            


            for (var j = 0; j < 4; j++)
            {
                int dust = Dust.NewDust(Projectile.position, player.width, player.height, DustID.Wraith, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].alpha = 130;
            }
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalPlayer>().UmbragelBonus == true)
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }
        public void Shoot()
        {
        }
    }
}