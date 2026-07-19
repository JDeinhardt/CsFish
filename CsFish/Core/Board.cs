using CsFish.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public CastlingRights CastlingRights = new(true, true, true, true);

        public Stack<UndoInfo> _history = [];

        public Board()
        {
            //LoadPositionFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 01");
            //LoadPositionFromFEN("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
            //WhiteKingSquare = FindKingSquare(Piece.White);
            //BlackKingSquare = FindKingSquare(Piece.Black);
        }

        public void LoadPositionFromFEN(string fen)
        {
            // position
            string fenBoard = fen.Split(' ')[0];
            int file = 0;
            int rank = 7;

            foreach (char c in fenBoard)
            {
                if (c == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(c))
                        file += (int)char.GetNumericValue(c);
                    else
                    {
                        int color = char.IsUpper(c) ? Piece.White : Piece.Black;
                        int type = Utils.PieceTypeFromSymbol[char.ToLower(c)];
                        Square[rank * 8 + file] = type | color;
                        file++;
                    }
                }
            }

            // color to move
            ColorToMove = fen.Split(' ')[1] == "w" ? Piece.White : Piece.Black;

            // castling rights
            string crfp = fen.Split(' ')[2]; // castling rights fen part (crfp

            CastlingRights = new(crfp.Contains('K'), crfp.Contains('Q'), crfp.Contains('k'), crfp.Contains('q'));

            // en passant
            string ep = fen.Split(' ')[3];
            if (ep == "-")
                EnPassantSquare = -1;
            else
                EnPassantSquare = Utils.SquareFromNotation(ep);

            // update king squares
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

            _history.Push(new UndoInfo(capturedPiece, EnPassantSquare, CastlingRights));

            if (move.Flag == MoveFlag.EnPassant)
                Square[capturedPawnSquare] = Piece.None;

            Square[move.To] = movedPiece;
            Square[move.From] = Piece.None;

            if (IsPromotion(move.Flag))
                Square[move.To] = Piece.Make(PromotionPieceType(move.Flag), Piece.GetColor(movedPiece));

            switch (move.Flag)
            {
                case MoveFlag.CastleWhiteKingSide:
                    Square[5] = Square[7];  // rook h1 → f1
                    Square[7] = Piece.None;
                    break;
                case MoveFlag.CastleWhiteQueenSide:
                    Square[3] = Square[0];  // rook a1 → d1
                    Square[0] = Piece.None;
                    break;
                case MoveFlag.CastleBlackKingSide:
                    Square[61] = Square[63]; // rook h8 → f8
                    Square[63] = Piece.None;
                    break;
                case MoveFlag.CastleBlackQueenSide:
                    Square[59] = Square[56]; // rook a8 → d8
                    Square[56] = Piece.None;
                    break;
            }

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

            if (Piece.IsType(movedPiece, Piece.King))
            {
                if (Piece.IsColor(movedPiece, Piece.White))
                    CastlingRights = new CastlingRights(false, false, CastlingRights.CanBlackCastleKingSide, CastlingRights.CanBlackCastleQueenSide);
                else
                    CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, CastlingRights.CanWhiteCastleQueenSide, false, false);
            }

            if (Piece.IsType(movedPiece, Piece.Rook))
            {
                if (move.From == 0) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, false, CastlingRights.CanBlackCastleKingSide, CastlingRights.CanBlackCastleQueenSide);
                if (move.From == 7) CastlingRights = new CastlingRights(false, CastlingRights.CanWhiteCastleQueenSide, CastlingRights.CanBlackCastleKingSide, CastlingRights.CanBlackCastleQueenSide);
                if (move.From == 56) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, CastlingRights.CanWhiteCastleQueenSide, CastlingRights.CanBlackCastleKingSide, false);
                if (move.From == 63) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, CastlingRights.CanWhiteCastleQueenSide, false, CastlingRights.CanBlackCastleQueenSide);
            }

            if (capturedPiece != Piece.None)
            {
                if (move.To == 0) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, false, CastlingRights.CanBlackCastleKingSide, CastlingRights.CanBlackCastleQueenSide);
                if (move.To == 7) CastlingRights = new CastlingRights(false, CastlingRights.CanWhiteCastleQueenSide, CastlingRights.CanBlackCastleKingSide, CastlingRights.CanBlackCastleQueenSide);
                if (move.To == 56) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, CastlingRights.CanWhiteCastleQueenSide, CastlingRights.CanBlackCastleKingSide, false);
                if (move.To == 63) CastlingRights = new CastlingRights(CastlingRights.CanWhiteCastleKingSide, CastlingRights.CanWhiteCastleQueenSide, false, CastlingRights.CanBlackCastleQueenSide);
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

            switch (move.Flag)
            {
                case MoveFlag.CastleWhiteKingSide:
                    Square[7] = Square[5];  // rook f1 → h1
                    Square[5] = Piece.None;
                    break;
                case MoveFlag.CastleWhiteQueenSide:
                    Square[0] = Square[3];  // rook d1 → a1
                    Square[3] = Piece.None;
                    break;
                case MoveFlag.CastleBlackKingSide:
                    Square[63] = Square[61]; // rook f8 → h8
                    Square[61] = Piece.None;
                    break;
                case MoveFlag.CastleBlackQueenSide:
                    Square[56] = Square[59]; // rook d8 → a8
                    Square[59] = Piece.None;
                    break;
            }

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

            CastlingRights = info.PreviousCastlingRights;
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
