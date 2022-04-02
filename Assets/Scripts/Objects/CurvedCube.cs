using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using QuickOutline;

namespace GD3D.Objects
{
    /// <summary>
    /// Generates a curved cube along the path
    /// </summary>
    [RequireComponent(typeof(AttachToPath))]
    public class CurvedCube : MonoBehaviour
    {
        [Header("Cube Settings")]
        [SerializeField] private float length;
        [SerializeField] private int resolution;
        [SerializeField] private Vector2 cubeSize;

        [Space]
        [SerializeField] private Material material;
        [SerializeField] private float textureTiling;

        [Header("Outline")]
        [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineVisible;
        [SerializeField] private Color outlineColor = Color.white;

        [Range(0, 10)]
        [SerializeField] private float outlineWidth = 10;

        private Outline _outline;

        private AttachToPath _attachToPath;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private VertexPath _path;

        private Material _topMaterial => _meshRenderer.materials[0];
        private Material _sideMaterial => _meshRenderer.materials[1];
        private Material _extraSideMaterial => _meshRenderer.materials[2];
        private Material _bottomMaterial => _meshRenderer.materials[3];

        private void Start()
        {
            // Get components
            _attachToPath = GetComponent<AttachToPath>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            // Set material if the material is not null
            if (material != null)
            {
                List<Material> newMaterials = new List<Material>();

                // Loop for 4 times because that's how many textures we have
                for (int i = 0; i < 4; i++)
                {
                    newMaterials.Add(material);
                }

                // Set the materials
                _meshRenderer.materials = newMaterials.ToArray();
            }

            // Set path
            _path = _attachToPath.PathCreator.path;

            if (resolution < 2)
            {
                resolution = 2;
            }

            GenerateMesh();

            UpdateTexture();

            // Add outline
            _outline = gameObject.AddComponent<Outline>();
            _outline.OutlineMode = outlineMode;
            _outline.OutlineColor = outlineColor;
            _outline.OutlineWidth = outlineWidth;
        }

        private void UpdateTexture()
        {
            // Calculate the texture tiling
            float tiling = textureTiling;

            // Set materials main texture scale
            Vector2 topDownScale = new Vector2(cubeSize.x / tiling, length / tiling);

            _topMaterial.mainTextureScale       = topDownScale;
            _sideMaterial.mainTextureScale      = new Vector2(length / tiling, cubeSize.y / tiling);
            _extraSideMaterial.mainTextureScale = new Vector2(cubeSize.x / tiling, cubeSize.y / tiling);
            _bottomMaterial.mainTextureScale    = topDownScale;
        }

        /// <summary>
        /// Generates the mesh for this object
        /// </summary>
        public void GenerateMesh()
        {
            if (_meshFilter.mesh == null)
            {
                _meshFilter.mesh = new Mesh();
            }

            Mesh mesh = _meshFilter.mesh;

            // Get start and end distances
            float startDist = _attachToPath.Distance - length / 2;
            float endDist = _attachToPath.Distance + length / 2;

            // Set vertex, uv and normal arrays
            int vertCount = (resolution * 8) + 8;
            Vector3[] verts = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            Vector3[] normals = new Vector3[vertCount];

            // Set triangle arrays
            int numTris = 2 * (resolution - 1);
            int[] topTriangles = new int[numTris * 3];
            int[] bottomTriangles = new int[numTris * 3];
            int[] sideTriangles = new int[numTris * 2 * 3];
            int[] extraSideTriangles = new int[12];

            // Index integers
            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap =
            {
                0, 8, 1,
                1, 8, 9
            };

            int[] sidesTriangleMap =
            {
                4, 6, 14,
                12, 4, 14,
                5, 15, 7,
                13, 15, 5
            };

            Vector3 localUp = transform.InverseTransformDirection(_path.up);

            // Resolution determines how many points we have in total
            // More resolution  = more points    = higher quality   = more demanding on system
            // Less resolution  = less points    = lower quality    = less demanding on system
            for (int i = 0; i < resolution; i++)
            {
                // Calculate current distance using lerp
                float t = (float)i / (float)(resolution - 1);
                float dist = Mathf.Lerp(startDist, endDist, t);

                // We use InverseTransformDirection because everything here needs to be in local space
                Vector3 localRight = transform.InverseTransformDirection(_path.GetNormalAtDistance(dist, EndOfPathInstruction.Stop));

                Vector3 width = localRight * Mathf.Abs(cubeSize.x / 2);

                // Calculate current point
                // We use InverseTransformPoint because everything here needs to be in local space
                Vector3 point = transform.InverseTransformPoint(_path.GetPointAtDistance(dist, EndOfPathInstruction.Stop));
                point.y = 0;

                // Add offset
                point += localRight * _attachToPath.ZOffset;

                // Get position from left and right
                Vector3 vertSideA = point - width;
                Vector3 vertSideB = point + width;

                Vector3 height = localUp * (cubeSize.y / 2);

                // Set top vertices
                verts[vertIndex + 0] = vertSideA + height;
                verts[vertIndex + 1] = vertSideB + height;
                // Set bottom vertices
                verts[vertIndex + 2] = vertSideA - height;
                verts[vertIndex + 3] = vertSideB - height;

                // Set vertices for sides
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, t);
                uvs[vertIndex + 1] = new Vector2(1, t);

                uvs[vertIndex + 2] = new Vector2(0, t);
                uvs[vertIndex + 3] = new Vector2(1, t);

                // Set side uvs
                uvs[vertIndex + 4] = new Vector2(t, 1); // Top Left
                uvs[vertIndex + 5] = new Vector2(t, 1); // Top Right
                uvs[vertIndex + 6] = new Vector2(t, 0); // Bottom Left
                uvs[vertIndex + 7] = new Vector2(t, 0); // Bottom Right
                
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

                // Set triangles
                if (i < resolution - 1)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        topTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % vertCount;
                        // reverse triangle map for bottom so that triangles wind the other way and are visible from underneath
                        bottomTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % vertCount;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % vertCount;
                    }

                    vertIndex += 8;
                    triIndex += 6;
                }
            }

