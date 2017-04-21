using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum LineType
{
    Wall,
    Window
};

public class Line
{
    int _a;
    int _b;
    public LineType LineType = LineType.Wall;

    public List<WallWindow> Windows = new List<WallWindow>();
    public List<WallDoor> Doors = new List<WallDoor>();

    /// <summary>
    /// The parent line, used if this is window (part of parentLine)
    /// </summary>
    public Line ParentLine = null;

    public float Height = 2;
    public float LedgeHeight = 0;
    public float WindowHeight = 0;
    public Vector3 a
    {
        get
        {
            return Vertices[_a];
        }
        set
        {
            Vertices[_a] = value;
			if (lr != null)
				lr.SetPosition (0, Vertices [_a]);
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Update();
            }

			for (int i = 0; i < Doors.Count; i++)
			{
				Doors[i].Update();
			}
        }
    }
    public Vector3 b
    {
        get
        {
            return Vertices[_b];
        }
        set
        {
            Vertices[_b] = value;
			if (lr != null)
				lr.SetPosition (1, Vertices [_b]);
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Update();
            }

			for (int i = 0; i < Doors.Count; i++)
			{
				Doors[i].Update();
			}
        }
    }

    public int aID
    {
        get
        {
            return _a;
        }
        set
        {
            _a = value;
            if (lr != null)
            {
                lr.SetPosition(0, Vertices[_a]);
                for (int i = 0; i < Windows.Count; i++)
                {
                    Windows[i].Update();
                }

				for (int i = 0; i < Doors.Count; i++)
				{
					Doors[i].Update();
				}
            }
        }
    }
    public int bID
    {
        get
        {
            return _b;
        }
        set
        {
            _b = value;
            if (lr != null)
            {
                lr.SetPosition(1, Vertices[_b]);
                for (int i = 0; i < Windows.Count; i++)
                {
                    Windows[i].Update();
                }

				for (int i = 0; i < Doors.Count; i++)
				{
					Doors[i].Update();
				}
            }
        }
    }

    public void DetachA()
    {
        int id = Vertices.Count;
        Vertices.Add(a);
        aID = id;
    }

    public void DetachB()
    {
        int id = Vertices.Count;
        Vertices.Add(b);
        bID = id;
    }

    public Material InnerMaterial;
    public Material OuterMaterial;
    public Material SideMaterial;

    public Material LineMaterial
    {
        get
        {
            return lr.material;
        }
        set
        {
            lr.material = value;
        }
    }
    LineRenderer lr;

    public float Thickness;

    public List<Vector3> Vertices { get; set; }

    public Line(List<Vector3> vertices, int a, int b, float thickness, Material mat, Material innerMaterial, Material outerMaterial, Material sideMaterial)
    {
        Vertices = vertices;
        if (mat != null)
        {
            lr = new GameObject("line").AddComponent<LineRenderer>();
            lr.gameObject.AddComponent<MeshCollider>();
            this.LineMaterial = mat;
            lr.numPositions = 2;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            //lr.SetVertexCount(2);
            //lr.SetWidth(0.05f, 0.05f);
            lr.useWorldSpace = true;
        }
        Thickness = thickness;
        aID = a;
        bID = b;
        InnerMaterial = innerMaterial;
        OuterMaterial = outerMaterial;
        SideMaterial = sideMaterial;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Line"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public bool Enabled
    {
        get
        {
            return lr.enabled;
        }
        set
        {
            lr.enabled = value;
        }
    }

    /// <summary>
    /// Gets the transform.
    /// </summary>
    /// <value>The transform.</value>
    public Transform transform
    {
        get
        {
            return lr.gameObject.transform;
        }
    }

    /// <summary>
    /// Gets or sets the parent.
    /// </summary>
    /// <value>The parent.</value>
    public Transform Parent
    {
        get
        {
            return lr.gameObject.transform.parent;
        }
        set
        {
            lr.gameObject.transform.parent = value;
        }
    }

    public void Destroy()
    {
        if (lr != null)
        {
            GameObject.Destroy(lr);
            GameObject.DestroyImmediate(lr.gameObject);
            lr = null;
        }

		for (int i = 0; i < Windows.Count; i++) {
			GameObject.Destroy (Windows [i].Window);
		}
		for (int i = 0; i < Doors.Count; i++) {
			GameObject.Destroy (Doors [i].Door);
		}
    }

    class helper
    {
        public static Bounds getBounds(Transform obj)
        {
            Renderer[] filters = obj.gameObject.GetComponentsInChildren<Renderer>(false);
            Bounds aabb;
            if (filters.Length == 0)
                return new Bounds();
            aabb = filters[0].bounds;
            for (int i = 0; i < filters.Length; ++i)
            {
                aabb.Encapsulate(filters[i].bounds);
            }
            return aabb;
        }
    }

    /// <summary>
    /// Locates the item in wall.
    /// </summary>
    /// <returns><c>true</c>, if item in wall was located in wall without problems, <c>false</c> otherwise.</returns>
    /// <param name="mousePosition">Mouse position.</param>
    /// <param name="item">Item.</param>
    /// <param name="location">Location.</param>
    public bool LocateItemInWall(Vector3 mousePosition, item item, out Vector2 location, int maxTries, out Vector2? correctedLocation)
    {
        correctedLocation = null;
        float xDistance = (new Vector3(mousePosition.x, 0, mousePosition.z) - a).magnitude;

        float wallWidth = (b - a).magnitude;
        Bounds aabb = helper.getBounds(item.prefabItem.gameObject.transform);
        float itemWidth = aabb.max.x - aabb.min.x;
        float itemHeight = aabb.max.y - aabb.min.y;
        xDistance -= 0.5f * itemWidth;
        location.x = xDistance;
        location.y = item.MarginDown;

        locateItemInWall(xDistance, wallWidth, itemWidth, itemHeight, item, maxTries, out correctedLocation);
        if (correctedLocation.HasValue)
        {
            if (correctedLocation.Value == location)
                return true;
            else
                return false;
        }
        else
            return false;

    }

    private bool locateItemInWall(float xDistance, float wallWidth, float itemWidth, float itemHeight, item item, int maxTries, out Vector2? correctedLocation)
    {
        if (maxTries <= 0)
        {
            correctedLocation = null;
            return false;
        }

        for (int i = 0; i < Windows.Count; i++)
        {
            if (xDistance + itemWidth + item.HorizontalMargin > Windows[i].Position.x &&
                xDistance - item.HorizontalMargin < Windows[i].Position.x + Windows[i].WindowWidth)
            {
                // is it (to the left or to the right) of window ?
                if (xDistance < Windows[i].Position.x)
                {
                    return locateItemInWall(Windows[i].Position.x - item.HorizontalMargin - itemWidth, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else if (xDistance > Windows[i].Position.x)
                {
                    return locateItemInWall(Windows[i].Position.x + Windows[i].WindowWidth + item.HorizontalMargin, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else if (Random.value > 0.5f)
                {
                    return locateItemInWall(Windows[i].Position.x - item.HorizontalMargin - itemWidth, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else
                {
                    return locateItemInWall(Windows[i].Position.x + Windows[i].WindowWidth + item.HorizontalMargin, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
            }
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (xDistance + itemWidth + item.HorizontalMargin > Doors[i].Position.x &&
                xDistance - item.HorizontalMargin < Doors[i].Position.x + Doors[i].DoorWidth)
            {

                // is it (to the left or to the right) of door ?
                if (xDistance < Doors[i].Position.x)
                {
                    return locateItemInWall(Doors[i].Position.x - item.HorizontalMargin - itemWidth, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else if (xDistance > Doors[i].Position.x)
                {
                    return locateItemInWall(Doors[i].Position.x + Doors[i].DoorWidth + item.HorizontalMargin, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else if (Random.value > 0.5f)
                {
                    return locateItemInWall(Doors[i].Position.x - item.HorizontalMargin - itemWidth, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
                else
                {
                    return locateItemInWall(Doors[i].Position.x + Doors[i].DoorWidth + item.HorizontalMargin, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
                }
            }
        }


		if (xDistance + epsilon >= item.HorizontalMargin)
        {
			if (xDistance + itemWidth <= wallWidth - item.HorizontalMargin + epsilon)
            {
                if (item.MarginTop + item.MarginDown + itemHeight < Height)
                {
                    correctedLocation = new Vector2?(new Vector2(xDistance, item.MarginDown));
                    return true;
                }
            }
            else
            {
                return locateItemInWall(wallWidth - item.HorizontalMargin - itemWidth, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
            }
        }
        else
        {
            return locateItemInWall(item.HorizontalMargin, wallWidth, itemWidth, itemHeight, item, maxTries - 1, out correctedLocation);
        }

        correctedLocation = null;
        return false;
    }



	public const float epsilon = 0.00001f;

    /// <summary>
    /// Ray - ray intersection.
    /// </summary>
    /// <returns><c>true</c>, if ray1, ray2 intersected, <c>false</c> otherwise.</returns>
    /// <param name="intersection">Intersection.</param>
    /// <param name="line1Point1">Line1 point1.</param>
    /// <param name="line1Point2">Line1 point2.</param>
    /// <param name="line2Point1">Line2 point1.</param>
    /// <param name="line2Point2">Line2 point2.</param>
    public static bool RayRayIntersection(out Vector3 intersection, Vector3 line1Point1, Vector3 line1Point2, Vector3 line2Point1, Vector3 line2Point2)
    {

        Vector3 lineVec3 = line2Point1 - line1Point1;
        Vector3 lineVec1 = line1Point1 - line1Point2;
        Vector3 lineVec2 = line2Point1 - line2Point2;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel

		if (Mathf.Abs(planarFactor) < epsilon)
        {

			if (crossVec1and2.sqrMagnitude > epsilon)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = line1Point1 + (lineVec1 * s);
                return true;
            }
            else
            { // parrallel
                float dst = float.MaxValue;
                float det;
                det = (line1Point1 - line2Point1).sqrMagnitude;
                {
                    dst = det;
                    intersection = line1Point1;
                }
                det = (line1Point1 - line2Point2).sqrMagnitude;
                if (det < dst)
                {
                    dst = det;
                    intersection = line1Point1;
                }
                det = (line1Point2 - line2Point1).sqrMagnitude;
                if (det < dst)
                {
                    dst = det;
                    intersection = line1Point2;
                }
                det = (line1Point2 - line2Point2).sqrMagnitude;
                if (det < dst)
                {
                    dst = det;
                    intersection = line1Point2;
                }

                return true;
            }
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }


    public static List<Line> Split(List<Line> lines, Vector3 point)
    {
        List<Line> output = new List<Line>(lines);
        for (int i = 0; i < lines.Count; i++)
        {
            if ((point - lines[i].a).sqrMagnitude < (lines[i].b - lines[i].a).sqrMagnitude)
            {
				if (Mathf.Abs((point - lines[i].a).magnitude + (point - lines[i].b).magnitude - (lines[i].b - lines[i].a).magnitude) <= epsilon)
                {
                    float length = (lines[i].b - lines[i].a).magnitude;
                    float dot = Vector3.Dot(point - lines[i].a, (lines[i].b - lines[i].a) / length);
					if (dot > epsilon && length - dot > epsilon)
                    {
                        int newid = output[i].Vertices.Count;
                        output[i].Vertices.Add(point);

                        Line l = new Line(output[i].Vertices, newid, output[i].bID, output[i].Thickness, output[i].LineMaterial, output[i].InnerMaterial, output[i].OuterMaterial, output[i].SideMaterial);
                        l.Enabled = output[i].Enabled;
                        l.Parent = output[i].Parent;
                        output[i].bID = newid;
                        output.Add(l);
                    }
                }
            }
        }
        return output;
    }



    public static List<Vector3> WeldVertices(List<Line> lines)
    {
        if (lines.Count == 0)
            return new List<Vector3>();



        List<Vector3> verts = new List<Vector3>(new HashSet<Vector3>(lines[0].Vertices));//, new vector3EqualityComparer()));

        for (int i = 0; i < lines.Count; i++)
        {
            int aid = verts.IndexOf(lines[i].a);
            int bid = verts.IndexOf(lines[i].b);
            lines[i].Vertices = verts;
            lines[i].aID = aid;
            lines[i].bID = bid;
        }
        return verts;
    }

    public static List<Vector3> Offsets(List<Line> edges)
    {
        List<Vector3> output = new List<Vector3>();
        for (int i = 0; i < edges.Count; i++)
        {

            Vector3 dir = Vector3.Cross(edges[i].a - edges[i].b, Vector3.up);
            dir.Normalize();
            dir *= edges[i].Thickness / 2.0f;

            output.Add(edges[i].a);
            output.Add(edges[i].b);

            output.Add(edges[i].a + dir);
            output.Add(edges[i].b + dir);

            output.Add(edges[i].a - dir);
            output.Add(edges[i].b - dir);
        }

        return output;
    }

    static bool isPointOverLine(Vector3 a, Vector3 b, Vector3 p)
    {
		return Mathf.Abs((a - b).magnitude - ((a - p).magnitude + (b - p).magnitude)) <= epsilon;
    }

    static void WeldInterstion(List<Vector3> segmentsWithContour, int l11, int l12, int l21, int l22)
    {
        Vector3 intersection;
        Vector3 mid = (segmentsWithContour[l11] + segmentsWithContour[l12]) * 0.5f;

        if (RayRayIntersection(out intersection, segmentsWithContour[l11], segmentsWithContour[l12], segmentsWithContour[l21], segmentsWithContour[l22]))
        {
            bool flag = true;//(mid - segmentsWithContour [l12]).sqrMagnitude > (mid - intersection).sqrMagnitude;
            if (flag)
            {
                for (int i = 0; i < segmentsWithContour.Count; i += 6)
                {
                    Vector3 tmp;
                    if (RayRayIntersection(out tmp, segmentsWithContour[l11], segmentsWithContour[l12], segmentsWithContour[i], segmentsWithContour[i + 1]) && isPointOverLine(segmentsWithContour[i], segmentsWithContour[i + 1], tmp))
                    {
                        if (Vector3.Dot(tmp - mid, intersection - mid) >= 0.0f && (intersection - mid).sqrMagnitude >= (tmp - mid).sqrMagnitude)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                mid = (segmentsWithContour[l21] + segmentsWithContour[l22]) * 0.5f;
                for (int i = 0; i < segmentsWithContour.Count; i += 6)
                {
                    Vector3 tmp;
                    if (RayRayIntersection(out tmp, segmentsWithContour[l21], segmentsWithContour[l22], segmentsWithContour[i], segmentsWithContour[i + 1]) && isPointOverLine(segmentsWithContour[i], segmentsWithContour[i + 1], tmp))
                    {
                        if (Vector3.Dot(tmp - mid, intersection - mid) >= 0.0f && (intersection - mid).sqrMagnitude >= (tmp - mid).sqrMagnitude)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                float f1 = (segmentsWithContour[l11] - intersection).sqrMagnitude;
                float f2 = (segmentsWithContour[l12] - intersection).sqrMagnitude;
                float f3 = (segmentsWithContour[l21] - intersection).sqrMagnitude;
                float f4 = (segmentsWithContour[l22] - intersection).sqrMagnitude;
                if (f1 < f2)
                {
                    segmentsWithContour[l11] = intersection;
                }
                else
                {
                    segmentsWithContour[l12] = intersection;
                }

                if (f3 < f4)
                {
                    segmentsWithContour[l21] = intersection;
                }
                else
                {
                    segmentsWithContour[l22] = intersection;
                }

            }
        }
        else
        {
            float f1 = (segmentsWithContour[l11] - segmentsWithContour[l21]).sqrMagnitude;
            float f2 = (segmentsWithContour[l11] - segmentsWithContour[l22]).sqrMagnitude;
            float f3 = (segmentsWithContour[l12] - segmentsWithContour[l21]).sqrMagnitude;
            float f4 = (segmentsWithContour[l12] - segmentsWithContour[l22]).sqrMagnitude;
            if (f1 < f2 && f1 < f3 && f1 < f4)
            {
                intersection = segmentsWithContour[l11] + segmentsWithContour[l21];
                intersection *= 0.5f;
                segmentsWithContour[l11] = intersection;
                segmentsWithContour[l21] = intersection;
            }
            else if (f2 < f1 && f2 < f3 && f2 < f4)
            {
                intersection = segmentsWithContour[l11] + segmentsWithContour[l22];
                intersection *= 0.5f;
                segmentsWithContour[l11] = intersection;
                segmentsWithContour[l22] = intersection;
            }
            else if (f3 < f1 && f3 < f2 && f3 < f4)
            {
                intersection = segmentsWithContour[l12] + segmentsWithContour[l21];
                intersection *= 0.5f;
                segmentsWithContour[l12] = intersection;
                segmentsWithContour[l21] = intersection;
            }
            else
            {
                intersection = segmentsWithContour[l12] + segmentsWithContour[l22];
                intersection *= 0.5f;
                segmentsWithContour[l12] = intersection;
                segmentsWithContour[l22] = intersection;
            }
        }
    }



    public static void WeldIntersections(List<Vector3> segmentsWithContour, out HashSet<Vector3> vertices)
    {
        //segments.Clear ();
        vertices = new HashSet<Vector3>();
        HashSet<int> linesContoured = new HashSet<int>();
        // loop all lines, find intersection between offsets on the same side, then replace intersection point
        for (int i = 0; i < segmentsWithContour.Count; i += 6)
        {

            for (int j = i + 6; j < segmentsWithContour.Count; j += 6)
            {



                if (segmentsWithContour[i] == segmentsWithContour[j])
                {
                    vertices.Add(segmentsWithContour[i]);



                    WeldInterstion(segmentsWithContour, i + 2, i + 3, j + 4, j + 5);
                    WeldInterstion(segmentsWithContour, i + 4, i + 5, j + 2, j + 3);






                    // Vector3 intersection = Vector3.zero;
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
                    //  if (!linesContoured.Contains (i + 2) || (linesContoured.Contains (i + 2) && (segmentsWithContour [i + 2] - segmentsWithContour [i + 3]).sqrMagnitude > (intersection - segmentsWithContour [i + 3]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 2] = intersection;
                    //      linesContoured.Add (i + 2);
                    //  }
                    //  if (!linesContoured.Contains (j + 4) || (linesContoured.Contains (j + 4) && (segmentsWithContour [j + 4] - segmentsWithContour [j + 5]).sqrMagnitude > (intersection - segmentsWithContour [j + 5]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 4] = intersection;
                    //      linesContoured.Add (j + 4);
                    //  }
                    // }
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
                    //  if (!linesContoured.Contains (i + 4) || (linesContoured.Contains (i + 4) && (segmentsWithContour [i + 4] - segmentsWithContour [i + 5]).sqrMagnitude > (intersection - segmentsWithContour [i + 5]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 4] = intersection;
                    //      linesContoured.Add (i + 4);
                    //  }
                    //  if (!linesContoured.Contains (j + 2) || (linesContoured.Contains (j + 2) && (segmentsWithContour [j + 2] - segmentsWithContour [j + 3]).sqrMagnitude > (intersection - segmentsWithContour [j + 3]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 2] = intersection;
                    //      linesContoured.Add (j + 2);
                    //  }
                    // }
                }
                else if (segmentsWithContour[i + 1] == segmentsWithContour[j])
                {
                    vertices.Add(segmentsWithContour[i + 1]);

                    WeldInterstion(segmentsWithContour, i + 2, i + 3, j + 2, j + 3);
                    WeldInterstion(segmentsWithContour, i + 4, i + 5, j + 4, j + 5);






                    // Vector3 intersection = Vector3.zero;
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
                    //  if (!linesContoured.Contains (i + 3) || (linesContoured.Contains (i + 3) && (segmentsWithContour [i + 3] - segmentsWithContour [i + 2]).sqrMagnitude > (intersection - segmentsWithContour [i + 2]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 3] = intersection;
                    //      linesContoured.Add (i + 3);
                    //  }
                    //  if (!linesContoured.Contains (j + 2) || (linesContoured.Contains (j + 2) && (segmentsWithContour [j + 2] - segmentsWithContour [j + 3]).sqrMagnitude > (intersection - segmentsWithContour [j + 3]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 2] = intersection;
                    //      linesContoured.Add (j + 2);
                    //  }
                    // }
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
                    //  if (!linesContoured.Contains (i + 5) || (linesContoured.Contains (i + 5) && (segmentsWithContour [i + 5] - segmentsWithContour [i + 4]).sqrMagnitude > (intersection - segmentsWithContour [i + 4]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 5] = intersection;
                    //      linesContoured.Add (i + 5);
                    //  }
                    //  if (!linesContoured.Contains (j + 4) || (linesContoured.Contains (j + 4) && (segmentsWithContour [j + 4] - segmentsWithContour [j + 5]).sqrMagnitude > (intersection - segmentsWithContour [j + 5]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 4] = intersection;
                    //      linesContoured.Add (j + 4);
                    //  }
                    // }
                }
                else if (segmentsWithContour[i] == segmentsWithContour[j + 1])
                {
                    vertices.Add(segmentsWithContour[i]);

                    WeldInterstion(segmentsWithContour, i + 2, i + 3, j + 2, j + 3);
                    WeldInterstion(segmentsWithContour, i + 4, i + 5, j + 4, j + 5);






                    // Vector3 intersection = Vector3.zero;
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
                    //  if (!linesContoured.Contains (i + 2) || (linesContoured.Contains (i + 2) && (segmentsWithContour [i + 2] - segmentsWithContour [i + 3]).sqrMagnitude > (intersection - segmentsWithContour [i + 3]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 2] = intersection;
                    //  }
                    //  if (!linesContoured.Contains (j + 3) || (linesContoured.Contains (j + 3) && (segmentsWithContour [j + 3] - segmentsWithContour [j + 2]).sqrMagnitude > (intersection - segmentsWithContour [j + 2]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 3] = intersection;
                    //  }
                    // }
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
                    //  if (!linesContoured.Contains (i + 4) || (linesContoured.Contains (i + 4) && (segmentsWithContour [i + 4] - segmentsWithContour [i + 5]).sqrMagnitude > (intersection - segmentsWithContour [i + 5]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 4] = intersection;
                    //      linesContoured.Add (i + 4);
                    //  }
                    //  if (!linesContoured.Contains (j + 5) || (linesContoured.Contains (j + 5) && (segmentsWithContour [j + 5] - segmentsWithContour [j + 4]).sqrMagnitude > (intersection - segmentsWithContour [j + 4]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 5] = intersection;
                    //      linesContoured.Add (j + 5);
                    //  }
                    // }
                }
                else if (segmentsWithContour[i + 1] == segmentsWithContour[j + 1])
                {
                    vertices.Add(segmentsWithContour[i + 1]);


                    WeldInterstion(segmentsWithContour, i + 2, i + 3, j + 4, j + 5);
                    WeldInterstion(segmentsWithContour, i + 4, i + 5, j + 2, j + 3);





                    // Vector3 intersection = Vector3.zero;
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
                    //  if (!linesContoured.Contains (i + 3) || (linesContoured.Contains (i + 3) && (segmentsWithContour [i + 3] - segmentsWithContour [i + 2]).sqrMagnitude > (intersection - segmentsWithContour [i + 2]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 3] = intersection;
                    //      linesContoured.Add (i + 3);
                    //  }
                    //  if (!linesContoured.Contains (j + 5) || (linesContoured.Contains (j + 5) && (segmentsWithContour [j + 5] - segmentsWithContour [j + 4]).sqrMagnitude > (intersection - segmentsWithContour [j + 4]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 5] = intersection;
                    //      linesContoured.Add (j + 5);
                    //  }
                    // }
                    // if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
                    //  if (!linesContoured.Contains (i + 5) || (linesContoured.Contains (i + 5) && (segmentsWithContour [i + 5] - segmentsWithContour [i + 4]).sqrMagnitude > (intersection - segmentsWithContour [i + 4]).sqrMagnitude)) {
                    //      segmentsWithContour [i + 5] = intersection;
                    //      linesContoured.Add (i + 5);
                    //  }
                    //  if (!linesContoured.Contains (j + 3) || (linesContoured.Contains (j + 3) && (segmentsWithContour [j + 3] - segmentsWithContour [j + 2]).sqrMagnitude > (intersection - segmentsWithContour [j + 2]).sqrMagnitude)) {
                    //      segmentsWithContour [j + 3] = intersection;
                    //      linesContoured.Add (j + 3);
                    //  }
                    // }
                }
            }
        }
    }




    //  static void WeldIntersections (List<Vector3> segmentsWithContour, out HashSet<Vector3> vertices)
    //  {
    //      //segments.Clear ();
    //      vertices = new HashSet<Vector3> ();
    //      HashSet<int> linesContoured = new HashSet<int> ();
    //      // loop all lines, find intersection between offsets on the same side, then replace intersection point
    //      for (int i = 0; i < segmentsWithContour.Count; i += 6) {
    //
    //          for (int j = i + 6; j < segmentsWithContour.Count; j += 6) {
    //              if (segmentsWithContour [i] == segmentsWithContour [j]) {
    //                  vertices.Add (segmentsWithContour [i]);
    //                  Vector3 intersection = Vector3.zero;
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
    //                      if (!linesContoured.Contains (i + 2) || (linesContoured.Contains (i + 2) && (segmentsWithContour [i + 2] - segmentsWithContour [i + 3]).sqrMagnitude > (intersection - segmentsWithContour [i + 3]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 2] = intersection;
    //                          linesContoured.Add (i + 2);
    //                      }
    //                      if (!linesContoured.Contains (j + 4) || (linesContoured.Contains (j + 4) && (segmentsWithContour [j + 4] - segmentsWithContour [j + 5]).sqrMagnitude > (intersection - segmentsWithContour [j + 5]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 4] = intersection;
    //                          linesContoured.Add (j + 4);
    //                      }
    //                  }
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
    //                      if (!linesContoured.Contains (i + 4) || (linesContoured.Contains (i + 4) && (segmentsWithContour [i + 4] - segmentsWithContour [i + 5]).sqrMagnitude > (intersection - segmentsWithContour [i + 5]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 4] = intersection;
    //                          linesContoured.Add (i + 4);
    //                      }
    //                      if (!linesContoured.Contains (j + 2) || (linesContoured.Contains (j + 2) && (segmentsWithContour [j + 2] - segmentsWithContour [j + 3]).sqrMagnitude > (intersection - segmentsWithContour [j + 3]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 2] = intersection;
    //                          linesContoured.Add (j + 2);
    //                      }
    //                  }
    //              } else if (segmentsWithContour [i + 1] == segmentsWithContour [j]) {
    //                  vertices.Add (segmentsWithContour [i + 1]);
    //                  Vector3 intersection = Vector3.zero;
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
    //                      if (!linesContoured.Contains (i + 3) || (linesContoured.Contains (i + 3) && (segmentsWithContour [i + 3] - segmentsWithContour [i + 2]).sqrMagnitude > (intersection - segmentsWithContour [i + 2]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 3] = intersection;
    //                          linesContoured.Add (i + 3);
    //                      }
    //                      if (!linesContoured.Contains (j + 2) || (linesContoured.Contains (j + 2) && (segmentsWithContour [j + 2] - segmentsWithContour [j + 3]).sqrMagnitude > (intersection - segmentsWithContour [j + 3]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 2] = intersection;
    //                          linesContoured.Add (j + 2);
    //                      }
    //                  }
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
    //                      if (!linesContoured.Contains (i + 5) || (linesContoured.Contains (i + 5) && (segmentsWithContour [i + 5] - segmentsWithContour [i + 4]).sqrMagnitude > (intersection - segmentsWithContour [i + 4]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 5] = intersection;
    //                          linesContoured.Add (i + 5);
    //                      }
    //                      if (!linesContoured.Contains (j + 4) || (linesContoured.Contains (j + 4) && (segmentsWithContour [j + 4] - segmentsWithContour [j + 5]).sqrMagnitude > (intersection - segmentsWithContour [j + 5]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 4] = intersection;
    //                          linesContoured.Add (j + 4);
    //                      }
    //                  }
    //              } else if (segmentsWithContour [i] == segmentsWithContour [j + 1]) {
    //                  vertices.Add (segmentsWithContour [i]);
    //                  Vector3 intersection = Vector3.zero;
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
    //                      if (!linesContoured.Contains (i + 2) || (linesContoured.Contains (i + 2) && (segmentsWithContour [i + 2] - segmentsWithContour [i + 3]).sqrMagnitude > (intersection - segmentsWithContour [i + 3]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 2] = intersection;
    //                      }
    //                      if (!linesContoured.Contains (j + 3) || (linesContoured.Contains (j + 3) && (segmentsWithContour [j + 3] - segmentsWithContour [j + 2]).sqrMagnitude > (intersection - segmentsWithContour [j + 2]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 3] = intersection;
    //                      }
    //                  }
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
    //                      if (!linesContoured.Contains (i + 4) || (linesContoured.Contains (i + 4) && (segmentsWithContour [i + 4] - segmentsWithContour [i + 5]).sqrMagnitude > (intersection - segmentsWithContour [i + 5]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 4] = intersection;
    //                          linesContoured.Add (i + 4);
    //                      }
    //                      if (!linesContoured.Contains (j + 5) || (linesContoured.Contains (j + 5) && (segmentsWithContour [j + 5] - segmentsWithContour [j + 4]).sqrMagnitude > (intersection - segmentsWithContour [j + 4]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 5] = intersection;
    //                          linesContoured.Add (j + 5);
    //                      }
    //                  }
    //              } else if (segmentsWithContour [i + 1] == segmentsWithContour [j + 1]) {
    //                  vertices.Add (segmentsWithContour [i + 1]);
    //                  Vector3 intersection = Vector3.zero;
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 2], segmentsWithContour [i + 3], segmentsWithContour [j + 4], segmentsWithContour [j + 5])) {
    //                      if (!linesContoured.Contains (i + 3) || (linesContoured.Contains (i + 3) && (segmentsWithContour [i + 3] - segmentsWithContour [i + 2]).sqrMagnitude > (intersection - segmentsWithContour [i + 2]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 3] = intersection;
    //                          linesContoured.Add (i + 3);
    //                      }
    //                      if (!linesContoured.Contains (j + 5) || (linesContoured.Contains (j + 5) && (segmentsWithContour [j + 5] - segmentsWithContour [j + 4]).sqrMagnitude > (intersection - segmentsWithContour [j + 4]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 5] = intersection;
    //                          linesContoured.Add (j + 5);
    //                      }
    //                  }
    //                  if (RayRayIntersection (out intersection, segmentsWithContour [i + 4], segmentsWithContour [i + 5], segmentsWithContour [j + 2], segmentsWithContour [j + 3])) {
    //                      if (!linesContoured.Contains (i + 5) || (linesContoured.Contains (i + 5) && (segmentsWithContour [i + 5] - segmentsWithContour [i + 4]).sqrMagnitude > (intersection - segmentsWithContour [i + 4]).sqrMagnitude)) {
    //                          segmentsWithContour [i + 5] = intersection;
    //                          linesContoured.Add (i + 5);
    //                      }
    //                      if (!linesContoured.Contains (j + 3) || (linesContoured.Contains (j + 3) && (segmentsWithContour [j + 3] - segmentsWithContour [j + 2]).sqrMagnitude > (intersection - segmentsWithContour [j + 2]).sqrMagnitude)) {
    //                          segmentsWithContour [j + 3] = intersection;
    //                          linesContoured.Add (j + 3);
    //                      }
    //                  }
    //              }
    //          }
    //      }
    //  }


    public static void Generate3DWallFacesFromLines(List<Line> _segments, Material WallWireframeMaterial, Material WallSelectedMaterial, out List<WallFace> outerWall, out List<WallFace> doorSides, out List<WallFace> innerWall, out GameObject upperWallFace, out List<Mesh> floors)
    {
        if (_segments.Count == 0)
        {
            outerWall = new List<WallFace>();
            innerWall = new List<WallFace>();
            doorSides = new List<WallFace>();
            upperWallFace = null;
            floors = new List<Mesh>();
            return;
        }
        List<Line> segments = new List<Line>();

        List<Vector3> vbuffer = new List<Vector3>();

        // split segment to multiple segments for windows
        for (int i = 0; i < _segments.Count; i++)
        {

            List<WallWindow> windows = new List<WallWindow>();
            windows.AddRange(_segments[i].Windows);
            for (int j = 0; j < _segments[i].Doors.Count; ++j)
                windows.Add(_segments[i].Doors[j]);


            if (windows.Count != 0)
            {

                windows.Sort(delegate (WallWindow x, WallWindow y) {
                    return x.Position.x.CompareTo(y.Position.x);
                });

                if (windows[0].Position.x != 0)
                {

                    int id1 = vbuffer.IndexOf(_segments[i].a);
                    if (id1 == -1)
                    {
                        id1 = vbuffer.Count;
                        vbuffer.Add(_segments[i].a);
                    }
                    Vector3 v2 = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * windows[0].Position.x;
                    int id2 = vbuffer.IndexOf(v2);
                    if (id2 == -1)
                    {
                        id2 = vbuffer.Count;
                        vbuffer.Add(v2);
                    }

                    Line firstSeg = new Line(vbuffer, id1, id2, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments[i].OuterMaterial, _segments[i].SideMaterial);
                    firstSeg.Height = _segments[i].Height;
                    firstSeg.LineType = LineType.Wall;
                    firstSeg.ParentLine = _segments[i];
                    segments.Add(firstSeg);
                }

                for (int j = 0; j < windows.Count - 1; j++)
                {
                    Vector3 start = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * windows[j].Position.x;
                    Vector3 end = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * (windows[j].Position.x + windows[j].WindowWidth);

                    int istart = vbuffer.IndexOf(start);
                    if (istart == -1)
                    {
                        istart = vbuffer.Count;
                        vbuffer.Add(start);
                    }
                    int iend = vbuffer.IndexOf(end);
                    if (iend == -1)
                    {
                        iend = vbuffer.Count;
                        vbuffer.Add(end);
                    }


                    Line windowSeg = new Line(vbuffer, istart, iend, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments[i].OuterMaterial, _segments[i].SideMaterial);
                    windowSeg.LedgeHeight = windows[j].Position.y;
                    windowSeg.WindowHeight = windows[j].WindowHeight;
                    windowSeg.LineType = LineType.Window;
                    windowSeg.Height = _segments[i].Height;
                    windowSeg.ParentLine = _segments[i];
                    segments.Add(windowSeg);

                    Vector3 nextStart = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * windows[j + 1].Position.x;
                    int inextStart = vbuffer.IndexOf(nextStart);
                    if (inextStart == -1)
                    {
                        inextStart = vbuffer.Count;
                        vbuffer.Add(nextStart);
                    }
                    Line nextSeg = new Line(vbuffer, iend, inextStart, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments[i].OuterMaterial, _segments[i].SideMaterial);
                    nextSeg.Height = _segments[i].Height;
                    nextSeg.LineType = LineType.Wall;
                    nextSeg.ParentLine = _segments[i];
                    segments.Add(nextSeg);
                }

                {
                    Vector3 start = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * windows[windows.Count - 1].Position.x;
                    Vector3 end = _segments[i].a + (_segments[i].b - _segments[i].a).normalized * (windows[windows.Count - 1].Position.x + windows[windows.Count - 1].WindowWidth);
                    int istart = vbuffer.IndexOf(start);
                    if (istart == -1)
                    {
                        istart = vbuffer.Count;
                        vbuffer.Add(start);
                    }
                    int iend = vbuffer.IndexOf(end);
                    if (iend == -1)
                    {
                        iend = vbuffer.Count;
                        vbuffer.Add(end);
                    }


                    Line windowSeg = new Line(vbuffer, istart, iend, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments[i].OuterMaterial, _segments[i].SideMaterial);
                    windowSeg.LedgeHeight = windows[windows.Count - 1].Position.y;
                    windowSeg.WindowHeight = windows[windows.Count - 1].WindowHeight;
                    windowSeg.LineType = LineType.Window;
                    windowSeg.Height = _segments[i].Height;
                    windowSeg.ParentLine = _segments[i];
                    segments.Add(windowSeg);

                    int id2 = vbuffer.IndexOf(_segments[i].b);
                    if (id2 == -1)
                    {
                        id2 = vbuffer.Count;
                        vbuffer.Add(_segments[i].b);
                    }

                    Line lastSeg = new Line(vbuffer, iend, id2, _segments[i].Thickness, _segments[i].LineMaterial, _segments[i].InnerMaterial, _segments[i].OuterMaterial, _segments[i].SideMaterial);
                    lastSeg.Height = _segments[i].Height;
                    lastSeg.LineType = LineType.Wall;
                    lastSeg.ParentLine = _segments[i];
                    segments.Add(lastSeg);
                }



            }
            else
            {
                segments.Add(_segments[i]);
            }
        }



        List<Vector3> segmentsWithContour = Line.Offsets(segments);
        // {lines, lines offseted, lines offseted backward}





        HashSet<Vector3> vertices;
        Line.WeldIntersections(segmentsWithContour, out vertices);


        // find end point edges
        List<Vector3> endpointsSegments = new List<Vector3>();
        for (int i = 0; i < segmentsWithContour.Count; i += 6)
        {
            if (!vertices.Contains(segmentsWithContour[i]))
            {
                endpointsSegments.Add(segmentsWithContour[i + 2]);
                endpointsSegments.Add(segmentsWithContour[i + 4]);
            }
            if (!vertices.Contains(segmentsWithContour[i + 1]))
            {
                endpointsSegments.Add(segmentsWithContour[i + 3]);
                endpointsSegments.Add(segmentsWithContour[i + 5]);
            }
        }

        // remove original lines
        for (int i = segmentsWithContour.Count - 6; i >= 0; i -= 6)
        {
            segmentsWithContour.RemoveAt(i);
            segmentsWithContour.RemoveAt(i);
        }

        // add endpoint edges to contour list
        segmentsWithContour.AddRange(endpointsSegments);

        // find directed paths (to determine inner, outer walls)
        List<List<Vector3>> directedPaths = new List<List<Vector3>>();
        directedPaths.Add(new List<Vector3>());
        directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[0]);
        directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[1]);
        segmentsWithContour.RemoveRange(0, 2);
        while (segmentsWithContour.Count > 0)
        {
            bool flag = true;
            for (int i = 0; i < segmentsWithContour.Count; i += 2)
            {
                if (directedPaths[directedPaths.Count - 1][directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour[i])
                {
                    directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[i]);
                    directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[i + 1]);
                    segmentsWithContour.RemoveRange(i, 2);
                    flag = false;
                    break;
                }
                else if (directedPaths[directedPaths.Count - 1][directedPaths[directedPaths.Count - 1].Count - 1] == segmentsWithContour[i + 1])
                {
                    directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[i + 1]);
                    directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[i]);
                    segmentsWithContour.RemoveRange(i, 2);
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                // to avoid infinite loop
                // make new directed path
                directedPaths.Add(new List<Vector3>());
                directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[0]);
                directedPaths[directedPaths.Count - 1].Add(segmentsWithContour[1]);
                segmentsWithContour.RemoveRange(0, 2);
            }

        }

        // find outer loop (nearest to infinite) and reverse it if needed
        for (int i = 0; i < directedPaths.Count; i++)
        {
            int minSeg = -1;
            float dst = float.MaxValue;
            for (int j = 0; j < directedPaths[i].Count; j += 2)
            {
                float det = (directedPaths[i][j] - Vector3.right * 1000.0f).sqrMagnitude;
                det += (directedPaths[i][j + 1] - Vector3.right * 1000.0f).sqrMagnitude;
                if (det < dst)
                {
					if (Mathf.Abs(Vector3.Dot(Vector3.Normalize(directedPaths[i][j] - directedPaths[i][j + 1]), Vector3.forward)) > epsilon)
					{
	                    dst = det;
	                    minSeg = j;
					}
                }
            }

            if (minSeg != -1)
            {
                if (Vector3.Dot(Vector3.Normalize(directedPaths[i][minSeg] - directedPaths[i][minSeg + 1]), Vector3.forward) < 0)
                {
                    directedPaths[i].Reverse();
                }
            }
        }






        outerWall = new List<WallFace>();
        innerWall = new List<WallFace>();
        doorSides = new List<WallFace>();
        floors = new List<Mesh>();

        float thicknessSqrd = _segments[0].Thickness * _segments[0].Thickness;//thickness * thickness;

        upperWallFace = null;


        // for each path find intersection count from (any vertex) to (infinite) which will determine if this is inner or outer wall
        // then generate walls or windows
        // destroy line if its for the window (not original line)
        for (int i = 0; i < directedPaths.Count; i++)
        {
            // bug .. [\] will not work correctly
            int rand = Random.Range(0, directedPaths[i].Count / 2) * 2;
            Vector3 randomVertex = Vector3.Lerp(directedPaths[i][rand], directedPaths[i][rand + 1], Random.Range(1, 99) / 100.0f);
            //if (i == 1)
            //  randomVertex = new Vector3 (2.9f, 0, 2.4f);

            Vector3 randomOutterPoint = new Vector3 (Random.Range(10000.0f, 50000.0f), 0, Random.Range(10000.0f, 50000.0f));

            int intersectionCount = 0;
            for (int j = 0; j < directedPaths.Count; j++)
            {
                if (i != j)
                {
                    for (int k = 0; k < directedPaths[j].Count; k += 2)
                    {

                        Vector3 tmp;

						if (RayRayIntersection(out tmp, directedPaths[j][k], directedPaths[j][k + 1], randomVertex, randomOutterPoint))
                        {
							if ((tmp - randomOutterPoint).magnitude <= (randomVertex - randomOutterPoint).magnitude)
                            {
                                if ((tmp - directedPaths[j][k]).magnitude <= (directedPaths[j][k + 1] - directedPaths[j][k]).magnitude)
                                {
									if ((tmp - directedPaths [j] [k]).sqrMagnitude > epsilon && (tmp - directedPaths [j] [k + 1]).sqrMagnitude > epsilon)
									{
										if (Vector3.Dot (tmp - directedPaths [j] [k], directedPaths [j] [k + 1] - directedPaths [j] [k]) >= 0) {
											intersectionCount++;
										}
									}
                                }
                            }
                        }
                    }
                }
            }



            if (intersectionCount % 2 == 1)
            {
                directedPaths[i].Reverse();

				{
					List<Line> toCap = new List<Line> ();
					List<Vector3> tmpverts = new List<Vector3> ();
					tmpverts.AddRange (directedPaths [i]);
					for (int j = 0; j < directedPaths [i].Count; j += 2) {
						Line ll = new Line (tmpverts, j, j + 1, 0, null, null, null, null);
						ll.Destroy ();
						toCap.Add (ll);
					}

					Line.WeldVertices (toCap);
					List<int> triangles;
					List<Vector3> verts;
					List<Vector2> uvs;
					List<Vector3> normals;
					try {
						Line.FillCap (toCap, out triangles, out verts, out uvs, out normals);
						if (triangles.Count % 3 == 0){
							Mesh mm = new Mesh () {
								vertices = verts.ToArray (),
								uv = uvs.ToArray (),
								normals = normals.ToArray (),
								triangles = triangles.ToArray ()
							};
							floors.Add (mm);
						}
					} catch {
						Debug.Log ("floor !");
					}


				}

                for (int j = 0; j < directedPaths[i].Count; j += 2)
                {
					if (Mathf.Abs((directedPaths[i][j] - directedPaths[i][j + 1]).sqrMagnitude - thicknessSqrd) <= epsilon)
                    {
                        Line l = null;
                        float dst = float.MaxValue;
                        for (int k = 0; k < segments.Count; k++)
                        {
                            float det1 = (segments[k].a - directedPaths[i][j]).sqrMagnitude;
                            if (det1 < dst)
                            {
                                dst = det1;
                                l = segments[k];
                            }
                            else
                            {
                                float det2 = (segments[k].b - directedPaths[i][j]).sqrMagnitude;
                                if (det2 < dst)
                                {
                                    dst = det2;
                                    l = segments[k];
                                }
                            }
                        }

                        if (l.LineType == LineType.Wall)
                        {
                            doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.Height, l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                        }
                        else
                        {
                            if (l.LedgeHeight != 0)
                                doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.LedgeHeight, l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                            doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], l.LedgeHeight + l.WindowHeight, l.Height - (l.LedgeHeight + l.WindowHeight), l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                        }
                        if (l.ParentLine != null)
                            l.Destroy();
                    }
                    else
                    {

                        Line l = null;
                        float dst = float.MaxValue;
                        for (int k = 0; k < segments.Count; k++)
                        {
                            float det1 = (segments[k].a - directedPaths[i][j]).sqrMagnitude;
                            det1 += (segments[k].b - directedPaths[i][j + 1]).sqrMagnitude;
                            if (det1 < dst)
                            {
                                dst = det1;
                                l = segments[k];
                            }
                            float det2 = (segments[k].b - directedPaths[i][j]).sqrMagnitude;
                            det2 += (segments[k].a - directedPaths[i][j + 1]).sqrMagnitude;
                            if (det2 < dst)
                            {
                                dst = det2;
                                l = segments[k];
                            }

                        }

                        if (l.LineType == LineType.Wall)
                        {
                            innerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.Height, l.InnerMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Inner });
                        }
                        else
                        {
                            if (l.LedgeHeight != 0)
                                innerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.LedgeHeight, l.InnerMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Inner });
                            innerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], l.LedgeHeight + l.WindowHeight, l.Height - (l.LedgeHeight + l.WindowHeight), l.InnerMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Inner });
                        }
                        if (l.ParentLine != null)
                            l.Destroy();
                    }
                }
            }
            else
            {
                for (int j = 0; j < directedPaths[i].Count; j += 2)
                {

					if (Mathf.Abs((directedPaths[i][j] - directedPaths[i][j + 1]).sqrMagnitude - thicknessSqrd) <= epsilon)
                    {
                        Line l = null;
                        float dst = float.MaxValue;
                        for (int k = 0; k < segments.Count; k++)
                        {
                            float det1 = (segments[k].a - directedPaths[i][j]).sqrMagnitude;
                            if (det1 < dst)
                            {
                                dst = det1;
                                l = segments[k];
                            }
                            else
                            {
                                float det2 = (segments[k].b - directedPaths[i][j]).sqrMagnitude;
                                if (det2 < dst)
                                {
                                    dst = det2;
                                    l = segments[k];
                                }
                            }
                        }

                        if (l.LineType == LineType.Wall)
                        {
                            doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.Height, l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                        }
                        else
                        {
                            if (l.LedgeHeight != 0)
                                doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.LedgeHeight, l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                            doorSides.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], l.LedgeHeight + l.WindowHeight, l.Height - (l.LedgeHeight + l.WindowHeight), l.SideMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.DoorSide });
                        }
                        if (l.ParentLine != null)
                            l.Destroy();
                    }
                    else
                    {

                        Line l = null;
                        float dst = float.MaxValue;
                        for (int k = 0; k < segments.Count; k++)
                        {
                            float det1 = (segments[k].a - directedPaths[i][j]).sqrMagnitude;
                            det1 += (segments[k].b - directedPaths[i][j + 1]).sqrMagnitude;
                            if (det1 < dst)
                            {
                                dst = det1;
                                l = segments[k];
                            }
                            float det2 = (segments[k].b - directedPaths[i][j]).sqrMagnitude;
                            det2 += (segments[k].a - directedPaths[i][j + 1]).sqrMagnitude;
                            if (det2 < dst)
                            {
                                dst = det2;
                                l = segments[k];
                            }

                        }

                        if (l.LineType == LineType.Wall)
                        {
                            outerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.Height, l.OuterMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Outer });
                        }
                        else
                        {
                            if (l.LedgeHeight != 0)
                                outerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], 0, l.LedgeHeight, l.OuterMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Outer });
                            outerWall.Add(new WallFace(directedPaths[i][j], directedPaths[i][j + 1], l.LedgeHeight + l.WindowHeight, l.Height - (l.LedgeHeight + l.WindowHeight), l.OuterMaterial, WallWireframeMaterial, WallSelectedMaterial, l.ParentLine == null ? l : l.ParentLine) { WallFaceType = WallFaceType.Outer });
                        }
                        if (l.ParentLine != null)
                            l.Destroy();
                    }

                }
            }
        }


        upperWallFace = new GameObject("UpperWallFace");
        upperWallFace.AddComponent<UpperWallFace>().CreateFromLines(_segments);
    }


    public static void OptimizePath(ref List<Line> lines)
	{
		// remove 0 length lines
		for (int i = lines.Count - 1; i >= 0; i--) {
			if ((lines [i].a - lines [i].b).sqrMagnitude <= epsilon) {
				lines [i].Destroy ();
				lines.RemoveAt (i);
			}
		}

		if (lines.Count == 0)
			return;

		// find paths
		List<List<Line>> topology = new List<List<Line>> ();
		topology.Add (new List<Line> ());
		topology [topology.Count - 1].Add (lines [0]);
		Vector3 lastPoint = lines [0].b;
		lines.RemoveAt (0);

		while (lines.Count > 0) {
			bool flag = true;
			for (int i = 0; i < lines.Count; i++) {
				if (lastPoint == lines [i].a) {
					topology [topology.Count - 1].Add (lines [i]);
					lastPoint = lines [i].b;
					lines.RemoveAt (i);
					flag = false;
					break;
				} else if (lastPoint == lines [i].b) {
					topology [topology.Count - 1].Add (lines [i]);
					lastPoint = lines [i].a;
					lines.RemoveAt (i);
					flag = false;
					break;
				}
			}

			if (flag) {
				// to avoid infinite loop
				// make new directed path
				topology.Add (new List<Line> ());
				topology [topology.Count - 1].Add (lines [0]);
				lastPoint = lines [0].b;
				lines.RemoveAt (0);
			}
		}

		// for each segment x, y if (x, y are in the same direction and connected) connect (x,y)
		for (int i = 0; i < topology.Count; i++) {
			for (int j = topology [i].Count - 1; j >= 1; j--) {
				
				if (topology [i] [j - 1].LineType == topology [i] [j].LineType) {
					Vector3 dir1 = topology [i] [j - 1].b - topology [i] [j - 1].a;
					Vector3 dir2 = topology [i] [j].b - topology [i] [j].a;
					dir1.Normalize ();
					dir2.Normalize ();
					//same direction
					if ((dir1 - dir2).sqrMagnitude <= epsilon) {
						//ab ab
						//ba ba
						if ((topology [i] [j].b - topology [i] [j - 1].a).sqrMagnitude <= epsilon) {
							//topology[i][j].b must be shared with 2 only
							int flag = 0;
							for (int k = 0; k < topology.Count; k++) {
								for (int kk = 0; kk < topology [k].Count; kk++) {
									if (topology [k] [kk].aID == topology [i] [j].bID) {
										flag++;
									}
									if (topology [k] [kk].bID == topology [i] [j].bID) {
										flag++;
									}
								}
							}
							if (flag == 2) {
								topology [i] [j - 1].a = topology [i] [j].a;
								topology [i] [j].Destroy ();
								topology [i].RemoveAt (j);
							}
						} else {
							//topology[i][j].a must be shared with 2 only
							int flag = 0;
							for (int k = 0; k < topology.Count; k++) {
								for (int kk = 0; kk < topology [k].Count; kk++) {
									if (topology [k] [kk].aID == topology [i] [j].aID) {
										flag++;
									}
									if (topology [k] [kk].bID == topology [i] [j].aID) {
										flag++;
									}
								}
							}
							if (flag == 2) {
								topology [i] [j - 1].b = topology [i] [j].b;
								topology [i] [j].Destroy ();
								topology [i].RemoveAt (j);
							}
						}
                        
					} else if ((dir1 - -dir2).sqrMagnitude <= epsilon) {
						//ab ba
						//ba ab
						if ((topology [i] [j].b - topology [i] [j - 1].b).sqrMagnitude <= epsilon) {
							//topology[i][j].b must be shared with 2 only
							int flag = 0;
							for (int k = 0; k < topology.Count; k++) {
								for (int kk = 0; kk < topology [k].Count; kk++) {
									if (topology [k] [kk].aID == topology [i] [j].bID) {
										flag++;
									}
									if (topology [k] [kk].bID == topology [i] [j].bID) {
										flag++;
									}
								}
							}
							if (flag == 2) {
								topology [i] [j - 1].b = topology [i] [j].a;
								topology [i] [j].Destroy ();
								topology [i].RemoveAt (j);
							}
						} else {
							//topology[i][j].a must be shared with 2 only
							int flag = 0;
							for (int k = 0; k < topology.Count; k++) {
								for (int kk = 0; kk < topology [k].Count; kk++) {
									if (topology [k] [kk].aID == topology [i] [j].aID) {
										flag++;
									}
									if (topology [k] [kk].bID == topology [i] [j].aID) {
										flag++;
									}
								}
							}
							if (flag == 2) {
								topology [i] [j - 1].a = topology [i] [j].b;
								topology [i] [j].Destroy ();
								topology [i].RemoveAt (j);
							}
						}
					}
				}
			}
		}
		

		// for each segment in path try to split if a vertex is between segment vertices
		for (int i = 0; i < topology.Count; i++) {
			HashSet<Vector3> vertices = new HashSet<Vector3> ();
			for (int j = 0; j < topology [i].Count; j++) {
				vertices.Add (topology [i] [j].a);
				vertices.Add (topology [i] [j].b);
			}

			for (int j = 0; j < topology.Count; j++) {

				if (i == j)
					continue;

				foreach (Vector3 v in (IEnumerable)vertices) {
					List<Line> nlines = Split (topology [j], v);
					topology [j] = nlines;
				}


			}
		}



		for (int i = 0; i < topology.Count; i++) {
			lines.AddRange (topology [i]);
		}
	}

	public static void FillCap(List<Line> _lines, out List<int> triangles, out List<Vector3> verts, out List<Vector2> uvs, out List<Vector3> normals)
	{
		int tries = 5;
		while (tries > 0) {
			try
			{
				_FillCap(_lines, out triangles, out verts, out uvs, out normals);
				return;
			}
			catch {
			}
			tries--;
		}
		throw new UnityException ("Unable to fill cap");
	}
	private static void _FillCap(List<Line> _lines, out List<int> triangles, out List<Vector3> verts, out List<Vector2> uvs, out List<Vector3> normals)
	{

		List<Line> lines = new List<Line> (_lines);
		OptimizePath (ref lines);

		if (lines.Count < 3)
			throw new UnityException("lines are less than 3");




		// outerEdge1, outerEdge2 any 2 connected edges where the angle less than 180
		int e1 = -1;
		int[] e2 = { -1, -1 };



		List<Line> list = new List<Line>(lines);
		triangles = new List<int>();


		int eeee = 0;
		while (list.Count > 2)
		{
			if (eeee > lines.Count * 5)
			{

				throw new UnityException("!!");
			}
			eeee++;
			e1 = Random.Range(0, list.Count);
			Vector3[] middlePoint = { Vector3.zero, Vector3.zero };
			Vector3[] x1 = { Vector3.zero, Vector3.zero };
			Vector3[] x2 = { Vector3.zero, Vector3.zero };
			int[] ix1 = { -1, -1 };
			int[] ix2 = { -1, -1 };
			int e2index = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (i != e1)
				{
					if (Mathf.Abs(Mathf.Abs(Vector3.Dot((list[i].a - list[i].b).normalized, (list[e1].a - list[e1].b).normalized)) - 1.0f) <= epsilon)
						continue;

					if ((list[i].a - list[e1].a).sqrMagnitude < epsilon)
					{
						e2[e2index] = i;
						middlePoint[e2index] = (list[i].b + list[e1].b) * 0.5f;
						x1[e2index] = list[i].b;
						x2[e2index] = list[e1].b;
						ix1[e2index] = list[i].bID;
						ix2[e2index] = list[e1].bID;
						e2index++;
						if (e2index == 2)
							break;
					}
					if ((list[i].a - list[e1].b).sqrMagnitude < epsilon)
					{
						e2[e2index] = i;
						middlePoint[e2index] = (list[i].b + list[e1].a) * 0.5f;
						x1[e2index] = list[i].b;
						x2[e2index] = list[e1].a;
						ix1[e2index] = list[i].bID;
						ix2[e2index] = list[e1].aID;
						e2index++;
						if (e2index == 2)
							break;
					}
					if ((list[i].b - list[e1].a).sqrMagnitude < epsilon)
					{
						e2[e2index] = i;
						middlePoint[e2index] = (list[i].a + list[e1].b) * 0.5f;
						x1[e2index] = list[i].a;
						x2[e2index] = list[e1].b;
						ix1[e2index] = list[i].aID;
						ix2[e2index] = list[e1].bID;
						e2index++;
						if (e2index == 2)
							break;
					}
					if ((list[i].b - list[e1].b).sqrMagnitude < epsilon)
					{
						e2[e2index] = i;
						middlePoint[e2index] = (list[i].a + list[e1].a) * 0.5f;
						x1[e2index] = list[i].a;
						x2[e2index] = list[e1].a;
						ix1[e2index] = list[i].aID;
						ix2[e2index] = list[e1].aID;
						e2index++;
						if (e2index == 2)
							break;
					}
				}
			}

			if (e2[0] == -1)
				continue;


			int[] intersectionCount = { 0, 0 };
			HashSet<int> e2indices = new HashSet<int>();
			Vector3 randomOutterVector = new Vector3 (Random.Range(5000.0f, 10000.0f), 0, Random.Range(5000.0f, 10000.0f));
			for (int i = 0; i < e2index; i++)
			{
				bool flag = true;

				for (int k = 0; k < lines.Count; ++k)
				{

					Vector3 tmp;
					if (RayRayIntersection(out tmp, x1[i], x2[i], lines[k].a, lines[k].b))
					{
						float dst = (tmp - x1 [i]).sqrMagnitude;
						if (dst < (x2[i] - x1[i]).sqrMagnitude && dst > epsilon)
						{
							if (Vector3.Dot(tmp - x1[i], (x2[i] - x1[i]).normalized) >= 0)
							{
								if (Mathf.Abs((tmp - lines[k].a).magnitude + (tmp - lines[k].b).magnitude - (lines[k].a - lines[k].b).magnitude) <= epsilon)
								{
									if ((tmp - lines [k].a).sqrMagnitude > epsilon && (tmp - lines [k].b).sqrMagnitude > epsilon)
									{
										//										if (eeee > 1000 && eeee < 1100) {
										//											{
										//												string sssss = ("e1 = " + e1 + "e2[] = " + e2 [0] + "" + e2 [1] + "\n");
										//
										//												for (int kkk = 0; kkk < lines.Count; ++kkk) {
										//													sssss += (lines [kkk].a.x + "\t" + lines [kkk].a.z + "\t" + (kkk * 2) + "\n");
										//													sssss += (lines [kkk].b.x + "\t" + lines [kkk].b.z + "\t" + (kkk * 2 + 1) + "\n");
										//												}
										//												sssss += "REFUSED " + x1 [i] + " " + x2 [i] + "\n";
										//												sssss += ("____\n");
										//												Debug.Log (msgCount + "\n" + sssss);
										//												msgCount++;
										//											}
										//										}
										flag = false;
										break;
									}
								}
							}
						}
					}
					if (RayRayIntersection(out tmp, lines[k].a, lines[k].b, middlePoint[i], randomOutterVector))
					{
						if ((tmp - randomOutterVector).magnitude <= (middlePoint[i] - randomOutterVector).magnitude)
						{
							if (Mathf.Abs((tmp - lines[k].a).magnitude + (tmp - lines[k].b).magnitude - (lines[k].b - lines[k].a).magnitude) <= epsilon)
							{
								if ((tmp - lines [k].a).sqrMagnitude > epsilon && (tmp - lines [k].b).sqrMagnitude > epsilon) {
									if (Vector3.Dot (tmp - lines [k].a, lines [k].b - lines [k].a) >= 0) {
										intersectionCount [i]++;
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					e2indices.Add(i);
				}
			}

			if (e2index == -1)
				continue;

			// if (ray cast count middle to infinite % 2 == 1 break
			if (e2indices.Contains(0) && intersectionCount[0] % 2 == 1)
			{
				//				triangles.Add (ix1);
				//				triangles.Add (list [e1].aID == ix1 ? list [e1].bID : list [e1].aID);
				//				triangles.Add (ix2);
				HashSet<int> abc = new HashSet<int>() { ix1[0], ix2[0], list[e1].aID, list[e1].bID };
				//                if (abc.Count == 3)
				triangles.AddRange(abc);
				if (abc.Count != 3) {


					Debug.Log ("abc != 3");
				}




				list.RemoveAt(Mathf.Max(e1, e2[0]));
				list.RemoveAt(Mathf.Min(e1, e2[0]));
				// if new line not already exist
				if (list.FindIndex(delegate (Line obj) {
					return (obj.aID == ix1[0] && obj.bID == ix2[0]) || (obj.aID == ix2[0] && obj.bID == ix1[0]);
				}) == -1)
					list.Add(new Line(list[0].Vertices, ix1[0], ix2[0], 1, null, null, null, null));
				continue;
			}

			if (e2indices.Contains(1) && intersectionCount[1] % 2 == 1)
			{
				//				triangles.Add (ix1);
				//				triangles.Add (list [e1].aID == ix1 ? list [e1].bID : list [e1].aID);
				//				triangles.Add (ix2);
				HashSet<int> abc = new HashSet<int>() { ix1[1], ix2[1], list[e1].aID, list[e1].bID };
				//                if (abc.Count == 3)
				triangles.AddRange(abc);
				if (abc.Count != 3) {


					Debug.Log ("abc != 3");
				}



				list.RemoveAt(Mathf.Max(e1, e2[1]));
				list.RemoveAt(Mathf.Min(e1, e2[1]));
				// if new line not already exist
				if (list.FindIndex(delegate (Line obj) {
					return (obj.aID == ix1[1] && obj.bID == ix2[1]) || (obj.aID == ix2[1] && obj.bID == ix1[1]);
				}) == -1)
					list.Add(new Line(list[1].Vertices, ix1[1], ix2[1], 1, null, null, null, null));
				continue;
			}

			if (list.Count <= 3)
			{

				//				triangles.Add (list [0].aID);
				//				triangles.Add (list [0].bID);
				//				triangles.Add (list [1].aID == list [0].aID ? list [1].bID : list [1].aID);

				HashSet<int> abc = new HashSet<int>() { list[0].aID, list[0].bID, list[1].aID, list[1].bID };
				//				if (abc.Count == 3)
				triangles.AddRange(abc);
				if (abc.Count != 3) {


					Debug.Log ("abc != 3");

				}


				//				for (int i = 0; i < triangles.Count; i += 3) {
				//					Debug.Log (triangles [i] + " " + triangles [i + 1] + " " + triangles [i + 2] + "\n");
				//				}
				break;
			}
		}


		verts = new List<Vector3>();
		normals = new List<Vector3>();
		uvs = new List<Vector2>();
		{
			int count = 0;
			for (int i = 0; i < triangles.Count; i++)
				count = Mathf.Max(count, triangles[i]);

			count++;
			for (int i = 0; i < count; i++)
			{
				verts.Add(lines[0].Vertices[i]);
				normals.Add(Vector3.up);
				uvs.Add(new Vector2(verts[i].x, verts[i].z));
			}


			//			while (triangles.Count % 3 != 0)
			//				triangles.RemoveAt (triangles.Count - 1);


			for (int i = 0; i < triangles.Count; i += 3)
			{
				if (i + 2 >= triangles.Count)
				{
					i--;
					break;
				}
				Plane p = new Plane(verts[triangles[i]], verts[triangles[i + 1]], verts[triangles[i + 2]]);
				if (Vector3.Dot(p.normal, Vector3.up) < 0)
				{
					int tmp = triangles[i];
					triangles[i] = triangles[i + 2];
					triangles[i + 2] = tmp;
				}
			}
		}



	}

}
