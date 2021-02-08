using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectHandle : MonoBehaviour
{
    public Nodes node;
    public bool isInput = false;
    public GameObject linkGeneratorPrefab;

    public void Click()
    {
        if(isInput)
        {
            LinkGenerator link = Manager.instance.linkGenerator.GetComponent<LinkGenerator>();
            link.EndLink();
            link.nodeStart.nextId = node.id;
            link.nodeStart.nextGameObject = node.gameObject;
        }
        else
        {
            Manager.instance.linkGenerator = Instantiate(linkGeneratorPrefab, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("NodeHolder").transform);
            Manager.instance.linkGenerator.GetComponent<LinkGenerator>().nodeStart = node;
        }
    }

    public void Show()
    {

    }
    public void UnShow()
    {

    }
}
