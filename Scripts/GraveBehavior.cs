using UnityEngine;

public class GraveBehavior : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // Grab animator reference.
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Bullet hitting grave.
        if (collider.gameObject.CompareTag("Opponent_bullet")
            || collider.gameObject.CompareTag("Player_bullet"))
        {
            // Kill bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Destroy collider to avoid hit repetition.
            if (GetComponent<BoxCollider2D>() != null)
            {
                Destroy(GetComponent<BoxCollider2D>());
            }

            // Start explode animation.
            animator.SetBool("IsHitByBullet", true);

        }
    }


    #region /*** Grave death : called with animation event, at the end of explosion animation. ***/
    [SerializeField] private GameObject goldNuggetReference;

    private GameObject goldNuggetToLeave = null;

    private void KillGrave()
    {

        #region /*** Leave gold nugget explosion. ***/
        if (goldNuggetToLeave == null)
        {
            int randGnNumber = Random.Range(1, 4);

            for (int i = 0; i < randGnNumber; i++)
            {
                goldNuggetToLeave = Instantiate(
                    original: goldNuggetReference,
                    position: transform.position,
                    rotation: Quaternion.identity);

                goldNuggetToLeave.GetComponent<GoldNuggetBehavior>().SetIsPartOfExplosion(true);
            }
        }
        #endregion /*** ^^^ Leave gold nugget explosion. ^^^ ***/


        CreateGraveExplosionParticles(Random.Range(1, 4));

        // Destroy grave.
        Destroy(obj: gameObject);

    }
    #endregion /*** ^^^ Grave death : called with animation event, at the end of explosion animation. ^^^ ***/




    #region /*** Generate grave explosion particles. ***/
    [SerializeField] private GameObject graveExplosionParticleSystemEffect;

    private void CreateGraveExplosionParticles(int repeat = 1)
    {
        for (int i = 0; i < repeat; i++)
        {
            // Instanciating grave explosion particles.
            _ = Instantiate(
                original: graveExplosionParticleSystemEffect,
                position: transform.position,
                rotation: Quaternion.identity);
        }
    }
    #endregion /*** ^^^ Generate grave explosion particles. ^^^ ***/
}
