using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

    public Image image;

    public bool fading = false;
    private float currentAlpha = 0;

	public void FadeAlphaIn() {
        fading = true;
    }

    void Update() {
        if (fading) {
            float targetAlpha = 1f;
            if (currentAlpha < targetAlpha) {
                currentAlpha += 0.01f * Time.deltaTime * 60;
                UpdateOverlayAlpha();
            } else if (currentAlpha > targetAlpha) {
                currentAlpha = targetAlpha;
                fading = false;
                UpdateOverlayAlpha();
            }
        }
    }

    public void UpdateOverlayAlpha() {
        image.color = new Color(image.color.r, image.color.g, image.color.b, currentAlpha);
    }
}
