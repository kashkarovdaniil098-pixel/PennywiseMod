
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
        private Texture2D left, right, down, idle, watch;
        private Vector2 position, target;
        private bool visible;
        private int timer, moveTimer, watchTimer;
        private readonly Random random = new Random();

        private Texture2D current;

        public override void Entry(IModHelper helper)
        {
            left = helper.ModContent.Load<Texture2D>("assets/Left.png");
            right = helper.ModContent.Load<Texture2D>("assets/Right.png");
            down = helper.ModContent.Load<Texture2D>("assets/Down.png");
            idle = helper.ModContent.Load<Texture2D>("assets/Idle.png");
            watch = helper.ModContent.Load<Texture2D>("assets/Watch.png");

            current = idle;

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
                position = Game1.player.Position +
                    new Vector2(random.Next(-300, 301),
                                random.Next(-300, 301));

                ChooseTarget();
            }

            if (!visible)
                return;

            moveTimer++;

            if (moveTimer >= 180)
            {
                moveTimer = 0;
                ChooseTarget();
            }

            Vector2 direction = target - position;

            if (direction.Length() > 5f)
            {
                direction.Normalize();
                position += direction * 1.2f;

                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                    current = direction.X < 0 ? left : right;
                else
                    current = direction.Y > 0 ? down : idle;
            }
            else
            {
                watchTimer++;

                current = watchTimer % 240 < 120
                    ? idle
                    : watch;
            }

            if (Vector2.Distance(Game1.player.Position, position) < 64)
            {
                visible = false;
                timer = 0;
                watchTimer = 0;
            }
        }

        private void ChooseTarget()
        {
            Vector2 player = Game1.player.Position;

            if (random.Next(100) < 35)
                target = player;
            else
                target = player + new Vector2(
                    random.Next(-300, 301),
                    random.Next(-300, 301));
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!visible || !Context.IsWorldReady)
                return;

            Vector2 screen = Game1.GlobalToLocal(
                Game1.viewport, position);

            e.SpriteBatch.Draw(
                current,
                screen,
                Color.White
            );
        }
    }
}
