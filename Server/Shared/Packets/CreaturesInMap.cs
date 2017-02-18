using Shared.Creature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared.Packets {
    public class CreaturesInMap : BaseNetworkPacket, INetworkPacket<CreaturesInMap> {
        public List<InstancedCreature> Creatures = new List<InstancedCreature>();

        public CreaturesInMap(int socketId, List<InstancedCreature> creatures) {
            SocketId = socketId;
            Creatures = creatures;
        }

        public CreaturesInMap() {

        }

        public CreaturesInMap FromByteArray(byte[] byteArray) {
            var br = new BinaryReader(new MemoryStream(byteArray));

            Header = (PacketHeader)br.ReadInt32();
            SocketId = br.ReadInt32();

            var count = br.ReadInt32();

            Creatures = new List<InstancedCreature>();

            for (int i = 0; i < count; i++) {
                var creature = new InstancedCreature(
                    br.ReadInt32(),
                    br.ReadInt32(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadInt32());

                Creatures.Add(creature);
            }

            return this;
        }

        public byte[] ToByteArray() {
            var bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)PacketHeader.CreaturesInMap);

            bw.Write(SocketId);
            bw.Write(Creatures.Count);

            foreach (var creature in Creatures) {
                bw.Write(creature.InstanceId);
                bw.Write(creature.TemplateId);
                bw.Write(creature.X);
                bw.Write(creature.Y);
                bw.Write(creature.Z);
                bw.Write(creature.MapId);
            }

            var data = ((MemoryStream)bw.BaseStream).ToArray();

            return data;
        }
    }
}
