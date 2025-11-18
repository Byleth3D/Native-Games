/// Credit jack.sydorenko, firagon
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/
/// Updated/Refactored from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/#post-2528050

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
    [RequireComponent(typeof(RectTransform))]
    public class UILineRenderer : UIPrimitiveBase
    {
        private enum SegmentType
        {
            Start,
            Middle,
            End,
            Full,
        }

        public enum JoinType
        {
            Bevel,
            Miter
        }

        public enum BezierType
        {
            None,
            Quick,
            Basic,
            Improved,
            Catenary,
        }

        public enum TextureTileMode
        {
            Stretch,
            Tile
        }

        private const float MIN_MITER_JOIN = 15 * Mathf.Deg2Rad;
        private const float MIN_BEVEL_NICE_JOIN = 30 * Mathf.Deg2Rad;

        private static Vector2 UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_TOP_CENTER_LEFT, UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_RIGHT, UV_BOTTOM_RIGHT;
        private static Vector2[] startUvs, middleUvs, endUvs, fullUvs;

        [SerializeField, Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
        internal Vector2[] m_points;
        [SerializeField, Tooltip("Segments to be drawn\n This is a list of arrays of points")]
        internal List<Vector2[]> m_segments;

        [SerializeField, Tooltip("Thickness of the line")]
        internal float lineThickness = 2;
        [SerializeField, Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
        internal bool relativeSize;
        [SerializeField, Tooltip("Do the points identify a single line or split pairs of lines")]
        internal bool lineList;
        [SerializeField, Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
        internal bool lineCaps;
        [SerializeField, Tooltip("Resolution of the Bezier curve, different to line Resolution")]
        internal int bezierSegmentsPerCurve = 10;

        [SerializeField, Tooltip("Texture tiling mode - stretch or tile")]
        internal TextureTileMode textureTileMode = TextureTileMode.Stretch;

        [SerializeField, Tooltip("Texture tile scale - controls how many times the texture repeats per world unit")]
        internal float textureTileScale = 1f;

        [SerializeField, Tooltip("Minimum segment length for tiling - prevents texture overlap on very short segments")]
        internal float minSegmentLength = 0.1f;

        public float LineThickness
        {
            get { return lineThickness; }
            set { lineThickness = value; SetAllDirty(); }
        }

        public bool RelativeSize
        {
            get { return relativeSize; }
            set { relativeSize = value; SetAllDirty(); }
        }

        public bool LineList
        {
            get { return lineList; }
            set { lineList = value; SetAllDirty(); }
        }

        public bool LineCaps
        {
            get { return lineCaps; }
            set { lineCaps = value; SetAllDirty(); }
        }

        public TextureTileMode TextureTile
        {
            get { return textureTileMode; }
            set { textureTileMode = value; SetAllDirty(); }
        }

        public float TextureTileScale
        {
            get { return textureTileScale; }
            set { textureTileScale = value; SetAllDirty(); }
        }

        public float MinSegmentLength
        {
            get { return minSegmentLength; }
            set { minSegmentLength = Mathf.Max(0.01f, value); SetAllDirty(); }
        }

        [Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
        public JoinType LineJoins = JoinType.Bevel;

        [Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
        public BezierType BezierMode = BezierType.None;

        public int BezierSegmentsPerCurve
        {
            get { return bezierSegmentsPerCurve; }
            set { bezierSegmentsPerCurve = value; }
        }

        [HideInInspector]
        public bool drivenExternally = false;

        /// <summary>
        /// Points to be drawn in the line.
        /// </summary>
        public Vector2[] Points
        {
            get
            {
                return m_points;
            }

            set
            {
                if (m_points == value) return;

                if (value == null || value.Length == 0)
                {
                    m_points = new Vector2[1];
                }
                else
                {
                    m_points = value;
                }

                SetAllDirty();
            }
        }

        /// <summary>
        /// List of Segments to be drawn.
        /// </summary>
        public List<Vector2[]> Segments
        {
            get
            {
                return m_segments;
            }

            set
            {
                m_segments = value;
                SetAllDirty();
            }
        }

        private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
        {
            //If Bezier is desired, pick the implementation
            if (BezierMode != BezierType.None && BezierMode != BezierType.Catenary && pointsToDraw.Length > 3)
            {
                BezierPath bezierPath = new BezierPath();

                bezierPath.SetControlPoints(pointsToDraw);
                bezierPath.SegmentsPerCurve = bezierSegmentsPerCurve;
                List<Vector2> drawingPoints;
                switch (BezierMode)
                {
                    case BezierType.Basic:
                        drawingPoints = bezierPath.GetDrawingPoints0();
                        break;
                    case BezierType.Improved:
                        drawingPoints = bezierPath.GetDrawingPoints1();
                        break;
                    default:
                        drawingPoints = bezierPath.GetDrawingPoints2();
                        break;
                }

                pointsToDraw = drawingPoints.ToArray();
            }
            if (BezierMode == BezierType.Catenary && pointsToDraw.Length == 2)
            {
                CableCurve cable = new CableCurve(pointsToDraw);
                cable.slack = Resolution;
                cable.steps = BezierSegmentsPerCurve;
                pointsToDraw = cable.Points();
            }

            if (ImproveResolution != ResolutionMode.None)
            {
                pointsToDraw = IncreaseResolution(pointsToDraw);
            }

            // scale based on the size of the rect or use absolute, this is switchable
            var sizeX = !relativeSize ? 1 : rectTransform.rect.width;
            var sizeY = !relativeSize ? 1 : rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * sizeX;
            var offsetY = -rectTransform.pivot.y * sizeY;

            // Generate the quads that make up the wide line
            var segments = new List<UIVertex[]>();

            // Calcular comprimento total e acumulado para UVs contínuas
            float totalLength = 0f;
            float[] segmentLengths = new float[pointsToDraw.Length - 1];
            float[] accumulatedLengths = new float[pointsToDraw.Length];

            if (textureTileMode == TextureTileMode.Tile)
            {
                for (var i = 1; i < pointsToDraw.Length; i++)
                {
                    var start = pointsToDraw[i - 1];
                    var end = pointsToDraw[i];
                    float segmentLength = Vector2.Distance(start, end);

                    // Aplicar comprimento mínimo para evitar sobreposição
                    if (segmentLength < minSegmentLength && i < pointsToDraw.Length - 1)
                    {
                        segmentLength = minSegmentLength;
                    }

                    segmentLengths[i - 1] = segmentLength;
                    totalLength += segmentLength;
                    accumulatedLengths[i] = totalLength;
                }
            }

            if (lineList)
            {
                // Para lineList, cada linha é independente - resetar UVs para cada par
                for (var i = 1; i < pointsToDraw.Length; i += 2)
                {
                    var start = pointsToDraw[i - 1];
                    var end = pointsToDraw[i];
                    start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
                    end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

                    float segmentLength = Vector2.Distance(start, end);

                    if (lineCaps)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.Start, 0f, segmentLength));
                    }

                    segments.Add(CreateLineSegment(start, end, SegmentType.Middle, 0f, segmentLength));

                    if (lineCaps)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.End, 0f, segmentLength));
                    }
                }
            }
            else
            {
                // Draw full lines with continuous UVs
                for (var i = 1; i < pointsToDraw.Length; i++)
                {
                    var start = pointsToDraw[i - 1];
                    var end = pointsToDraw[i];
                    start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
                    end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

                    float segmentLength = segmentLengths[i - 1];
                    float accumulatedLength = accumulatedLengths[i - 1];

                    if (lineCaps && i == 1)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.Start, accumulatedLength, segmentLength));
                    }

                    segments.Add(CreateLineSegment(start, end, SegmentType.Middle, accumulatedLength, segmentLength));

                    if (lineCaps && i == pointsToDraw.Length - 1)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.End, accumulatedLength + segmentLength, segmentLength));
                    }
                }
            }

            // Add the line segments to the vertex helper, creating any joins as needed
            for (var i = 0; i < segments.Count; i++)
            {
                if (!lineList && i < segments.Count - 1)
                {
                    var vec1 = segments[i][1].position - segments[i][2].position;
                    var vec2 = segments[i + 1][2].position - segments[i + 1][1].position;
                    var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

                    // Positive sign means the line is turning in a 'clockwise' direction
                    var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

                    // Calculate the miter point
                    var miterDistance = lineThickness / (2 * Mathf.Tan(angle / 2));
                    var miterPointA = segments[i][2].position - vec1.normalized * miterDistance * sign;
                    var miterPointB = segments[i][3].position + vec1.normalized * miterDistance * sign;

                    var joinType = LineJoins;
                    if (joinType == JoinType.Miter)
                    {
                        // Make sure we can make a miter join without too many artifacts.
                        if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN)
                        {
                            segments[i][2].position = miterPointA;
                            segments[i][3].position = miterPointB;
                            segments[i + 1][0].position = miterPointB;
                            segments[i + 1][1].position = miterPointA;
                        }
                        else
                        {
                            joinType = JoinType.Bevel;
                        }
                    }

                    if (joinType == JoinType.Bevel)
                    {
                        if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                        {
                            if (sign < 0)
                            {
                                segments[i][2].position = miterPointA;
                                segments[i + 1][1].position = miterPointA;
                            }
                            else
                            {
                                segments[i][3].position = miterPointB;
                                segments[i + 1][0].position = miterPointB;
                            }
                        }

                        var join = new UIVertex[] { segments[i][2], segments[i][3], segments[i + 1][0], segments[i + 1][1] };
                        vh.AddUIVertexQuad(join);
                    }
                }

                vh.AddUIVertexQuad(segments[i]);
            }
            if (vh.currentVertCount > 64000)
            {
                Debug.LogError("Max Verticies size is 64000, current mesh verticies count is [" + vh.currentVertCount + "] - Cannot Draw");
                vh.Clear();
                return;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (m_points != null && m_points.Length > 0)
            {
                GeneratedUVs();
                vh.Clear();

                PopulateMesh(vh, m_points);
            }
            if (m_segments != null && m_segments.Count > 0)
            {
                GeneratedUVs();
                vh.Clear();

                for (int s = 0; s < m_segments.Count; s++)
                {
                    Vector2[] pointsToDraw = m_segments[s];
                    PopulateMesh(vh, pointsToDraw);
                }
            }
        }

        private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type, float accumulatedLength, float segmentLength)
        {
            if (type == SegmentType.Start)
            {
                var capStart = start - ((end - start).normalized * lineThickness / 2);
                return CreateLineSegment(capStart, start, SegmentType.Start, accumulatedLength, segmentLength);
            }
            else if (type == SegmentType.End)
            {
                var capEnd = end + ((end - start).normalized * lineThickness / 2);
                return CreateLineSegment(end, capEnd, SegmentType.End, accumulatedLength, segmentLength);
            }

            Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
            return null;
        }

        // Sobrecarga para manter compatibilidade
        private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type, UIVertex[] previousVert = null)
        {
            Vector2 offset = new Vector2((start.y - end.y), end.x - start.x).normalized * lineThickness / 2;

            Vector2 v1 = Vector2.zero;
            Vector2 v2 = Vector2.zero;
            if (previousVert != null)
            {
                v1 = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
                v2 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
            }
            else
            {
                v1 = start - offset;
                v2 = start + offset;
            }

            var v3 = end + offset;
            var v4 = end - offset;

            // Usar o método de tiling se necessário
            float segmentLength = Vector2.Distance(start, end);
            return CreateLineSegment(start, end, type, 0f, segmentLength);
        }

        private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type, float accumulatedLength, float segmentLength)
        {
            Vector2 offset = new Vector2((start.y - end.y), end.x - start.x).normalized * lineThickness / 2;

            Vector2 v1 = start - offset;
            Vector2 v2 = start + offset;
            var v3 = end + offset;
            var v4 = end - offset;

            Vector2[] uvs = CalculateUVs(accumulatedLength, segmentLength, type);

            return SetVbo(new[] { v1, v2, v3, v4 }, uvs);
        }

        private Vector2[] CalculateUVs(float accumulatedLength, float segmentLength, SegmentType type)
        {
            if (textureTileMode == TextureTileMode.Tile)
            {
                // UVs contínuas baseadas no comprimento acumulado
                float startU = accumulatedLength * textureTileScale;
                float endU = (accumulatedLength + segmentLength) * textureTileScale;

                return new Vector2[]
                {
                    new Vector2(startU, 0f), // v1 - bottom left
                    new Vector2(startU, 1f), // v2 - top left  
                    new Vector2(endU, 1f),   // v3 - top right
                    new Vector2(endU, 0f)    // v4 - bottom right
                };
            }
            else
            {
                // Comportamento original - UVs esticadas
                switch (type)
                {
                    case SegmentType.Start:
                        return startUvs;
                    case SegmentType.End:
                        return endUvs;
                    case SegmentType.Full:
                        return fullUvs;
                    default:
                        return middleUvs;
                }
            }
        }

        protected override void GeneratedUVs()
        {
            if (activeSprite != null)
            {
                var outer = Sprites.DataUtility.GetOuterUV(activeSprite);
                var inner = Sprites.DataUtility.GetInnerUV(activeSprite);
                UV_TOP_LEFT = new Vector2(outer.x, outer.y);
                UV_BOTTOM_LEFT = new Vector2(outer.x, outer.w);
                UV_TOP_CENTER_LEFT = new Vector2(inner.x, inner.y);
                UV_TOP_CENTER_RIGHT = new Vector2(inner.z, inner.y);
                UV_BOTTOM_CENTER_LEFT = new Vector2(inner.x, inner.w);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(inner.z, inner.w);
                UV_TOP_RIGHT = new Vector2(outer.z, outer.y);
                UV_BOTTOM_RIGHT = new Vector2(outer.z, outer.w);
            }
            else
            {
                UV_TOP_LEFT = Vector2.zero;
                UV_BOTTOM_LEFT = new Vector2(0, 1);
                UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0);
                UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0);
                UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1);
                UV_TOP_RIGHT = new Vector2(1, 0);
                UV_BOTTOM_RIGHT = Vector2.one;
            }

            startUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER_LEFT, UV_TOP_CENTER_LEFT };
            middleUvs = new[] { UV_TOP_CENTER_LEFT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_CENTER_RIGHT };
            endUvs = new[] { UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_RIGHT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
            fullUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
        }

        protected override void ResolutionToNativeSize(float distance)
        {
            if (UseNativeSize)
            {
                m_Resolution = distance / (activeSprite.rect.width / pixelsPerUnit);
                lineThickness = activeSprite.rect.height / pixelsPerUnit;
            }
        }

        private int GetSegmentPointCount()
        {
            if (Segments?.Count > 0)
            {
                int pointCount = 0;
                foreach (var segment in Segments)
                {
                    pointCount += segment.Length;
                }
                return pointCount;
            }
            return Points.Length;
        }

        /// <summary>
        /// Get the Vector2 position of a line index
        /// </summary>
        /// <remarks>
        /// Positive numbers should be used to specify Index and Segment
        /// </remarks>
        /// <param name="index">Required Index of the point, starting from point 1</param>
        /// <param name="segmentIndex">(optional) Required Segment the point is held in, Starting from Segment 1</param>
        /// <returns>Vector2 position of the point within UI Space</returns>
        public Vector2 GetPosition(int index, int segmentIndex = 0)
        {
            if (segmentIndex > 0)
            {
                return Segments[segmentIndex - 1][index - 1];
            }
            else if (Segments?.Count > 0)
            {
                var segmentIndexCount = 0;
                var indexCount = index;
                foreach (var segment in Segments)
                {
                    if (indexCount - segment.Length > 0)
                    {
                        indexCount -= segment.Length;
                        segmentIndexCount += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                return Segments[segmentIndexCount][indexCount - 1];
            }
            else
            {
                return Points[index - 1];
            }
        }

        /// <summary>
        /// Calculates the position of a point on the curve, given t (0-1), start point, control points and end point.
        /// </summary>
        /// <param name="t">Required Percentage between start and end point, in the range 0 to 1</param>
        /// <param name="p1">Required Starting point</param>
        /// <param name="p2">Required Control point 1</param>
        /// <param name="p3">Required Control point 2</param>
        /// <param name="p4">Required End point</param>
        /// <returns>Vector2 position of point on curve at t percentage between p1 and p4</returns>
        public Vector2 CalculatePointOnCurve(float t, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var x = p1.x + (-p1.x * 3 + t * (3 * p1.x - p1.x * t)) * t + (3 * p2.x + t * (-6 * p2.x + p2.x * 3 * t)) * t +
                    (p3.x * 3 - p3.x * 3 * t) * t2 + p4.x * t3;

            var y = p1.y + (-p1.y * 3 + t * (3 * p1.y - p1.y * t)) * t + (3 * p2.y + t * (-6 * p2.y + p2.y * 3 * t)) * t +
                    (p3.y * 3 - p3.y * 3 * t) * t2 + p4.y * t3;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Get the Vector2 position of a line within a specific segment
        /// </summary>
        /// <param name="index">Required Index of the point, starting from point 1</param>
        /// <param name="segmentIndex"> Required Segment the point is held in, Starting from Segment 1</param>
        /// <returns>Vector2 position of the point within UI Space</returns>
        public Vector2 GetPositionBySegment(int index, int segment)
        {
            return Segments[segment][index - 1];
        }

        /// <summary>
        /// Get the closest point between two given Vector2s from a given Vector2 point
        /// </summary>
        /// <param name="p1">Starting position</param>
        /// <param name="p2">End position</param>
        /// <param name="p3">Desired / Selected point</param>
        /// <returns>Closest Vector2 position of the target within UI Space</returns>
        public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 from_p1_to_p3 = p3 - p1;
            Vector2 from_p1_to_p2 = p2 - p1;
            float dot = Vector2.Dot(from_p1_to_p3, from_p1_to_p2.normalized);
            dot /= from_p1_to_p2.magnitude;
            float t = Mathf.Clamp01(dot);
            return p1 + from_p1_to_p2 * t;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_points == null || m_points?.Length == 0)
            {
                m_points = new Vector2[1];
            }
        }
    }
}