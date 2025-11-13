using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class AutotilingWall : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private Vector2 baseTiling;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        baseTiling = rend.material.mainTextureScale;

        originalScale = transform.localScale;

        UpdateTiling();
    }

    private void OnEnable()
    {
        OfficeGenerator.OnFinishGenerateOffice += UpdateTiling;
    }

    private void OnDisable()
    {
        OfficeGenerator.OnFinishGenerateOffice -= UpdateTiling;
    }

    private void UpdateTiling()
    {
        Vector3 scale = transform.localScale;

        Vector2 newTiling = new Vector2(
            baseTiling.x * scale.x / originalScale.x,
            baseTiling.y * scale.z / originalScale.z
        );

        rend.material.mainTextureScale = newTiling;
    }
}
