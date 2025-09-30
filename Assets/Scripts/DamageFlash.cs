using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DamageFlash : MonoBehaviour
{
    [Header("Flash settings")]
    public Color flashColor = Color.red;

    public float duration = 1.0f;

    public float frequency = 6.0f;

    public bool startWithFlash = true;

    // 内部记录每个 Renderer 的属性名与原始色
    class RendInfo
    {
        public Renderer rend;
        public string colorProp;
        public Color originalColor;
        public bool hasEmission;
        public Color originalEmission;
    }

    private List<RendInfo> renderers = new List<RendInfo>();
    private Coroutine runningCoroutine = null;

    void Awake()
    {
        CollectRenderers();
    }


    public void CollectRenderers()
    {
        renderers.Clear();
        var rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
        {
            if (r == null) continue;

            var info = new RendInfo();
            info.rend = r;

            // get colors
            var mat = r.sharedMaterial;
            string colorProp = "_Color";
            if (mat != null)
            {
                if (mat.HasProperty("_BaseColor")) colorProp = "_BaseColor";
                else if (mat.HasProperty("_Color")) colorProp = "_Color";
            }
            info.colorProp = colorProp;
            
            if (mat != null && mat.HasProperty(colorProp))
                info.originalColor = mat.GetColor(colorProp);
            else
                info.originalColor = Color.white;

            // also add emission colors
            info.hasEmission = (mat != null && mat.HasProperty("_EmissionColor"));
            if (info.hasEmission)
                info.originalEmission = mat.GetColor("_EmissionColor");
            else
                info.originalEmission = Color.black;

            renderers.Add(info);
        }
    }
    
    public void Flash()
    {
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);
        runningCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsed = 0f;
        if (renderers.Count == 0) CollectRenderers();

        // 使用 MaterialPropertyBlock 每帧设置颜色
        var blocks = new Dictionary<Renderer, MaterialPropertyBlock>();
        foreach (var info in renderers)
        {
            blocks[info.rend] = new MaterialPropertyBlock();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // use sin function to create flash intensities
            float phase = Mathf.Sin(elapsed * frequency * 2f * Mathf.PI);
            float alpha = (phase + 1f) * 0.5f; // 0..1

            float mix;
            if (startWithFlash)
            {
                mix = alpha;
            }
            else
            {
                // 从 original -> flash -> original 的平滑过渡
                mix = Mathf.SmoothStep(0f, 1f, alpha);
            }

            foreach (var info in renderers)
            {
                var mpb = blocks[info.rend];
                Color newColor = Color.Lerp(info.originalColor, flashColor, mix);
                mpb.SetColor(info.colorProp, newColor);

                if (info.hasEmission)
                {
                    Color newEmission = Color.Lerp(info.originalEmission, flashColor * 1.2f, mix);
                    mpb.SetColor("_EmissionColor", newEmission);
                }

                info.rend.SetPropertyBlock(mpb);
            }

            yield return null;
        }

        // restore color
        foreach (var info in renderers)
        {
            var mpb = blocks[info.rend];
            mpb.SetColor(info.colorProp, info.originalColor);
            if (info.hasEmission) mpb.SetColor("_EmissionColor", info.originalEmission);
            info.rend.SetPropertyBlock(mpb);
        }
        runningCoroutine = null;
    }
}
