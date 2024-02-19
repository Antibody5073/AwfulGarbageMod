using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class VilethornSpawner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        int counter;
        Vector2 teleportPos;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


        public override void AI()
        {
            if (counter++ % 45 == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -28), ModContent.ProjectileType<VilethornHostileBase>(), 7, 0, Main.myPlayer, 0, 0);
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = true;
                Projectile.position += Projectile.velocity;
                teleportPos = GetTeleportPos();
                Projectile.position = teleportPos - new Vector2(Projectile.width / 2, Projectile.height / 2);
            }
        }
        private Vector2 GetTeleportPos()
        {
            Vector2 teleportPosition = Projectile.Center;

            Tile tile = Framing.GetTileSafely(teleportPosition);
            if (!tile.HasTile || tile.TileType == TileID.CorruptThorns || tile.TileType == TileID.Cactus || tile.TileType == TileID.Trees || TileID.Sets.Grass[tile.TileType])
            {
                for (int j = 0; j < 20; j++)
                {
                    if (tile.HasTile && tile.TileType != TileID.CorruptThorns && tile.TileType != TileID.Cactus && tile.TileType != TileID.Trees && !TileID.Sets.Grass[tile.TileType])
                    {
                        return teleportPosition;
                    }
                    else
                    {
                        teleportPosition.Y += 16;
                        tile = Framing.GetTileSafely(teleportPosition);
                    }
                }
            }
            else
            {
                for (int j = 0; j < 20; j++)
                {
                    if (!tile.HasTile || tile.TileType == TileID.CorruptThorns || tile.TileType == TileID.Cactus || tile.TileType == TileID.Trees || TileID.Sets.Grass[tile.TileType])
                    {
                        return teleportPosition + new Vector2(0, 16);
                    }
                    else
                    {
                        teleportPosition.Y -= 16;
                        tile = Framing.GetTileSafely(teleportPosition);
                    }
                }
            }
            return Projectile.Center;

        }
    }
}