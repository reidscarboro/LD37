using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPeasant : UnitBase {
    public override void ResetMoves() {

    }

    public override void IncrementMoves() {
        SetMoved(true);
    }
}
