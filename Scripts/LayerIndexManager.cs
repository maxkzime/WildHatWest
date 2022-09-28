using UnityEngine;

public class LayerIndexManager : MonoBehaviour
{
    private SpriteRenderer spr;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();

        SettingSortingOrder();
    }


    private float currentYPos = 0.0f;

    // Update is called once per frame
    private void LateUpdate()
    {
        // Compute sortig order when y position changed.
        if (currentYPos != transform.position.y)
        {
            currentYPos = transform.position.y;

            SettingSortingOrder();
        }
    }


    private void SettingSortingOrder()
    {
        spr.sortingOrder = -200 - (int)(10 * transform.position.y);
    }
}
