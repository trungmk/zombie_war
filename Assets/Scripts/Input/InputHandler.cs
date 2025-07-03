using System.Collections.Generic;
using UnityEngine;

public class InputHandler : IInputHandler
{
    private readonly IList<ITouchTarget> touchTargets = new List<ITouchTarget>();

    public void Reset()
    {
        touchTargets.Clear();
    }

    public void RegisterTouchTarget(ITouchTarget touchTarget)
    {
        if (touchTarget == null)
        {
            Debug.Log("Touch target cannot be null.");
        }

        if (!touchTargets.Contains(touchTarget))
        {
            touchTargets.Add(touchTarget);
        }
    }

    public void UnregisterTouchTarget(ITouchTarget touchTarget)
    {
        if (touchTarget == null)
        {
            Debug.Log("Touch target cannot be null.");
        }

        if (touchTargets.Contains(touchTarget))
        {
            touchTargets.Remove(touchTarget);
        }
    }

    public void OnUserDrag(InputData inputData)
    {
        for (int i = 0; i < touchTargets.Count; i++)
        {
            if (touchTargets[i].IsActive)
            {
                touchTargets[i].Dragged(inputData);
            }
        }
    }

    public void OnUserPress(InputData inputData)
    {
        for (int i = 0; i < touchTargets.Count; i++)
        {
            if (touchTargets[i].IsActive)
            {
                touchTargets[i].TouchedDown(inputData);
            }
        }
    }

    public void OnUserRelease(InputData inputData)
    {
        for (int i = 0; i < touchTargets.Count; i++)
        {
            if (touchTargets[i].IsActive)
            {
                touchTargets[i].TouchedUp(inputData);
            }
        }
    }
}
