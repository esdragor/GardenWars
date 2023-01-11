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
    private LineRenderer lineRenderer;
    
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
        lineRenderer.numCapVertices = 5;

        

    }
    
    void Update()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
