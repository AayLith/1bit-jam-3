using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveTextAnimation : MonoBehaviour
{
    public float bopSpeed = 0.5f;  // Slower speed for a gentle bop
    public float bopHeight = 2.0f; // Reduced height for subtlety
    private TMP_Text textMesh;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        string text = textMesh.text;
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;

        // Current time factor, used to make each character bop independently
        float timeFactor = Time.time * bopSpeed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            // Each character has a slightly different phase, making the bop independent
            Vector3 offset = Vector3.up * Mathf.Sin(timeFactor + i * 0.3f) * bopHeight;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Apply the offset to each vertex of the character
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        // Update the mesh with the new vertex positions
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}