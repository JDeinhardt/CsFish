using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public struct UndoInfo(int cp, int prevEPS, CastlingRights prevCasltlingRights)
    {
        public int CapturedPiece { get; init; } = cp;
        public int PreviousEnPassantSquare { get; init; } = prevEPS;
        public CastlingRights PreviousCastlingRights { get; init; } = prevCasltlingRights;

        // TODO: halvmove clock, maby more.
    }
}
