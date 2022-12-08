using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class EffectDestroyer : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime = 0.35f;
        void Start()
        {
            StartCoroutine(DeathTimer());
        }

        IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);
        }

    }
}
