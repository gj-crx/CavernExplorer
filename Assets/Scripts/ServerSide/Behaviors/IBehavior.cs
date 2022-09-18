using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IBehavior
{
    bool Active { get; set; }
    bool HaveOrder { get; set; }
    int CurrentTargetID { get; set; }
    Task StartIterations(int ActualDelay, int PreDelay);
    void BehaviorAction();

    void Clear();
}
