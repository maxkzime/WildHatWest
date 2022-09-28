using UnityEngine;

public class CactusBehavior : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private bool oneHit = false;
    private bool twoHit = false;
    private bool threeHit = false;

    [SerializeField] private GameObject cactusBloodParticleSystemEffect;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Bullet hitting cactus
        if (collider.gameObject.CompareTag("Opponent_bullet")
            || collider.gameObject.CompareTag("Player_bullet"))
        {
            // Instanciating cactus blood particle system effect,
            // and destroy bullet,
            // except when the cactus is already destroy.
            if (!threeHit)
            {
                collider.GetComponent<BulletLife>().InitBulletDeath();

                _ = Instantiate(
                    original: cactusBloodParticleSystemEffect,
                    position: transform.position,
                    rotation: Quaternion.identity);

                SoundManager.Instance.PlayRandomBulletHittingCactusSound();
            }

            if (!oneHit)
            {
                animator.SetBool("First_Hit", true);
                oneHit = true;
            }
            else if(!twoHit)
            {
                animator.SetBool("Second_Hit", true);
                twoHit = true;
            }
            else if (!threeHit)
            {
                animator.SetBool("Third_Hit", true);
                threeHit = true;

                // Increment cactus killed counter for cactus trophy.
                TrophiesManager.Instance.IncrementCactusKilledCounter();
            }
        }
    }
}
