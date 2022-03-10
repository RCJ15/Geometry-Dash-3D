using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Utility;
using PathCreation.Examples;

namespace GD3D
{
    public class BackgroundMeshCreator : PathSceneTool
    {
        [Header("Background settings")]
        [SerializeField] private float yOffset;
        [SerializeField] private float roundDistance = .4f;
        [SerializeField] private float width = .4f;
        [SerializeField] private float height = .15f;

        [Header("Material settings")]
        [SerializeField] private Material material;

        [SerializeField] private float textureTiling = 1;

        [SerializeField, HideInInspector]
        private GameObject meshHolder;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        private Mesh oldMesh;

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            meshFilter = meshHolder.GetComponent<MeshFilter>();

            // Calculate the side texture tiling
            float length = path.length;
            float tiling = textureTiling;

            meshRenderer.material.mainTextureScale = new Vector2(length / tiling, height / tiling);
        }

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateBackgroundMesh();
            }
        }

        void CreateBackgroundMesh()
        {
            VertexPath[] paths = GetFixedPaths(roundDistance);

            int numTrisLeft = 2 * (paths[0].NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int numTrisRight = 2 * (paths[1].NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] triangles = new int[(numTrisLeft + numTrisRight) * 2 * 3];

            Vector3[] verts = new Vector3[(paths[0].NumPoints + paths[1].NumPoints) * 4];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int vertIndex = 0;
            int triIndex = 0;

            int[] triangleMapLeft = {
                1, 0, 2,
                1, 2, 3,
            };
            int[] triangleMapRight = {
                0, 1, 2,
                2, 1, 3,
            };

            // Loop through the paths
            int pathIndex = 0;
            foreach (VertexPath newPath in paths)
            {
                bool isLeft = pathIndex == 0;

                // Generate 2 triangles
                for (int i = 0; i < newPath.NumPoints; i++)
                {
                    Vector3 localUp = Vector3.up; // Force it to be the world up regardless
                    Vector3 localRight = Vector3.Cross(localUp, newPath.GetTangent(i));

                    Vector3 position = newPath.GetPoint(i);

                    // Add vertices
                    Vector3 offset = new Vector3(0, yOffset, 0);
                    verts[vertIndex + 0] = position + (localUp * height / 2) + offset;
                    verts[vertIndex + 1] = position - (localUp * height / 2) + offset;

                    // Set uv
                    uvs[vertIndex + 0] = new Vector2(newPath.times[i], 1);
                    uvs[vertIndex + 1] = new Vector2(newPath.times[i], 0);

                    // Normals
                    Vector2 normal = localUp;

                    normals[vertIndex + 0] = normal;
                    normals[vertIndex + 1] = normal;

                    if (i + 1 < newPath.NumPoints - 1)
                    {
                        int[] triangleMap = isLeft ? triangleMapLeft : triangleMapRight;

                        for (int j = 0; j < triangleMap.Length; j++)
                        {
                            int index = vertIndex + triangleMap[j];

                            if (index > verts.Length - 1)
                            {
                                continue;
                            }

                            triangles[triIndex + j] = index;
                        }
                    }

                    vertIndex += 2;
                    triIndex += 6;
                }

                pathIndex++;
            }

            //int[] triangleMap = {
            //    2, 0, 4,
            //    2, 4, 6,

            //    1, 3, 5,
            //    5, 3, 7,
            //};

            //for (int i = 0; i < path.NumPoints; i++)
            //{
            //    Vector3 localUp = Vector3.up; // Force it to be the world up regardless
            //    Vector3 localRight = Vector3.Cross(localUp, path.GetTangent(i));

            //    // Find position to left and right of current path vertex
            //    Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width);
            //    Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width);

            //    // Add vertices
            //    verts[vertIndex + 0] = vertSideA + (localUp * height / 2); // Top Left
            //    verts[vertIndex + 1] = vertSideB + (localUp * height / 2); // Top Right
            //    verts[vertIndex + 2] = vertSideA - (localUp * height / 2); // Bottom Left
            //    verts[vertIndex + 3] = vertSideB - (localUp * height / 2); // Bottom Right

            //    // Set uvs
            //    uvs[vertIndex + 0] = new Vector2(path.times[i], 1); // Top Left
            //    uvs[vertIndex + 1] = new Vector2(path.times[i], 1); // Top Right
            //    uvs[vertIndex + 2] = new Vector2(path.times[i], 0); // Bottom Left
            //    uvs[vertIndex + 3] = new Vector2(path.times[i], 0); // Bottom Right

            //    // Normals
            //    normals[vertIndex + 0] = localRight;
            //    normals[vertIndex + 1] = -localRight;
            //    normals[vertIndex + 2] = localRight;
            //    normals[vertIndex + 3] = -localRight;

            //    // Set triangle indices
            //    if (i < path.NumPoints - 1 || path.isClosedLoop)
            //    {
            //        for (int j = 0; j < triangleMap.Length; j++)
            //        {
            //            int index = vertIndex + triangleMap[j];

            //            if (index > verts.Length - 1)
            //            {
            //                continue;
            //            }

            //            triangles[triIndex + j] = index;
            //        }
            //    }

            //    vertIndex += 4;
            //    triIndex += triangleMap.Length;
            //}

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {
            if (meshHolder == null)
            {
                meshHolder = new GameObject("Background Mesh Holder");
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            // Ensure mesh renderer and filter components are assigned
            if (!meshHolder.gameObject.GetComponent<MeshFilter>())
            {
                meshHolder.gameObject.AddComponent<MeshFilter>();
            }
            if (!meshHolder.GetComponent<MeshRenderer>())
            {
                meshHolder.gameObject.AddComponent<MeshRenderer>();
            }

            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            meshFilter = meshHolder.GetComponent<MeshFilter>();

            if (mesh == null)
            {
                if (oldMesh != null)
                {
                    DestroyImmediate(oldMesh);
                }

                mesh = new Mesh();
                mesh.name = gameObject.name + " Mesh";

                oldMesh = mesh;
            }

            meshFilter.sharedMesh = mesh;
        }

        void AssignMaterials()
        {
            if (material != null)
            {
                meshRenderer.sharedMaterials = new Material[] { material };
            }
        }

        private VertexPath[] GetFixedPaths(float distance)
        {
            List<Vector3> rightPoints = new List<Vector3>();
            List<Vector3> leftPoints = new List<Vector3>();

            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = Vector3.up; // Force it to be the world up regardless
                Vector3 localRight = Vector3.Cross(localUp, path.GetTangent(i));

                Vector3 position = path.GetPoint(i);
                Vector3 positionRight = position + localRight * Mathf.Abs(width);
                Vector3 positionLeft = position - localRight * Mathf.Abs(width);

                if (Vector3.Distance(path.GetClosestPointOnPath(positionRight), positionRight) >= distance)
                {
                    rightPoints.Add(positionRight);
                }
                if (Vector3.Distance(path.GetClosestPointOnPath(positionLeft), positionLeft) >= distance)
                {
                    leftPoints.Add(positionLeft);
                }
            }

            BezierPath leftPath = new BezierPath(leftPoints, pathCreator.bezierPath.IsClosed, pathCreator.bezierPath.Space);
            BezierPath rightPath = new BezierPath(rightPoints, pathCreator.bezierPath.IsClosed, pathCreator.bezierPath.Space);

            leftPath.ControlPointMode = pathCreator.bezierPath.ControlPointMode;
            rightPath.ControlPointMode = pathCreator.bezierPath.ControlPointMode;

            leftPath.AutoControlLength = pathCreator.bezierPath.AutoControlLength;
            rightPath.AutoControlLength = pathCreator.bezierPath.AutoControlLength;

            return new VertexPath[] { new VertexPath(leftPath, transform), new VertexPath(rightPath, transform) };
        }
    }
}
