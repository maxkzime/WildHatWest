using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopStartButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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


    #region /*** Close shop when pressing start button. ***/
    [SerializeField] private Animator shopAnimator;
    public void Clicked()
    {
        // Close shop and start new game when boss is destroyed (no more on field) and shop is displayed.
        if (ObjectsGeneration.Instance.displayingShop &&
            !ObjectsGeneration.Instance.GetBossIsOnField() &&
            GameObject.FindGameObjectsWithTag("CactusOutlaw").Length <= 0)
        {
            ObjectsGeneration.Instance.displayingShop = false;

            // Save durable data to parachute if the game crash.
            FindObjectOfType<SaveLoadSystem>().Save();

            SoundManager.Instance.PlayPressedButtonSoundEffect();

            // Hide shop.
            shopAnimator.SetTrigger("DisplayShop");

            // Start new game.
            ObjectsGeneration.Instance.GenerateArena();
        }
    }
    #endregion /*** ^^^ Close shop when pressing start button. ^^^ ***/
}
