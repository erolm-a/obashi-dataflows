using UnityEngine;

struct SelectionState
{
    /// <summary>
    /// The object that is currently being selected.
    /// </summary>
    public GameObject focusedObject;

    /// <summary>
    /// The id of the object being focused
    /// </summary>
    public int focusedId;

    public void Unfocus()
    {
        focusedObject = null;
        focusedId = -1;
    }
}