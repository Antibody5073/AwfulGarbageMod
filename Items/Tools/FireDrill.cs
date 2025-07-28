using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Tools
{
    // ExampleDrill closely mimics Titanium Drill, except where noted.
    // Of note, this example showcases Item.tileBoost and teaches the basic concepts of a held projectile.
    public class FireDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            // As mentioned in the documentation, IsDrill and IsChainsaw automatically reduce useTime and useAnimation to 60% of what is set in SetDefaults and decrease tileBoost by 1, but only for vanilla items.
            // We set it here despite it doing nothing because it is likely to be used by other mods to provide special effects to drill or chainsaw items globally.
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.MeleeNoSpeed; // ignores melee speed bonuses. There's no need for drill animations to play faster, nor drills to dig faster with melee speed.
            Item.width = 20;
            Item.height = 12;
            // IsDrill/IsChainsaw effects must be applied manually, so 60% or 0.6 times the time of the corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
            // If you decide to copy values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.
            Item.useTime = 3;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(platinum: 3);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item23;
            Item.shoot = ModContent.ProjectileType<FireDrillProj>(); // Create the drill projectile
            Item.shootSpeed = 32f; // Adjusts how far away from the player to hold the projectile
            Item.noMelee = true; // Turns off damage from the item itself, as we have a projectile
            Item.noUseGraphic = true; // Stops the item from drawing in your hands, for the aforementioned reason
            Item.channel = true; // Important as the projectile checks if the player channels

            // tileBoost changes the range of tiles that the item can reach.
            // To match Titanium Drill, we should set this to -1, but we'll set it to 10 blocks of extra range for the sake of an example.
            Item.tileBoost = 1;

            Item.pick = 185; // How strong the drill is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CandesciteBar>(22)
                .AddTile<CandescentCrucible>()
                .Register();
        }
    }
    public class FireDrillProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = -1; // Replace with 20 if you do not want custom code
            Projectile.hide = true; // Hides the projectile, so it will draw in the player's hand when we set the player's heldProj to this one.
        }

        // This code is adapted and simplified from aiStyle 20 to use a different dust and more noises. If you want to use aiStyle 20, you do not need to do any of this.
        // It should be noted that this projectile has no effect on mining and is mostly visual.
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 60;

            // Animation code could go here if the projectile was animated. 

            // Plays a sound every 20 ticks. In aiStyle 20, soundDelay is set to 30 ticks.
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.soundDelay = 20;
            }

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (Main.myPlayer == Projectile.owner)
            {
                // This code must only be ran on the client of the projectile owner
                if (player.channel)
                {
                    float holdoutDistance = player.HeldItem.shootSpeed * Projectile.scale;
                    // Calculate a normalized vector from player to mouse and multiply by holdoutDistance to determine resulting holdoutOffset
                    Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        // This will sync the projectile, most importantly, the velocity.
                        Projectile.netUpdate = true;
                    }

                    // Projectile.velocity acts as a holdoutOffset for held projectiles.
                    Projectile.velocity = holdoutOffset;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.velocity.X > 0f)
            {
                player.ChangeDir(1);
            }
            else if (Projectile.velocity.X < 0f)
            {
                player.ChangeDir(-1);
            }

            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction); // Change the player's direction based on the projectile's own
            player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
            player.SetDummyItemTime(2); // Make sure the player's item time does not change while the projectile is out
            Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            // Gives the drill a slight jiggle
            Projectile.velocity.X *= 1f + Main.rand.Next(-3, 4) * 0.01f;
        }
    }
}