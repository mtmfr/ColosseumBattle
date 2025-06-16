using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start : MonoBehaviour
{
    [SerializeField] private GameObject heroSelectionGO;
    [SerializeField] private GameObject heroDescriptionGO;

    [SerializeField] private SO_UnitList heroes;

    [SerializeField] private Button[] heroButtons;
    [SerializeField] private Button startButton;

    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private Image selectedSprite;

    [SerializeField] private Button cancelButton;

    private Hero selectedHero;

    #region Unity fonctions
    private void Start()
    {
        //Bind SelectHero to the delegate of the buttons in heroButtons
        for (int button = 0;  button < heroButtons.Length; button++)
        {
            Button heroButton = heroButtons[button];

            if (heroButton == null)
                Debug.LogError($"No button at id {button}", this);

            //create an id variable so that every hero can be selected
            //without it the only called hero will be the one with the last id
            int id = button;
            heroButton.onClick.AddListener(delegate { SelectHero(id); });
        }

        startButton.onClick.AddListener(delegate { StartGame(selectedHero); });

        cancelButton.onClick.AddListener(Cancel);
    }

    private void OnDestroy()
    {
        for (int button = 0; button < heroButtons.Length; button++)
        {
            Button heroButton = heroButtons[button];

            heroButton.onClick.RemoveAllListeners();
        }

        startButton.onClick.RemoveListener(delegate { StartGame(selectedHero); });

        cancelButton.onClick.RemoveListener(Cancel);
    }
    #endregion

    private void SelectHero(int id)
    {
        if (heroes.units[id] is not Hero)
        {
            Debug.LogError("The selected character isn't a hero");
            return;
        }

        selectedHero = heroes.units[id] as Hero;

        UIParameters parameters = selectedHero.heroSO.uiParameters;
        heroDescription.text = parameters.heroDescription;
        selectedSprite.sprite = parameters.characterSprite;

        heroSelectionGO.SetActive(false);
        heroDescriptionGO.SetActive(true);
    }

    private void StartGame(Hero heroToStartWith)
    {
        heroSelectionGO.SetActive(true);
        heroDescriptionGO.SetActive(false);

        selectedHero = null;
        PartyManager.NewHeroBought(heroToStartWith);

        GameManager.UpdateGameState(GameState.Wave);
    }

    private void Cancel()
    {
        bool isSelecting = heroSelectionGO.activeInHierarchy;

        if (isSelecting)
            GameManager.UpdateGameState(GameState.MainMenu);

        heroSelectionGO.SetActive(true);
        heroDescriptionGO.SetActive(false);
    }

    public void SetActive(GameState gameState)
    {
        if (gameState == GameState.Start)
            gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}