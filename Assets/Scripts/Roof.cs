using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour {

	public Roof ()
	{
	}

	public void Awake()
	{
		_mesh = new Mesh ();
	}
		
	public void CreateFromLines(List<Line> _segments, float inset, float hatHeight)
	{
		if (_segments.Count == 0) {
			return;
		}

		Vector3 infiniteRandomPoint = new Vector3 (Random.value * 10000.0f + 1000.0f, 0.0f, Random.value * 1000.0f + 10000.0f);

		List<Line> segments = new List<Line>();

		List<Vector3> vbuffer = new List<Vector3> ();

		// split segment to multiple segments for windows
		for (int i = 0; i < _segments.Count; i++) {
			segments.Add (_segments [i]);
		}



		List<Vector3> segmentsWithContour = Line.Offsets (segments);
		// lines, lines offseted, lines offseted backward


		//segments.Clear ();
		HashSet<Vector3> vertices = new HashSet<Vector3> ();
		HashSet<int> linesContoured = new HashSet<int> ();
		// loop all lines, find intersection between offsets on the same side, then replace intersection point
		Line.WeldIntersections (segmentsWithContour, out vertices);

		// find end point edges
		List<Vector3> endpointsSegments = new List<Vector3> ();
		for (int i = 0; i < segmentsWithContour.Count; i += 6) {
			if (!vertices.Contains (segmentsWithContour [i])) {
				endpointsSegments.Add (segmentsWithContour [i + 2]);
				endpointsSegments.Add (segmentsWithContour [i + 4]);
			}
			if (!vertices.Contains (segmentsWithContour [i + 1])) {
				endpointsSegments.Add (segmentsWithContour [i + 3]);
				endpointsSegments.Add (segmentsWithContour [i + 5]);
			}
		}

		// remove original lines
		for (int i = segmentsWithContour.Count - 6; i >= 0; i -= 6) {
			segmentsWithContour.RemoveAt (i);
			segmentsWithContour.RemoveAt (i);
		}

		// add endpoint edges to contour list
		segmentsWithContour.AddRange (endpointsSegments);

		// find directed paths (to determine inner, outer walls)
		List<List<Vector3>> directedPaths = new List<List<Vector3>>();
		directedPaths.Add (new List<Vector3> ());
		directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [0]);
		directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [1]);
		segmentsWithContour.RemoveRange (0, 2);
		while (segmentsWithContour.Count > 0) {
			bool flag = true;
			for (int i = 0; i < segmentsWithContour.Count; i += 2) {
				if (directedPaths[directedPaths.Count - 1] [directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour [i]) {
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i]);
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i + 1]);
					segmentsWithContour.RemoveRange (i, 2);
					flag = false;
					break;
				}
				else if (directedPaths[directedPaths.Count - 1] [directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour [i + 1]) {
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i + 1]);
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i]);
					segmentsWithContour.RemoveRange (i, 2);
					flag = false;
					break;
				}
			}

			if (flag) {
				// to avoid infinite loop
				// make new directed path
				directedPaths.Add (new List<Vector3> ());
				directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [0]);
				directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [1]);
				segmentsWithContour.RemoveRange (0, 2);
			}

		}

		// find outer loop (nearest to infinite) and reverse it if needed
		int outerDP = -1;
		{
			float outerDst = float.MaxValue;
			for (int i = 0; i < directedPaths.Count; i++) {
				int minSeg = -1;
				float dst = float.MaxValue;
				for (int j = 0; j < directedPaths [i].Count; j += 2) {
					float det = (directedPaths [i] [j] - Vector3.right * 1000000).sqrMagnitude;
					det += (directedPaths [i] [j + 1] - Vector3.right * 1000000).sqrMagnitude;
					if (det < dst) {
						dst = det;
						minSeg = j;
					}
				}

				if (minSeg != -1) {
					if (Vector3.Dot (Vector3.Normalize (directedPaths [i] [minSeg] - directedPaths [i] [minSeg + 1]), Vector3.forward) < 0) {
						directedPaths [i].Reverse ();
					}
					if (outerDst > dst) {
						outerDst = dst;
						outerDP = i;
					}
				}
			}

			List<Line> outer = new List<Line> ();
			List<Vector3> tmpverts = new List<Vector3> ();
			tmpverts.AddRange (directedPaths [outerDP]);
			for (int j = 0; j < directedPaths [outerDP].Count; j += 2) {
				Line ll = new Line (tmpverts, j, j + 1, 0, null, null, null, null);
				ll.Destroy ();

				ll.Thickness = inset * 2.0f;
				outer.Add (ll);
			}
	
			Line.WeldVertices (outer);
			List<Line> inner = innerOffset (outer);
			//cap inner

			List<int> triangles;
			List<Vector3> verts;
			List<Vector2> uvs;
			List<Vector3> normals;

			try {
				Line.FillCap (inner, out triangles, out verts, out uvs, out normals);
			


				//bridge inner/outer
				Line[] near2Outer2 = new Line[4];
				float[] dst2 = { float.MaxValue, float.MaxValue };
				for (int i = 0; i < inner.Count; i++) {
					float det;
					det = ((inner [i].a + inner [i].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;
					if (dst2 [0] > det) {
						dst2 [0] = det;
						near2Outer2 [0] = inner [i];
						near2Outer2 [1] = null;
					} else if (Mathf.Abs (dst2 [0] - det) <= Line.epsilon) {
						near2Outer2 [1] = inner [i];
					}
				}

				for (int i = 0; i < outer.Count; i++) {
					float det;
					det = ((outer [i].a + outer [i].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;
					if (dst2 [1] > det) {
						dst2 [1] = det;
						near2Outer2 [2] = outer [i];
						near2Outer2 [3] = null;
					} else if (Mathf.Abs (dst2 [1] - det) <= Line.epsilon) {
						near2Outer2 [3] = outer [i];
					}
				}

				infiniteRandomPoint = new Vector3 (-infiniteRandomPoint.z, infiniteRandomPoint.y, infiniteRandomPoint.x);
				dst2 = new float[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue };

				dst2 [0] = ((near2Outer2 [0].a + near2Outer2 [0].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;
				if (near2Outer2 [1] != null)
					dst2 [1] = ((near2Outer2 [1].a + near2Outer2 [1].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;

				dst2 [2] = ((near2Outer2 [2].a + near2Outer2 [2].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;
				if (near2Outer2 [3] != null)
					dst2 [3] = ((near2Outer2 [3].a + near2Outer2 [3].b) * 0.5f - infiniteRandomPoint).sqrMagnitude;
			
				int min1 = dst2 [0] < dst2 [1] ? 0 : 1;
				int min2 = dst2 [2] < dst2 [3] ? 2 : 3;

				List<Vector3> newLines = new List<Vector3> ();
				for (int i = 0; i < inner.Count; i++) {
					if (inner [i] != near2Outer2 [min1]) {
						newLines.Add (inner [i].a);
						newLines.Add (inner [i].b);
					}
				}
				for (int i = 0; i < outer.Count; i++) {
					if (outer [i] != near2Outer2 [min2]) {
						newLines.Add (outer [i].a);
						newLines.Add (outer [i].b);
					}
				}

				dst2 = new float[2];
				dst2 [0] = (near2Outer2 [min1].a - near2Outer2 [min2].a).sqrMagnitude;
				dst2 [1] = (near2Outer2 [min1].a - near2Outer2 [min2].b).sqrMagnitude;

				List<Vector3> lastQuad = new List<Vector3> ();

				if (dst2 [0] < dst2 [1]) {
					newLines.Add (near2Outer2 [min1].a);
					newLines.Add (near2Outer2 [min2].a);
					newLines.Add (near2Outer2 [min1].b);
					newLines.Add (near2Outer2 [min2].b);

					lastQuad.Add (near2Outer2 [min1].a);
					lastQuad.Add (near2Outer2 [min2].a);
					lastQuad.Add (near2Outer2 [min1].b);
					lastQuad.Add (near2Outer2 [min1].b);
					lastQuad.Add (near2Outer2 [min2].a);
					lastQuad.Add (near2Outer2 [min2].b);


				} else {
					newLines.Add (near2Outer2 [min1].a);
					newLines.Add (near2Outer2 [min2].b);
					newLines.Add (near2Outer2 [min1].b);
					newLines.Add (near2Outer2 [min2].a);

					lastQuad.Add (near2Outer2 [min1].a);
					lastQuad.Add (near2Outer2 [min1].b);
					lastQuad.Add (near2Outer2 [min2].a);
					lastQuad.Add (near2Outer2 [min1].a);
					lastQuad.Add (near2Outer2 [min2].a);
					lastQuad.Add (near2Outer2 [min2].b);
				}



				List<Line> CapLines = new List<Line> ();
				for (int i = 0; i < newLines.Count; i += 2) {
					Line l = new Line (newLines, i, i + 1, 0, null, null, null, null);
					l.Destroy ();
					CapLines.Add (l);
				}
				Line.WeldVertices (CapLines);

				List<int> triangles2;
				List<Vector3> verts2;
				List<Vector2> uvs2;
				List<Vector3> normals2;

				try {
					Line.FillCap (CapLines, out triangles2, out verts2, out uvs2, out normals2);

					for (int i = 0; i < triangles2.Count; i++) {
						triangles2 [i] += verts.Count;
					}



					List<Vector3> innerVertices = new List<Vector3> (verts);


					triangles.AddRange (triangles2);
					verts.AddRange (verts2);
					uvs.AddRange (uvs2);
					normals.AddRange (normals2);

					for (int i = 0; i < lastQuad.Count; i++) {
						triangles.Add (verts.IndexOf (lastQuad [i]));
					}



					for (int i = 0; i < verts.Count; i++) {
						if (innerVertices.FindIndex (delegate(Vector3 obj) {
							return (verts [i] - obj).sqrMagnitude <= Line.epsilon;
						}) != -1)
							verts [i] += Vector3.up * hatHeight;
					}

					for (int i = 0; i < verts.Count; i++) {
						verts [i] += Vector3.up * segments [0].Height;
					}

					_mesh = new Mesh () { vertices = verts.ToArray (), uv = uvs.ToArray (), triangles = triangles.ToArray () };
					_mesh.RecalculateNormals ();
				} catch {
					Debug.Log ("Cap roof !");
				}
			} catch {
				Debug.Log ("Roof Cap !");
			}

		}

		_filter = this.gameObject.AddComponent<MeshFilter> ();


		update ();
		_renderer = this.gameObject.AddComponent<MeshRenderer> ();
	}


	List<Line> innerOffset(List<Line> segments)
	{
		
		List<Vector3> segmentsWithContour = Line.Offsets (segments);
		// lines, lines offseted, lines offseted backward


		//segments.Clear ();
		HashSet<Vector3> vertices = new HashSet<Vector3> ();
		HashSet<int> linesContoured = new HashSet<int> ();
		// loop all lines, find intersection between offsets on the same side, then replace intersection point
		Line.WeldIntersections (segmentsWithContour, out vertices);

		// find end point edges
		List<Vector3> endpointsSegments = new List<Vector3> ();
		for (int i = 0; i < segmentsWithContour.Count; i += 6) {
			if (!vertices.Contains (segmentsWithContour [i])) {
				endpointsSegments.Add (segmentsWithContour [i + 2]);
				endpointsSegments.Add (segmentsWithContour [i + 4]);
			}
			if (!vertices.Contains (segmentsWithContour [i + 1])) {
				endpointsSegments.Add (segmentsWithContour [i + 3]);
				endpointsSegments.Add (segmentsWithContour [i + 5]);
			}
		}

		// remove original lines
		for (int i = segmentsWithContour.Count - 6; i >= 0; i -= 6) {
			segmentsWithContour.RemoveAt (i);
			segmentsWithContour.RemoveAt (i);
		}

		// add endpoint edges to contour list
		segmentsWithContour.AddRange (endpointsSegments);

		// find directed paths (to determine inner, outer walls)
		List<List<Vector3>> directedPaths = new List<List<Vector3>>();
		directedPaths.Add (new List<Vector3> ());
		directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [0]);
		directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [1]);
		segmentsWithContour.RemoveRange (0, 2);
		while (segmentsWithContour.Count > 0) {
			bool flag = true;
			for (int i = 0; i < segmentsWithContour.Count; i += 2) {
				if (directedPaths[directedPaths.Count - 1] [directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour [i]) {
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i]);
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i + 1]);
					segmentsWithContour.RemoveRange (i, 2);
					flag = false;
					break;
				}
				else if (directedPaths[directedPaths.Count - 1] [directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour [i + 1]) {
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i + 1]);
					directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [i]);
					segmentsWithContour.RemoveRange (i, 2);
					flag = false;
					break;
				}
			}

			if (flag) {
				// to avoid infinite loop
				// make new directed path
				directedPaths.Add (new List<Vector3> ());
				directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [0]);
				directedPaths[directedPaths.Count - 1].Add (segmentsWithContour [1]);
				segmentsWithContour.RemoveRange (0, 2);
			}
		}


		int outerDP = -1;
		{
			float outerDst = float.MaxValue;
			for (int i = 0; i < directedPaths.Count; i++) {
				int minSeg = -1;
				float dst = float.MaxValue;
				for (int j = 0; j < directedPaths [i].Count; j += 2) {
					float det = (directedPaths [i] [j] - Vector3.right * 1000000).sqrMagnitude;
					det += (directedPaths [i] [j + 1] - Vector3.right * 1000000).sqrMagnitude;
					if (det < dst) {
						dst = det;
						minSeg = j;
					}
				}

				if (minSeg != -1) {
					if (Vector3.Dot (Vector3.Normalize (directedPaths [i] [minSeg] - directedPaths [i] [minSeg + 1]), Vector3.forward) < 0) {
						directedPaths [i].Reverse ();
					}
					if (outerDst > dst) {
						outerDst = dst;
						outerDP = i;
					}
				}
			}
			if (outerDP != -1)
			{
				directedPaths.RemoveAt (outerDP);
				List<Line> output = new List<Line> ();
				for (int i = 0; i < directedPaths[0].Count; i += 2)
				{
					Line l = new Line (directedPaths [0], i, i + 1, 0, null, null, null, null);
					l.Destroy ();
					output.Add (l);
				}
				Line.WeldVertices (output);
				return output;
			}
		}

		return new List<Line> ();
	}


	public Vector3[] Vertices {
		get {
			return _mesh.vertices;
		}
		set{
			_mesh.vertices = value;
			update ();
		}
	}

	public int[] Indices
	{ 
		get{
			return _mesh.GetIndices (0);
		}
		set{
			_mesh.SetIndices (value, MeshTopology.Triangles, 0);
			update ();
		}
	}

	Mesh _mesh;
	MeshRenderer _renderer;
	MeshFilter _filter;

	void update()
	{
		_filter.mesh = _mesh;
	}
}
