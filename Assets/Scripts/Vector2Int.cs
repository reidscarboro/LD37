using System.Collections;
using UnityEngine;

public class Vector2Int {

    public int x;
    public int y;

    public Vector2Int(int _x, int _y) {
        x = _x;
        y = _y;
    }

    public static Vector2Int operator +(Vector2Int v1, Vector2Int v2) {
        return new Vector2Int(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2Int operator -(Vector2Int v1, Vector2Int v2) {
        return new Vector2Int(v1.x - v2.x, v1.y - v2.y);
    }

    public static bool operator ==(Vector2Int a, Vector2Int b) {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b)) {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null)) {
            return false;
        }

        // Return true if the fields match:
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b) {
        return !(a == b);
    }

    public override bool Equals(System.Object obj) {
        // If parameter is null return false.
        if (obj == null) {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vector2Int p = obj as Vector2Int;
        if ((System.Object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public bool Equals(Vector2Int p) {
        // If parameter is null return false:
        if ((object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public override int GetHashCode() {
        return x ^ y;
    }

    public override string ToString() {
        return "Vector2Int(" + x.ToString() + ", " + y.ToString() + ")";
    }

    public Vector2 ToVector2() {
        return new Vector2(x, y);
    }
}
