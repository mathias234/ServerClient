namespace Shared.Packets {
    public interface INetworkPacket<T> where T : BaseNetworkPacket {
        byte[] ToByteArray();
        T FromByteArray(byte[] byteArray);
    }
}