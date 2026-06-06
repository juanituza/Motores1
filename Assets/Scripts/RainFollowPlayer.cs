using UnityEngine;

public class RainFollowPlayer : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float heightOffset = 15f;

    void LateUpdate()
    {
        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }

            return;
        }

        Vector3 pos = player.position;

        transform.position = new Vector3(
            pos.x,
            pos.y + heightOffset,
            pos.z
        );
    }
}