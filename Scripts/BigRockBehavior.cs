using UnityEngine;

public class BigRockBehavior : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        #region /*** Bullet hitting big rock. ***/
        if (collider.gameObject.CompareTag("Opponent_bullet") ||
            collider.gameObject.CompareTag("Player_bullet"))
        {
            // Kill bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            SoundManager.Instance.PlayRandomBulletHittingRockSound();
        }
        #endregion /*** ^^^ Bullet hitting big rock. ^^^ ***/
    }
}
