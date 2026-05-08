using AwfulGarbageMod;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Steamworks;
using StramClasses.Classes.Rogue;
using System;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
        public static readonly float OnePerentJumpSpeed = 0.066666666666666f;


        public static FlailProjectile flailProjectile(this Projectile proj)
        {
            return proj.GetGlobalProjectile<FlailProjectile>();
        }
        public static GlobalEnemyBossInfo globalEnemyBossInfo(this NPC npc)
        {
            return npc.GetGlobalNPC<GlobalEnemyBossInfo>();
        }

        public static ShieldItem shieldItem(this Item item)
        {
            return item.GetGlobalItem<ShieldItem>();
        }
        public static ScepterItem scepterItem(this Item item)
        {
            return item.GetGlobalItem<ScepterItem>();
        }
        public static bool IsOnScreen(this Projectile proj, Vector2 spriteDimensions)
        {
            Vector2 offsetFromCamera = Main.Camera.Center - proj.Center;
            if(Math.Abs(offsetFromCamera.X) - spriteDimensions.X > 1600 || Math.Abs(offsetFromCamera.Y) - spriteDimensions.Y > 900)
            {
                return false;
            }
            return true;
        }

        public static float TurnTowards(float turnSpd, Vector2 targetPos, float startingDir, Vector2 position, float smoothness = 1)
        {
            float currentDir = -MathHelper.ToDegrees(startingDir) - 90;
            float targetDir = -MathHelper.ToDegrees((targetPos - position).ToRotation()) - 90;
            if (targetDir > currentDir + 180)
            {
                targetDir -= 360;
                while (targetDir >= currentDir + 180)
                {
                    targetDir -= 360;
                }
            }
            if (targetDir < currentDir - 180)
            {
                targetDir += 360;
                while (targetDir <= currentDir - 180)
                {
                    targetDir += 360;
                }
            }
            float turn = (targetDir - currentDir) / smoothness;
            if (turn > turnSpd)
            {
                turn = turnSpd;
            }
            if (turn < -turnSpd)
            {
                turn = -turnSpd;
            }

            return -MathHelper.ToRadians(turn);
        }
        public static float TurnTowardsDirection(float turnSpd, float targetDir, float startingDir, float smoothness = 1)
        {
            float currentDir = -MathHelper.ToDegrees(startingDir) - 90;
            float targetDir2 = -MathHelper.ToDegrees(targetDir) - 90;
            if (targetDir > currentDir + 180)
            {
                targetDir -= 360;
                while (targetDir >= currentDir + 180)
                {
                    targetDir -= 360;
                }
            }
            if (targetDir < currentDir - 180)
            {
                targetDir += 360;
                while (targetDir <= currentDir - 180)
                {
                    targetDir += 360;
                }
            }
            float turn = (targetDir - currentDir) / smoothness;
            if (turn > turnSpd)
            {
                turn = turnSpd;
            }
            if (turn < -turnSpd)
            {
                turn = -turnSpd;
            }

            return -MathHelper.ToRadians(turn);
        }

        public static void ApplyEmpowermentEffects(Projectile proj, KnifeEmpowerment empowerment, bool ApplyDmgKb)
        {
            empowerment.EmpowermentEffects(proj, Main.player[proj.owner], ApplyDmgKb, false);
        }
        public static bool IsNotAmbientObject(int tileType)
        {
            if (Main.tileSolid[tileType] || Main.tileSolidTop[tileType])
            {
                return true;
            }
            return false;
        }

        public static int ScaleDamage(int origDmg, Player player, DamageClass damageClass)
        {
            return (int)(player.GetTotalDamage(damageClass).ApplyTo(origDmg));
        }

        public static int ScaleShieldEffectPower(int stat, Player player, float effectiveness = 1)
        {
            return (int)(stat * ((player.GetModPlayer<EarthenShield>().ShieldEffectPower - 1) * effectiveness + 1));
        }
        public static float ScaleShieldEffectPower(float stat, Player player, float effectiveness = 1)
        {
            return stat * ((player.GetModPlayer<EarthenShield>().ShieldEffectPower - 1) * effectiveness + 1);
        }
        public static int GetTotalShieldDamage(int stat, Player player, float effectiveness = 1)
        {
            return ScaleDamage(ScaleShieldEffectPower(stat, player, effectiveness), player, ModContent.GetInstance<SentinelDamageClass>());
        }
        public static void DrawCircle(Vector2 center, float radius, Color color, float width, int segments)
        {
            Vector2[] vertex = new Vector2[segments];
            for (int i = 0; i < segments; i++)
            {
                vertex[i] = center + new Vector2(radius, 0).RotatedBy(i * MathHelper.TwoPi / segments);
            }
            DrawPolygon(vertex, segments, color, width);
        }
        public static void DrawPolygon(Vector2[] vertex, int count, Color color, float width)
        {
            if (count > 0)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    DrawLine(vertex[i], vertex[i + 1], color, width);
                }
                DrawLine(vertex[count - 1], vertex[0], color, width);
            }
        }
        public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
        {

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end) / 2f;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("AwfulGarbageMod/Assets/dot", ReLogic.Content.AssetRequestMode.AsyncLoad).Value,
                start, (Rectangle?)null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
        public static int GetTileCounts(int tileType)
        {
            
            int tileCount = 0;
            for (int i = 0; i < (int)((Main.tile.Width * Main.tile.Height)); i++)
            {
                int x = i % Main.tile.Width;
                int y = i / Main.tile.Height;
                if (x < Main.tile.Width && y < Main.tile.Height)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType == tileType)
                    {
                        tileCount++;
                    }
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
        public static readonly int PurityEmpowerment = 4;
        public static readonly int HolyEmpowerment = 5;
        public static readonly int TerraEmpowerment = 6;
        public static readonly int RadiantEmpowerment = 7;
        public static readonly int VenomEmpowerment = 8;
        public static readonly int EctoplasmicEmpowerment = 9;
        public static readonly int LushEmpowerment = 10;
    }

    class NPCandValue : IComparable<NPCandValue>
    {
        public NPC npc { get; set; }
        public float value { get; set; }

        public int CompareTo(NPCandValue other)
        {
            return this.value.CompareTo(other.value);
        }
    }
}