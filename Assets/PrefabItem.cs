using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabItem : MonoBehaviour {

	public Vector3 Size
	{
		get{
			Renderer[] filters = GetComponentsInChildren<Renderer>(false);
			Bounds aabb;
			if (filters.Length == 0)
				return Vector3.zero;
			aabb = filters [0].bounds;
			for (int i = 0; i < filters.Length; ++i) {
				aabb.Encapsulate (filters [i].bounds);
			}
			return aabb.max - aabb.min;
		}
	}
}
