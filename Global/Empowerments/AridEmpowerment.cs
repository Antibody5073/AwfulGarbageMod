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
    public class AridEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Arid Empowerment!";
        public override int empowermentCooldown => 150;
        public override int empowermentID => Empowerments.AridEmpowerment; 
        public override Color textColor => new Color(235, 201, 66);
        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<AridEmpowermentPlayer>().hasSigil && player.GetModPlayer<AridEmpowermentPlayer>().sigilCooldown <= 0;
        }
        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<AridEmpowermentPlayer>().sigilCooldown = (int)(150 * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {

                projectile.damage = (int)(projectile.damage * 1.5f);
                projectile.damage += 5;
            }
        }

        public override void Visuals(Projectile projectile, Player player)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.AmberBolt, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            Main.dust[dust].velocity *= 0;
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            player.AddBuff(ModContent.BuffType<AridSpeedBuff>(), 240);
        }
    }
    public class AridEmpowermentPlayer : ModPlayer
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
