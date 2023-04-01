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

namespace AwfulGarbageMod.Global;
public class GlobalProjectiles : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
    {
        Player player = Main.LocalPlayer;

        if (projectile.DamageType == DamageClass.Ranged && player.GetModPlayer<GlobalPlayer>().spiderPendant == true)
        {
            target.AddBuff(BuffID.Poisoned, 600);

        }
    }
}


