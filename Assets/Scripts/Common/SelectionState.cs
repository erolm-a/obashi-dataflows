using UnityEngine;

struct SelectionState
{
    /// <summary>
    /// The object that is currently being selected.
    /// NB: The object should have a script that inherits from `DataFlow.Device`
    /// </summary>
    public GameObject focusedObject;

    /// <summary>
    /// The previous object that was being selected. Useful for links.
    /// NB: The object should have a script that inherits from `DataFlow.Device`
    /// <summary>
    public GameObject previousFocusedObject;

    /// <summary>
    /// Select the current object. After this call focusedObject will point to pawn.
    /// </summary>
    public void Select(GameObject pawn)
    {
        focusedObject = pawn;
    }

    /// <summary>
    /// Unfocus the previous object and select the given pawn.
    /// This is equivalent to perform `Unfocus(); Select(pawn);` .
    /// <param name="pawn">The object to focus</param>
    /// </summary>
    public void UnfocusAndSelect(GameObject pawn)
    {
        previousFocusedObject = focusedObject;
        focusedObject = pawn;
    }

    /// <summary>
    /// Unfocus the current element. The previous element becomes the current element.
    /// </summary>
    public void Unfocus()
    {
        previousFocusedObject = focusedObject;
        focusedObject = null;
    }
}