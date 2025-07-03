using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ITouchTarget
{
    public bool IsActive { get; }

    void TouchedDown(InputData inputData);

    void Dragged(InputData inputData);

    void TouchedUp(InputData inputData);

    void SetActive(bool active);
}