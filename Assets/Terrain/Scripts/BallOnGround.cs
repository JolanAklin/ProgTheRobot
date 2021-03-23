using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOnGround : MonoBehaviour
{
    public Ball ball;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            ball.BallReset();
        }
    }
}
