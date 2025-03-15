using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Printing;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using AwfulGarbageMod;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using Mono.Cecil;
using Microsoft.Xna.Framework.Input;
using AwfulGarbageMod.Items.Accessories;

namespace AwfulGarbageMod.Global
{
    public class LushEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Lush Empowerment!";
        public override int empowermentCooldown => 360;
        public override int empowermentID => Empowerments.LushEmpowerment;
        public override Color textColor => new Color(20, 203, 14);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<LushEmpowermentPlayer>().hasSigil && player.GetModPlayer<LushEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<LushEmpowermentPlayer>().sigilCooldown = (int)(360 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {
                projectile.damage += 6;
            }
            Projectile.NewProjectile(projectile.GetSource_ReleaseEntity(), projectile.Center, projectile.velocity, ProjectileType<ForestSigilProj>(), projectile.damage / 3, 0, player.whoAmI, projectile.whoAmI, 90, -10);
            Projectile.NewProjectile(projectile.GetSource_ReleaseEntity(), projectile.Center, projectile.velocity, ProjectileType<ForestSigilProj>(), projectile.damage / 3, 0, player.whoAmI, projectile.whoAmI, -90, 10);

        }

        public override void Visuals(Projectile projectile, Player player)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GreenFairy, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
        }
    }

    public class LushEmpowermentPlayer : ModPlayer
    {
        public bool hasSigil = false;
        public int sigilCooldown = 0;

        public override void ResetEffects()
        {
            hasSigil = false;
            sigilCooldown -= 1;
        }
    }
}
