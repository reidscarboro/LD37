using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitZombie : UnitEnemy {

    public override void TryMove() {
        Vector2Int movePosition = truePosition;
        float scoreToBeat = -10;
        float score;

        //scoring:
        // kills 20
        // damage 15
        // closness 0-9

        //so basically we're going to go through every move that we can and figure out the weights of goodness
        List<Vector2Int> availableMovePositions = gameController.GetAvailableMovePositions(this);

        foreach (Vector2Int p in availableMovePositions) {
            foreach (UnitBase unit in gameController.boardUnits) {
                if (unit != null) {
                    score = 0;

                    //we only care about attacking friendly units
                    if (unit.friendly) {
                        float distanceToUnit = Vector2.Distance(p.ToVector2(), unit.truePosition.ToVector2());

                        //we're right next to a unit
                        if (distanceToUnit < 1.1f) {

                            //if we can kill this unit
                            if (attackDamage >= unit.health) {
                                score = 20;
                            } else {
                                score = 15;
                            }
                        } else {
                            score = 10 - distanceToUnit;
                        }
                    }

                    if (score > scoreToBeat) {
                        scoreToBeat = score;
                        movePosition = p;
                    }
                }
            }
        }

        MoveTo(movePosition);
    }

    public override void TryAttack() {
        gameController.selectedUnit = this;
        UnitBase unitToAttack = null;
        float scoreToBeat = -10;
        float score;

        foreach (UnitBase unit in gameController.boardUnits) {
            if (unit != null) {
                if (unit.friendly) {
                    float distanceToUnit = Vector2.Distance(truePosition.ToVector2(), unit.truePosition.ToVector2());
                    if (distanceToUnit < 1.1f) {
                        if (attackDamage >= unit.health) {
                            score = 20;
                        } else {
                            score = 15;
                        }

                        if (score > scoreToBeat) {
                            scoreToBeat = score;
                            unitToAttack = unit;
                        }
                    }
                }
            }
        }

        if (unitToAttack != null) {
            unitToAttack.Attack();
        }
    }

    public override void ResetMoves() {

    }

    public override void IncrementMoves() {
        SetMoved(true);
    }
}
