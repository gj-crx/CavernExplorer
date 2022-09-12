using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestFieldOfView : MonoBehaviour
{

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        float fov = 90f;
        Vector3 origin = Vector3.zero;
        int rayCount = 2;
        float angle = 0f;
        float angleIncrease = fov / rayCount;
        float viewDistance = 50f;

        Vector3[] Vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[Vertices.Length];
        int[] triangles = new int[rayCount * 3];

        Vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            Vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            angle -= angleIncrease;
        }

        Vertices[0] = Vector3.zero;
        Vertices[1] = new Vector3(50, 0);
        Vertices[2] = new Vector3(0, -50);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = Vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }



    void Update()
    {
        
    }
}
