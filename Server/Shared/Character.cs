using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared {
    public class Character {
        public string Name { get; set; }
        public int Level { get; set; }

        public Character(string name, int level) {
            Name = name;
            Level = level;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());

            bw.Write(Name);
            bw.Write(Level);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
