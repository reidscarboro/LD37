using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

    private float lifeCounter = 0;
    private float lifeTime = 0.75f;

    public TextMesh text;
    
    public void SetDamage(int damage) {
        text.text = "-" + damage.ToString();
    }

    void Update() {
        lifeCounter += Time.deltaTime;
        if (lifeCounter > lifeTime) {
            Destroy(gameObject);
        } else {
            float scale = Mathf.Lerp(transform.localScale.x, 1f, Time.deltaTime * 60 * 0.075f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
