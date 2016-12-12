using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public GameObject highlightMove;
    public GameObject highlightAttack;

    public Vector2Int position;

    public void SetPosition(Vector2Int p) {
        SetPosition(p.x, p.y);
    }

    public void SetPosition(int x, int y) {
        if (position == null) {
            position = new Vector2Int(x, y);
        } else {
            position.x = x;
            position.y = y;
        }
        transform.position = new Vector3(position.x, position.y, 1);
    }

    public void HighlightMove() {
        highlightMove.SetActive(true);
    }

    public void HighlightAttack() {
        highlightAttack.SetActive(true);
    }

    public void UnHighlight() {
        highlightMove.SetActive(false);
        highlightAttack.SetActive(false);
    }
}
