using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

namespace Vision {
    public class VisionSystem : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public LayerMask ObstacleObjectsLayerMask;


        public int RaysCount = 3;
        public float AngleOfSight = 360f;
        public float DistanceOfSight = 20f;

        private bool IsStarted = false;
        void Start()
        {
            //  meshtestanother();
        }
        private void LateUpdate()
        {
            StartVisionSystem();
        }
        void StartVisionSystem()
        {
            if (IsStarted == false)
            {
                IsStarted = true;
                StartCoroutine(DrawVisionMesh());
            }
        }

        private IEnumerator DrawVisionMesh()
        {
            while (GameManager.GameIsRunning)
            {
                transform.position = GameManager.LocalPlayerHeroUnit.transform.position;

                float AngleIncrement = AngleOfSight / (RaysCount - 1);
                float CurrentAngle = 0;
                Vector3[] Verticles = new Vector3[RaysCount + 1];
                int[] Triangles = new int[(Verticles.Length - 2) * 3];
                int TriangleIndex = 0;
                int VertexIndex = 1;
                Verticles[0] = transform.InverseTransformPoint(transform.position);


                for (int i = 0; i < RaysCount; i++)
                {
                    var Hit = Physics2D.Raycast(transform.position, BasicFunctions.AngleToVector2(-CurrentAngle), DistanceOfSight, ObstacleObjectsLayerMask);
                    Vector3 HitPosition;

                    if (Hit)
                    {
                        //   Debug.Log("Raycast distance" + Vector3.Distance(transform.position, Hit.point) + " not raycast distance " + Vector3.Distance(transform.position, BasicFunctions.AngleToVector2(-CurrentAngle) * DistanceOfSight));
                        HitPosition = new Vector3(Hit.point.x, Hit.point.y, 0);
                    }
                    else
                    {
                        //    Debug.Log("distance of NOT hit " + Vector3.Distance(transform.position, BasicFunctions.AngleToVector2(AngleIncrement * -i) * DistanceOfSight));
                        HitPosition = (Vector3)BasicFunctions.AngleToVector2(-CurrentAngle) * DistanceOfSight + transform.position;
                    }
                    Verticles[VertexIndex] = transform.InverseTransformPoint(HitPosition);
                    if (i > 0)
                    {
                        Triangles[TriangleIndex + 0] = 0;
                        Triangles[TriangleIndex + 1] = VertexIndex - 1;
                        Triangles[TriangleIndex + 2] = VertexIndex;
                        TriangleIndex += 3;
                    }
                    VertexIndex++;
                    CurrentAngle += AngleIncrement;
                }
                meshFilter.mesh = CreateMesh(Verticles, Triangles);

                yield return new WaitForSeconds(0.2f);
            }
        }
        private Mesh CreateMesh(Vector3[] Verticles, int[] Triangles)
        {
            Mesh Mesh = new Mesh();

            Mesh.SetVertices(Verticles);
            Mesh.SetTriangles(Triangles, 0);

            return Mesh;
        }

        private void meshtestanother()
        {
            Mesh Mesh = new Mesh();

            Vector3[] Verticles = new Vector3[5]; //3 = 3, 4 = 6, 5 = 9
            Vector2[] UVs = new Vector2[5];
            int[] Triangles = new int[9];

            Verticles[0] = new Vector3(0, 0);
            Verticles[1] = new Vector3(10, 0);
            Verticles[2] = new Vector3(10, -10);
            Verticles[3] = new Vector3(0, -10);
            Verticles[4] = new Vector3(-10, -20);

            Vector3 global = transform.TransformPoint(Verticles[4]);
            Debug.Log("vertex 5 " + global);


            Triangles[0] = 0;
            Triangles[1] = 1;
            Triangles[2] = 2;

            Triangles[3] = 0;
            Triangles[4] = 2;
            Triangles[5] = 3;

            Triangles[6] = 0;
            Triangles[7] = 3;
            Triangles[8] = 4;

            Mesh.SetVertices(Verticles);
            //   Mesh.uv = UVs;
            Mesh.SetTriangles(Triangles, 0);

            meshFilter.mesh = Mesh;

        }
    }
}
