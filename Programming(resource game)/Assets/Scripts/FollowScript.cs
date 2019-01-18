using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 cameraOffset;

    [Range(0.1f, 1.0f)]
    [SerializeField] float smoothFactor;

    [SerializeField] bool lootatPlayer;

    void Start()
    {
        cameraOffset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 newPos = player.position + cameraOffset;

        transform.position = Vector3.Lerp(transform.position, newPos, smoothFactor);

        if (lootatPlayer)
        {
            transform.LookAt(player);
        }    
    }

}
