using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class StunStatusDataConfigItem : StatusDataConfigItem
    {}

    public class StunStatusDataConfig : StatusDataConfig<StunStatusDataConfigItem>
    {}
}
