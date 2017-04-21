using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UpperWallFace : MonoBehaviour
{
	public UpperWallFace ()
	{
	}

	public void Awake()
	{
		_mesh = new Mesh ();
	}

	class helper
	{
		public static bool anyEqualAny(List<Line> a, List<Line> b)
		{
			for (int i = 0; i < a.Count; i++) {
				for (int j = 0; j < b.Count; j++) {
					if (a [i] == b [j])
						return true;
				}
			}

			return false;
		}
	}

	public void CreateFromLines(List<Line> _segments)
	{
		if (_segments.Count == 0) {
			return;
		}

		Vector3 infiniteRandomPoint = new Vector3 (Random.value * 10000.0f + 1000.0f, 0.0f, Random.value * 1000.0f + 10000.0f);

		List<Line> segments = new List<Line> ();

		List<Vector3> vbuffer = new List<Vector3> ();

		// split segment to multiple segments for windows
		for (int i = 0; i < _segments.Count; i++) {
//			if (_segments [i].Windows.Count != 0) {
//
//				if (_segments [i].Windows [0].Position.x != 0) {
//
//					int id1 = vbuffer.IndexOf (_segments [i].a);
//					if (id1 == -1) {
//						id1 = vbuffer.Count;
//						vbuffer.Add (_segments [i].a);
//					}
//					Vector3 v2 = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * _segments [i].Windows [0].Position.x;
//					int id2 = vbuffer.IndexOf (v2);
//					if (id2 == -1) {
//						id2 = vbuffer.Count;
//						vbuffer.Add (v2);
//					}
//
//					Line firstSeg = new Line (vbuffer, id1, id2, _segments[i].Thickness, _segments [i].LineMaterial, _segments [i].InnerMaterial, _segments [i].OuterMaterial, _segments [i].SideMaterial);
//					firstSeg.Height = _segments [i].Height;
//					firstSeg.LineType = LineType.Wall;
//					firstSeg.ParentLine = _segments [i];
//					segments.Add (firstSeg);
//				}
//
//				for (int j = 0; j < _segments[i].Windows.Count - 1; j++) {
//					Vector3 start = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * _segments [i].Windows [j].Position.x;
//					Vector3 end = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * (_segments [i].Windows [j].Position.x + _segments [i].Windows [j].WindowWidth);
//
//					int istart = vbuffer.IndexOf (start);
//					if (istart == -1) {
//						istart = vbuffer.Count;
//						vbuffer.Add (start);
//					}
//					int iend = vbuffer.IndexOf (end);
//					if (iend == -1) {
//						iend = vbuffer.Count;
//						vbuffer.Add (end);
//					}
//
//
//					Line windowSeg = new Line (vbuffer, istart, iend, _segments[i].Thickness, _segments [i].LineMaterial, _segments [i].InnerMaterial, _segments [i].OuterMaterial, _segments [i].SideMaterial);
//					windowSeg.LedgeHeight = _segments [i].Windows [j].Position.y;
//					windowSeg.WindowHeight = _segments [i].Windows [j].WindowHeight;
//					windowSeg.LineType = LineType.Window;
//					windowSeg.Height = _segments [i].Height;
//					windowSeg.ParentLine = _segments [i];
//					segments.Add (windowSeg);
//
//					Vector3 nextStart = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * _segments [i].Windows [j + 1].Position.x;
//					int inextStart = vbuffer.IndexOf (nextStart);
//					if (inextStart == -1) {
//						inextStart = vbuffer.Count;
//						vbuffer.Add (nextStart);
//					}
//					Line nextSeg = new Line(vbuffer, iend, inextStart, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments [i].OuterMaterial, _segments [i].SideMaterial);
//					nextSeg.Height = _segments [i].Height;
//					nextSeg.LineType = LineType.Wall;
//					nextSeg.ParentLine = _segments [i];
//					segments.Add (nextSeg);
//				}
//
//				{
//					Vector3 start = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * _segments [i].Windows [_segments [i].Windows.Count - 1].Position.x;
//					Vector3 end = _segments [i].a + (_segments [i].b - _segments [i].a).normalized * (_segments [i].Windows [_segments [i].Windows.Count - 1].Position.x + _segments [i].Windows [_segments [i].Windows.Count - 1].WindowWidth);
//					int istart = vbuffer.IndexOf (start);
//					if (istart == -1) {
//						istart = vbuffer.Count;
//						vbuffer.Add (start);
//					}
//					int iend = vbuffer.IndexOf (end);
//					if (iend == -1) {
//						iend = vbuffer.Count;
//						vbuffer.Add (end);
//					}
//
//
//					Line windowSeg = new Line (vbuffer, istart, iend, _segments[i].Thickness, _segments [i].LineMaterial, _segments [i].InnerMaterial, _segments [i].OuterMaterial, _segments [i].SideMaterial);
//					windowSeg.LedgeHeight = _segments [i].Windows [_segments [i].Windows.Count - 1].Position.y;
//					windowSeg.WindowHeight = _segments [i].Windows [_segments [i].Windows.Count - 1].WindowHeight;
//					windowSeg.LineType = LineType.Window;
//					windowSeg.Height = _segments [i].Height;
//					windowSeg.ParentLine = _segments [i];
//					segments.Add (windowSeg);
//
//					int id2 = vbuffer.IndexOf (_segments [i].b);
//					if (id2 == -1) {
//						id2 = vbuffer.Count;
//						vbuffer.Add (_segments [i].b);
//					}
//
//					Line lastSeg = new Line(vbuffer, iend, id2, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments [i].OuterMaterial, _segments [i].SideMaterial);
//					lastSeg.Height = _segments [i].Height;
//					lastSeg.LineType = LineType.Wall;
//					lastSeg.ParentLine = _segments [i];
//					segments.Add (lastSeg);
//				}
//
//
//
//			} else 
			{
				segments.Add (_segments [i]);
			}
		}



		List<Vector3> segmentsWithContour = Line.Offsets (segments);
		// lines, lines offseted, lines offseted backward

		//segments.Clear ();
		HashSet<Vector3> vertices =	 new HashSet<Vector3> ();
		HashSet<int> linesContoured = new HashSet<int> ();
		// loop all lines, find intersection between offsets on the same side, then replace intersection point

		Line.WeldIntersections(segmentsWithContour, out vertices);

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
		for (int i = 0; i < directedPaths.Count; i++) 
		{
			int minSeg = -1;
			float dst = float.MaxValue;
			for (int j = 0; j < directedPaths[i].Count; j += 2) {
				float det = (directedPaths[i] [j] - Vector3.right * 1000.0f).sqrMagnitude;
				det += (directedPaths[i] [j + 1] - Vector3.right * 1000.0f).sqrMagnitude;
				if (det < dst) {
					dst = det;
					minSeg = j;
				}
			}

			if (minSeg != -1) {
				if (Vector3.Dot (Vector3.Normalize (directedPaths [i] [minSeg] - directedPaths [i] [minSeg + 1]), Vector3.forward) < 0) {
					directedPaths [i].Reverse ();
				}
			}
		}



		float thicknessSqrd = _segments [0].Thickness * _segments [0].Thickness;//thickness * thickness;

		List<List<Line>> pathId = new List<List<Line>> ();
		for (int i = 0; i < directedPaths.Count; i++)
			pathId.Add (new List<Line>());
		

		// for each path find intersection count from (any vertex) to (infinite) which will determine if this is inner or outer wall
		// then generate walls or windows
		// destroy line if its for the window (not original line)
		for (int i = 0; i < directedPaths.Count; i++) {

			float pathidDst = 0;
			// bug .. [\] will not work correctly
			int rand = Random.Range(0, directedPaths[i].Count / 2) * 2;
			Vector3 randomVertex = Vector3.Lerp (directedPaths [i] [rand], directedPaths [i] [rand + 1], Random.Range (1, 99) / 100.0f);

			Vector3 randomOutterPoint = new Vector3 (Random.Range(10000.0f, 50000.0f), 0, Random.Range(10000.0f, 50000.0f));

			int intersectionCount = 0;
			for (int j = 0; j < directedPaths.Count; j++) 
			{
				if (i != j) {
					for (int k = 0; k < directedPaths[j].Count; k += 2) {

						Vector3 tmp;
						if (Line.RayRayIntersection (out tmp, directedPaths [j] [k], directedPaths [j] [k + 1], randomVertex, randomOutterPoint)) {
							if ((tmp - randomOutterPoint).magnitude <= (randomVertex - randomOutterPoint).magnitude) {
								if ((tmp - directedPaths [j] [k]).magnitude <= (directedPaths [j] [k + 1] - directedPaths [j] [k]).magnitude){
									if (Vector3.Dot (tmp - directedPaths [j] [k], directedPaths [j] [k + 1] - directedPaths [j] [k]) >= 0) {
										intersectionCount++;
									}
								}
							}
						}
					}
				}
			}



			if (intersectionCount % 2 == 1) {
				directedPaths [i].Reverse ();

				for (int j = 0; j < directedPaths [i].Count; j += 2) {
					if (Mathf.Abs ((directedPaths [i] [j] - directedPaths [i] [j + 1]).sqrMagnitude - thicknessSqrd) <= 0.0001f) {
						Line l = null;
						float dst = float.MaxValue;
						for (int k = 0; k < segments.Count; k++) {
							float det1 = (segments [k].a - directedPaths [i] [j]).sqrMagnitude;
							if (det1 < dst) {
								dst = det1;
								l = segments [k];
							} else {
								float det2 = (segments [k].b - directedPaths [i] [j]).sqrMagnitude;
								if (det2 < dst) {
									dst = det2;
									l = segments [k];
								}
							}
						}


						if (pathId [i].Count == 0) {
							pathId [i].Add (l.ParentLine == null ? l : l.ParentLine);
							pathidDst = (infiniteRandomPoint - pathId [i] [0].a).sqrMagnitude + (infiniteRandomPoint - pathId [i] [0].b).sqrMagnitude;
						}
						else {
							Line tmp = l.ParentLine == null ? l : l.ParentLine;
							float det = (infiniteRandomPoint - tmp.a).sqrMagnitude + (infiniteRandomPoint - tmp.b).sqrMagnitude;
							if (det > pathidDst) {
								pathId [i].Clear ();
								pathidDst = det;
								pathId [i].Add (tmp);
							} else if (det == pathidDst) {
								pathId [i].Add (tmp);
							}
						}

						if (l.ParentLine != null)
							l.Destroy ();
					} else {

						Line l = null;
						float dst = float.MaxValue;
						for (int k = 0; k < segments.Count; k++) {
							float det1 = (segments [k].a - directedPaths [i] [j]).sqrMagnitude;
							det1 += (segments [k].b - directedPaths [i] [j + 1]).sqrMagnitude;
							if (det1 < dst) {
								dst = det1;
								l = segments [k];
							} 
							float det2 = (segments [k].b - directedPaths [i] [j]).sqrMagnitude;
							det2 += (segments [k].a - directedPaths [i] [j + 1]).sqrMagnitude;
							if (det2 < dst) {
								dst = det2;
								l = segments [k];
							}

						}

						if (pathId [i].Count == 0) {
							pathId [i].Add (l.ParentLine == null ? l : l.ParentLine);
							pathidDst = (infiniteRandomPoint - pathId [i] [0].a).sqrMagnitude + (infiniteRandomPoint - pathId [i] [0].b).sqrMagnitude;
						}
						else {
							Line tmp = l.ParentLine == null ? l : l.ParentLine;
							float det = (infiniteRandomPoint - tmp.a).sqrMagnitude + (infiniteRandomPoint - tmp.b).sqrMagnitude;
							if (det > pathidDst) {
								pathId [i].Clear ();
								pathidDst = det;
								pathId [i].Add (tmp);
							} else if (det == pathidDst) {
								pathId [i].Add (tmp);
							}
						}

						if (l.ParentLine != null)
							l.Destroy ();
					}
				}
			} else {
				for (int j = 0; j < directedPaths [i].Count; j += 2) {

					if (Mathf.Abs ((directedPaths [i] [j] - directedPaths [i] [j + 1]).sqrMagnitude - thicknessSqrd) <= 0.0001f) {
						Line l = null;
						float dst = float.MaxValue;
						for (int k = 0; k < segments.Count; k++) {
							float det1 = (segments [k].a - directedPaths [i] [j]).sqrMagnitude;
							if (det1 < dst) {
								dst = det1;
								l = segments [k];
							} else {
								float det2 = (segments [k].b - directedPaths [i] [j]).sqrMagnitude;
								if (det2 < dst) {
									dst = det2;
									l = segments [k];
								}
							}
						}

						if (pathId [i].Count == 0) {
							pathId [i].Add (l.ParentLine == null ? l : l.ParentLine);
							pathidDst = (infiniteRandomPoint - pathId [i][0].a).sqrMagnitude + (infiniteRandomPoint - pathId [i][0].b).sqrMagnitude;
						}
						else {
							Line tmp = l.ParentLine == null ? l : l.ParentLine;
							float det = (infiniteRandomPoint - tmp.a).sqrMagnitude + (infiniteRandomPoint - tmp.b).sqrMagnitude;
							if (det > pathidDst) {
								pathId [i].Clear ();
								pathidDst = det;
								pathId [i].Add (tmp);
							} else if (det == pathidDst) {
								pathId [i].Add (tmp);
							}
						}

						if (l.ParentLine != null)
							l.Destroy ();
					} else {

						Line l = null;
						float dst = float.MaxValue;
						for (int k = 0; k < segments.Count; k++) {
							float det1 = (segments [k].a - directedPaths [i] [j]).sqrMagnitude;
							det1 += (segments [k].b - directedPaths [i] [j + 1]).sqrMagnitude;
							if (det1 < dst) {
								dst = det1;
								l = segments [k];
							} 
							float det2 = (segments [k].b - directedPaths [i] [j]).sqrMagnitude;
							det2 += (segments [k].a - directedPaths [i] [j + 1]).sqrMagnitude;
							if (det2 < dst) {
								dst = det2;
								l = segments [k];
							}

						}

						if (pathId [i].Count == 0) {
							pathId [i].Add (l.ParentLine == null ? l : l.ParentLine);
							pathidDst = (infiniteRandomPoint - pathId [i] [0].a).sqrMagnitude + (infiniteRandomPoint - pathId [i] [0].b).sqrMagnitude;
						}
						else {
							Line tmp = l.ParentLine == null ? l : l.ParentLine;
							float det = (infiniteRandomPoint - tmp.a).sqrMagnitude + (infiniteRandomPoint - tmp.b).sqrMagnitude;
							if (det > pathidDst) {
								pathId [i].Clear ();
								pathidDst = det;
								pathId [i].Add (tmp);
							} else if (det == pathidDst) {
								pathId [i].Add (tmp);
							}
						}

						if (l.ParentLine != null)
							l.Destroy ();
					}

				}
			}
		}

		List<Vector3> __Vertices = new List<Vector3> ();
		List<CombineInstance> combineInstances = new List<CombineInstance> ();
		List<Material> sideMaterials = new List<Material> ();
		for (int i = directedPaths.Count - 1; i >= 0; i--) {
			int dpcount = 0;
			int secondID = -1;
			for (int j = 0; j < directedPaths.Count; j++) {
				if (i >= pathId.Count || j >= pathId.Count) {
					i--;
				
				}

				if (pathId [i] != null && i != j && helper.anyEqualAny (pathId [i], pathId [j])) {
					if (dpcount > 2)
						throw new UnityException ("More than 2 directed pathes for same line !");
					dpcount++;
					secondID = j;
				}
			}

			if (dpcount == 1) {
				// find nearest 2 lines from each directed path
				Vector3 l1v1 = directedPaths [i] [directedPaths [i].Count - 1];
				Vector3 l1v2 = directedPaths [i] [directedPaths [i].Count - 2];
				directedPaths [i].RemoveAt (directedPaths [i].Count - 1);
				directedPaths [i].RemoveAt (directedPaths [i].Count - 1);
				Vector3 tmp = (l1v1 + l1v2) * 0.5f;
				float dst = float.MaxValue;
				int selectedj = -1;
				for (int j = 0; j < directedPaths [secondID].Count; j += 2) {
					float det = ((directedPaths [secondID] [j] + directedPaths [secondID] [j + 1]) * 0.5f - tmp).sqrMagnitude;
					if (dst > det) {
						dst = det;
						selectedj = j;
					}
				}

				Vector3 l2v1 = directedPaths [secondID] [selectedj];
				Vector3 l2v2 = directedPaths [secondID] [selectedj + 1];
				directedPaths [secondID].RemoveAt (selectedj);
				directedPaths [secondID].RemoveAt (selectedj);

				if ((l2v2 - l1v1).sqrMagnitude < (l2v1 - l1v1).sqrMagnitude) {
					tmp = l2v1;
					l2v1 = l2v2;
					l2v2 = tmp;
				}


				List<Line> directedPath = new List<Line> ();
				for (int j = 0; j < directedPaths [i].Count; j += 2) {
					int id1 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [i] [j] - obj).sqrMagnitude <= 0.0000001f;
					});
					int id2 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [i] [j + 1] - obj).sqrMagnitude <= 0.0000001f;
					});

					if (id1 == -1) {
						id1 = __Vertices.Count;
						__Vertices.Add (directedPaths [i] [j]);
					}
					if (id2 == -1) {
						id2 = __Vertices.Count;
						__Vertices.Add (directedPaths [i] [j + 1]);
					}

					directedPath.Add (new Line (__Vertices, id1, id2, 1, null, null, null, pathId [i] [0].SideMaterial));
				}

				{
					int id1 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (l1v1 - obj).sqrMagnitude <= 0.0000001f;
					});
					int id2 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (l2v1 - obj).sqrMagnitude <= 0.0000001f;
					});

					if (id1 == -1) {
						id1 = __Vertices.Count;
						__Vertices.Add (l1v1);
					}
					if (id2 == -1) {
						id2 = __Vertices.Count;
						__Vertices.Add (l2v1);
					}

					directedPath.Add (new Line (__Vertices, id1, id2, 1, null, null, null, pathId [i] [0].SideMaterial));
				}

				{
					int id1 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (l1v2 - obj).sqrMagnitude <= 0.0000001f;
					});
					int id2 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (l2v2 - obj).sqrMagnitude <= 0.0000001f;
					});

					if (id1 == -1) {
						id1 = __Vertices.Count;
						__Vertices.Add (l1v2);
					}
					if (id2 == -1) {
						id2 = __Vertices.Count;
						__Vertices.Add (l2v2);
					}

					directedPath.Add (new Line (__Vertices, id1, id2, 1, null, null, null, pathId [i] [0].SideMaterial));
				}

				for (int j = 0; j < directedPaths [secondID].Count; j += 2) {
					int id1 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [secondID] [j] - obj).sqrMagnitude <= 0.0000001f;
					});
					int id2 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [secondID] [j + 1] - obj).sqrMagnitude <= 0.0000001f;
					});

					if (id1 == -1) {
						id1 = __Vertices.Count;
						__Vertices.Add (directedPaths [secondID] [j]);
					}
					if (id2 == -1) {
						id2 = __Vertices.Count;
						__Vertices.Add (directedPaths [secondID] [j + 1]);
					}

					directedPath.Add (new Line (__Vertices, id1, id2, 1, null, null, null, pathId [secondID] [0].SideMaterial));
				

				}

				// final quad
				Line.WeldVertices (directedPath);
				List<int> indices;
				List<Vector2> uvs;
				List<Vector3> verts;
				List<Vector3> normals;
