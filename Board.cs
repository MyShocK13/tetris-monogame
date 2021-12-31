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
        private Queue<int> _next_figures = new Queue<int>();
        private Queue<int> _next_figuresModification = new Queue<int>();
        private Random _random = new Random();
        private int _dynamicFigureNumber;
        private int _dynamicFigureModificationNumber;
        private int _dynamicFigureColor;
        private Vector2 _positionForDynamicFigure;
        private Vector2[] _dynamicFigure = new Vector2[_blocksCountInFigure];

        public Board(Game game, ref Texture2D textures, Rectangle[] rectangles)
            : base(game)
        {
            _spriteBatch = Game.Services.GetService<SpriteBatch>();

            // Textures for blocks
            _textures = textures;

            // Rectangles to draw _figures
            _rectangles = rectangles;

            // Create Tetris board
            _boardFields = new FieldState[_width, _height];
            _boardColor = new int[_width, _height];

            // Create _figures
            CreateFigures();

            _next_figures.Enqueue(_random.Next(7));
            _next_figures.Enqueue(_random.Next(7));
            _next_figures.Enqueue(_random.Next(7));
            _next_figures.Enqueue(_random.Next(7));

            _next_figuresModification.Enqueue(_random.Next(4));
            _next_figuresModification.Enqueue(_random.Next(4));
            _next_figuresModification.Enqueue(_random.Next(4));
            _next_figuresModification.Enqueue(_random.Next(4));
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
                _dynamicFigureNumber = _next_figures.Dequeue();
                _next_figures.Enqueue(_random.Next(7));

                _dynamicFigureModificationNumber = _next_figuresModification.Dequeue();
                _next_figuresModification.Enqueue(_random.Next(4));

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

        public void MoveFigureLeft()
        {
            // Sorting blocks fro dynamic figure to correct moving
            SortingVector2(ref _dynamicFigure, true, _dynamicFigure.GetLowerBound(0), _dynamicFigure.GetUpperBound(0));

            // Check colisions
            for (int i = 0; i < _blocksCountInFigure; i++)
            {
                if ((_dynamicFigure[i].X == 0))
                    return;
                if (_boardFields[(int)_dynamicFigure[i].X - 1, (int)_dynamicFigure[i].Y] == FieldState.Static)
                    return;
            }

            // Move figure on board
            for (int i = 0; i < _blocksCountInFigure; i++)
            {
                _boardFields[(int)_dynamicFigure[i].X - 1, (int)_dynamicFigure[i].Y] = _boardFields[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];
                _boardColor[(int)_dynamicFigure[i].X - 1, (int)_dynamicFigure[i].Y] = _boardColor[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];

                ClearBoardField((int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y);
        
                // Change position for blocks in DynamicFigure
                _dynamicFigure[i].X = _dynamicFigure[i].X - 1;
            }

            // Change position vector
            _positionForDynamicFigure.X--;
        }

        public void MoveFigureRight()
        {
            // Sorting blocks for dynamic figure to correct moving
            SortingVector2(ref _dynamicFigure, true, _dynamicFigure.GetLowerBound(0), _dynamicFigure.GetUpperBound(0));

            // Check colisions
            for (int i = 0; i < _blocksCountInFigure; i++)
            {
                if ((_dynamicFigure[i].X == _width - 1))
                    return;
                if (_boardFields[(int)_dynamicFigure[i].X + 1, (int)_dynamicFigure[i].Y] == FieldState.Static)
                    return;
            }

            // Move figure on board
            for (int i = _blocksCountInFigure - 1; i >= 0; i--)
            {
                _boardFields[(int)_dynamicFigure[i].X + 1, (int)_dynamicFigure[i].Y] = _boardFields[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];
                _boardColor[(int)_dynamicFigure[i].X + 1, (int)_dynamicFigure[i].Y] = _boardColor[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];

                ClearBoardField((int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y);
                
                // Change position for blocks in DynamicFigure
                _dynamicFigure[i].X = _dynamicFigure[i].X + 1;
            }

            // Change position vector
            _positionForDynamicFigure.X++;
        }

        public void MoveFigureDown()
        {
            // Sorting blocks fro dynamic figure to correct moving
            SortingVector2(ref _dynamicFigure, false, _dynamicFigure.GetLowerBound(0), _dynamicFigure.GetUpperBound(0));

            // Check colisions
            for (int i = 0; i < _blocksCountInFigure; i++)
            {
                if ((_dynamicFigure[i].Y == _height - 1))
                {
                    for (int k = 0; k < _blocksCountInFigure; k++)
                        _boardFields[(int)_dynamicFigure[k].X, (int)_dynamicFigure[k].Y] = FieldState.Static;
                    
                    _showNewBlock = true;
                    return;
                }

                if (_boardFields[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y + 1] == FieldState.Static)
                {
                    for (int k = 0; k < _blocksCountInFigure; k++)
                        _boardFields[(int)_dynamicFigure[k].X, (int)_dynamicFigure[k].Y] = FieldState.Static;
                    
                    _showNewBlock = true;
                    return;
                }
            }

            // Move figure on board
            for (int i = _blocksCountInFigure - 1; i >= 0; i--)
            {
                _boardFields[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y + 1] = _boardFields[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];
                _boardColor[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y + 1] = _boardColor[(int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y];

                ClearBoardField((int)_dynamicFigure[i].X, (int)_dynamicFigure[i].Y);

                // Change position for blocks in _dynamicFigure
                _dynamicFigure[i].Y = _dynamicFigure[i].Y + 1;
            }

            // Change position vector
            _positionForDynamicFigure.Y++;
        }

        public void SortingVector2(ref Vector2[] vector, bool sortByX, int a, int b)
        {
            if (a >= b)
                return;

            int i = a;
            for (int j = a; j <= b; j++)
            {
                if (sortByX)
                {
                    if (vector[j].X <= vector[b].X)
                    {
                        Vector2 tempVector = vector[i];
                        vector[i] = vector[j];
                        vector[j] = tempVector;
                        i++;
                    }
                }
                else
                {
                    if (vector[j].Y <= vector[b].Y)
                    {
                        Vector2 tempVector = vector[i];
                        vector[i] = vector[j];
                        vector[j] = tempVector;
                        i++;
                    }
                }
            }

            int c = i - 1;
            SortingVector2(ref vector, sortByX, a, c - 1);
            SortingVector2(ref vector, sortByX, c + 1, b);
        }

        private void CreateFigures()
        {
            // _figures[figure's number, figure's modification, figure's block number] = Vector2
            // At all _figures is 7, every has 4 modifications (for cube all modifications the same)
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

            // I-_figures
            for (int i = 0; i < 4; i += 2)
            {
                _figures[1, i, 0] = new Vector2(0, 0);
                _figures[1, i, 1] = new Vector2(1, 0);
                _figures[1, i, 2] = new Vector2(2, 0);
                _figures[1, i, 3] = new Vector2(3, 0);
                _figures[1, i + 1, 0] = new Vector2(1, 0);
                _figures[1, i + 1, 1] = new Vector2(1, 1);
                _figures[1, i + 1, 2] = new Vector2(1, 2);
                _figures[1, i + 1, 3] = new Vector2(1, 3);
            }

            // J-_figures
            _figures[2, 0, 0] = new Vector2(0, 0);
            _figures[2, 0, 1] = new Vector2(1, 0);
            _figures[2, 0, 2] = new Vector2(2, 0);
            _figures[2, 0, 3] = new Vector2(2, 1);
            _figures[2, 1, 0] = new Vector2(2, 0);
            _figures[2, 1, 1] = new Vector2(2, 1);
            _figures[2, 1, 2] = new Vector2(1, 2);
            _figures[2, 1, 3] = new Vector2(2, 2);
            _figures[2, 2, 0] = new Vector2(0, 0);
            _figures[2, 2, 1] = new Vector2(0, 1);
            _figures[2, 2, 2] = new Vector2(1, 1);
            _figures[2, 2, 3] = new Vector2(2, 1);
            _figures[2, 3, 0] = new Vector2(1, 0);
            _figures[2, 3, 1] = new Vector2(2, 0);
            _figures[2, 3, 2] = new Vector2(1, 1);
            _figures[2, 3, 3] = new Vector2(1, 2);

            // L-_figures
            _figures[3, 0, 0] = new Vector2(0, 0);
            _figures[3, 0, 1] = new Vector2(1, 0);
            _figures[3, 0, 2] = new Vector2(2, 0);
            _figures[3, 0, 3] = new Vector2(0, 1);
            _figures[3, 1, 0] = new Vector2(2, 0);
            _figures[3, 1, 1] = new Vector2(2, 1);
            _figures[3, 1, 2] = new Vector2(1, 0);
            _figures[3, 1, 3] = new Vector2(2, 2);
            _figures[3, 2, 0] = new Vector2(0, 1);
            _figures[3, 2, 1] = new Vector2(1, 1);
            _figures[3, 2, 2] = new Vector2(2, 1);
            _figures[3, 2, 3] = new Vector2(2, 0);
            _figures[3, 3, 0] = new Vector2(1, 0);
            _figures[3, 3, 1] = new Vector2(2, 2);
            _figures[3, 3, 2] = new Vector2(1, 1);
            _figures[3, 3, 3] = new Vector2(1, 2);

            // S-_figures
            for (int i = 0; i < 4; i += 2)
            {
                _figures[4, i, 0] = new Vector2(0, 1);
                _figures[4, i, 1] = new Vector2(1, 1);
                _figures[4, i, 2] = new Vector2(1, 0);
                _figures[4, i, 3] = new Vector2(2, 0);
                _figures[4, i + 1, 0] = new Vector2(1, 0);
                _figures[4, i + 1, 1] = new Vector2(1, 1);
                _figures[4, i + 1, 2] = new Vector2(2, 1);
                _figures[4, i + 1, 3] = new Vector2(2, 2);
            }

            // Z-_figures
            for (int i = 0; i < 4; i += 2)
            {
                _figures[5, i, 0] = new Vector2(0, 0);
                _figures[5, i, 1] = new Vector2(1, 0);
                _figures[5, i, 2] = new Vector2(1, 1);
                _figures[5, i, 3] = new Vector2(2, 1);
                _figures[5, i + 1, 0] = new Vector2(2, 0);
                _figures[5, i + 1, 1] = new Vector2(1, 1);
                _figures[5, i + 1, 2] = new Vector2(2, 1);
                _figures[5, i + 1, 3] = new Vector2(1, 2);
            }

            // T-_figures
            _figures[6, 0, 0] = new Vector2(0, 1);
            _figures[6, 0, 1] = new Vector2(1, 1);
            _figures[6, 0, 2] = new Vector2(2, 1);
            _figures[6, 0, 3] = new Vector2(1, 0);
            _figures[6, 1, 0] = new Vector2(1, 0);
            _figures[6, 1, 1] = new Vector2(1, 1);
            _figures[6, 1, 2] = new Vector2(1, 2);
            _figures[6, 1, 3] = new Vector2(2, 1);
            _figures[6, 2, 0] = new Vector2(0, 0);
            _figures[6, 2, 1] = new Vector2(1, 0);
            _figures[6, 2, 2] = new Vector2(2, 0);
            _figures[6, 2, 3] = new Vector2(1, 1);
            _figures[6, 3, 0] = new Vector2(2, 0);
            _figures[6, 3, 1] = new Vector2(2, 1);
            _figures[6, 3, 2] = new Vector2(2, 2);
            _figures[6, 3, 3] = new Vector2(1, 1);
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
