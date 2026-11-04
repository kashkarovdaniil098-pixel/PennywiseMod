using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PennywiseMod
{
    public class ModEntry : Mod
    {
        private Texture2D pennywiseTexture;
        private Vector2 pennywisePosition;
        private bool pennywiseVisible;
        private int timer;
        private readonly Random random = new Random();

        public override void Entry(IModHelper helper)
        {
            pennywiseTexture = helper.ModContent.Load<Texture2D>("assets/Pennywise.png");
            helper.Events.GameLoop.UpdateTicked += OnUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
        }

        private void OnUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            timer++;

            if (!pennywiseVisible && timer >= 1800)
            {
                timer = 0;
                pennywiseVisible = true;

                pennywisePosition = Game1.player.Position +
                    new Vector2(random.Next(-256, 257), random.Next(-256, 257));

                Monitor.Log("Pennywise has appeared!", LogLevel.Warn);
            }

            if (pennywiseVisible)
            {
                float distance = Vector2.Distance(
                    Game1.player.Position, pennywisePosition);

                if (distance < 96 || timer >= 900)
                {
                    pennywiseVisible = false;
                    timer = 0;
                }
            }
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!pennywiseVisible || !Context.IsWorldReady)
                return;

            Vector2 screenPosition =
                Game1.GlobalToLocal(Game1.viewport, pennywisePosition);

            e.SpriteBatch.Draw(
                pennywiseTexture,
                screenPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                1f
            );
        }
    }
}
