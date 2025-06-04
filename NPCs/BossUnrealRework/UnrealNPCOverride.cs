using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AwfulGarbageMod.NPCs.BossUnrealRework
{
    public abstract class UnrealNPCOverride : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public NPC Npc { get; private set; }
        protected abstract int OverriddenNpc { get; }

        private bool isFirstFrame = true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == OverriddenNpc;
        }
        public sealed override bool PreAI(NPC npc)
        {
            if (!ShouldOverride()) return true;

            // First frame setup
            if (isFirstFrame)
            {
                Npc = npc;
                isFirstFrame = false;
            }

            var runBase = UnrealAI(npc);

            return runBase;
        }
        /// <summary>
        /// The main AI method of unreal mode NPC.
        /// This is where the primary logic takes place.
        /// </summary>
        /// <param name="npc">Use <see cref="Npc"/> instead.</param>
        /// <returns>True to run vanilla AI, false to not. Same as <see cref="PreAI"/></returns>
        public virtual bool UnrealAI(NPC npc)
        {
            return true;
        }
        // Wrapper for PreDraw
        public virtual bool UnrealDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            return true;
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (!ShouldOverride()) return;
        }

        public sealed override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!ShouldOverride()) return true;
            if (isFirstFrame) return true; // Use vanilla rendering if unreal stuff hasn't taken over yet
            return UnrealDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public bool ShouldOverride()
        {
            return DifficultyModes.Difficulty > 0;
        }
    }
}