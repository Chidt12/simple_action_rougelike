using Runtime.Definition;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public struct FinishedLoadNextLevelMessage : IMessage
    {
        public readonly GameplayRoomType RoomType;

        public FinishedLoadNextLevelMessage(GameplayRoomType roomType)
        {
            RoomType = roomType;
        }
    }

    public struct EnteredNextLevelMessage : IMessage
    {
        public readonly GameplayRoomType RoomType;

        public EnteredNextLevelMessage(GameplayRoomType roomType)
        {
            RoomType = roomType;
        }
    }
}