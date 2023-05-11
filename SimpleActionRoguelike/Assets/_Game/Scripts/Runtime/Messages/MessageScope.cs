namespace Runtime.Message
{
    public struct MessageToEntity
    {
        public int entityUid;

        public MessageToEntity(int uid)
        {
            this.entityUid = uid;
        }
    }


    public enum MessageScope
    {
        EntityMessage,   
    }

}