using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitKnight : UnitBase {
    public override void ResetMoves() {

    }

    public override void IncrementMoves() {
        SetMoved(true);
    }
}
