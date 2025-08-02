public static class PartyManager
{
    public static Hero[] heroesInBattle = new Hero[5];
    public static Hero[] heroesInParty { get; private set; } = new Hero[10];

    /// <summary>
    /// Add a new hero to the party
    /// </summary>
    /// <param name="heroBought"></param>
    public static void NewHeroBought(Hero heroBought)
    {
        if (heroBought == null)
            return;

        int firstFreeId = heroesInBattle.GetFirstNullId();

        if (firstFreeId < 0)
            AddHeroToParty(heroBought);
        else heroesInBattle[firstFreeId] = heroBought;
    }

    /// <summary>
    /// Add a hero to the party
    /// </summary>
    /// <param name="heroToAdd"></param>
    private static void AddHeroToParty(Hero heroToAdd)
    {
        int heroId = heroesInParty.GetFirstNullId();

        heroesInParty[heroId] = heroToAdd;
    }

    /// <summary>
    /// Check if the hero can be bought
    /// </summary>
    /// <returns>true if it can be bought
    /// <br>false if it cannot be bought</br>
    /// </returns>
    public static bool CanHeroBeBought()
    {
        int inBattle = heroesInBattle.GetFirstNullId();

        int inParty = heroesInParty.GetFirstNullId();

        return inBattle >= 0 || inParty >= 0;
    }

    public static void ClearParty()
    {
        heroesInBattle = new Hero[5];
        heroesInParty = new Hero[1]; 
    }

    #region Party management
    /// <summary>
    /// Check if there will still be at least 1 hero in battle when moving one to the party
    /// </summary>
    /// <param name="toMove">the data of the hero to move</param>
    /// <returns>true if there will still be at least one hero in battle.
    /// <br>false otherwise.</br>
    /// </returns>
    public static bool CanHeroBeMovedToParty(HeroPartyData toMove)
    {
        Hero[] inBattleCopy = new Hero [5];
        heroesInBattle.CopyTo(inBattleCopy, 0);

        //Remove the hero at the it's id in the battle to simulate the exchange with a null party place
        inBattleCopy[toMove.position] = null;

        //Loop through the battle party to see if there at least one hero inside
        for (int id = 0;  id < inBattleCopy.Length; id++)
        {
            if (inBattleCopy[id] != null)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Exchange the placement of 2 heroes in the heroesInParty array
    /// </summary>
    public static void InvertPartyPlace(HeroPartyData fromParty, HeroPartyData toParty)
    {
        heroesInParty[toParty.position] = fromParty.hero;
        heroesInParty[fromParty.position] = toParty.hero;
    }

    /// <summary>
    /// Exchange the placement of 2 heroes in the heroesInBattle array
    /// </summary>
    public static void InvertBattlePosition(HeroPartyData fromBattle, HeroPartyData toBattle)
    {
        heroesInBattle[toBattle.position] = fromBattle.hero;
        heroesInBattle[fromBattle.position] = toBattle.hero;
    }

    /// <summary>
    /// Exchange a hero in the party to one in battle
    /// </summary>
    public static void MoveToParty(HeroPartyData fromBattle, HeroPartyData toParty)
    {
        heroesInParty[toParty.position] = fromBattle.hero;
        heroesInBattle[fromBattle.position] = toParty.hero;
    }
    #endregion
}

public struct HeroPartyData
{
    public Hero hero;
    public int position;

    public HeroPartyData(Hero hero, int position)
    {
        this.hero = hero;
        this.position = position;
    }
}