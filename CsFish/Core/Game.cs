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
        public readonly Search _search = new();

        public Board Board { get; private set; } = new();
        public List<Move> Moves { get; private set; } = [];

        public List<Move> PseudoLegalMoves { get; private set; } = [];
        public List<Move> LegalMoves { get; private set; } = [];

        public Game()
        {

        }

        public void Run()
        {
            while (true)
            {
                string? line = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                switch (line)
                {
                    case "uci":
                        // identify engine
                        Console.WriteLine("id name CsFish");
                        Console.WriteLine("id author Jonas Deinhardt");
                        Console.WriteLine("uciok");
                        break;

                    case "isready":
                        Console.WriteLine("readyok");
                        break;

                    case "ucinewgame":
                        Board = new Board(); // reset
                        break;

                    case string s when s.StartsWith("position"):
                        // "position startpos moves e2e4 e7e5"
                        // or "position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
                        Board = new Board();
                        ParsePosition(line); // setup board form fen then play moves
                        break;

                    case string s when s.StartsWith("go"):
                        // "go depth 8" or "go movetime 5000" or "go wtime 300000 btime 300000"
                        int depth = ParseDepth(line); // default 8
                        Move best = _search.FindBestMove(Board, depth, _moveGenerator);
                        string promotion = best.Flag switch
                        {
                            MoveFlag.PromoteToQueen => "q",
                            MoveFlag.PromoteToRook => "r",
                            MoveFlag.PromoteToBishop => "b",
                            MoveFlag.PromoteToKnight => "n",
                            _ => ""
                        };
                        Console.WriteLine($"bestmove {Utils.NotationFromSquare(best.From)}{Utils.NotationFromSquare(best.To)}{promotion}");
                        break;

                    case "quit":
                        return;

                    default:
                        // unknown uci commands can be handled here
                        break;
                }
            }
        }

        private void ParsePosition(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            int moveStartIndex;

            if (parts[1] == "startpos")
            {
                Board.LoadPositionFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                moveStartIndex = 3;
            }
            else if (parts[1] == "fen")
            {
                int movesIndex = Array.IndexOf(parts, "moves");

                string fen;
                if (movesIndex == -1)
                {
                    // Everything after "fen" is the FEN
                    fen = string.Join(" ", parts.Skip(2));
                    moveStartIndex = parts.Length;
                }
                else
                {
                    // FEN is between "fen" and "moves"
                    fen = string.Join(" ", parts.Skip(2).Take(movesIndex - 2));
                    moveStartIndex = movesIndex + 1;
                }

                Board.LoadPositionFromFEN(fen);
            }
            else
            {
                return;
            }

            // Play the moves
            for (int i = moveStartIndex; i < parts.Length; i++)
            {
                Move move = Utils.ParseMove(parts[i]);
                Board.MakeMove(move);
            }
        }

        private int ParseDepth(string line)
        {
            string[] tokens = line.Split(' ');
            for (int i = 1; i < tokens.Length; i++)
            {
                if (tokens[i] == "depth" && i + 1 < tokens.Length)
                    return int.Parse(tokens[i + 1]);
            }
            return 6; // default
        }

        public void FindBestMoveFromFEN()
        {
            Board.LoadPositionFromFEN("r1b1kb1r/pppp1ppp/5q2/4n3/3KP3/2N3PN/PPP4P/R1BQ1B1R b kq - 0 1");
            var search = new Search();

            Stopwatch sw = new();
            sw.Start();
            Move best = search.FindBestMove(Board, 6, _moveGenerator);
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
