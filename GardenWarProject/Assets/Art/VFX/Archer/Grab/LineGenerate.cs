using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineGenerate : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float lineWidth;
    public Color c1;
    public Color c2;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
        lineRenderer.numCapVertices = 5;

        

    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
