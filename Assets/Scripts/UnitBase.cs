using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour {

    public enum ATTACK_TYPE {
        MELEE,
        BOW
    }

    private SpriteRenderer sr;
    public TextMesh healthText;
    public TextMesh attackDamageText;
    public GameObject buttons;
    public GameObject crosshair;

    public Vector2Int truePosition;
    public bool friendly;
    public bool moved;

    public int moveRadius = 1;
    public int maxHealth = 10;
    public int attackDamage = 5;
    public int health;
    public ATTACK_TYPE attackType;

    protected List<Vector2Int> movePositions;

    private Vector2Int boardPosition;
    public Vector2Int toPosition;
    private float toScale = 1;

    private bool translating = false;
    private bool scaling = false;
    private bool attacking = false;

    private float attackTime = 1.0f;
    private float attackCounter = 0;

    protected GameController gameController;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        BuildMovePositionsFromRadius(moveRadius);
        health = maxHealth;
        UpdateHealthDamage();
    }

    void Update() {
        float deltaTime = Time.deltaTime * 60;
        if (translating) {
            if (Vector2.Distance(transform.position, toPosition.ToVector2()) < 0.04f) {
                translating = false;
                transform.position = toPosition.ToVector2();
                if (this == gameController.selectedUnit) TriggerStop();
            } else {
                transform.position = Vector2.Lerp(transform.position, toPosition.ToVector2(), deltaTime * 0.1f);
            }
        }
        if (scaling) {
            if (Mathf.Abs(transform.localScale.x - toScale) < 0.025f) {
                scaling = false;
                SetScale(toScale);
            } else {
                SetScale(Mathf.Lerp(transform.localScale.x, toScale, 0.2f * deltaTime));
            }
        }
        if (attacking) {
            attackCounter += Time.deltaTime;
            if (attackCounter > attackTime) {
                attackCounter = 0;
                attacking = false;
            }
        }
    }

    public void SetScale(float scale) {
        transform.localScale = new Vector3(scale, scale, scale);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1 - scale);
    }

    public void SetTruePosition(Vector2Int p) {
        SetTruePosition(p.x, p.y);
    }

    public void SetTruePosition(int x, int y) {
        if (truePosition == null) {
            truePosition = new Vector2Int(x, y);
        } else {
            truePosition.x = x;
            truePosition.y = y;
        }
        transform.position = new Vector3(truePosition.x, truePosition.y, 0);
    }

    public void MoveTo(Vector2Int p) {
        gameController.ClearCrosshairs();
        SetButtons(false);
        toPosition = p;
        translating = true;
    }

    public void PickUp() {
        toScale = 1.25f;
        scaling = true;
        toPosition = truePosition;
        if (!moved) CheckAvailableAttack();
    }

    public void SetDown(bool newPosition) {
        toScale = 1f;
        scaling = true;
        if (newPosition) {
            gameController.boardUnits[truePosition.x, truePosition.y] = null;
            truePosition = toPosition;
            gameController.boardUnits[truePosition.x, truePosition.y] = this;
            gameController.DeselectUnit();
            IncrementMoves();
            if (gameController.state == GameController.STATE.PLAYER_TURN && !gameController.PlayerMovesRemain()) {
                gameController.CycleTurn();
            }
        } else {
            MoveTo(truePosition);
            gameController.DeselectUnit();
        }
        SetButtons(false);
    }

    public List<Vector2Int> GetAvailableMovePositions() {
        List<Vector2Int> movePositionsOut = new List<Vector2Int>();
        foreach (Vector2Int movePosition in movePositions) {
            movePositionsOut.Add(movePosition + truePosition);
        }
        return movePositionsOut;
    }

    protected void BuildMovePositionsFromRadius(int moveRadius) {
        movePositions = new List<Vector2Int>();
        for (int x = -moveRadius; x <= moveRadius; x++) {
            for (int y = -moveRadius; y <= moveRadius; y++) {
                if (Vector2.Distance(new Vector2(x, y), Vector2.zero) <= moveRadius) {
                    movePositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public bool IsAnimating() {
        return (scaling || translating || attacking);
    }

    public void UpdateHealthDamage() {
        healthText.text = health.ToString();
        attackDamageText.text = attackDamage.ToString();
    }

    public void SetButtons(bool active) {
        buttons.SetActive(active);
    }

    public void SetGameController(GameController _gameController) {
        gameController = _gameController;
    }

    public void TriggerStop() {
        SetButtons(true);
        CheckAvailableAttack();

        UnitEnemy unitEnemy = GetComponent<UnitEnemy>();
        if (unitEnemy != null) {
            SetDown(true);
            unitEnemy.TryAttack();
        }
    }

    public void CheckAvailableAttack() {
        if (attackType == ATTACK_TYPE.MELEE) {
            foreach (UnitBase unit in gameController.boardUnits) {
                if (unit != null && unit != this && (unit.friendly != friendly)) {
                    if (Vector2.Distance(toPosition.ToVector2(), unit.truePosition.ToVector2()) < 1.1f) {
                        unit.SetCrosshair(true);
                    }
                }
            }
        } else if (attackType == ATTACK_TYPE.BOW) {
            Vector2Int north = toPosition + new Vector2Int(0, 1);
            Vector2Int south = toPosition + new Vector2Int(0, -1);
            Vector2Int east = toPosition + new Vector2Int(1, 0);
            Vector2Int west = toPosition + new Vector2Int(-1, 0);

            while (north.y < gameController.boardSize) {
                if (gameController.boardUnits[north.x, north.y] != null && (friendly != gameController.boardUnits[north.x, north.y].friendly)) {
                    gameController.boardUnits[north.x, north.y].SetCrosshair(true);
                    break;
                } else if (gameController.boardUnits[north.x, north.y] != null && gameController.boardUnits[north.x, north.y] != this && (friendly == gameController.boardUnits[north.x, north.y].friendly)) {
                    break;
                }
                north.y++;
            }

            while (south.y >= 0) {
                if (gameController.boardUnits[south.x, south.y] != null && (friendly != gameController.boardUnits[south.x, south.y].friendly)) {
                    gameController.boardUnits[south.x, south.y].SetCrosshair(true);
                    break;
                } else if (gameController.boardUnits[south.x, south.y] != null && gameController.boardUnits[south.x, south.y] != this && (friendly == gameController.boardUnits[south.x, south.y].friendly)) {
                    break;
                }
                south.y--;
            }

            while (east.x < gameController.boardSize) {
                if (gameController.boardUnits[east.x, east.y] != null && (friendly != gameController.boardUnits[east.x, east.y].friendly)) {
                    gameController.boardUnits[east.x, east.y].SetCrosshair(true);
                    break;
                } else if (gameController.boardUnits[east.x, east.y] != null && gameController.boardUnits[east.x, east.y] != this && (friendly == gameController.boardUnits[east.x, east.y].friendly)) {
                    break;
                }
                east.x++;
            }

            while (west.x >= 0) {
                if (gameController.boardUnits[west.x, west.y] != null && (friendly != gameController.boardUnits[west.x, west.y].friendly)) {
                    gameController.boardUnits[west.x, west.y].SetCrosshair(true);
                    break;
                } else if (gameController.boardUnits[west.x, west.y] != null && gameController.boardUnits[west.x, west.y] != this && (friendly == gameController.boardUnits[west.x, west.y].friendly)) {
                    break;
                }
                west.x--;
            }
        }
    }

    public void SetCrosshair(bool active) {
        if (!friendly) crosshair.SetActive(active);
    }

    public void Attack() {
        UnitBase attacker = gameController.selectedUnit;
        attacker.attacking = true;

        gameController.ClearCrosshairs();

        health -= attacker.attackDamage;
        UpdateHealthDamage();
        if (health <= 0) gameController.RemoveUnit(this);
        EffectsController.SpawnHit(truePosition.x, truePosition.y + 0.5f, attacker.attackDamage);

        attacker.SetDown(true);
    }

    public void SetMoved(bool _moved) {
        moved = _moved;
        if (moved) {
            sr.color = new Color(0.5f, 0.5f, 0.5f);
        } else {
            sr.color = new Color(1f, 1f, 1f);
            ResetMoves();
        }
    }

    public virtual void ResetMoves() {

    }

    public virtual void IncrementMoves() {

    }
}
