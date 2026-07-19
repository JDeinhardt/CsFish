using CsFish.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CsFish.Engine
{
    public static class Evaluate
    {
        public static readonly Dictionary<int, int> ValueByPieceType = new()
        {
            { Piece.Bishop, 300 },
            { Piece.Knight, 300 },
            { Piece.Pawn, 100 },
            { Piece.Rook, 500 },
            { Piece.Queen, 900 }
        };

        public static int EvaluatePosition(Board b)
        {
            int score = 0;

            // Count material
            foreach (var square in b.Square)
            {
                if (square == Piece.None)
                    continue;

                bool isWhite = Piece.IsColor(square, Piece.White);
                int type = Piece.GetType(square);

                int sign = Piece.GetColor(square) == Piece.White ? 1 : -1;

                if (type != Piece.King)
                    score += ValueByPieceType[type] * sign;

                int pstIndex = isWhite ? square : (square ^ 56);

                score += GetPstValue(type, pstIndex, b) * (Piece.GetColor(square) == Piece.White ? 1 : -1);
            }

            return score;
        }

        private static int GetPstValue(int type, int index, Board b)
        {
            return type switch
            {
                Piece.Pawn => PieceSquareTables.Pawn[index],
                Piece.Knight => PieceSquareTables.Knight[index],
                Piece.Bishop => PieceSquareTables.Bishop[index],
                Piece.Rook => PieceSquareTables.Rook[index],
                Piece.Queen => PieceSquareTables.Queen[index],
                Piece.King => IsEndgame(b)
                    ? PieceSquareTables.KingEndGame[index]
                    : PieceSquareTables.KingMiddleGame[index],
                _ => 0
            };
        }

        private static bool IsEndgame(Board b)
        {
            return false;
        }
    }
}
