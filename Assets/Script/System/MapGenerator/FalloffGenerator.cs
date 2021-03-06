using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int size, float smooth, float density) {
		float[,] map = new float[size,size];

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y));
				map [i, j] = Evaluate(value, smooth, density);
			}
		}

		return map;
	}

	static float Evaluate(float value, float smooth, float density) {

		return Mathf.Pow (value, smooth) / (Mathf.Pow (value, smooth) + Mathf.Pow (density - density * value, smooth));
	}
}