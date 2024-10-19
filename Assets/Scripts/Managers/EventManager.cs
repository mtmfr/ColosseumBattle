using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("EventManager is null");
            }
            return _instance;
        }
    }

    public CharacterEvent CharacterEvent { get; private set; }
    public MiscEvent MiscEvent { get; private set; }
    public WaveEvent WaveEvent { get; private set; }
    
    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);


        CharacterEvent = new();
        MiscEvent = new();
        WaveEvent = new();
    }
}
