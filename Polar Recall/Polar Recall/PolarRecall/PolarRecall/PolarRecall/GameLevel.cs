using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Audio;
using Engine.Extensions;
using Engine.Map;
using Engine.TaskManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PolarRecall
{
    class GameLevel
    {
        #region Fields
        Level _level;
        readonly Dictionary<char, Texture2D> _levelTextures = new Dictionary<char, Texture2D>();
        int _tileSize;
        Vector2 _levelSize;
        readonly List<Explosion> _explosions = new List<Explosion>();
        ContentManager _content;

        private const char FloorTile = '.';
        private const char IceBlockTile = '#';
        private const string PresentTiles = "rgby";
        private const string BucketTiles = "RGBY";
        static readonly Color InvalidColor = Color.Transparent;

        private SoundEffectInstance _explosionSound;
        private SoundEffectInstance _changeColorSound;

        private Pathfinder pathfinder;

        float[,] mapLayout;
        private List<BoundingBox> obsticles;

        #endregion

        #region Properties
        public Shayne Shayne { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public Penguin penguin { get; set; }
        public int Score { get; set; }
        public List<BoundingBox> Obsticles
        {
            get { return obsticles; }
        }
        Vector2 oldPosition;
        const float followDistance = 50.0f;
        const float chaseDistance = 100.0f;
        public int NumPresents { get; set; }

        #endregion

        #region Query Functions
        Color GetItemColor(char item)
        {
            if (item == 'r' || item == 'R') return Color.Red;
            if (item == 'g' || item == 'G') return Color.Green;
            if (item == 'b' || item == 'B') return Color.Blue;
            if (item == 'y' || item == 'Y') return Color.Yellow;

            return InvalidColor;
        }
        #endregion

        private Explosion CreateExplosion()
        {
            Explosion newExplosion = null;
            foreach (var explosion in _explosions)
            {
                if (explosion.IsDead)
                {
                    newExplosion = explosion;
                    newExplosion.Reset();
                    break;
                }
            }

            if (newExplosion == null)
            {
                newExplosion = new Explosion();
                newExplosion.LoadContent(_content);
                _explosions.Add(newExplosion);
            }

            return newExplosion;


        }

        public void LoadContent(ContentManager content, string levelFile)
        {
            _content = content;
            _level = new Level(levelFile);
            _levelTextures[' '] = content.Load<Texture2D>("Blank");
            _levelTextures['.'] = content.Load<Texture2D>("Floor");
            _levelTextures['#'] = content.Load<Texture2D>("IceBlock");
            _levelTextures['r'] = content.Load<Texture2D>(@"Presents\Red");
            _levelTextures['g'] = content.Load<Texture2D>(@"Presents\Green");
            _levelTextures['b'] = content.Load<Texture2D>(@"Presents\Blue");
            _levelTextures['y'] = content.Load<Texture2D>(@"Presents\Yellow");
            _levelTextures['R'] = content.Load<Texture2D>(@"Buckets\BKR");
            _levelTextures['G'] = content.Load<Texture2D>(@"Buckets\BKG");
            _levelTextures['B'] = content.Load<Texture2D>(@"Buckets\BKB");
            _levelTextures['Y'] = content.Load<Texture2D>(@"Buckets\BKY");

            _explosionSound = AudioEngine.Instance.GetSoundEffect(@"Sounds\place_boomer");
            _changeColorSound = AudioEngine.Instance.GetSoundEffect(@"Sounds\restore");

            _tileSize = _levelTextures['.'].Width;
            _levelSize.X = _level.Width * _tileSize;
            _levelSize.Y = _level.Height * _tileSize;

            mapLayout = new float[_level.Width * _tileSize, _level.Height * _tileSize];
            obsticles = new List<BoundingBox>();
            //populate map with move penalty values
            //since we currently don't have any kind of move penalty
            //just use 0 for non-passable and 1 for passable
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    char c = _level.TileAt(col, row);
                    switch (c)
                    {
                        //since the map tile is "floor" we set the lowest move penalty
                        case '.':
                            mapLayout[row, col] = 1.0f;
                            break;
                        case 'B':
                            mapLayout[row, col] = 1.0f;
                            break;
                        case 'R':
                            mapLayout[row, col] = 1.0f;
                            break;
                        case 'G':
                            mapLayout[row, col] = 1.0f;
                            break;
                        case 'Y':
                            mapLayout[row, col] = 1.0f;
                            break;

                        //since the map tile is anything but floor, we set the value to "non-passable"
                        default:
                            if (c == 'r' || c == 'g' || c == 'b' || c == 'y')
                                NumPresents++;
                            mapLayout[row, col] = 0.0f;
                            //obsticles.Add(new BoundingSphere(new Vector3(new Vector2(col*_tileSize + _tileSize / 2, row*_tileSize + _tileSize / 2), 0.0f), _tileSize / 2));
                            obsticles.Add(new BoundingBox(new Vector3(new Vector2(col * _tileSize - _tileSize / 2, row * _tileSize - _tileSize / 2), 0.0f), new Vector3(new Vector2((col + 1) * _tileSize + _tileSize / 2, (row + 1) * _tileSize + _tileSize / 2), 0.0f)));
                            break;
                    }
                }
            }

            pathfinder = new Pathfinder(mapLayout, _tileSize, _tileSize);
        }

         //<summary>
         //Used to change a tile in the game level map and pathfinder map
         //</summary>
         //<param name="row"></param>
         //<param name="col"></param>
         //<param name="tileType"></param>
        public void ChangeTile(int row, int col, char tileType)
        {
            float moveValue;
            BoundingBox tempBox;
            tempBox.Min = new Vector3(new Vector2(col * _tileSize - _tileSize / 2, row * _tileSize - _tileSize / 2), 0.0f);
            tempBox.Max = new Vector3(new Vector2((col + 1) * _tileSize + _tileSize / 2, (row + 1) * _tileSize + _tileSize / 2), 0.0f);
           
            switch (tileType)
                    {
                        //since the map tile is "floor" we set the lowest move penalty
                        case '.':
                            moveValue= 1.0f;
                            int index;
                            for (index = 0; index < obsticles.Count; index++)
                            {
                                if (obsticles[index].Max == tempBox.Max && obsticles[index].Min == tempBox.Min)
                                    break;
                            }
                            obsticles.RemoveAt(index);
                            break;
                        //since the map tile is anything but floor, we set the value to "non-passable"
                        default:
                            moveValue = 0.0f;
                            obsticles.Add(tempBox);
                            break;
                    }
             mapLayout[row, col] = moveValue;
             pathfinder.MapGrid = new Map(mapLayout, _tileSize, _tileSize);
        }


        public void Update(GameTime gameTime)
        {
            var tileRect = new Rectangle(0, 0, _tileSize, _tileSize);

            // Check for collisions
            for (int y = 0; y < _level.Width; ++y)
            {
                for (int x = 0; x < _level.Height; ++x)
                {
                    char tile = _level.TileAt(x, y);
                    tileRect.X = x * _tileSize;
                    tileRect.Y = y * _tileSize;
                    Vector2 newPosition;

                    if (tile != FloorTile && Shayne.CollidesWith(tileRect, out newPosition))
                    {
                        if (PresentTiles.Contains(tile) && GetItemColor(tile) == Shayne.Color)
                        {
                            _level.SetTileAt(x, y, FloorTile);
                            ChangeTile(y, x, FloorTile);

                            Explosion exp = CreateExplosion();
                            exp.Position = tileRect.Center.ToVector2();
                            exp.Color = GetItemColor(tile);
                            _explosionSound.Play();
                            NumPresents--;
                            Score += 100;
                        }
                        else if (BucketTiles.Contains(tile))
                        {
                            _level.SetTileAt(x, y, FloorTile);
                            Shayne.Color = GetItemColor(tile);
                            _changeColorSound.Play();

                            // Buckets go away for a few seconds
                            var task = new WaitTask(3000);
                            task.Append(new SetTileTask(_level, x, y, tile));
                            TaskManagerComponent.Instance.Attach(task);
                        }
                        else
                        {
                            Shayne.Position = newPosition;
                        }
                            
                        
                    }
                }
            }

            #region Penguin is Active

            if (penguin.IsActive)
            {
                #region Penguin follow Shayne
                if ((penguin.Position - Shayne.Position).Length() > followDistance)
                {
                    if (!penguin.Detect(obsticles))
                    {
                        penguin.Chase(Shayne.Position);
                        penguin.PathIndex = 0;
                        penguin.UsingPath = false;
                    }
                    //use pathfinding till penguin can find shayne
                    else if (penguin.Detect(obsticles))
                    {
                        if (!penguin.UsingPath)
                        {
                            penguin.Path = pathfinder.FindPath(new Point((int)penguin.Position.X / _tileSize, (int)penguin.Position.Y / _tileSize),
                                                      new Point((int)Shayne.Position.X / _tileSize, (int)Shayne.Position.Y / _tileSize));

                            //apply a catmull-rom spline to path for smoothing.  Will return original path of only 4 points
                            //splinePath = pathfinder.SplineTransform(ref path, 0.1f);
                            penguin.UsingPath = true;
                            oldPosition = Shayne.Position;
                        }
                        if (!oldPosition.Equals(Shayne.Position) || penguin.PathIndex == penguin.Path.Count)
                        {
                            penguin.PathIndex = 0;
                            penguin.UsingPath = false;
                        }
                        else if (penguin.PathIndex < penguin.Path.Count)
                        {
                            //modify you move so you end up in the center of the tiles
                            Vector2 modMove;
                            modMove.X = penguin.Path[penguin.PathIndex].X + (int)(_tileSize * .5);
                            modMove.Y = penguin.Path[penguin.PathIndex].Y + (int)(_tileSize * .5);
                            penguin.Move(modMove, Shayne);
                            if (penguin.Position.Equals(modMove))
                                penguin.PathIndex++;
                        }


                    }
                }
                #endregion
            }

            #endregion

            else if ((Shayne.Position - penguin.Position).Length() < 50.0f)
            {
                penguin.IsActive = true;
            }

            // Keep shayne in the bounds.
            Vector2 shaynePosition = Shayne.Position;
            shaynePosition.X = MathHelper.Clamp(shaynePosition.X, 1, _levelSize.X - 1);
            shaynePosition.Y = MathHelper.Clamp(shaynePosition.Y, 1, _levelSize.Y - 1);
            Shayne.Position = shaynePosition;

            // Update the explosions
            _explosions.ForEach(exp => exp.Update(gameTime));


        }

        public void Draw(DrawContext ctx)
        {
            var shayneTile = new Point((int)Shayne.Position.X / 24, (int)Shayne.Position.Y / 24);
            var penguinTile = new Point((int)penguin.Position.X / 24, (int)penguin.Position.Y / 24);
            Vector2 start = ctx.Offset;
            for (int y = 0; y < _level.Height; y++)
            {
                for (int x = 0; x < _level.Width; x++)
                {
                    char tile = _level.TileAt(x, y);
                    Texture2D texture = _levelTextures[tile];
                    var pos = new Vector2(start.X + x * texture.Width, start.Y + y * texture.Width - 14);

                    // Buckets and ice blocks need the floor drawn underneath them.
                    if (BucketTiles.Contains(tile) || tile == IceBlockTile)
                        ctx.Batch.Draw(_levelTextures[FloorTile], pos, Color.White);

                    ctx.Batch.Draw(texture, pos, Color.White);

                }
                if (y == shayneTile.Y)
                    Shayne.Draw(ctx);
                if (y == penguinTile.Y)
                    penguin.Draw(ctx);
            }

            _explosions.ForEach(exp => exp.Draw(ctx));
        }
    }
}
