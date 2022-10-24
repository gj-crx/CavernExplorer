using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vision {
    public class VisionSystem : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;
        [SerializeField]
        private LayerMask ObstacleObjectsLayerMask;
        [SerializeField]
        private Vector3 Offset;


        public int RaysCount = 3;
        public float AngleOfSight = 360f;
        public float DistanceOfSight = 20f;
        public float ObstacleRevealingMultiplier = 2;

        private bool IsStarted = false;
        void Start()
        {

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
                transform.position = GameManager.LocalPlayerHeroUnit.transform.position + Offset;

                float AngleIncrement = AngleOfSight / (RaysCount - 1);
                float CurrentAngle = 0;
                Vector3[] Verticles = new Vector3[RaysCount + 1];
                int[] Triangles = new int[(Verticles.Length - 2) * 3];
                int TriangleIndex = 0;
                int VertexIndex = 1;

                Verticles[0] = transform.InverseTransformPoint(transform.position);

                for (int i = 0; i < RaysCount; i++)
                {
                    Vector2 CurrentDirection = BasicFunctions.AngleToVector2(-CurrentAngle);
                    var Hit = Physics2D.Raycast(transform.position, CurrentDirection, DistanceOfSight, ObstacleObjectsLayerMask);
                    Vector3 HitPosition;

                    if (Hit)
                    { //add 2.0f offset to hit position to reveal the obstacle
                        HitPosition = new Vector3(Hit.point.x + (CurrentDirection.x * ObstacleRevealingMultiplier), Hit.point.y + (CurrentDirection.y * ObstacleRevealingMultiplier), 0);
                    }
                    else
                    {
                        HitPosition = (Vector3)CurrentDirection * DistanceOfSight + transform.position;
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

                yield return null;
              //  yield return new WaitForSeconds(0.2f);
            }
        }
        private Mesh CreateMesh(Vector3[] Verticles, int[] Triangles)
        {
            Mesh Mesh = new Mesh();

            Mesh.SetVertices(Verticles);
            Mesh.SetTriangles(Triangles, 0);

            return Mesh;
        }
    }
}
