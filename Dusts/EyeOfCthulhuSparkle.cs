using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace AwfulGarbageMod.Dusts
{
    public class EyeOfCthulhuSparkle : ModDust
    {
        int frame = 0;
        int frameCounter = 0;
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
            dust.noGravity = true; // Makes the dust have no gravity.
            dust.noLight = true; // Makes the dust emit no light.
            dust.scale *= 1.5f; // Multiplies the dust's initial scale by 1.5.
            dust.frame = new Rectangle(0, 0, 14, 14);
        }

        public override bool Update(Dust dust)
        { // Calls every frame the dust is active
            dust.position += dust.velocity;

            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);
            
            if (frameCounter++ >= 5)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 4)
                {
                    dust.active = false;
                    frame = 0;
                }
            }
            dust.frame = new Rectangle(0, 14 * frame, 14, 14);

            return false; // Return false to prevent vanilla behavior.
        }
    }
}