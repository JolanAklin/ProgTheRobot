using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScript : MonoBehaviour
{

    public int id;
    public static int nextid = 0;

    private void Awake()
    {
        // All robotscripts have a different id
        id = nextid;
        nextid++;
    }

    public void SerializeScript()
    {

    }
}
