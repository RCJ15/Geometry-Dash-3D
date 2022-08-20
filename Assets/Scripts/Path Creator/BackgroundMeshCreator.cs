using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Utility;
using PathCreation.Examples;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    public class BackgroundMeshCreator : PathSceneTool
    {
        [Header("Background settings")]
        [SerializeField] private float yOffset;
        [SerializeField] private float distanceStep = 10;
        [SerializeField] private float pointDistance = .4f;
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
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }
#endif

            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateBackgroundMesh();
            }
        }

        void CreateBackgroundMesh()
        {
            VertexPath[] paths = GetFixedPaths(roundDistance, pointDistance);

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

            bool only2Points = path.NumPoints == 2;

            // Loop through the paths
            int pathIndex = 0;
            foreach (VertexPath newPath in paths)
            {
                bool isLeft = pathIndex == 0;

                // Generate 2 triangles
                int numPoints = newPath.NumPoints;
                for (int i = 0; i < numPoints; i++)
                {
                    Vector3 localUp = Vector3.up; // Force it to be the world up regardless

                    Vector3 position = newPath.GetPoint(i);

                    // If there are only 2 points, then we will add some directional offset cuz otherwise this stuff doesn't work idk
                    if (only2Points)
                    {
                        Vector3 localRight = Vector3.Cross(localUp, newPath.GetTangent(i));

                        position += Mathf.Abs(width) * (isLeft ? -1 : 1) * localRight;
                    }

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

                    if (i < newPath.NumPoints - 1)
                    {
                        int[] triangleMap = isLeft ? triangleMapLeft : triangleMapRight;

                        int triangleLength = triangleMap.Length;
                        for (int j = 0; j < triangleLength; j++)
                        {
                            int index = vertIndex + triangleMap[j];

                            /*
                            if (index > verts.Length - 1)
                            {
                                continue;
                            }*/

                            triangles[triIndex + j] = index;
                        }
                    }

                    vertIndex += 2;
                    triIndex += 6;
                }

                pathIndex++;
            }

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

        private VertexPath[] GetFixedPaths(float distance, float pointDistance)
        {
            List<Vector3> rightPoints = new List<Vector3>();
            List<Vector3> leftPoints = new List<Vector3>();

            Vector3? oldRightPos = null;
            Vector3? oldLeftPos = null;

            float dist = 0;
            while (dist < path.length)
            {
                Vector3 localUp = Vector3.up; // Force it to be the world up regardless
                Vector3 localRight = Vector3.Cross(localUp, path.GetDirectionAtDistance(dist));

                Vector3 position = path.GetPointAtDistance(dist);
                Vector3 positionRight = position + (localRight * Mathf.Abs(width));
                Vector3 positionLeft = position - (localRight * Mathf.Abs(width));

                if (Vector3.Distance(path.GetClosestPointOnPath(positionRight), positionRight) >= distance)
                {
                    if (!oldRightPos.HasValue || Vector3.Distance(oldRightPos.Value, positionRight) >= pointDistance)
                    {
                        rightPoints.Add(positionRight);

                        oldRightPos = positionRight;
                    }
                }
                if (Vector3.Distance(path.GetClosestPointOnPath(positionLeft), positionLeft) >= distance)
                {
                    if (!oldLeftPos.HasValue || Vector3.Distance(oldLeftPos.Value, positionLeft) >= pointDistance)
                    {
                        leftPoints.Add(positionLeft);

                        oldLeftPos = positionLeft;
                    }
                }

                dist += distanceStep;
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

#if UNITY_EDITOR
    [CustomEditor(typeof(BackgroundMeshCreator))]
    public class BackgroundMeshCreatorAutoUpdater : Editor
    {
        private BackgroundMeshCreator backgroundMeshCreator;

        private void OnEnable()
        {
            // Get backgroundMeshCreator
            backgroundMeshCreator = (BackgroundMeshCreator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Manual Update"))
            {
                backgroundMeshCreator.TriggerUpdate();
            }
        }

        private void OnDisable()
        {
            // Auto update
            backgroundMeshCreator.TriggerUpdate();
        }
    }
#endif
}
