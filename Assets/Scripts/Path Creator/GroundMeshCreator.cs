using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Utility;
using PathCreation.Examples;
using UnityEngine.Rendering;

namespace GD3D
{
    /// <summary>
    /// Modified version of <see cref="RoadMeshCreator"/> with proper uv coordinates on the sides.
    /// </summary>
    public class GroundMeshCreator : PathSceneTool
    {
        [Header("Ground settings")]
        [SerializeField] private float width = 5;
        [SerializeField] private float height = 5;
        [SerializeField] private float extendedHeight = 10;

        [Header("Material settings")]
        [SerializeField] private Material groundTopMaterial;
        [SerializeField] private Material groundBottomMaterial;
        [SerializeField] private Material sidesMaterial;
        [SerializeField] private Material extendedSidesMaterial;

        [SerializeField] private float textureTiling = 1;

        private Dictionary<string, Mesh> oldMeshes = new Dictionary<string, Mesh>();

        [SerializeField, HideInInspector]
        private GameObject meshHolder;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        [SerializeField, HideInInspector]
        private GameObject extendedSidesMeshHolder;

        private MeshFilter extendedSidesMeshFilter;
        private MeshRenderer extendedSidesMeshRenderer;
        private Mesh extendedSidesMesh;

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            extendedSidesMeshRenderer = extendedSidesMeshHolder.GetComponent<MeshRenderer>();

            // Calculate the texture tiling
            float length = path.length;
            float tiling = textureTiling;

            meshRenderer.materials[0].mainTextureScale = new Vector2(width / tiling, length / tiling);

            meshRenderer.materials[2].mainTextureScale = new Vector2(length / tiling, height / tiling);

            extendedSidesMeshRenderer.material.mainTextureScale = new Vector2(length / tiling, extendedHeight / tiling);
        }

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents("Ground Mesh Holder", ref meshHolder, ref meshRenderer, ref meshFilter, ref mesh);
                AssignMeshComponents("Extended Sides Mesh Holder", ref extendedSidesMeshHolder, ref extendedSidesMeshRenderer, ref extendedSidesMeshFilter, ref extendedSidesMesh);
                AssignMaterials();
                CreateGroundMesh();
                GenerateGroundExtendedSides();
            }
        }

        void CreateGroundMesh()
        {
            VertexPath path = this.path;

            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = {
                0, 8, 1,
                1, 8, 9 
            };

            int[] sidesTriangleMap = {
                4, 6, 14,
                12, 4, 14,
                5, 15, 7,
                13, 15, 5
            };

            int numPoints = path.NumPoints;
            for (int i = 0; i < numPoints; i++)
            {
                Vector3 localUp = Vector3.up; // Force it to be the world up regardless
                Vector3 localRight = Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * height;
                verts[vertIndex + 3] = vertSideB - localUp * height;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                float pathTime = path.times[i];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, pathTime);
                uvs[vertIndex + 1] = new Vector2(1, pathTime);

                // Set side uvs
                uvs[vertIndex + 4] = new Vector2(pathTime, 1); // Top Left
                uvs[vertIndex + 5] = new Vector2(pathTime, 1); // Top Right
                uvs[vertIndex + 6] = new Vector2(pathTime, 0); // Bottom Left
                uvs[vertIndex + 7] = new Vector2(pathTime, 0); // Bottom Right

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;

                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    int triangleLength = triangleMap.Length;
                    for (int j = 0; j < triangleLength; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }

                    int sidesTriangleLength = sidesTriangleMap.Length;
                    for (int j = 0; j < sidesTriangleLength; j++)
                    {
                        int index = (vertIndex + sidesTriangleMap[j]) % verts.Length;

                        sideOfRoadTriangles[triIndex * 2 + j] = index;
                    }
                }

                vertIndex += 8;
                triIndex += 6;
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 3;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(underRoadTriangles, 1);
            mesh.SetTriangles(sideOfRoadTriangles, 2);
            mesh.RecalculateBounds();
        }

        public void GenerateGroundExtendedSides()
        {
            VertexPath path = this.path;

            Vector3[] verts = new Vector3[path.NumPoints * 4];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] triangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            int[] triangleMap = {
                0, 2, 4,
                4, 2, 6,

                3, 1, 5,
                3, 5, 7,
            };

            int numPoints = path.NumPoints;
            for (int i = 0; i < numPoints; i++)
            {
                Vector3 localUp = Vector3.up; // Force it to be the world up regardless
                Vector3 localRight = Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(width);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(width);

                // Add vertices
                verts[vertIndex + 0] = vertSideA - (localUp * height); // Top Left
                verts[vertIndex + 1] = vertSideB - (localUp * height); // Top Right
                verts[vertIndex + 2] = vertSideA - (localUp * (height + extendedHeight)); // Bottom Left
                verts[vertIndex + 3] = vertSideB - (localUp * (height + extendedHeight)); // Bottom Right

                float pathTime = path.times[i];

                // Set uvs
                uvs[vertIndex + 0] = new Vector2(pathTime, 1); // Top Left
                uvs[vertIndex + 1] = new Vector2(pathTime, 1); // Top Right
                uvs[vertIndex + 2] = new Vector2(pathTime, 0); // Bottom Left
                uvs[vertIndex + 3] = new Vector2(pathTime, 0); // Bottom Right

                // Normals
                normals[vertIndex + 0] = -localRight;
                normals[vertIndex + 1] = localRight;
                normals[vertIndex + 2] = -localRight;
                normals[vertIndex + 3] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
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

                vertIndex += 4;
                triIndex += triangleMap.Length;
            }

            extendedSidesMesh.Clear();
            extendedSidesMesh.vertices = verts;
            extendedSidesMesh.uv = uvs;
            extendedSidesMesh.normals = normals;
            extendedSidesMesh.subMeshCount = 1;
            extendedSidesMesh.SetTriangles(triangles, 0);
            extendedSidesMesh.RecalculateBounds();
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents(string name, ref GameObject meshHolder, ref MeshRenderer meshRenderer, ref MeshFilter meshFilter, ref Mesh mesh)
        {
            if (meshHolder == null)
            {
                meshHolder = new GameObject(name);
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
                if (oldMeshes.ContainsKey(name))
                {
                    DestroyImmediate(oldMeshes[name]);
                }

                mesh = new Mesh();
                mesh.name = gameObject.name + " Mesh";

                if (oldMeshes.ContainsKey(name))
                {
                    oldMeshes[name] = mesh;
                }
                else
                {
                    oldMeshes.Add(name, mesh);
                }
            }

            meshFilter.sharedMesh = mesh;
        }

        void AssignMaterials()
        {
            if (groundTopMaterial != null && groundBottomMaterial != null && sidesMaterial != null && extendedSidesMaterial != null)
            {
                meshRenderer.sharedMaterials = new Material[] { groundTopMaterial, groundBottomMaterial, sidesMaterial };
                extendedSidesMeshRenderer.sharedMaterials = new Material[] { extendedSidesMaterial };
            }
        }
    }
}