            #region Left side
            Vector3 leftDirection =
                Quaternion.AngleAxis(-45, localUp) *
                transform.InverseTransformDirection(_path.GetNormalAtDistance(startDist, EndOfPathInstruction.Stop));
            
            vertIndex += 8;

            // Set vertices
            verts[vertIndex + 0] = verts[0];
            verts[vertIndex + 1] = verts[1];
            verts[vertIndex + 2] = verts[2];
            verts[vertIndex + 3] = verts[3];

            // Set UV
            uvs[vertIndex + 0] = new Vector2(0, 1); // Top Left
            uvs[vertIndex + 1] = new Vector2(1, 1); // Top Right
            uvs[vertIndex + 2] = new Vector2(0, 0); // Bottom Left
            uvs[vertIndex + 3] = new Vector2(1, 0); // Bottom Right

            // Set normals
            normals[vertIndex + 0] = -leftDirection;
            normals[vertIndex + 1] = -leftDirection;
            normals[vertIndex + 2] = -leftDirection;
            normals[vertIndex + 3] = -leftDirection;

            // Set triangles
            extraSideTriangles[0] = vertIndex + 0;
            extraSideTriangles[1] = vertIndex + 1;
            extraSideTriangles[2] = vertIndex + 2;

            extraSideTriangles[3] = vertIndex + 3;
            extraSideTriangles[4] = vertIndex + 2;
            extraSideTriangles[5] = vertIndex + 1;

            vertIndex += 4;
            #endregion

            #region Right side
            Vector3 rightDirection =
                Quaternion.AngleAxis(-45, localUp) *
                transform.InverseTransformDirection(_path.GetNormalAtDistance(endDist, EndOfPathInstruction.Stop));

            // Set vertices
            verts[vertIndex + 0] = verts[vertIndex - 8];
            verts[vertIndex + 1] = verts[vertIndex - 7];
            verts[vertIndex + 2] = verts[vertIndex - 6];
            verts[vertIndex + 3] = verts[vertIndex - 5];

            // Set UV
            uvs[vertIndex + 0] = new Vector2(0, 1); // Top Left
            uvs[vertIndex + 1] = new Vector2(1, 1); // Top Right
            uvs[vertIndex + 2] = new Vector2(0, 0); // Bottom Left
            uvs[vertIndex + 3] = new Vector2(1, 0); // Bottom Right

            // Set normals
            normals[vertIndex + 0] = rightDirection;
            normals[vertIndex + 1] = rightDirection;
            normals[vertIndex + 2] = rightDirection;
            normals[vertIndex + 3] = rightDirection;

            // Set rightmost side
            extraSideTriangles[6] = vertIndex + 2;
            extraSideTriangles[7] = vertIndex + 1;
            extraSideTriangles[8] = vertIndex + 0;

