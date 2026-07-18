using CsFish.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public class Game
    {
        public readonly MoveGenerator _moveGenerator = new();

        public Board Board { get; private set; } = new();
        public List<Move> Moves { get; private set; } = [];

        public List<Move> PseudoLegalMoves { get; private set; } = [];

        public Game()
        {
            PseudoLegalMoves = _moveGenerator.GeneratePseudoLegalMoves(Board);
            Console.WriteLine(PseudoLegalMoves.Count);
        }
    }
}
