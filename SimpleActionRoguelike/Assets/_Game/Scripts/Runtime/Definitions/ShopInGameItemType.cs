namespace Runtime.Definition
{
    public enum ShopInGameItemType
    {
        None = 0,
        BuffStat = 1, // done
        CauseStatusAfterHits = 2, // done
        HealWhenDefeatEnemyInStatus = 3, // done
        GetMoreCoinsWhenGoShop = 4, // done
        CritAfterHits = 5, 
        HealWhenEndMap = 6, // done
        IncreaseStatWhenKillEnemies = 7, // done => improve with lifetime buffed.
        BuffStatWhenHealthEnough = 8, // done
        HealAfterCrit = 9, // done
        BuffStatWhenLowerHealth = 10,
        BuffStatWhenDamagedWithDuration = 11,
        BuffStatWhenKillEnemiesWithDuration = 12,
        ConvertRuneOnMapToCoins = 13,
        PersistantRune = 14, // concern
        MagnetoRune = 15, 
    }
}