            extraSideTriangles[9] = vertIndex + 1;
            extraSideTriangles[10] = vertIndex + 2;
            extraSideTriangles[11] = vertIndex + 3;
            #endregion

            // Apply mesh
            mesh.Clear();

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;

            mesh.subMeshCount = 4;
            mesh.SetTriangles(topTriangles, 0);
            mesh.SetTriangles(sideTriangles, 1);
            mesh.SetTriangles(extraSideTriangles, 2);
            mesh.SetTriangles(bottomTriangles, 3);

            mesh.RecalculateBounds();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                return;
            }

            // Set resolution to be at least 2
            if (resolution < 2)
            {
                resolution = 2;
            }

            // This will try to get some components needed for this to draw gizmos
            // But if this for some reason does not work, then we'll catch the error and not draw any gizmos
            try
            {
                if (_attachToPath == null)
                {
                    _attachToPath = GetComponent<AttachToPath>();
                }

                if (_path == null)
                {
                    _path = _attachToPath.PathCreator.path;
                }
            }
            catch (System.Exception)
            {
                return;
            }

            // Set color and matrix
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            // Get start and end distance
            float startDist = _attachToPath.Distance - length / 2;
            float endDist = _attachToPath.Distance + length / 2;

            // Tons of vectors for different important positions
            Vector3 startTopLeft = Vector3.zero;
            Vector3 startTopRight = Vector3.zero;
            Vector3 startBottomLeft = Vector3.zero;
            Vector3 startBottomRight = Vector3.zero;

            Vector3 endTopLeft = Vector3.zero;
            Vector3 endTopRight = Vector3.zero;
            Vector3 endBottomLeft = Vector3.zero;
            Vector3 endBottomRight = Vector3.zero;

            Vector3 previousTopLeftPoint = Vector3.zero;
            Vector3 previousTopRightPoint = Vector3.zero;
            Vector3 previousBottomLeftPoint = Vector3.zero;
            Vector3 previousBottomRightPoint = Vector3.zero;

            // The local up
            Vector3 localUp = transform.InverseTransformDirection(_path.up);

            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / (float)(resolution - 1);
                float dist = Mathf.Lerp(startDist, endDist, t);

                Vector3 localRight = transform.InverseTransformDirection(_path.GetNormalAtDistance(dist, EndOfPathInstruction.Stop));

                // Find position to left and right of current path vertex
                Vector3 width = localRight * Mathf.Abs(cubeSize.x / 2);

                Vector3 point = transform.InverseTransformPoint(_path.GetPointAtDistance(dist, EndOfPathInstruction.Stop));
                point.y = 0;

                point += localRight * _attachToPath.ZOffset;

                Vector3 height = localUp * (cubeSize.y / 2);

                if (i != 0)
                {
                    Gizmos.DrawLine(point - width + height, previousTopLeftPoint);
                    Gizmos.DrawLine(point + width + height, previousTopRightPoint);
                    Gizmos.DrawLine(point - width - height, previousBottomLeftPoint);
                    Gizmos.DrawLine(point + width - height, previousBottomRightPoint);
                }

                previousTopLeftPoint = point - width + height;
                previousTopRightPoint = point + width + height;
                previousBottomLeftPoint = point - width - height;
                previousBottomRightPoint = point + width - height;

                if (i == 0)
                {
                    startTopLeft = previousTopLeftPoint;
                    startTopRight = previousTopRightPoint;
                    startBottomLeft = previousBottomLeftPoint;
                    startBottomRight = previousBottomRightPoint;
                }
                else if (i == resolution - 1)
                {
                    endTopLeft = previousTopLeftPoint;
                    endTopRight = previousTopRightPoint;
                    endBottomLeft = previousBottomLeftPoint;
                    endBottomRight = previousBottomRightPoint;
                }
            }

            Gizmos.DrawLine(startTopLeft, startTopRight);
            Gizmos.DrawLine(startBottomLeft, startBottomRight);
            Gizmos.DrawLine(startTopLeft, startBottomLeft);
            Gizmos.DrawLine(startTopRight, startBottomRight);

            Gizmos.DrawLine(endTopLeft, endTopRight);
            Gizmos.DrawLine(endBottomLeft, endBottomRight);
            Gizmos.DrawLine(endTopLeft, endBottomLeft);
            Gizmos.DrawLine(endTopRight, endBottomRight);
        }
#endif
    }
}