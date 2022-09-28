using UnityEngine;

public class StartMenuButtonsController : MonoBehaviour
{
    public int currentIndex = 0;
    private bool keyDown = false;
    private int maxIndex = 2;

    void Update()
    {
        // Press down or up arrow key, and has been released before.
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!keyDown)
            {
                // Up arrow pressed.
                if (Input.GetAxis("Vertical") < 0.01f)
                {
                    // While index is inferior of maxIndex, increase index.
                    if (currentIndex < maxIndex)
                    {
                        currentIndex++;
                    }
                    // Else, reset index (Go to first button).
                    else
                    {
                        currentIndex = 0;
                    }
                }
                // Down arrow pressed.
                else if (Input.GetAxis("Vertical") > 0.01f)
                {
                    // While index is superior of 0, decrease index.
                    if (currentIndex > 0)
                    {
                        currentIndex--;
                    }
                    // Else, reset index to maxIndex (Go to last button).
                    else
                    {
                        currentIndex = maxIndex;
                    }
                }

                // Key is pressed.
                keyDown = true;
            }
        }
        else if (keyDown)
        {
            keyDown = false;
        }
    }
}
