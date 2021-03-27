// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//FileTeleporter is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptCollider : MonoBehaviour
{

    public BoxCollider2D boxCollider2d;
    public RectTransform rectTransform;
    public RectTransform canvas;
    public float resizeFactor = 100;

    public void Resize()
    {
        boxCollider2d.size = new Vector2(rectTransform.rect.width/resizeFactor, rectTransform.rect.height/resizeFactor);
        if(canvas != null)
            if (rectTransform.anchorMax.y == 1)
                boxCollider2d.offset = new Vector2(0, canvas.rect.height / (resizeFactor * 2) - rectTransform.rect.height / (resizeFactor * 2));
            else
                boxCollider2d.offset = new Vector2(0, -canvas.rect.height / (resizeFactor * 2) + rectTransform.rect.height / (resizeFactor * 2));
    }
}
