
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
        private Texture2D texture;
        private Vector2 position;
        private Vector2 target;
        private bool visible;
        private int timer;
        private int appearanceTimer;
        private readonly Random random = new Random();

        public override void Entry(IModHelper helper)
        {
            texture = helper.ModContent.Load<Texture2D>("assets/Pennywise.png");
            helper.Events.GameLoop.UpdateTicked += OnUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
        }

        private void OnUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            timer++;

            if (!visible && timer >= 1800)
            {
                timer = 0;
                visible = true;
                appearanceTimer = 0;

                position = Game1.player.Position +
                    new Vector2(
                        random.Next(-320, 321),
                        random.Next(-320, 321)
                    );

                ChooseTarget();
                Monitor.Log("Pennywise appeared!", LogLevel.Info);
            }

            if (!visible)
                return;

            appearanceTimer++;

            if (appearanceTimer % 180 == 0)
                ChooseTarget();

            Vector2 direction = target - position;

            if (direction.Length() > 4f)
            {
                direction.Normalize();
                position += direction * 1.2f;
            }

            float distance = Vector2.Distance(
                Game1.player.Position, position);

            if (distance < 64 || appearanceTimer >= 900)
            {
                visible = false;
                timer = 0;
            }
        }

        private void ChooseTarget()
        {
            Vector2 playerPosition = Game1.player.Position;

            if (random.Next(100) < 35)
            {
                target = playerPosition;
            }
            else
            {
                target = playerPosition +
                    new Vector2(
                        random.Next(-320, 321),
                        random.Next(-320, 321)
                    );
            }
        }

        private void OnRenderedWorld(
            object sender, RenderedWorldEventArgs e)
        {
            if (!visible || !Context.IsWorldReady)
                return;

            Vector2 screenPosition =
                Game1.GlobalToLocal(Game1.viewport, position);

            e.SpriteBatch.Draw(
                texture,
                screenPosition,
                Color.White
            );
        }
    }
}