<<<<<<< HEAD
				Line.FillCap (directedPath, out indices, out verts, out uvs, out normals);
				CombineInstance ci = new CombineInstance ();
				ci.mesh = new Mesh ();
				for (int j = 0; j < verts.Count; j++) {
					verts [j] = new Vector3 (verts [j].x, pathId [i] [0].Height, verts [j].z);
				}
				ci.mesh.vertices = verts.ToArray ();
				ci.mesh.uv = uvs.ToArray ();
				ci.mesh.normals = normals.ToArray ();
				ci.mesh.SetIndices (indices.ToArray (), MeshTopology.Triangles, 0);
				ci.transform = Matrix4x4.identity;

				ci.subMeshIndex = combineInstances.Count;
				combineInstances.Add (ci);
				sideMaterials.Add (pathId [i] [0].SideMaterial);
				pathId.Add (pathId [i]);
				pathId.RemoveAt (i);
				pathId.RemoveAt (secondID);
				directedPaths.RemoveAt (i);
				directedPaths.RemoveAt (secondID);
				List<Vector3> finalQuad = new List<Vector3> ();
				finalQuad.Add (l1v1);
				finalQuad.Add (l1v2);
				finalQuad.Add (l2v1);
				finalQuad.Add (l2v2);
				finalQuad.Add (l1v1);
				finalQuad.Add (l2v1);
				finalQuad.Add (l1v2);
				finalQuad.Add (l2v2);
				directedPaths.Add (finalQuad);
