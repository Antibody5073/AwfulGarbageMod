using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using gunrightsmod.Content.DamageClasses;
using Infernus;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using rail;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.TerMericaCrossMod
{

    [ExtendsFromMod("gunrightsmod")]
    public class SnakeOil : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithoutArgs", tooltipWithoutArgs.Format(AGUtils.ScaleDamage(15, player, ModContent.GetInstance<SummonStupidDamage>())));
            tooltips.Add(tooltip);
        }
        public int counter;

		public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<StupidDamage>() += 0.03f;
            player.GetDamage(DamageClass.Summon) += 0.03f;
            player.GetModPlayer<GlobalTerMericaPlayer>().SnakeOil = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SnakeOilMinion>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<SnakeOilMinion>(), 0, 0, player.whoAmI);
            }
        }
    }
}