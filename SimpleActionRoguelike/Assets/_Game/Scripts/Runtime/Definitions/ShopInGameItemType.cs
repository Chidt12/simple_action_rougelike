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
        DamageIncreaseWhenAttackSameTarget = 7,
        IncreaseStatWhenKillEnemies = 8, // done => improve with lifetime buffed.
        BuffStatWhenHealthEnough = 9,
        HealAfterCrit = 10, // done
        BuffStatWhenLowerHealth = 11,
        BuffStatWhenDamagedWithDuration = 12,
        BuffStatWhenKillEnemiesWithDuration = 13,
        ConvertRuneOnMapToCoins = 14,
        PersistantRune = 15, // concern
        MagnetoRune = 16, 
    }
}
