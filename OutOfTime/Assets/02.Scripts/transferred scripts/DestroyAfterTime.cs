using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;

    private void Awake()
    {
        Invoke(nameof(destroySelf), lifetime);
    }

    private void destroySelf() {
        Destroy(gameObject);
    }
}
