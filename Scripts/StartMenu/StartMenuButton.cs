using UnityEngine;

public class StartMenuButton : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    [SerializeField] int thisIndex;
    [SerializeField] private StartMenuButtonsController menuButtonsController;

    private void Update()
    {
        // If this button is selected.
        if (menuButtonsController.currentIndex == thisIndex && !isClicked && isActiveAndEnabled)
        {
            // Play selected animation.
            animator.SetBool("Selected", true);


            // If submit (space) pressed, start pressed animation.
            if (Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("Pressed", true);
            }
            else if (animator.GetBool("Pressed"))
            {
                animator.SetBool("Pressed", false);

                SoundManager.Instance.PlayPressedButtonSoundEffect();
            }
        }
        else if (animator.GetBool("Selected"))
        {
            // Deselect button.
            animator.SetBool("Selected", false);

            SoundManager.Instance.PlaySelectedButtonSoundEffect();
        }
    }




    #region /*** Start menu new game button, called with animator event on pressed animation. ***/
    [SerializeField] private GameObject startMenuButtons;
    [SerializeField] private Animator startMenuAnimator;
    private bool hasBeenClickedOnce = false;

    private void PlayNewGameButtonFunctions()
    {
        if (!hasBeenClickedOnce)
        {
            hasBeenClickedOnce = true;

            // Desactivate startMenu buttons.
            startMenuButtons.SetActive(false);

            startMenuAnimator.SetTrigger("CloseStartMenu");

            // Load saved data.
            FindObjectOfType<SaveLoadSystem>().Load();

            ObjectsGeneration.Instance.CreateNewGame();
        }

        Unpressed();
    }
    #endregion /*** ^^^ Start menu new game button, called with animator event on pressed animation. ^^^ ***/




    #region /*** Start menu credits button, called with animator event on pressed animation. ***/
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject gameTitle;

    private bool creditsHasBeenClickedOnce = false;

    private void PlayCreditsButtonFunctions()
    {
        if (!creditsHasBeenClickedOnce)
        {
            creditsHasBeenClickedOnce = true;

            // Desactivate startMenu buttons and game title.
            startMenuButtons.SetActive(false);
            gameTitle.SetActive(false);

            // Activate credits panel.
            creditsPanel.SetActive(true);
        }

        Unpressed();
    }
    #endregion /*** ^^^ Start menu credits button, called with animator event on pressed animation. ^^^ ***/


    #region /*** Used to display start menu back, is used on "back" button in credits panel. ***/
    public void DisplayStartMenuBack()
    {
        SoundManager.Instance.PlayPressedButtonSoundEffect();

        // Desactivate startMenu buttons and game title.
        startMenuButtons.SetActive(true);
        gameTitle.SetActive(true);

        // Activate credits panel.
        creditsPanel.SetActive(false);

        creditsHasBeenClickedOnce = false;
    }
    #endregion /*** Used to display start menu back, is used on "back" button in credits panel. ***/




    #region /*** Start menu quit button, called with animator event on pressed animation. ***/
    private void PlayQuitButtonFunctions()
    {
        if (!hasBeenClickedOnce)
        {
            hasBeenClickedOnce = true;

            // Save durable data before quitting (not really necessary as there's no new data on start menu).
            FindObjectOfType<SaveLoadSystem>().Save();

            // Quit game.
            Application.Quit();
        }

        Unpressed();

    }
    #endregion /*** ^^^ Start menu quit button, called with animator event on pressed animation. ^^^ ***/



    #region /*** Pressing button mechanic, called when user click on button. Start the pressed animation. ***/
    private bool isClicked = false;
    public void AnimateByClick()
    {
        if(!isClicked)
        {
            isClicked = true;

            animator.SetBool("Pressed", true);
        }
    }
    #endregion /*** ^^^ Pressing button mechanic, called when user click on button. Start the pressed animation. ^^^ ***/



    #region /*** Unpressed button mechanic, called in each pressed button functions. ***/
    private void Unpressed()
    {
        if (isClicked)
        {
            animator.SetBool("Pressed", false);

            SoundManager.Instance.PlayPressedButtonSoundEffect();

            isClicked = false;
        }
    }
    #endregion /*** ^^^ Unpressed button mechanic, called in each pressed button functions. ^^^ ***/
}

