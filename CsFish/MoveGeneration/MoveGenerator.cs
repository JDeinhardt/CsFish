using CsFish.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.MoveGeneration
{
    public class MoveGenerator
    {
        public readonly int[] DirectionOffsets = [8, -8, 1, -1, 7, -7, 9, -9]; // (and king offsets)
        public readonly int[][] NumSquaresToEdge = new int[64][];

        public readonly int[] KnightOffsets = [-17, -15, -6, 10, 17, 15, 6, -10];

        public MoveGenerator()
        {
            PrecomputeMoveData();
        }

        public List<Move> GenerateLegalMoves(Board b)
        {
            List<Move> pseudoLegalMoves = GeneratePseudoLegalMoves(b);
            List<Move> legal = [];

            int movingColor = b.ColorToMove;
            
            foreach(Move m in pseudoLegalMoves)
            {
                b.MakeMove(m);
                if (!IsKingInCheck(b, movingColor))
                    legal.Add(m);
                b.UndoMove(m);
            }

            return legal;
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

                else if (type == Piece.Knight)
                {
                    moves.AddRange(GenerateKnightMoves(startSquare, b));
                }

                else if (type == Piece.King)
                {
                    moves.AddRange(GenerateKingMoves(startSquare, b));
                }

                else if (type == Piece.Pawn)
                {
                    moves.AddRange(GeneratePawnMoves(startSquare, b, piece));
                }
            }

            return moves;
        }

        private List<Move> GeneratePawnMoves(int startSquare, Board b, int piece)
        {
            List<Move> moves = [];

            bool isWhite = Piece.IsColor(piece, Piece.White);

            int forward = isWhite ? 8 : -8;
            int startRank = isWhite ? 1 : 6;
            int promotionRank = isWhite ? 7 : 0;

            int file = startSquare % 8;
            int rank = startSquare / 8;

            // single step forward
            int oneStep = startSquare + forward;
            if (IsOnBoard(oneStep) && b.Square[oneStep] == Piece.None)
            {
                AddPawnMove(moves, startSquare, oneStep, oneStep / 8 == promotionRank);

                // double push from start rank
                if (rank == startRank)
                {
                    int twoStep = startSquare + forward * 2;
                    if (b.Square[twoStep] == Piece.None)
                        moves.Add(new Move(startSquare, twoStep));
                }
            }

            // diagonal capture
            int[] captureOffsets = isWhite ? [7, 9] : [-7, -9];
            foreach (int offset in captureOffsets)
            {
                int target = startSquare + offset;
                if (!IsOnBoard(target))
                    continue;

                // don't allow warp
                int targetFile = target % 8;
                if (Math.Abs(targetFile - file) != 1)
                    continue;

                int targetPiece = b.Square[target];

                // normal captures
                if (targetPiece != Piece.None && !Piece.IsColor(targetPiece, b.ColorToMove))
                {
                    AddPawnMove(moves, startSquare, target, target / 8 == promotionRank);
                }
                // EnPassante
                else if (target == b.EnPassantSquare)
                {
                    moves.Add(new Move(startSquare, target, MoveFlag.EnPassant));
                }
            }


            return moves;
        }

        private static void AddPawnMove(List<Move> moves, int from, int to, bool isPromotion)
        {
            if (isPromotion)
            {
                moves.Add(new Move(from, to, MoveFlag.PromoteToQueen));
                moves.Add(new Move(from, to, MoveFlag.PromoteToRook));
                moves.Add(new Move(from, to, MoveFlag.PromoteToBishop));
                moves.Add(new Move(from, to, MoveFlag.PromoteToKnight));
            }
            else
            {
                moves.Add(new Move(from, to));
            }
        }

        private List<Move> GenerateKingMoves(int startSquare, Board b)
        {
            List<Move> moves = [];

            for (int dir = 0; dir < 8; dir++)
            {
                // board border reached
                if (NumSquaresToEdge[startSquare][dir] == 0)
                    continue;

                int targetSquare = startSquare + DirectionOffsets[dir];
                int targetPiece = b.Square[targetSquare];

                // blocked by own piece
                if (Piece.IsColor(targetPiece, b.ColorToMove))
                    continue;

                moves.Add(new Move(startSquare, targetSquare));
            }

            return moves;
        }

        private List<Move> GenerateKnightMoves(int startSquare, Board b)
        {
            List<Move> moves = [];

            int startFile = startSquare % 8;
            int startRank = startSquare / 8;

            foreach(int offset in KnightOffsets)
            {
                int targetSquare = startSquare + offset;

                // must be in 0-63 region
                if (targetSquare < 0 || targetSquare > 63)
                    continue;

                int targetFile = targetSquare % 8;
                int targetRank = targetSquare / 8;

                int fileDiff = Math.Abs(targetFile - startFile);
                int rankDiff = Math.Abs(targetRank - startRank);

                // valid knight move is either (1,2) or (2,1) (L-Shape)
                bool isValidKnightShape =
                    (fileDiff == 1 && rankDiff == 2) ||
                    (fileDiff == 2 && rankDiff == 1);

                if (!isValidKnightShape)
                    continue;

                // blocked by own piece
                int targetPiece = b.Square[targetSquare];
                if (Piece.IsColor(targetPiece, b.ColorToMove))
                    continue;

                moves.Add(new Move(startSquare, targetSquare));
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

        public bool IsKingInCheck(Board b, int color)
        {
            int kingSquare = b.FindKingSquare(color);
            int opponentColor = color == Piece.White ? Piece.Black : Piece.White;

            return IsSquareAttacked(b, kingSquare, opponentColor);
        }

        public bool IsSquareAttacked(Board b, int square, int byColor)
        {
            int squareFile = square % 8;
            int squareRank = square / 8;

            // sliding attacks
            for (int dir = 0; dir < 8; dir++)
            {
                for (int n = 0; n < NumSquaresToEdge[square][dir]; n++)
                {
                    int targetSquare = square + DirectionOffsets[dir] * (n + 1);
                    int piece = b.Square[targetSquare];

                    if (piece == Piece.None)
                        continue;

                    if (!Piece.IsColor(piece, byColor))
                        break; // blocked by own piece

                    int type = Piece.GetType(piece);
                    bool isDiagonal = dir >= 4;

                    if (type == Piece.Queen ||
                        (isDiagonal && type == Piece.Bishop) ||
                        (!isDiagonal && type == Piece.Rook))
                    {
                        return true;
                    }

                    // king on neighbour square counts too
                    if (n == 0 && type == Piece.King)
                        return true;

                    break; // every other piece blocks line
                }
            }

            // knight attacks
            foreach (int offset in KnightOffsets)
            {
                int targetSquare = square + offset;
                if (targetSquare < 0 || targetSquare > 63)
                    continue;

                int targetFile = targetSquare % 8;
                int targetRank = targetSquare / 8;
                int fileDiff = Math.Abs(targetFile - squareFile);
                int rankDiff = Math.Abs(targetRank - squareRank);

                bool isKnightShape = (fileDiff == 1 && rankDiff == 2) || (fileDiff == 2 && rankDiff == 1);
                if (!isKnightShape)
                    continue;

                int piece = b.Square[targetSquare];
                if (Piece.IsColor(piece, byColor) && Piece.IsType(piece, Piece.Knight))
                    return true;
            }

            // pawn attacks
            // direction from the attacker (byColor), not the target square
            int[] pawnAttackOffsets = byColor == Piece.White ? [-9, -7] : [9, 7];
            foreach (int offset in pawnAttackOffsets)
            {
                int targetSquare = square + offset;
                if (targetSquare < 0 || targetSquare > 63)
                    continue;

                int targetFile = targetSquare % 8;
                if (Math.Abs(targetFile - squareFile) != 1)
                    continue; // warp protection

                int piece = b.Square[targetSquare];
                if (Piece.IsColor(piece, byColor) && Piece.IsType(piece, Piece.Pawn))
                    return true;
            }

            return false;
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

        private static bool IsOnBoard(int square) => square >= 0 && square <= 63;
    }
}