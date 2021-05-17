// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeContextMenuScript : MonoBehaviour
{
    [HideInInspector]
    public Nodes nodeToModify;

    /// <summary>
    /// Unlock all node's input
    /// </summary>
    public void Modify()
    {
        nodeToModify.LockUnlockAllInput(false);
        UIRaycaster.instance.nodeContextMenuOpen = false;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Remove the ndoe targeted by the context menu
    /// </summary>
    public void Remove()
    {
        Destroy(nodeToModify.gameObject);
        Destroy(this.gameObject);
    }
}
