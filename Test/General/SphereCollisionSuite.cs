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
            var a = PointInside(Vector3.right);
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(CollisionUtility.Circle.Point(origin, radius, a)).True();
        }

        [Test]
        public static void PointDoesNotIntersect()
        {
            var a = PointOutside(Vector3.right);
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
            var a = PointInside(Vector3.left);
            var b = PointInside(Vector3.right);
            
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) < radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue).Is.False();
        }

        [Test]
        public static void LineSegmentPokeLeft()
        {
            TestPoke(Vector3.left);
        }
        
        [Test]
        public static void LineSegmentPokeRight()
        {
            TestPoke(Vector3.right);
        }
        
        [Test]
        public static void LineSegmentPokeUp()
        {
            TestPoke(Vector3.up);
        }
        
        [Test]
        public static void LineSegmentPokeDown()
        {
            TestPoke(Vector3.down);
        }

        [Test]
        public static void LineSegmentExitWoundLeft()
        {
            TestExitWound(Vector3.left);
        }
        
        [Test]
        public static void LineSegmentExitWoundRight()
        {
            TestExitWound(Vector3.right);
        }
        
        [Test]
        public static void LineSegmentExitWoundUp()
        {
            TestExitWound(Vector3.up);
        }
        
        [Test]
        public static void LineSegmentExitWoundDown()
        {
            TestExitWound(Vector3.down);
        }
        
        public static void LineSegmentExitWoundForward()
        {
            TestExitWound(Vector3.forward);
        }
        
        [Test]
        public static void LineSegmentExitWoundBack()
        {
            TestExitWound(Vector3.back);
        }

        [Test]
        public static void LineSegmentImpaleLeft()
        {
            TestImpale(Vector3.left);
        }
        
        [Test]
        public static void LineSegmentImpaleRight()
        {
            TestImpale(Vector3.right);
        }

        [Test]
        public static void LineSegmentTouchOuterRight()
        {
            TestTouchOuter(Vector3.right);
        }
        
        [Test]
        public static void LineSegmentTouchInnerRight()
        {
            TestTouchInner(Vector3.right);
        }
        
        private static Vector3 PointInside(Vector3 unit)
        {
            return origin + unit * (radius - 1);
        }
        
        private static Vector3 PointOutside(Vector3 unit)
        {
            return origin + unit * (radius + 1);
        }
        
        private static Vector3 PointOnEdge(Vector3 unit)
        {
            return origin + unit * radius;
        }
        
        private static void TestPoke(Vector3 unit)
        {
            Assert(unit.magnitude < 1.001 && unit.magnitude > 0.999);

            var a = PointOutside(unit);
            var b = PointInside(unit);

            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) < radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == PointOnEdge(unit)).True();
        }

        private static void TestExitWound(Vector3 unit)
        {
            Assert(unit.magnitude < 1.001 && unit.magnitude > 0.999);

            var a = PointInside(unit);
            var b = PointOutside(unit);
            
            Assert(Vector3.Distance(a, origin) < radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == PointOnEdge(unit)).True();
        }
        
        private static void TestTouchInner(Vector3 unit)
        {
            Assert(unit.magnitude < 1.001 && unit.magnitude > 0.999);

            var a = PointOnEdge(unit);
            var b = PointOutside(unit);

            Assert(Vector3.Distance(a, origin) - radius <= 0.01).True();
            Assert(Vector3.Distance(b, origin) > radius).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == PointOnEdge(unit)).True();
        }
        
        private static void TestTouchOuter(Vector3 unit)
        {
            Assert(unit.magnitude < 1.001 && unit.magnitude > 0.999);

            var a = PointOutside(unit);
            var b = PointOnEdge(unit);

            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) - radius < 0.01f).True();

            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == PointOnEdge(unit)).True();
        }
        
        private static void TestImpale(Vector3 unit)
        {
            Assert(unit.magnitude < 1.001 && unit.magnitude > 0.999);

            var a = PointOutside(unit);
            var b = PointOutside(-unit);
            
            Assert(Vector3.Distance(a, origin) > radius).True();
            Assert(Vector3.Distance(b, origin) > radius).True();
            
            var actual = CollisionUtility.Circle.LineSegment(origin, radius, a, b);
            Assert(actual.HasValue && actual.Value == PointOnEdge(unit)).True();
        }
    }
}