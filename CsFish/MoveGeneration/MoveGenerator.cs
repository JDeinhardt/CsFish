using CsFish.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.MoveGeneration
{
    public class MoveGenerator
    {
        public readonly int[] DirectionOffsets = [8, -8, 1, -1, 7, -7, 9, -9];
        public readonly int[][] NumSquaresToEdge = new int[64][];

        public MoveGenerator()
        {
            PrecomputeMoveData();
        }

        public List<Move> GeneratePseudoLegalMoves(Board b)
        {
            List<Move> moves = [];

            for(int startSquare = 0; startSquare < 64; startSquare++)
            {
                int piece = b.Square[startSquare];
                if (!Piece.IsColor(piece, b.ColorToMove))
                    continue;

                int type = Piece.GetType(piece);
                if (type == Piece.Bishop || type == Piece.Rook || type == Piece.Queen)
                {
                    moves.AddRange(GenerateSlidingMoves(startSquare, piece, b));
                }
            }

            return moves;
        }

        private List<Move> GenerateSlidingMoves(int startSquare, int piece, Board b)
        {
            int startDirIndex = Piece.IsType(piece, Piece.Bishop) ? 4 : 0;
            int endDirIndex = Piece.IsType(piece, Piece.Rook) ? 4 : 8;

            List<Move> moves = [];

            for (int dir = startDirIndex; dir < endDirIndex; dir++)
            {
                for (int n = 0; n < NumSquaresToEdge[startSquare][dir]; n++)
                {
                    int targetSquare = startSquare + DirectionOffsets[dir] * (n + 1);
                    int pieceOnTargetSquare = b.Square[targetSquare];

                    // Blocked by own piece, can't move further
                    if (Piece.IsColor(pieceOnTargetSquare, b.ColorToMove))
                    {
                        break;
                    }

                    moves.Add(new Move(startSquare, targetSquare));

                    // Capture opponents piece
                    if (Piece.IsColor(pieceOnTargetSquare, b.ColorToMove == Piece.White ? Piece.Black : Piece.White))
                        break;
                }
            }

            return moves;
        }

        private void PrecomputeMoveData()
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    int numNorth = 7 - rank;
                    int numSouth = rank;
                    int numWest = file;
                    int numEast = 7 - file;

                    int squareIndex = rank * 8 + file;

                    NumSquaresToEdge[squareIndex] = [
                        numNorth,
                        numSouth,
                        numEast,
                        numWest,
                        Math.Min(numNorth, numWest),
                        Math.Min(numSouth, numEast),
                        Math.Min(numNorth, numEast),
                        Math.Min(numSouth, numWest),
                    ];
                }
            }
        }
    }
}