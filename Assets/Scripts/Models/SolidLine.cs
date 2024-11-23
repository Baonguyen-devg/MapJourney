using System.Collections.Generic;
using UnityEngine;

public enum SolidLineType
{
    Straight,
    Bezier,
}

public class SolidLine : MonoBehaviour
{
    [Header("Commonly components"), Space(6)]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private SolidLineType solidLineType = SolidLineType.Straight;

    public Vector3 StartPosition => startPosition;
    public Vector3 EndPosition => endPosition;
    public SolidLineType SolidLineType => solidLineType;

    [Header("Road mesh generation"), Space(6)]
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float widthRoad = 1f;
    [SerializeField] private float widthCenterLine = 0.2f;
    [SerializeField] private Vector3 startGeneratePosition;
    [SerializeField] private Vector3 endGeneratePosition;
    [SerializeField] private MeshFilter roadMeshFilter;
    [SerializeField] private MeshFilter centerLineMeshFilter;

    public Vector3 StartGeneratePosition => startGeneratePosition;
    public Vector3 EndGeneratePosition => endGeneratePosition;
    public float Spacing => spacing;

    //-------------------------------------------------------------------------------------
    public void CommonlyInit(Vector3 startPosition, Vector3 endPosition, SolidLineType solidLineType = default)
    {
        Debug.Log($"[SolidLine] | CommonlyInit | startPosition: {startPosition}, " +
            $"endPosition {endPosition}, solidLineType {solidLineType}");
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.solidLineType = solidLineType;
    }

    public void MeshGenerateInit(Vector3 startGeneratePosition, Vector3 endGeneratePosition)
    {
        Debug.Log($"[SolidLine] | MeshGenerateInit | startGeneratePosition: " +
            $"{startGeneratePosition}, endGeneratePosition {endGeneratePosition}");
        this.startGeneratePosition = startGeneratePosition;
        this.endGeneratePosition = endGeneratePosition;

        SetRoadActive(true);
        SetCenterLineActive(true);
    }

    public void SetRoadActive(bool status) => roadMeshFilter.gameObject.SetActive(status); 
    public void SetCenterLineActive(bool status) => centerLineMeshFilter.gameObject.SetActive(status); 

    //----------------------------------------------------------------------------------------------
    [ContextMenu("Generate road")]
    public void GenerateStraightRoad()
    {
        List<Vector3> pointPositions = new List<Vector3>() { startGeneratePosition, endGeneratePosition };
        List<Vector3> straightPoints = CreateStraightPoints(startGeneratePosition, endGeneratePosition, spacing);
        
        Debug.Log($"[SolidLine] | GenerateStraightRoad | pointPositions " +
            $"{pointPositions.Count}, straightPoints {straightPoints}");

        Mesh roadMesh = GenerateRoadMesh(straightPoints, widthRoad);
        roadMeshFilter.mesh = roadMesh;

        for (int i = 0; i < straightPoints.Count; i++) straightPoints[i] += new Vector3(0, 0.1f, 0);
        Mesh centerLine = GenerateRoadMesh(straightPoints, widthCenterLine);
        centerLineMeshFilter.mesh = centerLine;
    }

    public void GenerateBezierRoad(List<Vector3> bezierPoints, Vector3 header = default, Vector3 footer = default)
    {
        Debug.Log($"[SolidLine] | GenerateStraightRoad | bezierPoints: {bezierPoints}");
        Mesh roadMesh = GenerateRoadMesh(bezierPoints, widthRoad, header, footer);
        roadMeshFilter.mesh = roadMesh;

        for (int i = 0; i < bezierPoints.Count; i++) bezierPoints[i] += new Vector3(0, 0.1f, 0);
        Mesh centerLine = GenerateRoadMesh(bezierPoints, widthCenterLine, header, footer);
        centerLineMeshFilter.mesh = centerLine;
    }

    private Mesh GenerateRoadMesh(List<Vector3> points, float width, Vector3 header = default, Vector3 footer = default)
    {
        Vector3[] verticals = new Vector3[points.Count * 2];
        Vector2[] uvs = new Vector2[verticals.Length];
     
        int[] triangles = new int[2 * (points.Count - 1) * 3];
        int verticalIndex = 0;
        int trianglesIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 forward = Vector3.zero;
            if (i < points.Count - 1) forward += points[i + 1] - points[i];
            if (i > 0) forward += points[i] - points[i - 1];
            if (i == 0 && header != default) forward = header;
            if (i == points.Count - 1 && footer != default) forward = footer;
            forward.Normalize();

            Vector3 left = Vector3.Cross(forward, Vector3.up).normalized;

            verticals[verticalIndex] = points[i] + left * width / 2;
            verticals[verticalIndex + 1] = points[i] - left * width / 2;

            float completionPercent = i / (float)(points.Count - 1);
            uvs[verticalIndex] = new Vector2(0, completionPercent);
            uvs[verticalIndex + 1] = new Vector2(1, completionPercent);

            if (i < points.Count - 1)
            {
                triangles[trianglesIndex] = verticalIndex;
                triangles[trianglesIndex + 1] = verticalIndex + 2;
                triangles[trianglesIndex + 2] = verticalIndex + 1;

                triangles[trianglesIndex + 3] = verticalIndex + 1;
                triangles[trianglesIndex + 4] = verticalIndex + 2;
                triangles[trianglesIndex + 5] = verticalIndex + 3;
            }
            verticalIndex = verticalIndex + 2;
            trianglesIndex = trianglesIndex + 6;
        }
        return CreateMesh(verticals, uvs, triangles);
    }

    private Mesh CreateMesh(Vector3[] verticals, Vector2[] uvs, int[] triangles)
    {
        Mesh meshGenerated = new Mesh();
        meshGenerated.vertices = verticals;
        meshGenerated.triangles = triangles;
        meshGenerated.uv = uvs;
        return meshGenerated;
    }

    private List<Vector3> CreateStraightPoints(Vector3 start, Vector3 end, float spacing)
    {
        List<Vector3> midPoints = new List<Vector3>();
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int numPoints = Mathf.FloorToInt(distance / spacing);

        midPoints.Add(start + new Vector3(0, 0.1f, 0));
        for (int i = 1; i < numPoints; i++)
        {
            Vector3 newPoint = start + direction * (spacing * i);
            midPoints.Add(newPoint + new Vector3(0, 0.1f, 0));
        }
        midPoints.Add(end + new Vector3(0, 0.1f, 0));
        return midPoints;
    }
}
