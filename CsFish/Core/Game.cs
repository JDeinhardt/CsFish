using CsFish.Engine;
using CsFish.Helpers;
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
        public List<Move> LegalMoves { get; private set; } = [];

        public Game()
        {
            Board.LoadPositionFromFEN("r2qkb1r/pp2nppp/3p4/2pNN1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 1 0");
            var search = new Search();

            Stopwatch sw = new();
            sw.Start();
            Move best = search.FindBestMove(Board, 4, _moveGenerator);
            sw.Stop();

            Console.WriteLine("Best move from {0} to {1}, took {2} ms", 
                Utils.NotationFromSquare(best.From), 
                Utils.NotationFromSquare(best.To),
                sw.ElapsedMilliseconds);
        }

        public void PrintPerft()
        {
            for (int depht = 1; depht <= 4; depht++)
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
            if (depth == 0) return 1;

            long nodes = 0;
            List<Move> pseudoLegal = new(64);
            List<Move> legal = new(64);
            _moveGenerator.GenerateLegalMoves(b, pseudoLegal, legal);

            for (int i = 0; i < legal.Count; i++)
            {
                Move move = legal[i];
                b.MakeMove(move);
                nodes += Perft(b, depth - 1);
                b.UndoMove(move);
            }
            return nodes;
        }
    }
}
