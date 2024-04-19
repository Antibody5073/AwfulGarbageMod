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
    public abstract class KnifeEmpowerment : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public virtual string empowermentText => "";
        public virtual Color textColor => Color.White;
        public virtual int empowermentCooldown => 60;
        public virtual int empowermentID => -1; //dont forget to set this


        public bool hasThisEmpowerment = false;

        bool didit = false;
        int origdmg;

        public override bool PreAI(Projectile projectile)
        {
            if (!didit)
            {
                hasThisEmpowerment = projectile.GetGlobalProjectile<KnifeProjectile>().Empowerments[empowermentID];

                if (projectile.DamageType == ModContent.GetInstance<KnifeDamageClass>() && !projectile.GetGlobalProjectile<KnifeProjectile>().hasEmpowerment && projectile.GetGlobalProjectile<KnifeProjectile>().canBeEmpowered)
                {
                    Player player = Main.player[projectile.owner];
                    //Knife empowerments
                    if (empowermentAvailable(player))
                    {
                        origdmg = projectile.damage;
                        hasThisEmpowerment = true;
                        projectile.GetGlobalProjectile<KnifeProjectile>().Empowerments[empowermentID] = true;
                        projectile.GetGlobalProjectile<KnifeProjectile>().hasEmpowerment = true;
                        EmpowermentEffects(projectile, player, true, true);
                        string key = empowermentText;
                        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), textColor, key, dramatic: true);
                        if (player.GetModPlayer<GlobalPlayer>().CobaltRanged) 
                        {
                            projectile.damage += (int)(origdmg * 1.1f);
                        }
                    }
                }
                didit = true;
            }
            return base.PreAI(projectile);
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (hasThisEmpowerment)
            {
                BehaviorChanges(projectile, player);

                Visuals(projectile, player);
            }
        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            if (hasThisEmpowerment)
            {
                HitEffect(projectile, target, hit, damageDone, player);
                if (player.GetModPlayer<GlobalPlayer>().PalladiumRanged)
                {
                    player.AddBuff(BuffID.RapidHealing, 360);
                }
            }
        }
        public virtual bool empowermentAvailable(Player player)
        {
            return false;
        }

        public virtual void EmpowermentEffects(Projectile projectile, Player player, bool ApplyDmgKb, bool setCooldown)
        {

        }

        public virtual void Visuals(Projectile projectile, Player player)
        {

        }

        public virtual void BehaviorChanges(Projectile projectile, Player player)
        {

        }

        public virtual void HitEffect(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {

        }
    }

    
}
