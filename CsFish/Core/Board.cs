using CsFish.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public class Board
    {
        public int[] Square = new int[64];
        public int ColorToMove = Piece.White;
        public int EnPassantSquare = -1;
        public int WhiteKingSquare = -1;
        public int BlackKingSquare = -1;

        public Stack<UndoInfo> _history = [];

        public Board()
        {
            Square = Utils.LoadPositionFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 01");
            WhiteKingSquare = FindKingSquare(Piece.White);
            BlackKingSquare = FindKingSquare(Piece.Black);
        }

        public void MakeMove(Move move)
        {
            int movedPiece = Square[move.From];
            int capturedPiece = Square[move.To];
            int capturedPawnSquare = -1;

            if (move.Flag == MoveFlag.EnPassant)
            {
                capturedPawnSquare = move.To + (ColorToMove == Piece.White ? -8 : 8);
                capturedPiece = Square[capturedPawnSquare];
            }

            _history.Push(new UndoInfo(capturedPiece, EnPassantSquare));

            if (move.Flag == MoveFlag.EnPassant)
                Square[capturedPawnSquare] = Piece.None;

            Square[move.To] = movedPiece;
            Square[move.From] = Piece.None;

            if (IsPromotion(move.Flag))
                Square[move.To] = Piece.Make(PromotionPieceType(move.Flag), Piece.GetColor(movedPiece));

            EnPassantSquare = (Piece.GetType(movedPiece) == Piece.Pawn && Math.Abs(move.To - move.From) == 16)
                ? (move.From + move.To) / 2
                : -1;

            ColorToMove = ColorToMove == Piece.White ? Piece.Black : Piece.White;

            // update king square
            if(Piece.IsType(movedPiece, Piece.King))
            {
                if (Piece.IsColor(movedPiece, Piece.White))
                    WhiteKingSquare = move.To;
                else
                    BlackKingSquare = move.To;
            }
        }

        public void UndoMove(Move move)
        {
            ColorToMove = ColorToMove == Piece.White ? Piece.Black : Piece.White;

            UndoInfo info = _history.Pop();

            int movedPiece = Square[move.To];

            // Bei Promotion: die Figur war vorher ein Bauer
            if (IsPromotion(move.Flag))
                movedPiece = Piece.Make(Piece.Pawn, ColorToMove);

            Square[move.From] = movedPiece;
            Square[move.To] = Piece.None;

            if (move.Flag == MoveFlag.EnPassant)
            {
                int capturedPawnSquare = move.To + (ColorToMove == Piece.White ? -8 : 8);
                Square[capturedPawnSquare] = info.CapturedPiece;
            }
            else
            {
                Square[move.To] = info.CapturedPiece; // normale Figur zurücksetzen (oder Piece.None)
            }

            EnPassantSquare = info.PreviousEnPassantSquare;

            // King square
            if (Piece.IsType(movedPiece, Piece.King))
            {
                if (Piece.IsColor(movedPiece, Piece.White))
                    WhiteKingSquare = move.From;
                else
                    BlackKingSquare = move.From;
            }
        }

        public int FindKingSquare(int color)
        {
            for (int i = 0; i < 64; i++)
            {
                if (Square[i] == Piece.Make(Piece.King, color))
                    return i;
            }

            throw new InvalidOperationException("König nicht gefunden");
        }
        private static int PromotionPieceType(MoveFlag flag) => flag switch
        {
            MoveFlag.PromoteToQueen => Piece.Queen,
            MoveFlag.PromoteToRook => Piece.Rook,
            MoveFlag.PromoteToBishop => Piece.Bishop,
            MoveFlag.PromoteToKnight => Piece.Knight,
            _ => throw new ArgumentException("Flag ist keine Promotion", nameof(flag))
        };

        private static bool IsPromotion(MoveFlag flag) =>
            flag is MoveFlag.PromoteToQueen
                 or MoveFlag.PromoteToRook
                 or MoveFlag.PromoteToBishop
                 or MoveFlag.PromoteToKnight;

        public void PrintBoard()
        {
            for (int rank = 7; rank >= 0; rank--)
            {
                Console.Write((rank + 1) + " ");

                for (int file = 0; file < 8; file++)
                {
                    int piece = Square[rank * 8 + file];
                    char symbol = Utils.PieceTypeToSymbol[Piece.GetType(piece)];

                    if (Piece.IsColor(piece, Piece.White))
                        symbol = char.ToUpper(symbol);

                    Console.Write(symbol + " ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("  a b c d e f g h");
        }
    }
}
