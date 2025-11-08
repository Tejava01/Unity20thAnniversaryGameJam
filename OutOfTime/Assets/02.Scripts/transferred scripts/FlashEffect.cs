using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;

    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    private Coroutine running;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) throw new System.Exception("must be attached to object with sprite renderer");

        defaultColor = spriteRenderer.color;
        defaultMaterial = spriteRenderer.sharedMaterial;
    }

    public void flash(float duration)
    {
        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(flashCoroutine(duration));
    }

    private IEnumerator flashCoroutine(float duration)
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = defaultColor;
        spriteRenderer.material = defaultMaterial;
    }
}
