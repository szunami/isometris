using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriangleOffset : MonoBehaviour
{
    [SerializeField]
    private int a;

    [SerializeField]
    private int b;

    [SerializeField]
    private bool twoSided;

    [SerializeField]
    private float lambda = 10f;

    private void OnValidate()
    {
        if (Application.isEditor)
        {
            transform.localPosition = new Vector3(
             (a - b / 2f) / 3f,
             (b * Mathf.Sqrt(3f) / 2f) / 3f,
             transform.localPosition.z
         );
        }
    }

    // void OnDrawGizmos()
    // {
    //     Handles.Label(transform.position, $"{EffectivePosition()}");
    // }

    void Update()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,
            new Vector3(
            (a - b / 2f) / 3,
            (b * Mathf.Sqrt(3) / 2) / 3,
            transform.localPosition.z
        ),
            lambda * Time.deltaTime
        );
    }

    public (int a, int b) EffectivePosition()
    {
        var shapeLocation = GetComponentInParent<ShapeLocation>();
        return (shapeLocation.A() + a, shapeLocation.B() + b);
    }

    public void Rotate()
    {
        Debug.Log($"start: {a}, {b}");

        var x = (a - b / 2f);
        var y = (b * Mathf.Sqrt(3) / 2);

        var v = new Vector3(x, y, 0);
        var q = Quaternion.AngleAxis(120f, Vector3.back) * v;

        Debug.Log($"q: {q}");

        var shapeLocation = GetComponentInParent<ShapeLocation>();
        var newAFloat = q.x + q.y / Mathf.Sqrt(3);
        var newBFloat = 2 * q.y / Mathf.Sqrt(3);

        Debug.Log($"new floats: {newAFloat} {newBFloat}");

        this.a = Mathf.RoundToInt(newAFloat);
        this.b = Mathf.RoundToInt(newBFloat);

        Debug.Log($"end: {a}, {b}");

    }

    public int A()
    {
        return a;
    }

    public int B()
    {
        return b;
    }

    public bool TwoSided()
    {
        return twoSided;
    }

    public void Fall()
    {
        b -= 3;
    }
}
