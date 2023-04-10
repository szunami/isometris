using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShadowShapeLocation : MonoBehaviour
{

    [SerializeField]
    private int a;

    [SerializeField]
    private int b;

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
    //     Handles.Label(transform.position, $"{(a, b)}");
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

    public int A()
    {
        return a;
    }

    public int B()
    {
        return b;
    }

    public void Descend()
    {
        b -= 3;
    }

    public void GoRight()
    {
        a += 3;
    }

    public void GoLeft()
    {
        a -= 3;
    }

    public void SetA(int newA)
    {
        this.a = newA;
    }

    public void SetB(int newB)
    {
        this.b = newB;
    }
}
