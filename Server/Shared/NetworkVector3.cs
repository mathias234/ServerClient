using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    public class NetworkVector3 {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public NetworkVector3() {

        }

        public NetworkVector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public static float Distance(NetworkVector3 v1, NetworkVector3 v2) {
            NetworkVector3 v = new NetworkVector3();

            v.X = v1.X - v2.X;
            v.Y = v1.Y - v2.Y;
            v.Z = v1.Z - v2.Z;

            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y * v.Z * v.Z);
        }
    }
}
