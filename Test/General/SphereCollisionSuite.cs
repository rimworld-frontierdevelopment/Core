using RimTest;
using UnityEngine;
using static RimTest.Assertion;

namespace FrontierDevelopments.General
{
    [TestSuite]
    public static class SphereCollisionSuite
    {
        static float radius = 10f;
        static Vector3 origin = Vector3.right * radius;
        
        [Test]
        public static void PointIntersects()
        {
            var a = origin + Vector3.right;
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(CollisionUtility.Circle.Point(origin, radius, a)).True();
        }

        [Test]
        public static void PointDoesNotIntersect()
        {
            var a = origin + Vector3.right * (radius + 1);
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(CollisionUtility.Circle.Point(origin, radius, a)).False();
        }

        [Test]
        public static void LineSegmentMiss()
        {
            var a = origin * radius;
            var b = origin * (radius + 1);
            
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue).Is.False();
        }

        [Test]
        public static void LineSegmentInside()
        {
            var a = origin + Vector3.right;
            var b = origin - Vector3.right;
            
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) < radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue).Is.False();
        }
        
        [Test]
        public static void LineSegmentPokeLeft()
        {
            var a = origin - Vector3.right * (radius + 1);
            var b = origin - Vector3.right * (radius - 1);
            
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) < radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin - Vector3.right * radius).True();
        }
        
        [Test]
        public static void LineSegmentPokeRight()
        {
            var a = origin + Vector3.right * (radius + 1);
            var b = origin + Vector3.right * (radius - 1);
            
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) < radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin + Vector3.right * radius).True();
        }
        
        [Test]
        public static void LineSegmentExitWoundLeft()
        {
            var a = origin - Vector3.right * (radius - 1);
            var b = origin - Vector3.right * (radius + 1);
            
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin - Vector3.right * radius).True();
        }
        
        [Test]
        public static void LineSegmentExitWoundRight()
        {
            var a = origin + Vector3.right * (radius - 1);
            var b = origin + Vector3.right * (radius + 1);
            
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();
            
            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin + Vector3.right * radius).True();
        }
        
        [Test]
        public static void LineSegmentExitWoundUp()
        {
            var a = origin + Vector3.forward * (radius - 1);
            var b = origin + Vector3.forward * (radius + 1);

            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();
            
            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin + Vector3.forward * radius).True();
        }
        
        [Test]
        public static void LineSegmentExitWoundDown()
        {
            var a = origin - Vector3.forward * (radius - 1);
            var b = origin - Vector3.forward * (radius + 1);

            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();
            
            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin - Vector3.forward * radius).True();
        }
        
        [Test]
        public static void LineSegmentImpaleLeft()
        {
            var a = origin - Vector3.right * (radius + 1);
            var b = origin + Vector3.right * (radius + 1);
            
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();
            
            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin - Vector3.right * radius).True();
        }
        
        [Test]
        public static void LineSegmentImpaleRight()
        {
            var a = origin + Vector3.right * (radius + 1);
            var b = origin - Vector3.right * (radius + 1);

            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == origin + Vector3.right * radius).True();
        }
    }
}