=======
				try {
					Line.FillCap (directedPath, out indices, out verts, out uvs, out normals);
				
					CombineInstance ci = new CombineInstance ();
					ci.mesh = new Mesh ();
					for (int j = 0; j < verts.Count; j++) {
						verts [j] = new Vector3 (verts [j].x, pathId [i] [0].Height, verts [j].z);
					}
					ci.mesh.vertices = verts.ToArray ();
					ci.mesh.uv = uvs.ToArray ();
					ci.mesh.normals = normals.ToArray ();
					ci.mesh.SetIndices (indices.ToArray (), MeshTopology.Triangles, 0);
					ci.transform = Matrix4x4.identity;

					ci.subMeshIndex = combineInstances.Count;
					combineInstances.Add (ci);
					sideMaterials.Add (pathId [i] [0].SideMaterial);
					pathId.Add (pathId [i]);
					pathId.RemoveAt (i);
					pathId.RemoveAt (secondID);
					directedPaths.RemoveAt (i);
					directedPaths.RemoveAt (secondID);
					List<Vector3> finalQuad = new List<Vector3> ();
					finalQuad.Add (l1v1);
					finalQuad.Add (l1v2);
					finalQuad.Add (l2v1);
					finalQuad.Add (l2v2);
					finalQuad.Add (l1v1);
					finalQuad.Add (l2v1);
					finalQuad.Add (l1v2);
					finalQuad.Add (l2v2);
					directedPaths.Add (finalQuad);
>>>>>>> master
//				pathId.Add (null);
//				i--;
				} catch {
					Debug.Log ("Cap Upper wall face !!");
				}
			} else {
				List<Line> directedPath = new List<Line> ();
				for (int j = 0; j < directedPaths [i].Count; j += 2) {
					int id1 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [i] [j] - obj).sqrMagnitude <= 0.0000001f;
					});
					int id2 = __Vertices.FindIndex (delegate(Vector3 obj) {
						return (directedPaths [i] [j + 1] - obj).sqrMagnitude <= 0.0000001f;
					});

					if (id1 == -1) {
						id1 = __Vertices.Count;
						__Vertices.Add (directedPaths [i] [j]);
					}
					if (id2 == -1) {
						id2 = __Vertices.Count;
						__Vertices.Add (directedPaths [i] [j + 1]);
					}

					directedPath.Add (new Line (__Vertices, id1, id2, 1, null, null, null, pathId [i] [0].SideMaterial));
				}

				Line.WeldVertices (directedPath);
				List<int> indices;
				List<Vector2> uvs;
				List<Vector3> verts;
				List<Vector3> normals;
				try {
					Line.FillCap (directedPath, out indices, out verts, out uvs, out normals);
					CombineInstance ci = new CombineInstance ();
					ci.mesh = new Mesh ();
					for (int j = 0; j < verts.Count; j++) {
						verts [j] = new Vector3 (verts [j].x, pathId [i] [0].Height, verts [j].z);
					}


					ci.mesh.vertices = verts.ToArray ();
					ci.mesh.uv = uvs.ToArray ();
					ci.mesh.normals = normals.ToArray ();
					ci.mesh.SetIndices (indices.ToArray (), MeshTopology.Triangles, 0);
					ci.transform = Matrix4x4.identity;

					ci.subMeshIndex = 0;//combineInstances.Count;
					combineInstances.Add (ci);
				} catch {
<<<<<<< HEAD
=======
					Debug.Log ("Cap upper wall face !");
>>>>>>> master
				}
				sideMaterials.Add (pathId [i] [0].SideMaterial);
			}
		}


		_filter = this.gameObject.AddComponent<MeshFilter> ();


		_mesh.Clear ();
		_mesh.CombineMeshes (combineInstances.ToArray ());
		update ();
		_renderer = this.gameObject.AddComponent<MeshRenderer> ();
		_renderer.materials = sideMaterials.ToArray ();

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


