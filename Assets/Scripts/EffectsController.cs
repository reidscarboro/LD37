using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour {

    public static EffectsController instance;

    public DamageIndicator prefab_hit;
    public GameObject prefab_explosion;

    void Awake() {
        instance = this;
    }

    public static void SpawnHit(float x, float y, int damage) {
        DamageIndicator di = (DamageIndicator)Instantiate(instance.prefab_hit, new Vector3(x, y, -2), Quaternion.identity);
        di.SetDamage(damage);
        di.gameObject.SetActive(true);
    }

    public static void SpawnExplosion(float x, float y) {
        Instantiate(instance.prefab_explosion, new Vector3(x, y, -2), Quaternion.identity);
    }
}
