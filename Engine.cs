using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public class Engine : Game
    {
        // Graphics
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _background, _textures;
        private readonly Rectangle[] _blocks = new Rectangle[7];

        // Game
        private Board _board;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService<SpriteBatch>(_spriteBatch);

            _background = Content.Load<Texture2D>("background");
            _textures = Content.Load<Texture2D>("tetris");

            _board = new Board(this, ref _textures, _blocks);
            _board.Initialize();
            Components.Add(_board);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Game exit
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Pause
            _board.FindDynamicFigure();
            _board.CreateNewFigure();

            if (keyboardState.IsKeyDown(Keys.A))
                _board.MoveFigureLeft();

            if (keyboardState.IsKeyDown(Keys.D))
                _board.MoveFigureRight();

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
