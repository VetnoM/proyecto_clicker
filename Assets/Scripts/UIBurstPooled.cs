using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBurstPooled : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform canvasRoot;   // Canvas (RectTransform)
    public Image particlePrefab;       // Prefab UI (Image 8x8, raycast OFF)

    [Header("Burst")]
    public int count = 24;                 // nº partículas por burst
    public float life = 0.50f;              // duración
    public Vector2 speedRange = new Vector2(260f, 420f); // px/s
    public Vector2 scaleRange = new Vector2(1.1f, 1.8f);
    public float spreadDeg = 360f;               // 360 = en todas direcciones
    public Vector2 angleJitter = new Vector2(-10f, 10f);
    public Vector2 angularVelRange = new Vector2(-420f, 420f);
    public float damping = 0.86f;                  // desaceleración por frame

    [Header("Color")]
    public Color color = Color.white;

    [Header("Rendimiento")]
    public int maxAlive = 100;   // máximo simultáneo
    public int maxPoolSize = 120;   // tope absoluto de imágenes
    public float minInterval = 0.02f; // anti-spam entre bursts
    public bool adaptiveOnLowFps = true; // reduce count si FPS < 50

    // ===== Interno =====
    struct P
    {
        public RectTransform rt;
        public Vector2 vel;
        public float angVel;
        public float t;
        public float life;
        public Color startColor;
        public Image img;
    }

    readonly List<P> active = new List<P>(128);
    readonly Queue<Image> freePool = new Queue<Image>(128);
    float lastPlay;
    bool isUpdating;

    // --- API ---
    public void PlayAt(Vector2 uiAnchoredPos, int countOverride = -1, Color? overrideColor = null, float? lifeOverride = null)
    {
        if (!canvasRoot || !particlePrefab) return;

        // anti-spam
        if (Time.unscaledTime - lastPlay < minInterval) return;
        lastPlay = Time.unscaledTime;

        int emitCount = (countOverride > 0) ? countOverride : count;

        // degradación por FPS
        if (adaptiveOnLowFps)
        {
            float fps = 1f / Mathf.Max(Time.smoothDeltaTime, 0.0001f);
            if (fps < 50f) emitCount = Mathf.CeilToInt(emitCount * 0.6f);
            if (fps < 35f) emitCount = Mathf.CeilToInt(emitCount * 0.4f);
        }

        // respetar presupuesto
        int canSpawn = Mathf.Max(0, maxAlive - active.Count);
        if (canSpawn <= 0) return;
        emitCount = Mathf.Min(emitCount, canSpawn);

        float L = lifeOverride.HasValue ? lifeOverride.Value : life;
        Color col = overrideColor.HasValue ? overrideColor.Value : color;

        float baseAng = Random.Range(0f, 360f);
        float step = spreadDeg / Mathf.Max(1, emitCount);

        for (int i = 0; i < emitCount; i++)
        {
            var img = Borrow();
            if (!img) break;

            var rt = (RectTransform)img.transform;
            rt.SetParent(canvasRoot, false);
            rt.anchoredPosition = uiAnchoredPos;
            rt.localScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);
            rt.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

            var c = col; c.a = 1f; img.color = c;

            float ang = baseAng + (step * i) + Random.Range(angleJitter.x, angleJitter.y);
            float rad = ang * Mathf.Deg2Rad;
            float spd = Random.Range(speedRange.x, speedRange.y);
            Vector2 vel = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * spd;
            float angVel = Random.Range(angularVelRange.x, angularVelRange.y);

            img.gameObject.SetActive(true);

            active.Add(new P
            {
                rt = rt,
                vel = vel,
                angVel = angVel,
                t = 0f,
                life = L,
                startColor = c,
                img = img
            });
        }

        if (!isUpdating) StartCoroutine(UpdateParticles());
    }

    // --- Pooling ---
    Image Borrow()
    {
        // reusar libre si hay
        while (freePool.Count > 0)
        {
            var img = freePool.Dequeue();
            if (img) return img;
        }
        // crear nuevo si no superamos el tope
        int totalKnown = active.Count + freePool.Count;
        if (totalKnown >= maxPoolSize) return null;

        var p = Instantiate(particlePrefab, canvasRoot);
        p.raycastTarget = false;
        p.gameObject.SetActive(false);
        return p;
    }

    void Release(Image img)
    {
        if (!img) return;
        img.gameObject.SetActive(false);
        freePool.Enqueue(img);
    }

    // --- Update loop ---
    IEnumerator UpdateParticles()
    {
        isUpdating = true;

        while (active.Count > 0)
        {
            float dt = Time.unscaledDeltaTime;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                var p = active[i];
                p.t += dt;

                // mover + desacelerar
                var pos = p.rt.anchoredPosition;
                pos += p.vel * dt;
                p.vel *= damping;
                p.rt.anchoredPosition = pos;

                // rotación
                var e = p.rt.localEulerAngles;
                e.z += p.angVel * dt;
                p.rt.localEulerAngles = e;

                // fade
                float k = Mathf.Clamp01(p.t / p.life);
                var c = p.startColor; c.a = 1f - k;
                p.img.color = c;

                if (p.t >= p.life)
                {
                    Release(p.img);
                    active.RemoveAt(i);
                }
                else
                {
                    active[i] = p;
                }
            }

            yield return null;
        }

        isUpdating = false;
    }
}
