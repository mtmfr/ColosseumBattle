public static class PartyManager
{
    public static Hero[] heroesInBattle { get; private set; } = new Hero[5];
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
        {
            AddHeroToParty(heroBought);
            return;
        }

        heroesInBattle[firstFreeId] = heroBought;
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
}
