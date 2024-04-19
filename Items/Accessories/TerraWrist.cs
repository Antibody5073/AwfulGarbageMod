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
using System.Collections.Generic;
using Terraria.Localization;
using AwfulGarbageMod.DamageClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.CodeAnalysis;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class TerraWrist : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithoutArgs", tooltipWithoutArgs.Format(Math.Round(3.5f * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier * 100) / 100));
            tooltips.Add(tooltip);
        }


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 150000;
            Item.rare =8;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().knifeVelocity += 0.12f;
            player.GetModPlayer<GlobalPlayer>().terraSigil = true;
            player.GetDamage<KnifeDamageClass>() += 0.12f;
            player.GetModPlayer<GlobalPlayer>().mechanicalArm += 0.5f;
            player.GetModPlayer<TerraEmpowermentPlayer>().hasSigil = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SigilOfShadows>()
                .AddIngredient<HolyHandOfJudgement>()
                .AddIngredient<MechanicalArm>()
                .AddIngredient(ItemID.AvengerEmblem)
                .AddIngredient(ItemID.BrokenHeroSword)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class TerraWristSparkProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 1;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.ArmorPenetration = 30;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            Main.dust[dust].velocity *= 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class TerraWristSwordProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 30;
            Projectile.light = 1f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        Vector2 EnemyPos;

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active)
            {
                
                EnemyPos = npc.Bottom;
            }
            if (Projectile.timeLeft > 19)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, 0, 0, Projectile.alpha, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
                Main.dust[dust].velocity *= 0;
            }
            Projectile.Center = new Vector2(EnemyPos.X, MathHelper.Lerp(EnemyPos.Y - 128, EnemyPos.Y + 64, Projectile.timeLeft / 30f));

            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 17;
            }
            Projectile.rotation = MathHelper.ToRadians(-45);
            
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ProjectileID.TerraBeam].Value;
            Rectangle rectangle25 = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin33 = rectangle25.Size() / 2f;
            Color alpha13 = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + 32), rectangle25, alpha13, Projectile.rotation, origin33, Projectile.scale, SpriteEffects.None);
            

            return false;
        }
    }
    public class TerraWristNightProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 30;
            Projectile.light = 1f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        Vector2 EnemyPos;

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active)
            {

                EnemyPos = npc.Bottom;
            }
            if (Projectile.timeLeft > 19)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, 0, 0, Projectile.alpha, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
                Main.dust[dust].velocity *= 0;
            }
            Projectile.Center = new Vector2(EnemyPos.X, MathHelper.Lerp(EnemyPos.Y - 128, EnemyPos.Y + 64, Projectile.timeLeft / 30f));

            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 17;
            }
            Projectile.rotation = MathHelper.ToRadians(-45);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ProjectileID.TerraBeam].Value;
            Rectangle rectangle25 = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin33 = rectangle25.Size() / 2f;
            Color alpha13 = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + 32), rectangle25, alpha13, Projectile.rotation, origin33, Projectile.scale, SpriteEffects.None);


            return false;
        }
    }

}