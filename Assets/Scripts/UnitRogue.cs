using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRogue : UnitBase {

    private int moves = 2;

    public override void ResetMoves() {
        moves = 2;
    }

    public override void IncrementMoves() {
        moves--;
        if (moves <= 0) {
            SetMoved(true);
        }
    }
}
