using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Creature {
    public class Waypoint {
        public int StayTime { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Waypoint(int stayTime, float x, float y, float z) {
            StayTime = stayTime;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
