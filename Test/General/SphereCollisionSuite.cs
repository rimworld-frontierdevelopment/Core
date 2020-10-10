using System;
using System.Linq;
using System.Text;
using RimTest;
using UnityEngine;
using Verse;
using static RimTest.Assertion;

namespace FrontierDevelopments.General
{
    [TestSuite]
    public static class SphereCollisionSuite
    {
        private const float Radius = 10f;
        private static readonly Vector3 Origin = Vector3.right * Radius;

        [Test]
        public static void ValidatePointInside()
        {
            Assert(Vector3.Distance(PointInside(Vector3.right), Origin) < Radius).True();
        }

        [Test]
        public static void ValidatePointOutside()
        {
            Assert(Vector3.Distance(PointOutside(Vector3.right), Origin) > Radius).True();
        }

        [Test]
        public static void ValidatePointOnEdge()
        {
            Assert(Vector3.Distance(PointOnEdge(Vector3.right), Origin) == Radius).True();
        }

        [Test]
        public static void PointIntersects()
        {
            var a = PointInside(Vector3.right);
            Assert(Vector3.Distance(a, Origin) < Radius).True();
            Assert(CollisionUtility.Circle.Point(Origin, Radius, a)).True();
        }

        [Test]
        public static void PointDoesNotIntersect()
        {
            var a = PointOutside(Vector3.right);
            Assert(Vector3.Distance(a, Origin) > Radius).True();
            Assert(CollisionUtility.Circle.Point(Origin, Radius, a)).False();
        }

        [Test]
        public static void LineSegmentEnter()
        {
            TestAllAngles("Enter", TestPoke, ShouldHaveResult);
        }
        
        [Test]
        public static void LineSegmentExit()
        {
            TestAllAngles("Exit", TestExitWound, ShouldHaveResult);
        }

        [Test]
        public static void LineSegmentThrough()
        {
            TestAllAngles("Through", TestImpale, ShouldHaveResult);
        }

        [Test]
        public static void LineSegmentTouchOutside()
        {
            TestAllAngles("Touch outer", TestTouchOuter, ShouldHaveResult);
        }
        
        [Test]
        public static void LineSegmentTouchInside()
        {
            TestAllAngles("Touch inner", TestTouchInner, ShouldHaveResult);
        }
        
        [Test]
        public static void LineSegmentOutside()
        {
            TestAllAngles("Outside", TestOutside, ShouldHaveNoResult);
        }

        [Test]
        public static void LineSegmentInside()
        {
            TestAllAngles("Inside", TestInside, ShouldHaveNoResult);
        }

        private static void TestAllAngles(string name, Func<Vector3, Vector3?> doTest, Func<Vector3?, Vector3, bool> validator)
        {
            var failures = 0;
            var failureText = new StringBuilder();

            var degrees = CircleUtility.Degrees(0.1f).ToList();

            string TestUnit(Vector3 direction)
            {
                var ray = new Ray(Vector3.zero, direction);
                var unit = ray.GetPoint(1f);

                var actual = doTest.Invoke(unit);
                var passed = validator.Invoke(actual, unit);

                if (!passed)
                {
                    failures++;

                    if (actual == null)
                    {
                        return "no result when one was expected";
                    }
                    else
                    {
                        return "Distance between actual and expected: " + Vector3.Distance(PointOnEdge(unit), actual.Value);
                    }
                }

                return null;
            }

            var errorMessage = TestUnit(Vector3.up);
            if (errorMessage != null)
            {
                failureText.AppendLine("Up: " + errorMessage);
            }
            
            errorMessage = TestUnit(Vector3.down);
            if (errorMessage != null)
            {
                failureText.AppendLine("Down: " + errorMessage);
            }
            
            foreach (var degree in degrees)
            {
                errorMessage = TestUnit(CircleUtility.PolarToCartesian(degree));
                if (errorMessage != null)
                {
                    failureText.AppendLine("" + degree + ": " + errorMessage);
                }
            }

            if (failures > 0)
            {
                Log.Error(name + " Failures: " + failures + " out of " + degrees.Count + 2 + " cases\n" + failureText.ToString().TrimEndNewlines());
                Assert(false).True();
            }
        }

        private static bool ShouldHaveResult(Vector3? actual, Vector3 unit)
        {
            return actual.HasValue && actual.Value == PointOnEdge(unit);
        }

        private static bool ShouldHaveNoResult(Vector3? actual, Vector3 unit)
        {
            return !actual.HasValue;
        }
        
        private static Vector3 PointInside(Vector3 unit, float offset=1f) => Origin + unit * (Radius - offset);

        private static Vector3 PointOutside(Vector3 unit, float offset=1f) => Origin + unit * (Radius + offset);

        private static Vector3 PointOnEdge(Vector3 unit) => Origin + unit * Radius;

        private static Vector3? TestPoke(Vector3 unit)
        {
            var a = PointOutside(unit);
            var b = PointInside(unit);

            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestExitWound(Vector3 unit)
        {
            var a = PointInside(unit);
            var b = PointOutside(unit);

            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestTouchInner(Vector3 unit)
        {
            var a = PointOnEdge(unit);
            var b = PointInside(unit);

            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestTouchOuter(Vector3 unit)
        {
            var a = PointOutside(unit);
            var b = PointOnEdge(unit);

            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestImpale(Vector3 unit)
        {
            var a = PointOutside(unit);
            var b = PointOutside(-unit);
            
            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestOutside(Vector3 unit)
        {
            var a = PointOutside(unit, 0.01f);
            var b = PointOutside(unit, 2f);

            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }

        private static Vector3? TestInside(Vector3 unit)
        {
            var a = PointInside(unit, 0.01f);
            var b = PointInside(unit, 2f);
            
            return CollisionUtility.Circle.LineSegment(Origin, Radius, a, b);
        }
    }
}