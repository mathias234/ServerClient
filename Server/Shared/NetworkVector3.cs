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

        public static NetworkVector3 Deserialze(string vector) {
            var vector3 = new NetworkVector3();

            var position = vector.Split(' ');
            vector3.X = float.Parse(position[0]);
            vector3.Y = float.Parse(position[1]);
            vector3.Z = float.Parse(position[2]);

            return vector3;
        }

        public string Serialize() {
            return X + " " + Y + " " + Z;
        }
    }
}
