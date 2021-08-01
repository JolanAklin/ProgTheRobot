using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PopUpFillNodeInputInterface
{
    List<object> Inputs { get; }

    public abstract bool Validate();

    public abstract void SetInputsContent(string[] content);
}
