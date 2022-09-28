using UnityEngine;

public class EggBulletLife : MonoBehaviour
{

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Grab animator reference
        animator = GetComponent<Animator>();

        // Initialize egg bullet life parameters.
        startTime = Time.time;
        maxLifeTime = Random.Range(2.0f, 6.0f);
    }


    private readonly float fireSpeed = 0.9f;

    private float startTime;
    private float maxLifeTime;

    // Used to force egg bullet destruction when the player is hit by it.
    public void ForcedKill() => maxLifeTime = 0;


    // Update is called once per frame
    void Update()
    {
        #region /*** Egg bullet shaking. ***/

        transform.Translate(
                translation: fireSpeed * Time.deltaTime *
                new Vector3(
                    Random.Range(-1, 2),
                    Random.Range(-1, 2),
                    0),
                relativeTo: Space.Self);

        #endregion /*** ^^^ Egg bullet shaking. ^^^ ***/



        #region /*** Start destruction animation when it reaches its max life time. ***/
        if (startTime + maxLifeTime < Time.time)
        {
            animator.SetBool(
                name: "destroyEggBullet",
                value: true);
        }
        #endregion /*** ^^^ Start destruction animation when it reaches its max life time. ^^^ ***/
    }



    #region /*** Kill this egg bullet. Called with animation event at the end of breaking animation. ***/
    [SerializeField] private AudioClip eggBulletCrackingSoundEffect;

    [SerializeField] private GameObject breakingParticleSystemEffect;

    public void KillBullet()
    {
        // Play cracking sound effect.
        SoundManager.Instance.PlaySound(
            clip: eggBulletCrackingSoundEffect,
            volume: 0.3f);

        // Instantiate egg bullet breacking particles.
        _ = Instantiate(
            original: breakingParticleSystemEffect,
            position: transform.position,
            rotation: Quaternion.identity);

        Destroy(obj: gameObject);
    }

    #endregion /*** ^^^ Kill this egg bullet. Called with animation event at the end of breaking animation. ^^^ ***/

}
