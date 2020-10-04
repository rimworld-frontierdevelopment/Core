using UnityEngine;
using Verse;

namespace FrontierDevelopments.General
{
    public class GraphicsUtility
    {
        public static void DrawLine(Vector3 a, Vector3 b, Material mat, float? altitude=null, float thickness=0.2f)
        {
            if (a == b) 
                return;
            if (Mathf.Abs(a.x - b.x) < 0.009999999776482582 
                && Mathf.Abs(a.z - b.z) < 0.009999999776482582)
                return;
            Vector3 position = (a + b) / 2f;
            if(altitude != null)
                position.y = altitude.Value;
            a.y = b.y;
            float z = (a - b).MagnitudeHorizontal();
            Quaternion q = Quaternion.LookRotation(a - b);
            Vector3 scaling = new Vector3(thickness, 1f, z);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, q, scaling);
            Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
        }
    }
}