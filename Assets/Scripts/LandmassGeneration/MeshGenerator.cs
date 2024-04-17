using UnityEngine;

namespace LandmassGeneration
{
    public static class MeshGenerator
    {
        public static MeshData GenerateMesh(float[,] heightMap, float heightScale)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;

            MeshData meshData = new MeshData(width, height);
            int vertexIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightScale, topLeftZ - y);
                    meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                    
                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }
                    
                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Vector2[] UVs;

        private int _triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            Vertices = new Vector3[meshWidth * meshHeight];
            Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
            UVs = new Vector2[meshWidth * meshHeight];
        }

        public void AddTriangle(int v1, int v2, int v3)
        {
            Triangles[_triangleIndex] = v1;
            Triangles[_triangleIndex + 1] = v2;
            Triangles[_triangleIndex + 2] = v3;
            _triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh {
                vertices = Vertices,
                triangles = Triangles,
                uv = UVs
            };

            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
