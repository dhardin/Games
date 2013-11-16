using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework;

namespace Topgun
{
    class OptionsConfirmation : MessageBoxScreen
    {
        private int select;

        public OptionsConfirmation()
            : base("The game must be restarted to apply changes,\n do you want to restart now?\n                Yes\n                No", false)
        {

        }

        public override void Activate(bool instancePreserved)
        {
            select = 1;
            BackgroundTextureName = @"HUD\MessageBox";

            base.Activate(instancePreserved);
        }

        public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, Engine.GameStateManagerment.InputState input)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            Keys[] pressedKeys = input.CurrentKeyboardStates[0].GetPressedKeys();

            InputAction menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true);
            InputAction menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true);
            InputAction Continue = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.Space, Keys.Enter },
                true);
            InputAction Cancel = new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.Escape },
                true);

            PlayerIndex temp;

            if (menuDown.Evaluate(input, ControllingPlayer, out temp))
            {
                select = (int)MathHelper.Clamp(++select, 1, 2);
            }
            else if (menuUp.Evaluate(input, ControllingPlayer, out temp))
            {
                select = (int)MathHelper.Clamp(--select, 1, 2);
            }

            if (Continue.Evaluate(input, ControllingPlayer, out temp))
            {
                if (select == 1)
                    OptionsConfirmation_Accepted(this, new PlayerIndexEventArgs(temp));

                ExitScreen();
            }
            else if (Cancel.Evaluate(input, ControllingPlayer, out temp))
            {
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Vector2 center = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, ">        <", new Vector2(center.X - 125, center.Y - 50 + 40 * select), Color.Yellow);
            ScreenManager.SpriteBatch.End();
        }

        void OptionsConfirmation_Accepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
            Process.Start("Topgun.exe");
        }

        
    }
}
