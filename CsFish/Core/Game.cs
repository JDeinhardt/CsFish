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
                        ParseGoCommand(line, out int depth, out int timeLeftMs, out int incrementMs);

                        Move best = _search.FindBestMove(Board, timeLeftMs, incrementMs, _moveGenerator, depth);

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
                    case "printboard": // custom debug command
                        Board.PrintBoard();
                        break;
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

        private void ParseGoCommand(string line, out int depth, out int timeLeftMs, out int incrementMs)
        {
            depth = 64;
            timeLeftMs = int.MaxValue;
            incrementMs = 0;

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i];

                switch (token)
                {
                    case "depth":
                        if (i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int d))
                            depth = d;
                        break;

                    case "movetime":
                        if (i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int mt))
                        {
                            timeLeftMs = mt * 40;
                            incrementMs = 0;
                        }
                        break;

                    case "wtime":
                        if (Board.ColorToMove == Piece.White && i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int wt))
                            timeLeftMs = wt;
                        break;

                    case "btime":
                        if (Board.ColorToMove == Piece.Black && i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int bt))
                            timeLeftMs = bt;
                        break;

                    case "winc":
                        if (Board.ColorToMove == Piece.White && i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int wi))
                            incrementMs = wi;
                        break;

                    case "binc":
                        if (Board.ColorToMove == Piece.Black && i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int bi))
                            incrementMs = bi;
                        break;
                }
            }
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
