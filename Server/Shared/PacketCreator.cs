﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shared.Packets;

namespace Shared {
    // SHOULD BE THE SAME NAME AS THE CLASS!
    public enum PacketHeader {
        Connected, // send this to the client that connects
        CharacterDisconnect,
        Login,
        AuthenticationRespons,
        AccountRegister,
        RegisterRespons,
        RequestCharacters,
        CreateCharacter,
        CreateCharacterRespons,
        FullCharacterUpdate,
        Movement,
        ConnectedToMap,
        CharactersInMap,
        NotifyOtherPlayerMapChange, // Notify the other players that a character either DCed or changed map
        ChangeMap,
        CreaturesInMap,
        MoveCreature,
        CreatureTemplates
    }

    public class PacketCreator {
        public static BaseNetworkPacket ReadPacket(byte[] data) {
            var br = new BinaryReader(new MemoryStream(data));
            var header = (PacketHeader)br.ReadInt32();

            var packetType = Type.GetType("Shared.Packets." + header);

            if (packetType == null)
                return null;

            var instance = (BaseNetworkPacket)Activator.CreateInstance(packetType);

            return instance.FromByteArray(data);
        }
    }
}