using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ShapeController : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> prefabs;

    [SerializeField]
    private GameObject fallingShape;

    [SerializeField]
    private float beat = 1f;

    [SerializeField]
    private GameObject gridWrapper;

    [SerializeField]
    private int maxA;

    [SerializeField]
    private float timer;

    [SerializeField]
    private Score score;

    [SerializeField]
    private AudioSource player;

    [SerializeField]
    private AssetReference descendReference;
    private AudioClip descend;

    [SerializeField]
    private AssetReference rotateReference;
    private AudioClip rotate;

    [SerializeField]
    private AssetReference leftRightReference;
    private AudioClip leftRight;

    [SerializeField]
    private AssetReference clearReference;
    private AudioClip clearClip;

    [SerializeField]
    private AssetReference spawnReference;
    private AudioClip spawnClip;

    void Awake()
    {
        StartCoroutine(loadClips());
    }

    IEnumerator loadClips()
    {
        var task = descendReference.LoadAssetAsync<AudioClip>();
        yield return task;
        descend = task.Result;

        var task2 = rotateReference.LoadAssetAsync<AudioClip>();
        yield return task2;
        rotate = task2.Result;

        task = leftRightReference.LoadAssetAsync<AudioClip>();
        yield return task;
        leftRight = task.Result;

        task = clearReference.LoadAssetAsync<AudioClip>();
        yield return task;
        clearClip = task.Result;

        task = spawnReference.LoadAssetAsync<AudioClip>();
        yield return task;
        spawnClip = task.Result;
    }

    // Update is called once per frame
    void Update()
    {
        var fallingShapeLocation = fallingShape.GetComponent<ShapeLocation>();

        // check for collision / w/ bottom

        if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0f)
        {
            Debug.Log("Pressed vertical");

            foreach (var triangleOffset in fallingShape.GetComponentsInChildren<TriangleOffset>())
            {
                triangleOffset.Rotate();
                player.PlayOneShot(rotate);
            }
        }

        Dictionary<(int, int), GameObject> occupiedTriangles = new Dictionary<(int, int), GameObject>();

        foreach (var shapeLocation in FindObjectsOfType<ShapeLocation>())
        {
            if (shapeLocation.gameObject != fallingShape)
            {
                foreach (var triangleOffset in shapeLocation.GetComponentsInChildren<TriangleOffset>())
                {
                    occupiedTriangles.Add((
                        (shapeLocation.A() + triangleOffset.A()),
                        (shapeLocation.B() + triangleOffset.B())
                        ),
                        triangleOffset.gameObject);
                }
            }
        }
        // Debug.Log($"{occupiedTriangles.Count} occuped triangles:");
        foreach (var occupiedTriangle in occupiedTriangles)
        {
            // Debug.Log($"{occupiedTriangle.Key} {occupiedTriangle.Value}");
        }

        var spawn = false;
        var rightBlocked = false;
        var leftBlocked = false;

        foreach (var triangleOffset in fallingShape.GetComponentsInChildren<TriangleOffset>())
        {
            var a = fallingShapeLocation.A() + triangleOffset.A();
            var b = fallingShapeLocation.B() + triangleOffset.B();

            if (a > maxA)
            {
                rightBlocked = true;
            }
            if (a < 3)
            {
                leftBlocked = true;
            }

            // TODO: horizontal blockage can occur too...

            if (b < 3)
            {
                // spawn check shouldn't happen here, but rather before descending
                spawn = true;
            }

            if (a % 3 == 2) // two sided
            {
                if (
                    occupiedTriangles.ContainsKey((a - 1, b - 2)) ||
                    occupiedTriangles.ContainsKey((a, b - 3))
                      )
                {
                    spawn = true;
                }

                if (occupiedTriangles.ContainsKey((a + 2, b + 1)) ||
                    occupiedTriangles.ContainsKey((a + 3, b))
                )
                {
                    rightBlocked = true;
                }

                if (occupiedTriangles.ContainsKey((a - 1, b + 1)) ||
             occupiedTriangles.ContainsKey((a - 3, b))
         )
                {
                    leftBlocked = true;
                }
            }
            else
            {
                if (
                    occupiedTriangles.ContainsKey((a + 1, b - 2)) ||
                    occupiedTriangles.ContainsKey((a, b - 3))
                )
                {
                    spawn = true;
                }

                if (occupiedTriangles.ContainsKey((a + 1, b - 1)) ||
                     occupiedTriangles.ContainsKey((a + 3, b))
                     )
                {
                    rightBlocked = true;
                }

                if (occupiedTriangles.ContainsKey((a - 2, b - 1)) ||
                occupiedTriangles.ContainsKey((a - 3, b))
                )
                {
                    leftBlocked = true;
                }
            }
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") > 0f && !rightBlocked)
            {
                fallingShapeLocation.GoRight();
                player.PlayOneShot(leftRight);
            }
            if (Input.GetAxis("Horizontal") < 0f && !leftBlocked)
            {
                fallingShapeLocation.GoLeft();
                player.PlayOneShot(leftRight);
            }
        }

        timer += Time.deltaTime;

        if (timer > beat)
        {
            timer -= beat;
            if (spawn)
            {
                handleClears();
                int index = Random.Range(0, prefabs.Count);
                fallingShape = Instantiate(prefabs[index]);
                fallingShape.transform.SetParent(gridWrapper.transform);
                player.PlayOneShot(spawnClip);
            }
            else
            {
                fallingShapeLocation.Descend();
                player.PlayOneShot(descend);
            }
        }
        else if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0f)
        {
            if (spawn)
            {
                handleClears();
                int index = Random.Range(0, prefabs.Count);
                fallingShape = Instantiate(prefabs[index]);
                fallingShape.transform.SetParent(gridWrapper.transform);
                player.PlayOneShot(spawnClip);
            }
            else
            {
                fallingShapeLocation.Descend();
                player.PlayOneShot(descend);
            }
            timer = 0f;
        }
    }

    void handleClears()
    {
        var clearCheck = new Dictionary<(int, int), GameObject>();

        foreach (var shapeLocation in FindObjectsOfType<ShapeLocation>())
        {
            {
                foreach (var triangleOffset in shapeLocation.GetComponentsInChildren<TriangleOffset>())
                {
                    clearCheck.Add((
                        (shapeLocation.A() + triangleOffset.A()),
                        (shapeLocation.B() + triangleOffset.B())
                        ),
                        triangleOffset.gameObject);
                }
            }
        }

        int b = 0;
        while (b < 30)
        {
            var clear = true;
            for (int a = 0; a < 18; a += 3)
            {
                if (!(clearCheck.ContainsKey((a + 1, b + 2)) &&
                    clearCheck.ContainsKey((a + 2, b + 1))))
                {
                    clear = false;
                }
            }

            if (clear)
            {
                Debug.Log($"{b} is clear!");
                score.ClearLine();
                player.PlayOneShot(clearClip);

                for (int a = 0; a < 18; a += 3)
                {
                    Destroy(clearCheck[(a + 1, b + 2)]);
                    clearCheck.Remove((a + 1, b + 2));
                    Destroy(clearCheck[(a + 2, b + 1)]);
                    clearCheck.Remove((a + 2, b + 1));
                }

                for (int newb = b + 3; newb < 30; newb++)
                {
                    for (int a = 0; a < 18; a += 3)
                    {
                        if (clearCheck.ContainsKey((a + 1, newb + 2)))
                        {
                            var tri = clearCheck[(a + 1, newb + 2)];
                            clearCheck.Remove((a + 1, newb + 2));
                            var offset = tri.GetComponent<TriangleOffset>();
                            offset.Fall();
                            clearCheck[offset.EffectivePosition()] = offset.gameObject;
                        }
                        if (clearCheck.ContainsKey((a + 2, newb + 1)))
                        {
                            var tri = clearCheck[(a + 2, newb + 1)];
                            clearCheck.Remove((a + 2, newb + 1));
                            var offset = tri.GetComponent<TriangleOffset>();
                            offset.Fall();
                            clearCheck[offset.EffectivePosition()] = offset.gameObject;

                        }
                    }
                }
                // todo: fall!


            }
            else
            {
                b += 3;
            }
        }
    }
}
