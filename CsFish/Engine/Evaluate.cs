using CsFish.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            bool isEndgame = IsEndgame(b);

            // Count material
            for(int i = 0; i < 64; i++)
            {
                var square = b.Square[i];

                if (square == Piece.None)
                    continue;

                bool isWhite = Piece.IsColor(square, Piece.White);
                int type = Piece.GetType(square);

                int sign = Piece.GetColor(square) == Piece.White ? 1 : -1;

                if (type != Piece.King)
                    score += ValueByPieceType[type] * sign;

                int pstIndex = isWhite ? i : (i ^ 56);

                score += GetPstValue(type, pstIndex, isEndgame) * sign;
            }

            return score;
        }

        private static int GetPstValue(int type, int index, bool isEndgame)
        {
            return type switch
            {
                Piece.Pawn => PieceSquareTables.Pawn[index],
                Piece.Knight => PieceSquareTables.Knight[index],
                Piece.Bishop => PieceSquareTables.Bishop[index],
                Piece.Rook => PieceSquareTables.Rook[index],
                Piece.Queen => PieceSquareTables.Queen[index],
                Piece.King => isEndgame
                    ? PieceSquareTables.KingEndGame[index]
                    : PieceSquareTables.KingMiddleGame[index],
                _ => 0
            };
        }

        public static bool IsEndgame(Board b)
        {
            int whiteMaterial = 0;
            int blackMaterial = 0;

            foreach (int piece in b.Square)
            {
                if (piece == Piece.None)
                    continue;

                int type = Piece.GetType(piece);

                if (type == Piece.Pawn || type == Piece.King)
                    continue;

                if (Piece.IsColor(piece, Piece.White))
                    whiteMaterial += ValueByPieceType[type];
                else
                    blackMaterial += ValueByPieceType[type];
            }

            return whiteMaterial <= 1300 && blackMaterial <= 1300;
        }
    }
}
