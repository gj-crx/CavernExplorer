using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestMesh : MonoBehaviour
{
    public GameObject TestObjectToSpawn;
    public MeshFilter meshFilter; 
    public LayerMask Mask;

    public int RaysCount = 3;
    public float AngleOfSight = 360f;
    public float DistanceOfSight = 20f;
    void Start()
    {
        DrawMesh(transform.position);
    }
    private void Update()
    {
        
    }

    private void DrawMesh(Vector3 Origin)
    {
        float AngleIncrement = AngleOfSight / RaysCount;
        Vector3[] Verticles = new Vector3[RaysCount + 1];
        int[] Triangles = new int[RaysCount * 3 - 3];
        int TriangleIndex = 0;
        int VertexIndex = 1;
        Verticles[0] = Origin;
        for (int i = 0; i < RaysCount; i++)
        {
            var Hit = Physics2D.Raycast(Origin, BasicFunctions.AngleToVector2(AngleIncrement * i), DistanceOfSight, Mask);
            Vector3 HitPosition;
            if (Hit)
            {
                Debug.Log("Hit " + Hit.collider.gameObject.name);
                Instantiate(TestObjectToSpawn, new Vector3(Hit.point.x, Hit.point.y, 0), Quaternion.identity);
                HitPosition = new Vector3(Hit.point.x, Hit.point.y, 0);
            }
            else
            {
                Instantiate(TestObjectToSpawn, BasicFunctions.AngleToVector2(AngleIncrement * i) * DistanceOfSight, Quaternion.identity);
                HitPosition = BasicFunctions.AngleToVector2(AngleIncrement * i) * DistanceOfSight;
            }
            Verticles[VertexIndex] = HitPosition;
            if (i > 0)
            {
                Triangles[TriangleIndex + 0] = RaysCount;
                Triangles[TriangleIndex + 1] = VertexIndex - 1;
                Triangles[TriangleIndex + 2] = VertexIndex;
                TriangleIndex += 3;
            }
            VertexIndex++;
        }
        meshFilter.mesh = CreateMesh(Verticles, Triangles);
        
    }
    private Mesh CreateMesh(Vector3[] Verticles, int[] Triangles)
    {
        Mesh Mesh = new Mesh(); 

        Vector2[] UV = new Vector2[Verticles.Length];

        Mesh.vertices = Verticles;
        Mesh.uv = UV;
        Mesh.triangles = Triangles;

        return Mesh;
    }
}
