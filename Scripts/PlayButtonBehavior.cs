using UnityEngine;

public class PlayButtonBehavior : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private bool hasBeenHit = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Bullet hitting play button.
        if (collider.gameObject.CompareTag("Player_bullet") && !hasBeenHit)
        {
            hasBeenHit = true;

            // Delete bullet hitting.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Destroy collider to avoid hit repetition and let bullets pass through.
            if (GetComponent<BoxCollider2D>() != null)
                Destroy(GetComponent<BoxCollider2D>());


            animator.SetBool("IsTriggered", true);

            SoundManager.Instance.PlayBreakingWoodSoundEffect();

            SoundManager.Instance.PlayCollectingGoldNuggetSoundEffect();

            // Set sorting order to the ground.
            if (GetComponent<LayerIndexManager>() != null)
            {
                Destroy(GetComponent<LayerIndexManager>());
                GetComponent<SpriteRenderer>().sortingOrder = -300;
            }

            // Initial opponent call.
            ObjectsGeneration.Instance.GenerateAnOpponent();
        }
    }
}
