using UnityEngine;

public class BezierCurve
{
    public Vector3[] points;

    public BezierCurve()
    {
        points = new Vector3[4];
    }

    public BezierCurve(Vector3[] points)
    {
        this.points = points;
    }

    public Vector3 StartPosition => points[0];
    public Vector3 EndPosition => points[3];

    public Vector3 GetSegment(float time)
    {
        time = Mathf.Clamp01(time);
        var mTime = 1 - time;
        return (mTime * mTime * mTime * points[0])
               + (3 * mTime * mTime * time * points[1])
               + (3 * mTime * time * time * points[2])
               + (time * time * time * points[3]);
    }

    public Vector3[] GetSegments(int subdivisions)
    {
        Vector3[] segments = new Vector3[subdivisions];

        for (var i = 0; i < subdivisions; i++)
        {
            var time = (float) i / subdivisions;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}
