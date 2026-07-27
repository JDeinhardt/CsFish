using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public struct UndoInfo(int cp, int prevEPS, CastlingRights prevCasltlingRights, int prevHMC, int pwnpm, int pbnpm)
    {
        public int CapturedPiece { get; init; } = cp;
        public int PreviousEnPassantSquare { get; init; } = prevEPS;
        public CastlingRights PreviousCastlingRights { get; init; } = prevCasltlingRights;
        public int PreviousHalfMoveClock { get; init; } = prevHMC;
        public int PreviousWhiteNonPawnMaterial { get; init; } = pwnpm;
        public int PreviousBlackNonPawnMaterial { get; init; } = pbnpm;
    }
}
