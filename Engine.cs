using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Tetris
{
    public class Engine : Game
    {
        // Graphics
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _background, _textures;
        SpriteFont _gameFont;
        private readonly Rectangle[] _blocks = new Rectangle[7];

        // Game
        private Board _board;
        Score _score;
        bool _pause;

        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // O figure
            _blocks[0] = new Rectangle(312, 0, 24, 24);
            // I figure
            _blocks[1] = new Rectangle(0, 24, 24, 24);
            // J figure
            _blocks[2] = new Rectangle(120, 0, 24, 24);
            // L figure
            _blocks[3] = new Rectangle(216, 24, 24, 24);
            // S figure
            _blocks[4] = new Rectangle(48, 96, 24, 24);
            // Z figure
            _blocks[5] = new Rectangle(240, 72, 24, 24);
            // T figure
            _blocks[6] = new Rectangle(144, 96, 24, 24);
        }

        protected override void Initialize()
        {
            Window.Title = "Tetris 2D";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 10.0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService<SpriteBatch>(_spriteBatch);

            _background = Content.Load<Texture2D>("background");
            _textures = Content.Load<Texture2D>("tetris");

            _gameFont = Content.Load<SpriteFont>("font");

            _board = new Board(this, ref _textures, _blocks);
            _board.Initialize();
            Components.Add(_board);

            _score = new Score(this, _gameFont);
            _score.Initialize();
            Components.Add(_score);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Game exit
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Pause
            if (keyboardState.IsKeyDown(Keys.P))
                _pause = !_pause;

            if (!_pause)
            {
                // Find dynamic figure position
                _board.FindDynamicFigure();

                // Increase player score
                int lines = _board.DestroyLines();
                if (lines > 0)
                {
                    _score.Value += (int)((5.0f / 2.0f) * lines * (lines + 3));
                    _board.Speed += 0.005f;
                }

                _score.Level = (int)(10 * _board.Speed);

                if (!_board.CreateNewFigure())
                {
                    return;
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.A))
                        _board.MoveFigureLeft();

                    if (keyboardState.IsKeyDown(Keys.D))
                        _board.MoveFigureRight();

                    if (keyboardState.IsKeyDown(Keys.S))
                        _board.MoveFigureDown();

                    if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space))
                        _board.RotateFigure();

                    // Moving figure
                    if (_board.Movement >= 1)
                    {
                        _board.Movement = 0;
                        _board.MoveFigureDown();
                    }
                    else
                        _board.Movement += _board.Speed;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            base.Draw(gameTime);
            _spriteBatch.End();
        }


    }
}
