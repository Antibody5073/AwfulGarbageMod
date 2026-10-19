using AwfulGarbageMod.Global;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class Enlightenment : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Crystal Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("25% reduced mana cost when not moving vertically\n2% increased magic crit chance");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.08f;
            player.GetCritChance(DamageClass.Melee) += 8f;
            player.GetModPlayer<GlobalPlayer>().Enlightenment += 200;
        }
	}
    public class EnlightenmentProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 360;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.extraUpdates = 3;
        }
        Vector2 EnemyPos;
        bool didit = false;
        float dir;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = 60f;

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.localAI[0] += 1f;

            
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            /*Texture2D texture = TextureAssets.Projectile[932].Value;
            Rectangle rectangle25 = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin33 = rectangle25.Size() / 2f;
            Color alpha13 = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle25, alpha13, Projectile.rotation, origin33, Projectile.scale, SpriteEffects.None);
            */
            DrawProj_FairyQueenLance(Projectile);

            return false;
        }
        private void DrawProj_FairyQueenLance(Projectile proj)
        {
            Vector2 vector = proj.Center - Main.screenPosition;
            int num = 40;
            int num2 = 180 * num;
            num2 /= 2;
            Color color = Main.hslToRgb((proj.ai[1] + 0.5f) % 1f, 1f, 0.5f) * proj.Opacity;
            Color color2 = color;
            color.A = 0;
            color2.A = (byte)(color2.A / 2);
            Texture2D value = TextureAssets.Extra[178].Value;
            Vector2 origin = value.Frame().Size() * new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2((num2 / value.Width), 2f);
            Vector2 scale2 = new Vector2((num2 / value.Width) * 0.5f, 2f);
            Color color3 = color * Utils.GetLerpValue(60f, 55f, proj.localAI[0], clamped: true) * Utils.GetLerpValue(0f, 10f, proj.localAI[0], clamped: true);
            
            Main.spriteBatch.Draw(value, vector, (Rectangle?)null, color3, proj.rotation, origin, scale2, (SpriteEffects)0, 0f);
            Main.spriteBatch.Draw(value, vector, (Rectangle?)null, color3 * 0.3f, proj.rotation, origin, scale, (SpriteEffects)0, 0f);
            
            Texture2D value2 = TextureAssets.Projectile[932].Value;
            Vector2 origin2 = value2.Frame().Size() / 2f;
            Color color4 = Color.White * Utils.GetLerpValue(0f, 20f, proj.localAI[0], clamped: true);
            color4.A = (byte)(color4.A / 2);
            float num3 = MathHelper.Lerp(0.7f, 1f, Utils.GetLerpValue(55f, 60f, proj.localAI[0], clamped: true));
            float lerpValue = Utils.GetLerpValue(10f, 60f, proj.localAI[0]);
            if (lerpValue > 0f)
            {
                float lerpValue2 = Utils.GetLerpValue(0f, 1f, ((Vector2)(proj.velocity)).Length(), clamped: true);
                for (float num4 = 1f; num4 > 0f; num4 -= 1f / 6f)
                {
                    Vector2 vector2 = proj.rotation.ToRotationVector2() * -120f * num4 * lerpValue2;
                    Main.spriteBatch.Draw(value2, vector + vector2, (Rectangle?)null, color * lerpValue * (1f - num4), proj.rotation, origin2, num3, (SpriteEffects)0, 0f);
                    Main.spriteBatch.Draw(value2, vector + vector2, (Rectangle?)null, new Color(255, 255, 255, 0) * 0.15f * lerpValue * (1f - num4), proj.rotation, origin2, num3 * 0.85f, (SpriteEffects)0, 0f);
                }
                for (float num5 = 0f; num5 < 1f; num5 += 0.25f)
                {
                    Vector2 vector3 = (num5 * ((float)Math.PI * 2f) + proj.rotation).ToRotationVector2() * 2f * num3;
                    Main.spriteBatch.Draw(value2, vector + vector3, (Rectangle?)null, color2 * lerpValue, proj.rotation, origin2, num3, (SpriteEffects)0, 0f);
                }
                Main.spriteBatch.Draw(value2, vector, (Rectangle?)null, color2 * lerpValue, proj.rotation, origin2, num3 * 1.1f, (SpriteEffects)0, 0f);
            }
            Main.spriteBatch.Draw(value2, vector, (Rectangle?)null, color4, proj.rotation, origin2, num3, (SpriteEffects)0, 0f);
            
        }
    }
}