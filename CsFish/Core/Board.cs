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

        public Board()
        {
            Square = Utils.LoadPositionFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 01");
        }

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
