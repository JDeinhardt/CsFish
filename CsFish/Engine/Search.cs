using CsFish.Core;
using CsFish.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Engine
{
    public class Search
    {
        private const int Infinity = 1_000_000;
        private const int MateScore = 100_000;

        public Move FindBestMove(Board b, int depth, MoveGenerator moveGenerator)
        {
            var pseudoLegalMoves = new List<Move>();
            var legalMoves = new List<Move>();
            moveGenerator.GenerateLegalMoves(b, pseudoLegalMoves, legalMoves);

            Move bestMove = default;
            int bestScore = -Infinity;

            foreach (var move in legalMoves)
            {
                b.MakeMove(move);
                int score = -NegaMax(b, depth - 1, 1, -Infinity, Infinity, moveGenerator);
                b.UndoMove(move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        public int NegaMax(Board b, int depth, int ply, int alpha, int beta, MoveGenerator moveGenerator)
        {
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

            foreach (var move in legalMoves)
            {
                b.MakeMove(move);
                int score = -NegaMax(b, depth - 1, ply + 1, -beta, -alpha, moveGenerator);
                b.UndoMove(move);

                if (score > bestScore)
                    bestScore = score;

                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                    break; // pruning
            }

            return bestScore;
        }
    }
}
