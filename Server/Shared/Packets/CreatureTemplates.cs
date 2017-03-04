using Shared.Creature;
using System.Collections.Generic;
using System.IO;

namespace Shared.Packets {
    public class CreatureTemplates : BaseNetworkPacket {
        public List<CreatureTemplate> Creatures;

        public CreatureTemplates() {
            SocketId = -1;
            Creatures = new List<CreatureTemplate>();
        }

        public CreatureTemplates(int socketId, List<CreatureTemplate> creatureTemplates) {
            SocketId = socketId;
            Creatures = creatureTemplates;
        }

        public override BaseNetworkPacket FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            var count = br.ReadInt32();

            Creatures = new List<CreatureTemplate>();

            for (int i = 0; i < count; i++) {
                Creatures.Add(new CreatureTemplate(
                    br.ReadInt32(),
                    br.ReadString(), 
                    br.ReadInt32(),
                    br.ReadInt32(), 
                    br.ReadInt32()));
            }


            return this;
        }

        public override byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreatureTemplates);

            bw.Write(SocketId);

            bw.Write(Creatures.Count);

            foreach (var creature in Creatures) {
                bw.Write(creature.TemplateId);
                bw.Write(creature.Name);
                bw.Write(creature.Health);
                bw.Write(creature.MinLevel);
                bw.Write(creature.MaxLevel);
            }

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
