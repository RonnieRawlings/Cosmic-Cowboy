// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ObjectFade : MonoBehaviour
{
    // Fade amount, time of fade, & original alpha value.
    [SerializeField] private float fadeSpeed = 1, fadeAmount = 1;
    private float originalOpacity;

    // Rend component, all materials.
    [SerializeField] private Renderer _renderer;
    private List<Material> _material;

    // Should the obj fade.
    [SerializeField] private bool doFade = false;

    // Replacement URP shader.
    private List<Material> originalMaterials;
    [SerializeField] Material litReplacement;

    // Has the obj been faded.
    private bool hasFaded = false;

    #region Properties

    public bool DoFade
    {
        set { doFade = value; }
    }

    #endregion

    /// <summary> method <c>SetFade</c> fades the obj to set alpha value over fadeSpeed. </summary>
    public IEnumerator SetFade()
    {
        // Don't fade again until false.
        hasFaded = true;

        Material[] originalMaterials = _renderer.materials;
        Material[] newMaterials = new Material[originalMaterials.Length];

        // Switch the shaders immediately
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = litReplacement;
        }
        _renderer.materials = newMaterials;

        // Then gradually change the alpha for all materials at the same time
        float elapsedTime = 0;
        while (elapsedTime < fadeSpeed)
        {
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                Color originalColour = originalMaterials[i].color;
                float newAlpha = Mathf.Lerp(originalColour.a, fadeAmount, (elapsedTime / fadeSpeed));
                Color smoothColour = new Color(originalColour.r, originalColour.g, originalColour.b, newAlpha);
                _renderer.materials[i].color = smoothColour;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _renderer.enabled = false;
    }

    /// <summary> method <c>ResetFade</c> returns obj alpha back to originalOpacity over fadeSpeed. </summary>
    public IEnumerator ResetFade()
    {
        // Allow another fade.
        hasFaded = false;
        
        Material[] newMaterials = new Material[originalMaterials.Count];

        for (int i = 0; i < originalMaterials.Count; i++)
        {
            newMaterials[i] = originalMaterials[i];
            Color originalColour = originalMaterials[i].color;
            Color smoothColour = new Color(originalColour.r, originalColour.g, originalColour.b,
                Mathf.Lerp(originalColour.a, originalOpacity, fadeSpeed));
            newMaterials[i].color = smoothColour;
        }

        // Yield return null will wait until the next frame to continue executing the code
        yield return null;

        _renderer.materials = newMaterials;        
    } 

    void Awake()
    {
        // Remove comp if no renderer found.
        if (GetComponent<Renderer>() == null) { Destroy(this); }

        // Obj mesh rend comp.
        _renderer = GetComponent<Renderer>();

        // Set fadeOut mat.
        litReplacement = Resources.Load("Materials/fadeOut") as Material;     

        // Set fade values.
        fadeSpeed = 0.5f;
        fadeAmount = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {       
        // Adds all obj materials to list.
        _material = new List<Material>();
        _material.AddRange(_renderer.materials);

        originalMaterials = new List<Material>();
        foreach (Material mat in _material)
        {
            originalMaterials.Add(mat);
        }

        // Gets starting opacity.
        originalOpacity = _material[0].color.a;       
    }

    // Update is called once per frame
    void Update()
    {
        // Fades obj to set value when true, resets otherwise.
        if (doFade && !hasFaded)
        {
            StartCoroutine(SetFade());
        }
        else if (!doFade && hasFaded)
        {
            StartCoroutine(ResetFade());
        }

        // Prevents renderer disabled issues.
        if (!doFade) { _renderer.enabled = true; }
    }
}
