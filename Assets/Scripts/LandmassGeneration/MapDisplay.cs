using System;
using UnityEngine;

namespace LandmassGeneration
{
    public class MapDisplay: MonoBehaviour
    {
        public MeshRenderer Renderer;

        public void DrawNoiseMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Texture2D texture = new(width, height);

            Color[] colourMap = new Color[width * height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }
            texture.SetPixels(colourMap);
            texture.Apply();
            Renderer.sharedMaterial.mainTexture = texture;
            Renderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}
