using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public enum STATE {
        PLAYER_TURN,
        ENEMY_TURN
    }

    

    public Tile prefab_tile1;
    public Tile prefab_tile2;

    public UnitBase prefab_unit_peasant;
    public UnitBase prefab_unit_knight;
    public UnitBase prefab_unit_rogue;
    public UnitBase prefab_unit_archer;

    public UnitBase prefab_unit_zombie;
    public UnitBase prefab_unit_fastZombie;
    public UnitBase prefab_unit_zombieArcher;

    public GameObject parent_tiles;
    public GameObject parent_units;

    public int boardSize = 8;
    public Tile[,] boardTiles;
    public UnitBase[,] boardUnits;

    public Button endTurnButton;
    public Text dayText;


    public Fade overlay;

    public STATE state = STATE.PLAYER_TURN;
    private bool animating = false;
    public UnitBase selectedUnit;

    private List<Vector2Int> availableMovePositions;

    public CycleTurnMessage turnMessage;

    void Start() {
        Application.targetFrameRate = 60;

        dayText.text = "Day " + GameManager.GetInstance().levelIndex;

        boardTiles = new Tile[boardSize, boardSize];
        boardUnits = new UnitBase[boardSize, boardSize];

        BuildTiles();
        SpawnUnits();
    }

    public void SpawnUnits() {

        int levelIndex = GameManager.GetInstance().levelIndex;

        int numRogues = GameManager.GetInstance().numRogues;
        int numKnights = GameManager.GetInstance().numKnights;
        int numPeasants = GameManager.GetInstance().numPeasants;
        int numArchers = GameManager.GetInstance().numArchers;

        int numRoguesSpawned = 0;
        int numKnightsSpawned = 0;
        int numPeasantsSpawned = 0;
        int numArchersSpawned = 0;

        Vector2Int p = new Vector2Int(0, 0); ;
        int attempts = 0;

        while (numRoguesSpawned < numRogues && attempts < 64) {
            p.x = Random.Range(0, boardSize);
            p.y = Random.Range(0, boardSize/2);
            if (boardUnits[p.x, p.y] == null) {
                SpawnUnit(prefab_unit_rogue, p.x, p.y);
                numRoguesSpawned++;
            }
            attempts++;
        }

        attempts = 0;
        while (numKnightsSpawned < numKnights && attempts < 64) {
            p.x = Random.Range(0, boardSize);
            p.y = Random.Range(0, boardSize / 2);
            if (boardUnits[p.x, p.y] == null) {
                SpawnUnit(prefab_unit_knight, p.x, p.y);
                numKnightsSpawned++;
            }
            attempts++;
        }

        attempts = 0;
        while (numPeasantsSpawned < numPeasants && attempts < 64) {
            p.x = Random.Range(0, boardSize);
            p.y = Random.Range(0, boardSize / 2);
            if (boardUnits[p.x, p.y] == null) {
                SpawnUnit(prefab_unit_peasant, p.x, p.y);
                numPeasantsSpawned++;
            }
            attempts++;
        }

        attempts = 0;
        while (numArchersSpawned < numArchers && attempts < 64) {
            p.x = Random.Range(0, boardSize);
            p.y = Random.Range(0, boardSize / 2);
            if (boardUnits[p.x, p.y] == null) {
                SpawnUnit(prefab_unit_archer, p.x, p.y);
                numArchersSpawned++;
            }
            attempts++;
        }


        float difficulty = 0;
        float targetDifficulty;
        if (levelIndex == 1) {
            targetDifficulty = 2;
        } else if (levelIndex < 3) {
            targetDifficulty = levelIndex * 2;
        } else if (levelIndex < 6) {
            targetDifficulty = levelIndex * 3;
        } else {
            targetDifficulty = levelIndex * 3.5f;
        }


            attempts = 0;
        while (difficulty < targetDifficulty && attempts < 256) {
            p.x = Random.Range(0, boardSize);
            p.y = boardSize / 2 + Random.Range(0, boardSize / 2);
            if (boardUnits[p.x, p.y] == null) {
                int type = 0;
                if (levelIndex == 1) {
                    type = 0;
                } else if (levelIndex < 3) {
                    type = Random.Range(0, 2);
                } else {
                    type = Random.Range(0, 3);
                }
                
                switch (type) {
                    case 0:
                        SpawnUnit(prefab_unit_zombie, p.x, p.y);
                        difficulty += 1;
                        break;
                    case 1:
                        SpawnUnit(prefab_unit_zombieArcher, p.x, p.y);
                        difficulty += 4;
                        break;
                    case 2:
                        SpawnUnit(prefab_unit_fastZombie, p.x, p.y);
                        difficulty += 5;
                        break;
                }
            }
        }

    }   

    void Update() {
        if (!IsAnimating()) {
            switch (state) {
                case STATE.PLAYER_TURN:
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                        Vector3 touchPosition3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2Int touchPosition = new Vector2Int(Mathf.RoundToInt(touchPosition3d.x), Mathf.RoundToInt(touchPosition3d.y));
                        if (IsOnBoard(touchPosition)) {
                            UnitBase unit = boardUnits[touchPosition.x, touchPosition.y];
                            
                            //we touched a new unit
                            if (unit != null && unit != selectedUnit) {
                                if (unit != selectedUnit) {
                                    if (selectedUnit != null) selectedUnit.SetDown(false);
                                    SelectUnit(unit);
                                }
                                //if unit is in attack units of selected and on the other team
                                //attack
                            
                            //we touched an empty board
                            } else {
                                if (selectedUnit != null && selectedUnit.friendly && !selectedUnit.moved) {
                                    if (touchPosition == selectedUnit.toPosition && selectedUnit.toPosition != selectedUnit.truePosition) {
                                        selectedUnit.SetDown(true);
                                    } else if (availableMovePositions.Contains(touchPosition)) {
                                        selectedUnit.MoveTo(touchPosition);
                                    } else {
                                        selectedUnit.SetDown(false);
                                        DeselectUnit();
                                    }
                                    //if the selected position is in the players move positions
                                    //move to the location
                                } else if (selectedUnit != null && (!selectedUnit.friendly || selectedUnit.moved)) {
                                    selectedUnit.SetDown(false);
                                    DeselectUnit();
                                }
                            }
                        }
                    }
                    break;
                case STATE.ENEMY_TURN:
                    if (!IsAnimating()) {
                        List<UnitBase> availableEnemyUnits = new List<UnitBase>();
                        foreach (UnitBase unit in boardUnits) {
                            if (unit != null && !unit.friendly && !unit.moved) availableEnemyUnits.Add(unit);
                        }

                        if (availableEnemyUnits.Count > 0) {
                            UnitBase enemyUnit = availableEnemyUnits[Random.Range(0, availableEnemyUnits.Count - 1)];
                            SelectUnit(enemyUnit);
                            enemyUnit.GetComponent<UnitEnemy>().TryMove();
                        } else {
                            CycleTurn();
                        }
                    }
                    break;
            }
        }
    }

    public void CycleTurn() {
        if (!animating) {
            foreach (UnitBase unit in boardUnits) {
                if (unit != null) unit.SetMoved(false);
            }

            if (state == STATE.PLAYER_TURN) {
                state = STATE.ENEMY_TURN;
                endTurnButton.interactable = false;
            } else {
                state = STATE.PLAYER_TURN;
                endTurnButton.interactable = true;
            }

            if (!overlay.fading) turnMessage.CycleTurn(state == STATE.PLAYER_TURN);
        }
    }

    public bool PlayerMovesRemain() {
        foreach (UnitBase unit in boardUnits) {
            if (unit != null && unit.friendly) {
                if (!unit.moved) return true;
            }
        }
        return false;
    }

    public void CheckProgress() {
        if (LoseCondition()) {
            overlay.FadeAlphaIn();
            StartCoroutine(GameOver(2.0f));
        }
        if (WinCondition()) {
            overlay.FadeAlphaIn();
            StartCoroutine(NextLevel(2.0f));
        }
    }

    private IEnumerator GameOver(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        GameManager.GameOver();
    }

    private IEnumerator NextLevel(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        GameManager.LoadNextLevel();
    }

    public bool LoseCondition() {
        //if no friendly units exist
        foreach (UnitBase unit in boardUnits) {
            if (unit != null && unit.friendly) return false;
        }
        return true;
    }

    public bool WinCondition() {
        //if no enemy units exist
        foreach (UnitBase unit in boardUnits) {
            if (unit != null && !unit.friendly) return false;
        }
        return true;
    }


    public void SelectUnit(UnitBase unit) {
        DeselectUnit();
        selectedUnit = unit;
        selectedUnit.PickUp();
        availableMovePositions = GetAvailableMovePositions(selectedUnit);

        HighlightMoveTiles(availableMovePositions);
        HighlightAttackTiles(GetAvailableAttackPositions(unit, availableMovePositions));
        //set the stats and stuff
    }
    public void DeselectUnit() {
        UnHighlightTiles();
        selectedUnit = null;
        ClearCrosshairs();
    }

    public void HighlightMoveTiles(List<Vector2Int> tiles) {
        foreach (Vector2Int tile in tiles) {
            boardTiles[tile.x, tile.y].HighlightMove();
        }
    }

    public void HighlightAttackTiles(List<Vector2Int> tiles) {
        foreach (Vector2Int tile in tiles) {
            boardTiles[tile.x, tile.y].HighlightAttack();
        }
    }

    public void UnHighlightTiles() {
        foreach (Tile tile in boardTiles) {
            tile.UnHighlight();
        }
    }

    public List<Vector2Int> GetAvailableMovePositions(UnitBase unit) {
        List<Vector2Int> unitMovePositions = unit.GetAvailableMovePositions();
        List<Vector2Int> tiles = new List<Vector2Int>();

        tiles.Add(unit.truePosition);
        Vector2Int north = unit.truePosition + new Vector2Int(0, 1);
        Vector2Int south = unit.truePosition + new Vector2Int(0, -1);
        Vector2Int east = unit.truePosition + new Vector2Int(1, 0);
        Vector2Int west = unit.truePosition + new Vector2Int(-1, 0);

        RecurseNonBlockedTiles(north, tiles, unitMovePositions);
        RecurseNonBlockedTiles(south, tiles, unitMovePositions);
        RecurseNonBlockedTiles(east, tiles, unitMovePositions);
        RecurseNonBlockedTiles(west, tiles, unitMovePositions);

        return tiles;
    }

   
    public void RecurseNonBlockedTiles(Vector2Int p, List<Vector2Int> tiles, List<Vector2Int> unitMovePositions) {
        if (!tiles.Contains(p) && IsOnBoard(p) && unitMovePositions.Contains(p) && boardUnits[p.x, p.y] == null) {
            tiles.Add(p);

            Vector2Int north = p + new Vector2Int(0, 1);
            Vector2Int south = p + new Vector2Int(0, -1);
            Vector2Int east = p + new Vector2Int(1, 0);
            Vector2Int west = p + new Vector2Int(-1, 0);

            RecurseNonBlockedTiles(north, tiles, unitMovePositions);
            RecurseNonBlockedTiles(south, tiles, unitMovePositions);
            RecurseNonBlockedTiles(east, tiles, unitMovePositions);
            RecurseNonBlockedTiles(west, tiles, unitMovePositions);
        }
    }

    public List<Vector2Int> GetAvailableAttackPositions(UnitBase unit, List<Vector2Int> availableMovePositions) {
        List<Vector2Int> tiles = new List<Vector2Int>();
        switch (unit.attackType) {
            case UnitBase.ATTACK_TYPE.MELEE:
                foreach (Vector2Int p in availableMovePositions) {
                    Vector2Int north = p + new Vector2Int(0, 1);
                    Vector2Int south = p + new Vector2Int(0, -1);
                    Vector2Int east = p + new Vector2Int(1, 0);
                    Vector2Int west = p + new Vector2Int(-1, 0);

                    if (IsOnBoard(north) && !availableMovePositions.Contains(north) && !tiles.Contains(north) && (boardUnits[north.x, north.y] == null || boardUnits[north.x, north.y].friendly != unit.friendly)) tiles.Add(north);
                    if (IsOnBoard(south) && !availableMovePositions.Contains(south) && !tiles.Contains(south) && (boardUnits[south.x, south.y] == null || boardUnits[south.x, south.y].friendly != unit.friendly)) tiles.Add(south);
                    if (IsOnBoard(east) && !availableMovePositions.Contains(east) && !tiles.Contains(east) && (boardUnits[east.x, east.y] == null || boardUnits[east.x, east.y].friendly != unit.friendly)) tiles.Add(east);
                    if (IsOnBoard(west) && !availableMovePositions.Contains(west) && !tiles.Contains(west) && (boardUnits[west.x, west.y] == null || boardUnits[west.x, west.y].friendly != unit.friendly)) tiles.Add(west);
                }
                break;
            case UnitBase.ATTACK_TYPE.BOW:
                foreach (Vector2Int p in availableMovePositions) {
                    Vector2Int north = p + new Vector2Int(0, 1);
                    Vector2Int south = p + new Vector2Int(0, -1);
                    Vector2Int east = p + new Vector2Int(1, 0);
                    Vector2Int west = p + new Vector2Int(-1, 0);

                    while (north.y < boardSize) {
                        if (!availableMovePositions.Contains(north) && !tiles.Contains(north)) {
                            if (boardUnits[north.x, north.y] == null) {
                                tiles.Add(new Vector2Int(north.x, north.y));
                            } else if (boardUnits[north.x, north.y].friendly != unit.friendly) {
                                tiles.Add(new Vector2Int(north.x, north.y));
                                break;
                            } else if (boardUnits[north.x, north.y].friendly == unit.friendly) {
                                break;
                            }
                        }
                        north.y++;
                    }
                    
                    while (south.y >= 0) {
                        if (!availableMovePositions.Contains(south) && !tiles.Contains(south)) {
                            if (boardUnits[south.x, south.y] == null) {
                                tiles.Add(new Vector2Int(south.x, south.y));
                            } else if (boardUnits[south.x, south.y].friendly != unit.friendly) {
                                tiles.Add(new Vector2Int(south.x, south.y));
                                break;
                            } else if (boardUnits[south.x, south.y].friendly == unit.friendly) {
                                break;
                            }
                        }
                        south.y--;
                    }

                    while (east.x < boardSize) {
                        if (!availableMovePositions.Contains(east) && !tiles.Contains(east)) {
                            if (boardUnits[east.x, east.y] == null) {
                                tiles.Add(new Vector2Int(east.x, east.y));
                            } else if (boardUnits[east.x, east.y].friendly != unit.friendly) {
                                tiles.Add(new Vector2Int(east.x, east.y));
                                break;
                            } else if (boardUnits[east.x, east.y].friendly == unit.friendly) {
                                break;
                            }
                        }
                        east.x++;
                    }

                    while (west.x >= 0) {
                        if (!availableMovePositions.Contains(west) && !tiles.Contains(west)) {
                            if (boardUnits[west.x, west.y] == null) {
                                tiles.Add(new Vector2Int(west.x, west.y));
                            } else if (boardUnits[west.x, west.y].friendly != unit.friendly) {
                                tiles.Add(new Vector2Int(west.x, west.y));
                                break;
                            } else if (boardUnits[west.x, west.y].friendly == unit.friendly) {
                                break;
                            }
                        }
                        west.x--;
                    }
                }
                break;
        }
        return tiles;
    }

    public bool IsOnBoard(Vector2Int p) {
        return (p.x >= 0 && p.x < boardSize && p.y >= 0 && p.y < boardSize);
    }

    public bool IsAnimating() {
        if (turnMessage.animating) return true;
        if (boardUnits != null) {
            foreach (UnitBase unit in boardUnits) {
                if (unit != null && unit.IsAnimating()) return true;
            }
        }
        return false;
    }

    public void RemoveUnit(UnitBase unit) {
        boardUnits[unit.truePosition.x, unit.truePosition.y] = null;
        Destroy(unit.gameObject);
        EffectsController.SpawnExplosion(unit.truePosition.x, unit.truePosition.y);
        CheckProgress();
    }

    public void ClearCrosshairs() {
        foreach (UnitBase unit in boardUnits) {
            if (unit != null) unit.SetCrosshair(false);
        }
    }

    public void BuildTiles() {
        for (int x = 0; x < boardSize; x++) {
            for (int y = 0; y < boardSize; y++) {
                Tile tile;
                if ((x + y) % 2 == 0) {
                    tile = (Tile)Instantiate(prefab_tile1, new Vector3(x, y, 0), Quaternion.identity);
                } else {
                    tile = (Tile)Instantiate(prefab_tile2, new Vector3(x, y, 0), Quaternion.identity);
                }
                tile.SetPosition(new Vector2Int(x, y));
                tile.transform.SetParent(parent_tiles.transform);
                boardTiles[x, y] = tile;
            }
        }
    }

    public void SpawnUnit(UnitBase unitPrefab, int x, int y) {
        UnitBase unit = (UnitBase)Instantiate(unitPrefab);
        SetUnit(x, y, unit);
        unit.transform.SetParent(parent_units.transform);
        unit.SetGameController(this);
    }

    public void SetUnit(int x, int y, UnitBase unit) {
        boardUnits[x, y] = unit;
        unit.SetTruePosition(x, y);
    }
}
