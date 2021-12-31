using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    class Board : DrawableGameComponent
    {
        private enum FieldState
        {
            Free,
            Static,
            Dynamic
        }

        private const int _width = 10;
        private const int _height = 20;
        private const int _blocksCountInFigure = 4;
        private readonly Vector2 _startPositionForNewFigure = new Vector2(3, 0);

        private SpriteBatch _spriteBatch;
        private Texture2D _textures;
        private Rectangle[] _rectangles;

        private FieldState[,] _boardFields;
        private int[,] _boardColor;

        private Vector2[,,] _figures;
        private bool _showNewBlock;
        private Queue<int> _nextFigures = new Queue<int>();
        private Queue<int> _nextFiguresModification = new Queue<int>();
        private Random _random = new Random();
        private int _dynamicFigureNumber;
        private int _dynamicFigureModificationNumber;
        private int _dynamicFigureColor;
        private Vector2 _positionForDynamicFigure;
        private Vector2[] _dynamicFigure = new Vector2[_blocksCountInFigure];

        public Board(Game game, ref Texture2D textures, Rectangle[] rectangles)
            : base(game)
        {
            //_spriteBatch = Game.Services.GetService<SpriteBatch>();
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Textures for blocks
            _textures = textures;

            // Rectangles to draw figures
            _rectangles = rectangles;

            // Create Tetris board
            _boardFields = new FieldState[_width, _height];
            _boardColor = new int[_width, _height];

            // Create figures
            CreateFigures();

            _nextFigures.Enqueue(0);
            //_nextFigures.Enqueue(_random.Next(7));
            //_nextFigures.Enqueue(_random.Next(7));
            //_nextFigures.Enqueue(_random.Next(7));
            //_nextFigures.Enqueue(_random.Next(7));

            _nextFiguresModification.Enqueue(_random.Next(4));
            _nextFiguresModification.Enqueue(_random.Next(4));
            _nextFiguresModification.Enqueue(_random.Next(4));
            _nextFiguresModification.Enqueue(_random.Next(4));
        }

        public override void Initialize()
        {
            _showNewBlock = true;

            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    ClearBoardField(i, j);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 startPosition;

            // Drawing the blocks
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    if (_boardFields[i, j] != FieldState.Free)
                    {
                        startPosition = new Vector2((10 + i) * _rectangles[0].Width, (2 + j) * _rectangles[0].Height);
                        _spriteBatch.Draw(_textures, startPosition, _rectangles[_boardColor[i, j]], Color.White);
                    }

            base.Draw(gameTime);
        }

        public void FindDynamicFigure()
        {
            int blockNumberInDynamicFigure = 0;
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    if (_boardFields[i, j] == FieldState.Dynamic)
                        _dynamicFigure[blockNumberInDynamicFigure++] = new Vector2(i, j);
        }

        public bool CreateNewFigure()
        {
            if (_showNewBlock)
            {
                _dynamicFigureNumber = _nextFigures.Dequeue();
                _nextFigures.Enqueue(_random.Next(7));

                _dynamicFigureModificationNumber = _nextFiguresModification.Dequeue();
                _nextFiguresModification.Enqueue(_random.Next(4));

                _dynamicFigureColor = _dynamicFigureNumber;

                _positionForDynamicFigure = _startPositionForNewFigure;

                for (int i = 0; i < _blocksCountInFigure; i++)
                {
                    _dynamicFigure[i] = _figures[_dynamicFigureNumber, _dynamicFigureModificationNumber, i] + _positionForDynamicFigure;
                }

                if (!DrawFigureOnBoard(_dynamicFigure, _dynamicFigureColor))
                    return false;

                _showNewBlock = false;
            }

            return true;
        }

        private void CreateFigures()
        {
            // Figures[figure's number, figure's modification, figure's block number] = Vector2
            // At all figures is 7, every has 4 modifications (for cube all modifications the same)
            // and every figure consists from 4 blocks
            _figures = new Vector2[7, 4, 4];

            // O-figure
            for (int i = 0; i < 4; i++)
            {
                _figures[0, i, 0] = new Vector2(1, 0);
                _figures[0, i, 1] = new Vector2(2, 0);
                _figures[0, i, 2] = new Vector2(1, 1);
                _figures[0, i, 3] = new Vector2(2, 1);
            }
        }

        private void ClearBoardField(int i, int j)
        {
            _boardFields[i, j] = FieldState.Free;
            _boardColor[i, j] = -1;
        }

        private bool DrawFigureOnBoard(Vector2[] position, int color)
        {
            if (!TryPlaceFigureOnBoard(position))
                return false;

            for (int i = 0; i <= position.GetUpperBound(0); i++)
            {
                _boardFields[(int)position[i].X, (int)position[i].Y] = FieldState.Dynamic;
                _boardColor[(int)position[i].X, (int)position[i].Y] = color;
            }

            return true;
        }

        private bool TryPlaceFigureOnBoard(Vector2[] position)
        {
            for (int i = 0; i < position.GetUpperBound(0); i++)
            {
                if ((position[i].X < 0) || (position[i].X >= _width) || (position[i].Y >= _height))
                    return false;
            }

            for (int i = 0; i < position.GetUpperBound(0); i++)
            {
                if (_boardFields[(int)position[i].X, (int)position[i].Y] == FieldState.Static)
                    return false;
            }

            return true;
        }
    }
}
