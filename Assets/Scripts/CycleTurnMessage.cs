using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleTurnMessage : MonoBehaviour {

    public Vector2 inPos = new Vector2(20, 0);
    public Vector2 midPos = new Vector2(0, 0);
    public Vector2 outPos = new Vector2(-20, 0);

    public TextMesh text;
    public SpriteRenderer overlay;
    public bool animating;

    public float currentAlpha = 0;

    public void CycleTurn(bool player) {
        if (player) {
            text.text = "Player Turn";
        } else {
            text.text = "Enemy Turn";
        }

        animating = true;
        text.transform.localPosition = new Vector3(inPos.x, inPos.y, text.transform.localPosition.z);
        text.gameObject.SetActive(true);
    }

    void Update() {
        if (animating) {
            //termination condition
            if (text.transform.localPosition.x -outPos.x < 0.1f) {
                animating = false;
                text.gameObject.SetActive(false);
            } else {
                if (Mathf.Abs(midPos.x - text.transform.localPosition.x) < 3) {
                    text.transform.localPosition += new Vector3(-0.1f * Time.deltaTime * 60, 0, 0);
                } else {
                    text.transform.localPosition += new Vector3(-1.25f * Time.deltaTime * 60, 0, 0);
                }
            }

            float targetAlpha = 0.75f;
            if (currentAlpha < targetAlpha) {
                currentAlpha += 0.1f * Time.deltaTime * 60;
                UpdateOverlayAlpha();
            } else if (currentAlpha > targetAlpha) {
                currentAlpha = targetAlpha;
                UpdateOverlayAlpha();
            }

        } else {
            if (currentAlpha > 0) {
                currentAlpha -= 0.1f * Time.deltaTime * 60;
                UpdateOverlayAlpha();
            } else if (currentAlpha < 0) {
                currentAlpha = 0;
                UpdateOverlayAlpha();
            }

        }
    }

    public void UpdateOverlayAlpha() {
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, currentAlpha);
    }
}
