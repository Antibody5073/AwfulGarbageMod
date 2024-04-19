using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.DamageClasses;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class RadiantPrism : ModItem
	{
        public static LocalizedText tooltipWithoutArgs { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
            tooltipWithoutArgs = this.GetLocalization(nameof(tooltipWithoutArgs));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithoutArgs", tooltipWithoutArgs.Format(Math.Round(4f * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier * 100) / 100));
            tooltips.Add(tooltip);
        }


        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = 8;
			Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.04f;
            player.GetCritChance(DamageClass.Ranged) += 4f;
            player.GetModPlayer<RadiantEmpowermentPlayer>().hasSigil = true;
        }
    }
    public class RadiantPrismSwordProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.light = 1f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
        }
        Vector2 EnemyPos;
        bool didit = false;
        float dir;

        public override bool? CanDamage()
        {
            if (didit)
            {
                return base.CanDamage();
            }
            return false;
        }

        public override void AI()
        {
            if (!didit)
            {
                dir = Main.rand.NextFloatDirection();
                didit = true;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active)
            {

                EnemyPos = npc.Center;
            }
            Projectile.Center = new Vector2(MathHelper.Lerp(EnemyPos.X - (float)(192 * Math.Cos(dir)), EnemyPos.X + (float)(192 * Math.Cos(dir)), Projectile.timeLeft / 60f), MathHelper.Lerp(EnemyPos.Y - (float)(192 * Math.Sin(dir)), EnemyPos.Y + (float)(192 * Math.Sin(dir)), Projectile.timeLeft / 60f));

            if (Projectile.timeLeft >= 45)
            {
                Projectile.alpha -= 17;
            }
            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 17;
            }
            Projectile.rotation = dir;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[932].Value;
            Rectangle rectangle25 = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin33 = rectangle25.Size() / 2f;
            Color alpha13 = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle25, alpha13, Projectile.rotation, origin33, Projectile.scale, SpriteEffects.None);


            return false;
        }
    }
}