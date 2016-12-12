using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitZombieArcher : UnitEnemy {

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
            List<UnitBase> unitsAvailableForAttack = CheckAvailableAttackFromPosition(p);
            foreach (UnitBase unit in gameController.boardUnits) {
                if (unit != null) {
                    score = 0;

                    //we only care about attacking friendly units
                    if (unit.friendly) {
                        float distanceToUnit = Vector2.Distance(p.ToVector2(), unit.truePosition.ToVector2());
                        if (unitsAvailableForAttack.Contains(unit)) {
                            //if we can kill this unit
                            if (attackDamage >= unit.health) {
                                score = 20;
                                score += distanceToUnit / 5f;
                            } else {
                                score = 15;
                                score += distanceToUnit / 5f;
                            }
                        } else {
                            //we want to align ourselves with the target

                            float xDiff = Mathf.Abs(p.x - unit.truePosition.x);
                            float yDiff = Mathf.Abs(p.y - unit.truePosition.y);

                            if (xDiff > yDiff) {
                                score = 14 - yDiff;
                            } else {
                                score = 14 - xDiff;
                            }
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

        List<UnitBase> unitsAvailableForAttack = CheckAvailableAttackFromPosition(toPosition);

        foreach (UnitBase unit in unitsAvailableForAttack) {
            if (unit != null) {
                score = 0;

                //we only care about attacking friendly units
                if (unit.friendly) {
                    //if we can kill this unit
                    if (attackDamage >= unit.health) {
                        score = 20;
                    } else {
                        score = 15;
                    }
                }

                if (score > scoreToBeat) {
                    scoreToBeat = score;
                    unitToAttack = unit;
                }
            }
        }

        if (unitToAttack != null) {
            unitToAttack.Attack();
        }
    }

    public List<UnitBase> CheckAvailableAttackFromPosition(Vector2Int p) {
        List<UnitBase> unitsAvailableForAttack = new List<UnitBase>();

        Vector2Int north = p + new Vector2Int(0, 1);
        Vector2Int south = p + new Vector2Int(0, -1);
        Vector2Int east = p + new Vector2Int(1, 0);
        Vector2Int west = p + new Vector2Int(-1, 0);



        while (north.y < gameController.boardSize) {
            if (gameController.boardUnits[north.x, north.y] != null && gameController.boardUnits[north.x, north.y].friendly) {
                unitsAvailableForAttack.Add(gameController.boardUnits[north.x, north.y]);
                break;
            } else if (gameController.boardUnits[north.x, north.y] != null && gameController.boardUnits[north.x, north.y] != this && !gameController.boardUnits[north.x, north.y].friendly) {
                break;
            }
            north.y++;
        }

        while (south.y >= 0) {
            if (gameController.boardUnits[south.x, south.y] != null && gameController.boardUnits[south.x, south.y].friendly) {
                unitsAvailableForAttack.Add(gameController.boardUnits[south.x, south.y]);
                break;
            } else if (gameController.boardUnits[south.x, south.y] != null && gameController.boardUnits[south.x, south.y] != this && !gameController.boardUnits[south.x, south.y].friendly) {
                break;
            }
            south.y--;
        }

        while (east.x < gameController.boardSize) {
            if (gameController.boardUnits[east.x, east.y] != null && gameController.boardUnits[east.x, east.y].friendly) {
                unitsAvailableForAttack.Add(gameController.boardUnits[east.x, east.y]);
                break;
            } else if (gameController.boardUnits[east.x, east.y] != null && gameController.boardUnits[east.x, east.y] != this && !gameController.boardUnits[east.x, east.y].friendly) {
                break;
            }
            east.x++;
        }

        while (west.x >= 0) {
            if (gameController.boardUnits[west.x, west.y] != null && gameController.boardUnits[west.x, west.y].friendly) {
                unitsAvailableForAttack.Add(gameController.boardUnits[west.x, west.y]);
                break;
            } else if (gameController.boardUnits[west.x, west.y] != null && gameController.boardUnits[west.x, west.y] != this && !gameController.boardUnits[west.x, west.y].friendly) {
                break;
            }
            west.x--;
        }

        return unitsAvailableForAttack;
    }

    public override void ResetMoves() {

    }

    public override void IncrementMoves() {
        SetMoved(true);
    }
}
