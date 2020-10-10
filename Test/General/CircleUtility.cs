using System.Collections.Generic;
using UnityEngine;

namespace FrontierDevelopments.General
{
    public static class CircleUtility
    {
        public static Vector3 PolarToCartesian(float degrees, float radius = 1f)
        {
            var angleRad = Mathf.Deg2Rad * degrees;
            var x = radius * Mathf.Cos(angleRad);
            var y = radius * Mathf.Sin(angleRad);
            return new Vector3(x, 0f, y);
        }
        
        public static IEnumerable<Vector3> ValuesForDegrees(float degree, float radius = 1.0f)
        {
            for (var current = 0f; current < 360; current += degree)
            {
                yield return PolarToCartesian(current, radius);
            }
        }

        public static IEnumerable<float> Degrees(float increment = 1f)
        {
            for (var current = 0f; current < 360; current += increment)
            {
                yield return current;
            }
        }
    }
}