using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared {
    public class Character {
        public string Name { get; set; }
        public int Level { get; set; }
        public CharacterClasses Class { get; set; }

        public Character(string name, int level, CharacterClasses charClass) {
            Name = name;
            Level = level;
            Class = charClass;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());

            bw.Write(Name);
            bw.Write(Level);
            bw.Write((byte)Class);

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
