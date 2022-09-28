using UnityEngine;
using Cinemachine;

public class FollowingPlayer : MonoBehaviour
{
    private GameObject player = null;

    // Used to make camera follow player.
    private void FixedUpdate()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if(player != null)
            {
                //Debug.Log("Setting cam to follow player.");

                // Set camera to follow player.
                GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
            }
        }
    }
}
