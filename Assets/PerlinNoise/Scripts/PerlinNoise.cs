using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PerlinNoise
{
	/**
	 * 
	 */
	public static class PerlinNoise
	{
		#region Static Stuff

		private static readonly int[] PermutationTable =
		{
			151, 160, 137, 91, 90, 15, // Hash lookup table as defined by Ken Perlin. This is a randomly
			131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, // arranged array of all numbers from 0-255 inclusive.
			190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
			88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
			77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
			102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
			135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
			5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
			223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
			129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
			251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
			49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
			138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
		};
		private static readonly Vector2[] FixedGradientVectors = new Vector2[16]
		                                                         {
			                                                         new Vector2(-1f, -1f), new Vector2(-1f, 0f), new Vector2(-1f, 0f), new Vector2(-1f, 1f),
			                                                         new Vector2(0f, -1f), new Vector2(0f, -1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
			                                                         new Vector2(1f, -1f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f),
			                                                         new Vector2(-1f, 0f), new Vector2(0f, -1f), new Vector2(1f, 1f), new Vector2(1f, 0f)
		                                                         };
		private static readonly float[] fixedGradientX = new float[16]
		                                                 {
			                                                 -1f, -1f, -1f, -1f,
			                                                 0f, 0f, 0f, 0f,
			                                                 1f, 1f, 1f, 1f,
			                                                 -1f, 0f, 1f, 1f
		                                                 };
		private static readonly float[] fixedGradientY = new float[16]
		                                                 {
			                                                 -1f, 0f, 0f, 1f,
			                                                 -1f, -1f, 1f, 1f,
			                                                 -1f, 0f, 0f, 1f,
			                                                 0f, -1f, 1f, 0f
		                                                 };
		private static readonly int[] RandomPermutationTable = new int[512];
		private static readonly Vector2[] RandomGradientVectors = new Vector2[512];

		public static float WithOctaves2D(float sampleX, float sampleY, int octaves, float lacunarity, float persistance, Vector2[] octaveOffsets, bool useOldSmoothing = false, bool useOldVectorDistribution = false)
		{
			float frequency = 1f;
			float amplitude = 1f;
			float noiseHeight = 0f;

			for (int i = 0; i < octaves; i++)
			{
				float x = (sampleX + octaveOffsets[i].x) * frequency;
				float y = (sampleY + octaveOffsets[i].y) * frequency;

				float noiseValue2D = In2D(x, y, useOldSmoothing, useOldVectorDistribution) * 2 - 1; //Map from -1 to 1 again, so it can rise and falls
				noiseHeight += noiseValue2D * amplitude;

				amplitude *= persistance;
				frequency *= lacunarity;
			}

			return noiseHeight;
		}

		public static float In2D(float x, float y, bool useOldSmoothing = false, bool useOldVectorDistribution = false)
		{
			//Calculate corners (on a unit cube)
			int x0Integer = (int) Mathf.Floor(x) & 0xFF;
			int y0Integer = (int) Mathf.Floor(y) & 0xFF;
			int x1Integer = (x0Integer + 1) & 0xFF;
			int y1Integer = (y0Integer + 1) & 0xFF;

			//Get Gradients and Calc Dot products in it
			float s = Gradient(PermutationTable[(PermutationTable[x0Integer] + y0Integer) & 0xFF], x0Integer, y0Integer, x, y, useOldVectorDistribution);
			float t = Gradient(PermutationTable[(PermutationTable[x1Integer] + y0Integer) & 0xFF], x1Integer, y0Integer, x, y, useOldVectorDistribution);
			float u = Gradient(PermutationTable[(PermutationTable[x0Integer] + y1Integer) & 0xFF], x0Integer, y1Integer, x, y, useOldVectorDistribution);
			float v = Gradient(PermutationTable[(PermutationTable[x1Integer] + y1Integer) & 0xFF], x1Integer, y1Integer, x, y, useOldVectorDistribution);

			//Calc Weights Sx + Sy
			float sx = Smoothing(x - (int) Mathf.Floor(x), useOldSmoothing);
			float sy = Smoothing(y - (int) Mathf.Floor(y), useOldSmoothing);

			//trilinear interpolation
			float a = Lerp(s, t, sx);
			float b = Lerp(u, v, sx);
			float c = Lerp(a, b, sy);

			return (c + 1) / 2; //Map value to Interval [0,1]
		}

		//Dont use Mathf.Pow() for performance reasons
		//3x^2 - 2*x^3 = x * x * (3 - 2 * x) OR 6*x^5 - 15x^4 + 10x^3 = x * x * x * ( x * ( 6 * x - 15) + 10))
		private static float Smoothing(float x, bool useOldSmoothing)
		{
			return useOldSmoothing ? x * x * (3 - 2 * x) : x * x * x * (x * (6 * x - 15) + 10);
		}

		private static float Gradient(int hash, int x, int y, float floatX, float floatY, bool useOldVectorDistribution)
		{
			if (useOldVectorDistribution)
			{
				Vector2 randomGradientVector = RandomGradientVectors[RandomPermutationTable[RandomPermutationTable[x] + y]];
				float gradX = randomGradientVector.x;
				float gradY = randomGradientVector.y;
				return Dot(floatX - x, floatY - y, gradX, gradY);
			}

			int gradientIndex = hash & 0xF;
			return Dot(floatX - x, floatY - y, fixedGradientX[gradientIndex], fixedGradientY[gradientIndex]);
		}

		private static float Dot(float x0, float y0, float x1, float y1)
		{
			return x0 * x1 + y0 * y1;
		}

		private static float Lerp(float a, float b, float weight)
		{
			return a + weight * (b - a);
		}

		static PerlinNoise()
		{
			int i, j, k;

			//Create P with random values between 0 - 1, from 0 - 512 for more variety, normalized by 256 (0x100)
			for (i = 0; i < 256; i++)
			{
				RandomPermutationTable[i] = i;
				float x = (float) (Random.Range(0, 512) - 256) / 256;
				float y = (float) (Random.Range(0, 512) - 256) / 256;
				RandomGradientVectors[i] = new Vector2(x, y);
				RandomGradientVectors[i].Normalize();
			}

			//Shuffle first half of P
			while (--i > 0)
			{
				k = RandomPermutationTable[i];
				RandomPermutationTable[i] = RandomPermutationTable[j = Random.Range(0, 256)];
				RandomPermutationTable[j] = k;
			}

			//copy first half of arrays to second one
			for (i = 0; i < 256; i++)
			{
				RandomPermutationTable[256 + i] = RandomPermutationTable[i];
				RandomGradientVectors[256 + i] = RandomGradientVectors[i];
			}
		}

		#endregion
	}
}