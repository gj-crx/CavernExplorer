using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public interface IBehavior
{
    bool Active { get; set; }
    bool HaveExternalOrder { get; set; }
    Task StartIterationsAsync(int ActualDelay, int PreDelay);
    void BehaviorAction();

    void Clear();
}
