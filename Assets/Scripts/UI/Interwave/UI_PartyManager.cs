using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PartyController
{
    [Header("Prefab")]
    [SerializeField] private HeroFrame baseHeroFrame;

    [Header("Parents")]
    [SerializeField] private Transform inBattleFrameParent;
    [SerializeField] private Transform partyFrameParent;

    private readonly Dictionary<int, HeroFrame> battleFrames = new();
    private readonly Dictionary<int, HeroFrame> partyFrames = new();

    //Wether a frame has previously been selected
    private bool isFrameSelected = false;

    //The frame currently in wait
    KeyValuePair<int, HeroFrame> frameInWait = new (-1, null);

    public void Initialize()
    {
        int maxHeroesInBattle = PartyManager.heroesInBattle.Length;
        for (int heroInBattle = 0; heroInBattle < maxHeroesInBattle; heroInBattle++)
        {
            //Create a new frame that will have a key corresponding to an id of the heroesInBattle array
            CreateNewFrame(heroInBattle, true);
        }

        int maxHeroesInParty = PartyManager.heroesInParty.Length;
        for (int heroInParty = 0; heroInParty < maxHeroesInParty; heroInParty++)
        {
            //Create a new frame that will have a key corresponding to an id of the heroesInParty array
            CreateNewFrame(heroInParty, false);
        }
    }

    public void Destroy()
    {
        DeleteAllFrames();
    }

    public void Disable()
    {
        isFrameSelected = false;
    }

    #region frame Creation/destruction
    /// <summary>
    /// Create a new frame
    /// </summary>
    /// <param name="key">the id of the frame</param>
    /// <param name="isInBattle">wether the frame is for the heroes in battle or in the party</param>
    public void CreateNewFrame(int key, bool isInBattle)
    {
        HeroFrame frameToCreate = ObjectPool.GetObject(baseHeroFrame);

        frameToCreate.transform.SetParent(isInBattle ? inBattleFrameParent : partyFrameParent, false);

        if (isInBattle)
        {
            frameToCreate.button.onClick.AddListener(delegate { SelectInBattleFrame(key); });
            battleFrames.Add(key, frameToCreate);
        }
        else
        {
            frameToCreate.button.onClick.AddListener(delegate { SelectPartyFrame(key); });
            partyFrames.Add(key, frameToCreate);
        }
    }

    /// <summary>
    /// Delete all created frames
    /// </summary>
    private void DeleteAllFrames()
    {
        foreach (KeyValuePair<int, HeroFrame> kvpFrame in battleFrames)
        {
            kvpFrame.Value.button.onClick.RemoveAllListeners();
        }

        foreach (KeyValuePair<int, HeroFrame> kvpFrame in partyFrames)
        {
            kvpFrame.Value.button.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region frame update
    /// <summary>
    /// Used to update the frames
    /// </summary>
    public void UpdateFrames()
    {
        UpdateHeroesInBattleFrame();
        UpdateHeroesInPartyFrame();
    }

    /// <summary>
    /// Update the sprite of the frames of the heroes in battle
    /// </summary>
    private void UpdateHeroesInBattleFrame()
    {
        Hero[] heroes = PartyManager.heroesInBattle;

        for (int id = 0; id < heroes.Length; id++)
        {
            Image imageToUpdate = battleFrames[id].heroImage;

            //Check if the correponding id of heroes is null
            if (heroes[id] == null)
            {
                imageToUpdate.sprite = null;
                imageToUpdate.color = Color.clear;
            }
            else
            {
                Sprite heroSprite = heroes[id].heroSO.uiParameters.characterSprite;
                imageToUpdate.sprite = heroSprite;

                imageToUpdate.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Update the sprite of the frames of the heroes in party
    /// </summary>
    private void UpdateHeroesInPartyFrame()
    {
        Hero[] heroes = PartyManager.heroesInParty;

        for (int id = 0; id < heroes.Length; id++)
        {
            Image imageToUpdate = partyFrames[id].heroImage;

            //Check if the correponding id of heroes is null
            if (heroes[id] == null)
            {
                imageToUpdate.sprite = null;
                imageToUpdate.color = Color.clear;
            }
            else
            {
                Sprite heroSprite = heroes[id].heroSO.uiParameters.characterSprite;
                imageToUpdate.sprite = heroSprite;

                imageToUpdate.color = Color.white;
            }
        }
    }
    #endregion

    #region frame selection
    /// <summary>
    /// Select a party frame to to interchage the position of 2 heroes in the ui
    /// <param name="newSelectedId">the key of the frame in partyFrames</param>
    private void SelectPartyFrame(int newSelectedId)
    {
        //Check if another frame has been selected
        if (isFrameSelected)
        {
            int frameInWaitKey = GetWaitingFrameKey();

            bool isPartyFrames = false;

            //check if the previously selected frame is a party one
            if (partyFrames.ContainsKey(frameInWaitKey))
                isPartyFrames = partyFrames[frameInWaitKey] == frameInWait.Value;

            //Get the hero at the id linked to the key
            Hero newlySelectedHero = PartyManager.heroesInParty[newSelectedId];
            HeroPartyData currentPartyPair = new(newlySelectedHero, newSelectedId);

            if (isPartyFrames)
            {
                //Get the hero at the id linked to the key
                Hero inWaitHero = PartyManager.heroesInParty[frameInWaitKey];

                HeroPartyData inWaitPartyPair = new(inWaitHero, frameInWaitKey);

                PartyManager.InvertPartyPlace(inWaitPartyPair, currentPartyPair);
            }
            else
            {
                Hero inWaitHero = PartyManager.heroesInBattle[frameInWaitKey];

                HeroPartyData inWaitBattlePair = new(inWaitHero, frameInWaitKey);

                if (currentPartyPair.hero == null)
                {
                    if (!PartyManager.CanHeroBeMovedToParty(inWaitBattlePair))
                    {
                        isFrameSelected = false;
                        return;
                    }
                }
                PartyManager.MoveToParty(inWaitBattlePair, currentPartyPair);
            }

            isFrameSelected = false;
            UpdateFrames();
        }
        else
        {
            //Check if the linked party id has a value
            if (PartyManager.heroesInParty[newSelectedId] == null)
                return;

            frameInWait = new(newSelectedId, partyFrames[newSelectedId]);
            isFrameSelected = true;
        }
    }

    /// <summary>
    /// Select a party frame to to interchage the position of 2 heroes in the ui
    /// </summary>
    /// <param name="newSelectedId">the key of the frame in battleFrames</param>
    private void SelectInBattleFrame(int newSelectedId)
    {
        //Check if another frame has been selected
        if (isFrameSelected)
        {
            int frameInWaitKey = GetWaitingFrameKey();

            bool isBattleFrame = false;

            //check if the previously selected frame is a party one
            if (battleFrames.ContainsKey(frameInWaitKey))
                isBattleFrame = battleFrames[frameInWaitKey] == frameInWait.Value;

            Hero newlySelected = PartyManager.heroesInBattle[newSelectedId];

            HeroPartyData currentBattlePair = new(newlySelected, newSelectedId);

            if (isBattleFrame)
            {
                Hero inWaitHero = PartyManager.heroesInBattle[frameInWaitKey];

                HeroPartyData inWaitBattlePair = new(inWaitHero, frameInWaitKey);

                PartyManager.InvertBattlePosition(inWaitBattlePair, currentBattlePair);
            }
            else
            {
                Hero inWaitHero = PartyManager.heroesInParty[frameInWaitKey];

                HeroPartyData inWaitPartyPair = new(inWaitHero, frameInWaitKey);

                if (inWaitPartyPair.hero == null)
                {
                    if (!PartyManager.CanHeroBeMovedToParty(currentBattlePair))
                    {
                        isFrameSelected = false;
                        return;
                    }
                }

                PartyManager.MoveToParty(currentBattlePair, inWaitPartyPair);
            }

            isFrameSelected = false;
            UpdateFrames();
        }
        else
        {
            //Check if the linked inBattle id has a value
            if (PartyManager.heroesInBattle[newSelectedId] == null)
                return;

            frameInWait = new (newSelectedId, battleFrames[newSelectedId]);
            isFrameSelected = true;
        }
    }
    #endregion

    /// <summary>
    /// Get the frame that is currently in wait
    /// </summary>
    private int GetWaitingFrameKey()
    {
        if (frameInWait.Key < 0)
        {
            isFrameSelected = false;
            throw new ArgumentNullException("No coreesonding id", "There is no id that correspond to the selected frame key");
        }

        return frameInWait.Key;
    }
}