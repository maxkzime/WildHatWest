using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopQuitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite buttonDefault;
    [SerializeField] private Sprite buttonPressed;


    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = buttonPressed;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = buttonDefault;
    }

    public void OnClick()
    {
        SoundManager.Instance.PlayPressedButtonSoundEffect();

        // Save durable data before quitting.
        FindObjectOfType<SaveLoadSystem>().Save();

        // Quit game.
        Application.Quit();
    }
}
