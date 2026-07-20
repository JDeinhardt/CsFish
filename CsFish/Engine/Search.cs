using CsFish.Core;
using CsFish.MoveGeneration;
using CsFish.Ressources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CsFish.Engine
{
    public class Search
    {
        private const int Infinity = 1_000_000;
        private const int MateScore = 100_000;

        private Stopwatch _timer = new();
        private long _allocatedTimeMs;
        private bool _isTimeOver;

        private OpeningBook _book = new();

        private TranspositionTable _tt = new();

        private Move _bestMoveFound;

        public Search()
        {
            // load opening book
            string path = Path.Combine(
                AppContext.BaseDirectory,
                "Resources",
                "Book.txt"
            );

            _book.LoadOpeningBook(path);
            _tt.Clear();
        }

        public Move FindBestMove(Board b, int timeLeftMs, int incrementMs, MoveGenerator moveGenerator, int maxDepth = 64)
        {
            // opening book move
            if (_book.TryGetMove(b, out Move bookMove))
            {
                Console.WriteLine($"info string book {bookMove}");
                return bookMove;
            }

            // reset state
            _isTimeOver = false;

            // calculate allocated budget for this move
            _allocatedTimeMs = (timeLeftMs / 40) + (incrementMs / 2);

            // ensure small safety margin
            _allocatedTimeMs = Math.Max(20, Math.Min(_allocatedTimeMs, (long)(timeLeftMs * 0.9)));

            var pseudoLegalMoves = new List<Move>();
            var legalMoves = new List<Move>();
            moveGenerator.GenerateLegalMoves(b, pseudoLegalMoves, legalMoves);

            // if no moves, return immediate default
            if (legalMoves.Count == 0)
                return default;

            // set a fallback move, so no null move returned
            _bestMoveFound = legalMoves[0];

            _timer.Restart();

            // iterative deepening loop
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                int bestScore = -Infinity;
                Move bestMoveCurrentLoop = default;

                //TODO: move ordering here
                //var (ttHit, _, ttBestMove) = _tt.Probe(b.ZobristKey, -Infinity, Infinity, depth);
                //if(ttBestMove.From != 0 || ttBestMove.To != 0)
                //{

                //}

                foreach (var move in legalMoves)
                {
                    // check if time ran out
                    CheckTime();
                    if (_isTimeOver)
                        break;

                    b.MakeMove(move);
                    int score = -NegaMax(b, depth - 1, 1, -Infinity, Infinity, moveGenerator);
                    b.UndoMove(move);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMoveCurrentLoop = move;
                    }
                }

                // dont trust results of mid-depth aborted search
                if (_isTimeOver)
                {
                    break;
                }

                // update best move overall
                _bestMoveFound = bestMoveCurrentLoop;

                // print info to uci
                Console.WriteLine($"info depth {depth} score cp {bestScore} time {_timer.ElapsedMilliseconds} pv {_bestMoveFound}");

                // forced checkmate found
                if (Math.Abs(bestScore) >= MateScore - depth)
                {
                    break;
                }
            }

            _timer.Stop();
            return _bestMoveFound;
        }

        private void CheckTime()
        {
            if (!_isTimeOver && _timer.ElapsedMilliseconds >= _allocatedTimeMs)
            {
                _isTimeOver = true;
            }
        }

        public int NegaMax(Board b, int depth, int ply, int alpha, int beta, MoveGenerator moveGenerator)
        {
            // TODO: maby check after n nodes
            // periodically check if time is up 
            if ((ply & 127) == 0)
                CheckTime();

            if (_isTimeOver)
                return 0; // return dummy value, will get discarted anyway

            if (b.IsDraw())
                return 0;

            // TT PROBE
            var (ttHit, ttScore, ttBestMove) = _tt.Probe(b.ZobristKey, alpha, beta, depth);
            if (ttHit)
                return ttScore; // cutoff from TT


            if (depth == 0)
                return Evaluate.EvaluatePosition(b) * (b.ColorToMove == Piece.White ? 1 : -1);

            var pseudoLegalMoves = new List<Move>();
            var legalMoves = new List<Move>();
            moveGenerator.GenerateLegalMoves(b, pseudoLegalMoves, legalMoves);

            if (legalMoves.Count == 0)
            {
                bool isInCheck = moveGenerator.IsKingInCheck(b, b.ColorToMove);
                return isInCheck ? -MateScore + ply : 0;
            }

            int bestScore = -Infinity;

            Move bestMove = default;

            int originalAlpha = alpha;

            foreach (var move in legalMoves)
            {
                b.MakeMove(move);
                int score = -NegaMax(b, depth - 1, ply + 1, -beta, -alpha, moveGenerator);
                b.UndoMove(move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                    break; // pruning
            }

            if (!_isTimeOver)
            {
                TTFlag flag;
                if (bestScore <= originalAlpha) flag = TTFlag.HigherBound; // Nothing improved alpha
                else if (bestScore >= beta) flag = TTFlag.LowerBound; // Beta-Cutoff
                else flag = TTFlag.Exact;       // Exact Score

                _tt.Store(b.ZobristKey, bestScore, depth, flag, bestMove);
            }

            return bestScore;
        }

        public void ClearTT()
        {
            _tt.Clear();
        }
    }
}