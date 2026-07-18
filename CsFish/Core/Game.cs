using CsFish.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            PseudoLegalMoves = _moveGenerator.GenerateLegalMoves(Board);

            for(int depht = 1; depht <= 6; depht++)
            {
                Stopwatch sw = new();
                sw.Start();
                long result = Perft(Board, depht);
                sw.Stop();

                Console.WriteLine("Perft({0}): {1} {2}ms", depht, result, sw.ElapsedMilliseconds);

                sw.Reset();
            }
        }

        public long Perft(Board b, int depth)
        {
            if (depth == 0)
                return 1;

            long nodes = 0;
            List<Move> moves = _moveGenerator.GenerateLegalMoves(b);

            foreach (Move move in moves)
            {
                b.MakeMove(move);
                nodes += Perft(b, depth - 1);
                b.UndoMove(move);
            }

            return nodes;
        }
    }
}
