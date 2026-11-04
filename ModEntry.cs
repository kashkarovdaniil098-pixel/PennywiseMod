
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
        private Texture2D[] down = new Texture2D[4];
        private Texture2D[] left = new Texture2D[4];
        private Texture2D[] right = new Texture2D[4];
        private Texture2D[] up = new Texture2D[4];

        private Texture2D idle;
        private Texture2D current;

        private Vector2 position;
        private Vector2 target;

        private bool visible;
        private bool watching;

        private int spawnTimer;
        private int moveTimer;
        private int animationTimer;
        private int watchTimer;
        private int frame;

        private int direction = 0;

        private readonly Random random = new Random();

        public override void Entry(IModHelper helper)
        {
            string folder = "PennywiseNPC/assets/";

            for (int i = 0; i < 4; i++)
            {
                down[i] = helper.ModContent.Load<Texture2D>(
                    folder + "Down" + (i + 1) + ".png");

                left[i] = helper.ModContent.Load<Texture2D>(
                    folder + "Left" + (i + 1) + ".png");

                right[i] = helper.ModContent.Load<Texture2D>(
                    folder + "right" + (i + 1) + ".png");

                up[i] = helper.ModContent.Load<Texture2D>(
                    folder + "up" + (i + 1) + ".png");
            }

            idle = helper.ModContent.Load<Texture2D>(
                folder + "idie2.png");

            current = idle;

            helper.Events.GameLoop.UpdateTicked += OnUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;

            Monitor.Log("Pennywise loaded!", LogLevel.Info);
        }

        private void OnUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            spawnTimer++;

            if (!visible && spawnTimer >= 1800)
            {
                spawnTimer = 0;
                visible = true;
                watching = false;
                moveTimer = 0;
                animationTimer = 0;

                position = Game1.player.Position +
                    new Vector2(
                        random.Next(-250, 251),
                        random.Next(-250, 251));

                ChooseTarget();
            }

            if (!visible)
                return;

            if (watching)
            {
                watchTimer++;

                if (watchTimer >= 240)
                {
                    watching = false;
                    watchTimer = 0;
                    ChooseTarget();
                }

                return;
            }

            moveTimer++;
            animationTimer++;

            if (moveTimer >= 180)
            {
                moveTimer = 0;
                ChooseTarget();
            }

            Vector2 directionVector = target - position;

            if (directionVector.Length() > 5f)
            {
                directionVector.Normalize();

                position += directionVector * 1.2f;

                if (Math.Abs(directionVector.X) >
                    Math.Abs(directionVector.Y))
                {
                    direction = directionVector.X < 0 ? 1 : 2;
                }
                else
                {
                    direction = directionVector.Y < 0 ? 3 : 0;
                }

                if (animationTimer >= 8)
                {
                    animationTimer = 0;
                    frame = (frame + 1) % 4;
                }

                SetCurrentFrame();
            }
            else
            {
                watching = true;
                watchTimer = 0;
                current = idle;
            }

            if (Vector2.Distance(
                Game1.player.Position, position) < 64)
            {
                visible = false;
                spawnTimer = 0;
                watching = false;
            }
        }

        private void SetCurrentFrame()
        {
            switch (direction)
            {
                case 0:
                    current = down[frame];
                    break;

                case 1:
                    current = left[frame];
                    break;

                case 2:
                    current = right[frame];
                    break;

                case 3:
                    current = up[frame];
                    break;
            }
        }

        private void ChooseTarget()
        {
            Vector2 player = Game1.player.Position;

            target = player + new Vector2(
                random.Next(-250, 251),
                random.Next(-250, 251));
        }

        private void OnRenderedWorld(
            object sender,
            RenderedWorldEventArgs e)
        {
            if (!visible || !Context.IsWorldReady)
                return;

            Vector2 screen = Game1.GlobalToLocal(
                Game1.viewport, position);

            e.SpriteBatch.Draw(
                current,
                screen,
                Color.White);
        }
    }
}
