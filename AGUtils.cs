using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.CodeAnalysis;
using AwfulGarbageMod.DamageClasses;
using StramClasses.Classes.Rogue;
using Terraria.WorldBuilding;
using System.Linq;

namespace AwfulGarbageMod
{
    public static class AGUtils
    {
        public static bool AnyMeleeDmg(DamageClass damageType)
        {
            return damageType.CountsAsClass(DamageClass.Melee);
        }
        public static bool AnyRangedDmg(DamageClass damageType)
        {
            return damageType.CountsAsClass(DamageClass.Ranged);
        }
        public static bool AnyMagicDmg(DamageClass damageType)
        {
            return damageType.CountsAsClass(DamageClass.Magic);
        }
        public static bool AnySummonDmg(DamageClass damageType)
        {
            return damageType.CountsAsClass(DamageClass.Summon);
        }

        public static FlailProjectile flailProjectile(this Projectile proj)
        {
            return proj.GetGlobalProjectile<FlailProjectile>();
        }
        public static GlobalEnemyBossInfo globalEnemyBossInfo(this NPC npc)
        {
            return npc.GetGlobalNPC<GlobalEnemyBossInfo>();
        }

        public static bool IsOnScreen(this Projectile proj, Vector2 spriteDimensions)
        {
            Vector2 offsetFromCamera = Main.Camera.Center - proj.Center;
            if(Math.Abs(offsetFromCamera.X) - spriteDimensions.X > 800 || Math.Abs(offsetFromCamera.Y) - spriteDimensions.Y > 450)
            {
                return false;
            }
            return true;
        }


        public static void ApplyEmpowermentEffects(Projectile proj, KnifeEmpowerment empowerment, bool ApplyDmgKb)
        {
            empowerment.EmpowermentEffects(proj, Main.player[proj.owner], ApplyDmgKb, false);
        }
        public static bool IsAmbientObject(int tileType)
        {
            if (tileType == TileID.Plants || tileType == TileID.Plants2 || tileType == TileID.CorruptThorns || tileType == TileID.CrimsonThorns || tileType == TileID.JungleThorns || tileType == TileID.AshPlants || tileType == TileID.CrimsonPlants || tileType == TileID.CorruptPlants || tileType == TileID.JunglePlants || tileType == TileID.JunglePlants2 || tileType == TileID.Pots || tileType == TileID.CorruptPlants || tileType == TileID.Torches || tileType == TileID.Sunflower)
            {
                return true;
            }
            return false;
        }

        public static int GetTileCounts(int tileType)
        {
            int tileCount = 0;
            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY)); i++)
            {
                int x = i % Main.maxTilesX;
                int y = i / Main.maxTilesY;
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.TileType == tileType)
                {
                    tileCount++;
                }
            }
            return tileCount;
        }
        public static NPC GetClosestNPC(Vector2 position, float maxDetectDistance, params NPC[] ignoreNPCs)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy() && !ignoreNPCs.Contains(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
    public static class Empowerments
    {
        public static readonly int FireEmpowerment = 0;
        public static readonly int ShadowEmpowerment = 1;
        public static readonly int AridEmpowerment = 2;
        public static readonly int StormEmpowerment = 3;

    }
}