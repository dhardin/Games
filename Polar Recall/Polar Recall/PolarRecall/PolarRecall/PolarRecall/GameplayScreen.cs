using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework;
using Engine.TaskManagement;
using Engine.Sprites;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine.Primitives;

namespace PolarRecall
{
    class GameplayScreen : GameScreen
    {
        Shayne shayne = new Shayne();
        Penguin penguin = new Penguin();
        GameLevel _level = new GameLevel();
        Texture2D _background;
        private string _levelFile;
        Task Root = new WaitTask(0);
        PrimitiveBatch primitiveBatch;// = new PrimitiveBatch(ScreenManager.GraphicsDevice;);
        Color tempColor = Color.LawnGreen;
        SpriteFont score;

        public GameplayScreen(string levelFile)
        {
            _levelFile = levelFile;
        }

        public override void Activate(bool instancePreserved)
        {
            shayne.LoadContent(ScreenManager.Game.Content);
            penguin.LoadContent(ScreenManager.Game.Content);
            _level.LoadContent(ScreenManager.Game.Content, _levelFile);
            _level.Shayne = shayne;
            _level.penguin = penguin;

            ChaseTask Search = new ChaseTask(penguin, shayne, (float)Math.PI, penguin.MoveSpeed, penguin.FollowDistance);
            Root.Append(Search);
            primitiveBatch = new PrimitiveBatch(ScreenManager.GraphicsDevice);
            _background = ScreenManager.Game.Content.Load<Texture2D>("wide_playField");

            score = ScreenManager.Game.Content.Load<SpriteFont>("ScoreFont");
            
            base.Activate(instancePreserved);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            Vector2 shaynesDirection = Vector2.Zero;
            if(input.CurrentGamePadStates[0].IsConnected)
                shaynesDirection += input.CurrentGamePadStates[0].ThumbSticks.Left;

            Keys[] pressedKeys = input.CurrentKeyboardStates[0].GetPressedKeys();
            if(pressedKeys.Contains(Keys.A))
                shaynesDirection -= Vector2.UnitX;
            if(pressedKeys.Contains(Keys.D))
                shaynesDirection += Vector2.UnitX;
            if(pressedKeys.Contains(Keys.W))
                shaynesDirection += Vector2.UnitY;
            if(pressedKeys.Contains(Keys.S))
                shaynesDirection -= Vector2.UnitY;

            if (shaynesDirection.Length() > 1.0f)
                shaynesDirection.Normalize();

            shayne.Direction = shaynesDirection;

            base.HandleInput(gameTime, input);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            shayne.Update(gameTime);
            penguin.Update(gameTime);
            _level.Update(gameTime);

            //Root.Next.Update(gameTime.ElapsedGameTime.Milliseconds);

            if (_level.NumPresents.Equals(0))
                base.ExitScreen();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.White);

            DrawContext ctx = new DrawContext();
            ctx.Batch = ScreenManager.SpriteBatch;
            ctx.Blank = ScreenManager.BlankTexture;
            ctx.Device = ScreenManager.GraphicsDevice;
            ctx.Time = gameTime;

            ctx.Batch.Begin();
            ctx.Batch.Draw(_background, Vector2.Zero, Color.White);

            ctx.Batch.DrawString(score, "Score: " + _level.Score, new Vector2(600.0f, 400.0f), Color.Black);

            ctx.Offset = new Vector2(41, 85);
            _level.Draw(ctx);



            // TODO: Draw more stuff. 'cause shayne is boring.
            ctx.Batch.End();

            //Draw bounding boxes for debugging with obsticle avoidance
            //DrawVelocity(penguin, ctx.Offset);
            //DrawBoundingBox(penguin.ObjectBounds, ctx.Offset, penguin.Color);
            //for (int i = 0; i < _level.Obsticles.Count; i++)
            //    DrawBoundingBox(_level.Obsticles[i], ctx.Offset, Color.LawnGreen);

            //Draw game score
            

            base.Draw(gameTime);
        }

        public void DrawVelocity(GameObject o, Vector2 offset)
        {
            Vector2 arrow = new Vector2(o.Position.X + o.Velocity.X - 10, o.Position.Y + o.Velocity.Y - 10);
            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(offset + o.Position, Color.Maroon);
            primitiveBatch.AddVertex(offset + o.Position + (o.Velocity * 30), Color.Maroon);
            primitiveBatch.End();
        }
        public void DrawRotatingBox(RotatingRectangle box, Vector2 offset, Color color)
        {
            primitiveBatch.Begin(PrimitiveType.LineList);

            // from the top left to the top right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.TopLeft.X, box.TopLeft.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.TopRight.X, box.TopRight.Y), color);

            // from the top right to the bottom right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.TopRight.X, box.TopRight.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.BottomRight.X, box.BottomRight.Y), color);

            // from the bottom right to the bottom left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.BottomRight.X, box.BottomRight.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.BottomLeft.X, box.BottomLeft.Y), color);

            // from the bottom left to the top left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.BottomLeft.X, box.BottomLeft.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.TopLeft.X, box.TopLeft.Y), color);

            // and we're done.
            primitiveBatch.End();

        }
        public void DrawBoundingBox(BoundingBox box, Vector2 offset, Color color)
        {

            // tell the primitive batch to start drawing lines
            primitiveBatch.Begin(PrimitiveType.LineList);

            // from the top left to the top right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Min.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Min.Y), color);

            // from the top right to the bottom right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Min.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Max.Y), color);

            // from the bottom right to the bottom left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Max.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Max.Y), color);

            // from the bottom left to the top left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Max.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Min.Y), color);

            // and we're done.
            primitiveBatch.End();
        }
    }
}
