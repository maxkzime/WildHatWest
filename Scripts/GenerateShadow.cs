using UnityEngine;

public class GenerateShadow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreateShadow();
    }


    private GameObject shadow;
    private SpriteRenderer spr;
    private readonly float sunAngle = 35;

    private void CreateShadow()
    {
        // Delete old shadow if existing.
        var oldShadow = transform.Find("shadow");

        if (oldShadow != null)
        {
            Destroy(oldShadow.gameObject);
        }

        // Create new shadow.
        shadow = new GameObject("shadow");

        // Set to parent pos.
        shadow.transform.SetParent(transform, false);

        // Set size.
        shadow.transform.localScale = new Vector3(0.6f, 0.95f, 1);

        // Adding parent sprite.
        spr = shadow.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = GetComponent<SpriteRenderer>().sprite;

        // Order in layer as parent order.
        spr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 5;

        // Shadow color.
        spr.color = new Color(0.15f, 0.15f, 0.15f, 0.4f);

        // rotate accordingly with sun angle.
        shadow.transform.rotation = Quaternion.Euler(0, 0, -sunAngle);

    }


    [SerializeField] private bool hasToBeUpdated = false;


    private bool turn = false;

    private void Update()
    {
        if (hasToBeUpdated)
        {
            // Update with animation.
            spr.sprite = GetComponent<SpriteRenderer>().sprite;

            shadow.transform.rotation = Quaternion.Euler(0, 0, -sunAngle);

            // Update order in layer as parent order.
            spr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 5;
        }


        // Keep rotation when entity turn (cancel scale modification).
        if (transform.GetComponent<BoxCollider2D>() != null)
        {
            if (Mathf.Sign(transform.localScale.x) > 0 && !turn)
            {
                turn = true;
                shadow.transform.localPosition = new Vector3(0 + (transform.GetComponent<BoxCollider2D>().size.x / 2),
                0,
                0f);
            }
            else if (Mathf.Sign(transform.localScale.x) < 0 && turn)
            {
                turn = false;
                shadow.transform.localPosition = new Vector3(0 - (transform.GetComponent<BoxCollider2D>().size.x / 2),
                0,
                0f);
            }
        }
    }
}
