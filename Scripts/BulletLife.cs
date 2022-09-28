using UnityEngine;

public class BulletLife : MonoBehaviour
{

    private Animator animator;
    private float fireSpeed = 10.0f;

    private int floorSize;

    private void Start()
    {
        // Grab animator reference.
        animator = GetComponent<Animator>();

        // Adding randomness in bullet speed.
        fireSpeed += Random.Range(-1.0f, 3.0f);

        // Getting floorsize limit.
        floorSize = ObjectsGeneration.Instance.GetFloorSize() / 2;
    }


    private bool isDying = false;
    private Vector3 currentPos;

    // Update is called once per frame.
    private void Update()
    {

        #region /*** Bullet endless journey while it don't collides. ***/
        if (!isDying)
        {
            transform.Translate(
                translation: fireSpeed * Time.deltaTime * Vector2.right,
                relativeTo: Space.Self);
        }
        #endregion /*** ^^^ Bullet endless journey while it don't collides. ^^^ ***/



        currentPos = transform.localPosition;

        #region /*** Bullet out of bounds (out of player view). ***/
        if (currentPos.y > floorSize ||
            currentPos.y < -floorSize ||
            currentPos.x > floorSize ||
            currentPos.x < -floorSize)
        {
            InitBulletDeath();
        }
        #endregion /*** ^^^ Bullet out of bounds (out of player view). ^^^ ***/
    }



    #region /*** Initialize bullet death : start death animation. ***/
    public void InitBulletDeath()
    {
        // Stop bullet movements.
        isDying = true;

        // Destroy collider to avoid hit repetition
        if (GetComponent<BoxCollider2D>() != null)
        {
            Destroy(GetComponent<BoxCollider2D>());
        }

        // Start death animation.
        if (gameObject.CompareTag("Player_bullet"))
        {
            animator.SetBool("BulletHasHit", true);
        }
        else if (gameObject.CompareTag("Opponent_bullet"))
        {
            animator.SetBool("OpponentBulletHasHit", true);
        }
    }
    #endregion /*** ^^^ Initialize bullet death : start death animation. ^^^ ***/



    #region /*** Kill this bullet. Called with animation event at the end of breaking animation. ***/
    [SerializeField] private GameObject breakingParticleSystemEffect;

    public void KillBullet()
    {
        // Instantiate bullet breacking particles.
        _ = Instantiate(
            original: breakingParticleSystemEffect,
            position: transform.position,
            rotation: Quaternion.identity);

        Destroy(obj: gameObject);
    }
    #endregion /*** ^^^ Kill this bullet. Called with animation event at the end of breaking animation. ^^^ ***/
}
