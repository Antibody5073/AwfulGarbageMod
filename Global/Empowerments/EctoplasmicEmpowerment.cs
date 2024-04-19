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
using AwfulGarbageMod.Items.Weapons.Melee;

namespace AwfulGarbageMod.Global
{
    public class EctoplasmicEmpowerment : KnifeEmpowerment
    {
        public override string empowermentText => "Ectoplasmic Empowerment!";
        public override int empowermentCooldown => 210;
        public override int empowermentID => Empowerments.EctoplasmicEmpowerment; 
        public override Color textColor => new Color(253, 246, 39);

        public override bool empowermentAvailable(Player player)
        {
            return player.GetModPlayer<EctoplasmicEmpowermentPlayer>().hasSigil && player.GetModPlayer<EctoplasmicEmpowermentPlayer>().sigilCooldown <= 0;
        }

        public override void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {
            if (setCooldown)
            {
                player.GetModPlayer<EctoplasmicEmpowermentPlayer>().sigilCooldown = (int)(empowermentCooldown * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier);
            }
            if (ApplyDmgKb)
            {
                projectile.damage = (int)(projectile.damage * 1.2f);
                projectile.damage += 50;
            }
        }

        public override void Visuals(Projectile projectile, Player player)
        {
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.SpectreStaff, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.75f, 1.25f);
            }
        }

        public override void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {

            for (int i = 0; i < 3; i++)
            {
                int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(6, 6), ModContent.ProjectileType<IceSpiritPikeSpiritProj>(), projectile.damage /2, 0, player.whoAmI, 0, 30);
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].penetrate = 1;
                Main.projectile[proj].DamageType = ModContent.GetInstance<KnifeDamageClass>();
            }
        }
    }

    public class EctoplasmicEmpowermentPlayer : ModPlayer
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
