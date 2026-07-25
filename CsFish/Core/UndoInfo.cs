using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public struct UndoInfo(int cp, int prevEPS, CastlingRights prevCasltlingRights, int prevHMC)
    {
        public int CapturedPiece { get; init; } = cp;
        public int PreviousEnPassantSquare { get; init; } = prevEPS;
        public CastlingRights PreviousCastlingRights { get; init; } = prevCasltlingRights;
        public int PreviousHalfMoveClock { get; init; } = prevHMC;
    }
}
