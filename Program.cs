﻿using System;

namespace Tetris
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Engine();
            game.Run();
        }
    }
}
