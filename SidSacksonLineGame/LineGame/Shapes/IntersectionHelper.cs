using System;
using System.Collections.Generic;
using System.Drawing;

namespace SidSacksonLineGame
{
    public class IntersectionHelper
    {
        static Boolean onSegment(Point p, Point q, Point r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }

        static int orientation(Point p, Point q, Point r)
        {
            int val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0; 

            return (val > 0) ? 1 : 2;
        }

        public static Boolean doIntersect(Point p1, Point q1, Point p2, Point q2)
        {

            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4)
                return true;
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; 
        }
        public static PointF FindIntersection(PointF s1, PointF e1, PointF s2, PointF e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;

            float delta = a1 * b2 - a2 * b1;
            
            return delta == 0 ? new PointF(float.NaN, float.NaN)
                : new PointF((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        public static bool drawnAway(Point start, Point endOne, Point endTwo)
        {
            if (start.x == endOne.x)
            {
                bool directionOne = (endOne.y - start.y) > 0;
                bool directionTwo = (endTwo.y - start.y) > 0;
                if (directionOne != directionTwo) return true;
                else return false;
            }
            else if (start.y == endOne.y)
            {
                bool directionOne = (endOne.x - start.x) > 0;
                bool directionTwo = (endTwo.x - start.x) > 0;
                if (directionOne != directionTwo) return true;
                else return false;
            }
            else
            {
                bool directionOneX = (endOne.x - start.x) > 0;
                bool directionTwoX = (endTwo.x - start.x) > 0;
                bool directionOneY = (endOne.x - start.x) > 0;
                bool directionTwoY = (endTwo.x - start.x) > 0;
                if (directionOneX != directionTwoX && directionOneY != directionTwoY) return true;
                else return false;
            }
        }
    }
}
