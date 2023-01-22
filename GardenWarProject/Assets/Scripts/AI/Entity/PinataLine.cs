using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public partial class Pinata
    {
        [Header("Indicator")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float lineHeight = 1;
        [SerializeField] private bool smoothPath = true;
        
        private Vector3[] initialState = new Vector3[1];
        [SerializeField] private float SmoothingLength = 2f;
        private float smoothingLength = 2f;
        [SerializeField] private int SmoothingSections = 10;
        private int smoothingSections = 10;

        private BezierCurve[] curves;
        
        private NavMeshPath path;
        private Vector3[] pathVectors;

        private bool isOnScreen;

        private void SetupLine()
        {
            path = new NavMeshPath();
        }
        
        private void UpdatePath()
        {
            if (isOnScreen)
            {
                lineRenderer.positionCount = 0;
                return;
            }
            
            NavMesh.CalculatePath(gsm.GetPlayerChampion().position, position,NavMesh.AllAreas,path);

            pathVectors = path.corners;
            
            lineRenderer.positionCount = pathVectors.Length;

            for (int i = 0; i < pathVectors.Length; i++)
            {
                pathVectors[i].y = lineHeight;
            }
            
            lineRenderer.SetPositions(pathVectors);
            
            if (!smoothPath) return;

            GetInitialValues();
            
            if(lineRenderer.positionCount <= 2) return;
            
            SetupCurves();

            SmoothPath();
           

        }

        private void GetInitialValues()
        {
            initialState = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(initialState);

            smoothingLength = SmoothingLength;
            smoothingSections = SmoothingSections;
        }

        private void SetupCurves()
        {
            curves = new BezierCurve[lineRenderer.positionCount - 1];

            for (int i = 0; i < curves.Length; i++)
            {
                curves[i] = new BezierCurve();
            }
            
            if (curves == null || curves.Length != lineRenderer.positionCount - 1)
            {
                curves = new BezierCurve[lineRenderer.positionCount - 1];
                for (int i = 0; i < curves.Length; i++)
                {
                    curves[i] = new BezierCurve();
                }
            }
            
            for (int i = 0; i < curves.Length; i++)
            {
                var point = lineRenderer.GetPosition(i);
                var lastPosition = i == 0 ? lineRenderer.GetPosition(0) : lineRenderer.GetPosition(i - 1);
                var nextPosition = lineRenderer.GetPosition(i + 1);

                var lastDirection = (point - lastPosition).normalized;
                var nextDirection = (nextPosition - point).normalized;

                var startTangent = (lastDirection + nextDirection) * smoothingLength;
                var endTangent = (nextDirection + lastDirection) * -1 * smoothingLength;
                
                curves[i].points[0] = point; // Start Position (P0)
                curves[i].points[1] = point + startTangent; // Start Tangent (P1)
                curves[i].points[2] = nextPosition + endTangent; // End Tangent (P2)
                curves[i].points[3] = nextPosition; // End Position (P3)
            }
        }

        private void SmoothPath()
        {
            lineRenderer.positionCount = curves.Length * smoothingSections;

            int index = 0;

            foreach (var curve in curves)
            {
                var segments = curve.GetSegments(smoothingSections);
                foreach (var segment in segments)
                {
                    lineRenderer.SetPosition(index,segment);
                    index++;
                }
            }

            //smoothingSections = 1;
            //SmoothingLength = 0;
        }
    }
}