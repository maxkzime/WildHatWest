using UnityEngine;

public class TutorialObject : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // Grab animator reference
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Opponent_bullet")
            || collider.gameObject.CompareTag("Player_bullet"))
        {
            // Kill bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Destroy collider to avoid hit repetition.
            if (GetComponent<BoxCollider2D>() != null)
                Destroy(GetComponent<BoxCollider2D>());


            SoundManager.Instance.PlayBreakingWoodSoundEffect();

            // Start explode animation.
            animator.SetBool("IsHitByBullet", true);

            // Lock sorting layer order on the ground as it is broken.
            Destroy(GetComponent<LayerIndexManager>());
            GetComponent<SpriteRenderer>().sortingOrder = -400;
        }
    }
}
