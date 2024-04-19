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

namespace AwfulGarbageMod.Global
{
    public class PurityEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Purity Empowerment!";
        public override int empowermentCooldown => 240;
        public override int empowermentID => Empowerments.PurityEmpowerment; 
        public override Color textColor => new Color(166, 246, 126);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<PurityEmpowermentPlayer>().hasSigil && player.GetModPlayer<PurityEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<PurityEmpowermentPlayer>().sigilCooldown = (int)(240 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {
                projectile.damage = (int)(projectile.damage * 1.3f);
            }
        }

        public override void Visuals(Projectile projectile, Player player)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PurificationPowder, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            target.AddBuff(ModContent.BuffType<PurityBuff>(), 300);
            hasThisEmpowerment = false;

        }
    }

    public class PurityEmpowermentPlayer : ModPlayer
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
