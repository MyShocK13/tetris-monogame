using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class Score : DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        SpriteFont _font;

        int _value;
        int _level;
        int _recordScore = 0;
        string _recordPlayer = "Player 1";

        public int Value
        {
            set { _value = value; }
            get { return _value; }
        }

        public int Level
        {
            set { _level = value; }
            get { return _level; }
        }

        public int RecordScore
        {
            set { _recordScore = value; }
            get { return _recordScore; }
        }

        public string RecordPlayer
        {
            set { _recordPlayer = value; }
            get { return _recordPlayer; }
        }

        public Score(Game game, SpriteFont font)
            : base(game)
        {
            _spriteBatch = Game.Services.GetService<SpriteBatch>();
            _font = font;
        }

        public override void Initialize()
        {
            _value = 0;
            _level = 1;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawString(_font, "Score:\n" + _value + "\nLevel: " + _level, new Vector2(1.5f * 24, 3 * 24), Color.Green);
            _spriteBatch.DrawString(_font, "Record:\n" + _recordPlayer + "\n" + _recordScore, new Vector2(1.5f * 24, 13 * 24), Color.Orange);

            base.Draw(gameTime);
        }
    }
}
