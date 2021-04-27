using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start tpi
public class ScriptCloner : MonoBehaviour
{
    /// <summary>
    /// Clone the specified script
    /// </summary>
    /// <param name="robotScriptToClone">The source to clone from</param>
    /// <returns>The cloned script</returns>
    public static RobotScript CloneScript(RobotScript robotScriptToClone)
    {
        RobotScript robotScriptClone = new RobotScript();
        robotScriptClone.name = robotScriptToClone.name;
        robotScriptClone.isMainScript = robotScriptToClone.isMainScript;

        // clone nodes
        Nodes[] nodesToClone = new Nodes[robotScriptToClone.nodes.Count];
        int i = 0;
        foreach (GameObject node in robotScriptToClone.nodes)
        {
            nodesToClone[i] = node.GetComponent<Nodes>();
            i++;
        }
        int idDelta;
        Nodes[] clonedNodesArray = CloneNodes(nodesToClone, robotScriptClone, out idDelta);
        foreach (Nodes node in clonedNodesArray)
        {
            robotScriptClone.nodes.Add(node.gameObject);
        }

        // clone splines
        robotScriptClone.splines.AddRange(CloneSpline(robotScriptToClone.splines.ToArray(), robotScriptClone, idDelta));
        

        return robotScriptClone;
    }

    /// <summary>
    /// Clone nodes inside the node array
    /// </summary>
    /// <param name="nodesToClone">The array containing nodes to clone</param>
    /// <param name="robotScript">The script to clone nodes to</param>
    /// <returns>An array with cloned nodes</returns>
    public static Nodes[] CloneNodes(Nodes[] nodesToClone, RobotScript robotScript, out int idDelta)
    {
        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        Nodes[] clonedNode = new Nodes[nodesToClone.Length];

        // used to calculate the delta between the ids
        int lowestId = nodesToClone[0].id;
        int lowestNewId = Nodes.nextid;

        int i = 0;

        // create cloned nodes
        foreach (Nodes nodeToClone in nodesToClone)
        {
            GameObject node = SaveManager.instance.nodeObjects.Find(x => x.nodeType == nodeToClone.NodeType.ToString()).gameObject; // find the correct gameobject to instantiate
            GameObject nodeInstance = SaveManager.instance.InstantiateSavedObj(node, nodeToClone.transform.position, Quaternion.identity, nodeHolder);

            Nodes nodeInstanceScript = nodeInstance.GetComponent<Nodes>();
            nodeInstanceScript.rs = robotScript;

            if (nodeToClone.id < lowestId)
                lowestId = nodeToClone.id;

            // change the id of the serialized object to keep the right id
            Nodes.SerializableNode serializableNode = nodeToClone.SerializeNode();
            serializableNode.id = nodeInstanceScript.id;

            // set the right value for the node
            nodeInstanceScript.DeSerializeNode(serializableNode);

            if (nodeToClone.NodeType == Nodes.NodeTypes.start)
            {
                robotScript.nodeStart = nodeInstance.GetComponent<Nodes>();
            }
            clonedNode[i] = nodeInstanceScript;
            i++;
        }

        idDelta = lowestNewId - lowestId;
        i = 0;
        // update the nextNodeId of all nodes
        foreach (Nodes node in clonedNode)
        {
            node.UpdateNextNodeId(idDelta);
        }

        return clonedNode;
    }

    /// <summary>
    /// Cloned the given spline array
    /// </summary>
    /// <param name="splinesToClone">The array to clone the splines from</param>
    /// <param name="robotScript">The script that will hold the splines</param>
    /// <param name="idDelta">The idDelta</param>
    /// <returns></returns>
    public static GameObject[] CloneSpline(GameObject[] splinesToClone, RobotScript robotScript, int idDelta)
    {
        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        GameObject[] clonedSplines = new GameObject[splinesToClone.Length];
        int i = 0;
        foreach (GameObject splineToClone in splinesToClone)
        {
            SplineManager.SerializedSpline serializedSpline = splineToClone.GetComponent<SplineManager>().SerializeSpline();

            // change the node and script to the new cloned ones
            serializedSpline.idNodeStart += idDelta;
            serializedSpline.idNodeEnd += idDelta;
            serializedSpline.robotScriptId = robotScript.id;

            // create and set the right value for the spline
            GameObject splineLinkInstance = Instantiate(SaveManager.instance.splineLink, new Vector3(0, 0, -899), Quaternion.identity, nodeHolder);
            SplineManager splineManager = splineLinkInstance.GetComponent<SplineManager>();
            splineManager.DeSerializeSpline(serializedSpline);
            clonedSplines[i] = splineLinkInstance;
            i++;
        }
        return clonedSplines;
    }
}
// end tpi
