
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System;

namespace PennywiseMod
{
    public class ModEntry : Mod
    {
        private int timer;
        private readonly Random random = new Random();

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdate;
        }

        private void OnUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            timer++;

            if (timer >= 1800)
            {
                timer = 0;
                Monitor.Log("Pennywise is watching you...", LogLevel.Warn);
            }
        }
    }
}
