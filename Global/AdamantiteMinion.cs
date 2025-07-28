using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Global
{

    public class AdamantiteMinion : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool boost = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            boost = false;

            if (source is EntitySource_Parent { Entity: Projectile { minionPos: 0, IsMinionOrSentryRelated: true } })
            {
                if (Main.player[projectile.owner].GetModPlayer<GlobalPlayer>().AdamantiteMinion)
                {
                    boost = true;
                }
            }
        }
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.IsMinionOrSentryRelated;
        }
        public override void AI(Projectile projectile)
        {
            if (Main.player[projectile.owner].GetModPlayer<GlobalPlayer>().AdamantiteMinion && ((projectile.minionPos == 0 && projectile.minion) || boost))
            {
                int dust = Dust.NewDust(projectile.position - new Vector2(2f, 2f), projectile.width + 4, projectile.height + 4, DustID.Firework_Red, 0, 0);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= Main.rand.NextFloat(0.5f, 1f);
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].GetModPlayer<GlobalPlayer>().AdamantiteMinion && ((projectile.minionPos == 0 && projectile.minion) || boost))
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }
}