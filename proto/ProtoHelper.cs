namespace Paint.Proto
{
    public static class ProtoHelper
    {
        public static PlayerData ToPlayerData(this byte[] bytes)
        {
            return PlayerData.Parser.ParseFrom(bytes);
        }

        public static LobbyData ToLobbyData(this byte[] bytes)
        {
            return LobbyData.Parser.ParseFrom(bytes);
        }
    }
}