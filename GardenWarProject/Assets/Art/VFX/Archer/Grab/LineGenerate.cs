using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineGenerate : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public int endCapVertices = 5;

    public float lineWidth;
    /*public Color c1;
    public Color c2;*/
    public int vertexCount = 2;
    public Material mat;
    private LineRenderer lineRenderer;
    public Gradient grad;
    public bool isHorizontal = false;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        if(mat != null)
        {
            lineRenderer.material = mat;
        }
        else lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = lineWidth;
        /*lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;*/
        lineRenderer.colorGradient = grad;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCapVertices = endCapVertices;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        if(isHorizontal) lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.positionCount = vertexCount;
        

    }
    
    void Update()
    {
        for(int i = 0; i < vertexCount; i++)
        {
            lineRenderer.SetPosition(i, Vector3.Lerp(startPoint.position, new Vector3(endPoint.position.x, startPoint.position.y - 0.01f, endPoint.position.z),(float)i/(vertexCount-1)));
        }/*
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, new Vector3(endPoint.position.x, startPoint.position.y-0.01f, endPoint.position.z));*/
    }
}
