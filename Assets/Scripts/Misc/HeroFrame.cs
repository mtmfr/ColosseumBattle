using UnityEngine;
using UnityEngine.UI;

public class HeroFrame : MonoBehaviour
{
    [field: SerializeField] public Button button {  get; private set; }
    [field: SerializeField] public Image heroImage { get; private set; }
}
