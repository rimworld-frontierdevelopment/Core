using System;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General
{
    public class CollisionUtility
    {
        public static class Circle
        {
            public static Vector3? LineSegment(Vector3 circleOrigin, float radius, Vector3 origin, Vector3 destination)
            {
                var d = destination - origin;
                var f = origin - circleOrigin;

                var a = Vector3.Dot(d, d);
                var b = Vector3.Dot(2*f, d) ;
                var c = Vector3.Dot(f, f) - radius * radius;

                var discriminant = b*b-4*a*c;

                if (discriminant < 0) return null;

                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.
                discriminant = (float)Math.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                var t1 = (-b - discriminant)/(2*a);
                var t2 = (-b + discriminant)/(2*a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                if( t1 >= 0 && t1 <= 1 )
                {
                    // t1 is the intersection, and it's closer than t2
                    // (since t1 uses -b - discriminant)
                    // Impale, Poke
                    return new Vector3(origin.x + t1 * d.x, origin.y + t1 * d.y, origin.z + t1 * d.z);
                }

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                if( t2 >= 0 && t2 <= 1 )
                {
                    // ExitWound
                    return new Vector3(origin.x + t1 * d.x, origin.y + t1 * d.y, origin.z + t1 * d.z);
                }

                // no intersection: FallShort, Past, CompletelyInside
                return null;
            }

            public static bool CellRect(Vector3 circleOrigin, float radius, CellRect rect)
            {
                if (rect.minX <= circleOrigin.x
                    && circleOrigin.x <= rect.maxX
                    && rect.minZ <= circleOrigin.z
                    && circleOrigin.z <= rect.maxZ) return true;

                var a = new Vector3(rect.minX + 0.5f, 0, rect.minZ + 0.5f);
                var b = new Vector3(rect.minX + 0.5f, 0, rect.maxZ + 0.5f);
                var c = new Vector3(rect.maxX + 0.5f, 0, rect.maxZ + 0.5f);
                var d = new Vector3(rect.maxX + 0.5f, 0, rect.minZ + 0.5f);

                return LineSegment(circleOrigin, radius, a, b) != null
                       || LineSegment(circleOrigin, radius, b, c) != null
                       || LineSegment(circleOrigin, radius, c, d) != null
                       || LineSegment(circleOrigin, radius, d, a) != null;
            }

            public static bool Point(Vector3 circleOrigin, float radius, Vector3 point)
            {
                return Vector3.Distance(circleOrigin, point) < radius;
            }
        }
    }
}