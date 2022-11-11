using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public interface IBehavior
{
    bool Active { get; set; }
    bool HaveExternalOrder { get; set; }
    void BehaviorInteraction();

    void Clear();
}
