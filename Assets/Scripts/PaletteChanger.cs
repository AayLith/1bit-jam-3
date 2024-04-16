using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.IK;

public class PaletteChanger : MonoBehaviour
{
    [SerializeField]
    private Material ColourSwapMaterial;
    [SerializeField]
    private Color defaultCameraBackgroundColor;
    [SerializeField]
    private Color colourA;
    [SerializeField]
    private Color colourAReplacement;
    [SerializeField]
    private Color colourB;
    [SerializeField]
    private Color colourBReplacement;
    // Start is called before the first frame update
    private List<SpriteRenderer> spritesToUpdate = new List<SpriteRenderer>();
    private List<TilemapRenderer> tilemapsToUpdate = new List<TilemapRenderer>();
    private List<Camera> cameras;
    void Start()
    {
        setMaterialObjects();
        

        UpdatePalette(colourA,colourAReplacement, colourB, colourBReplacement, defaultCameraBackgroundColor);
    }

    private void setMaterialObjects()
    {
        var sprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sprite in sprites)
        {
            sprite.material = ColourSwapMaterial;
            spritesToUpdate.Add(sprite);
        }
        var tileMaps = FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);
        foreach (var tilemap in tileMaps)
        {
            tilemap.material = ColourSwapMaterial;
            tilemapsToUpdate.Add(tilemap);
        }
        cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None).ToList();
        foreach (var camera in cameras)
        {
            camera.backgroundColor = defaultCameraBackgroundColor;
        }
    }

    public void UpdatePalette(Color cToReplaceA,Color cReplacementA,Color cToReplaceB, Color cReplacementB,Color backdropColor)
    {
        ColourSwapMaterial.SetColor("_ColorA",cToReplaceA);
        ColourSwapMaterial.SetColor("_ColorAReplacement",cReplacementA);
        ColourSwapMaterial.SetColor("_ColorB",cToReplaceB);
        ColourSwapMaterial.SetColor("_ColorBReplacement",cReplacementB);
        foreach(var camera in cameras)
        {
            camera.backgroundColor = backdropColor;
        }
        // foreach(var spriteRenderer in spritesToUpdate)
        // {
        //     spriteRenderer.material = ColourSwapMaterial;
        // }
        // foreach(var tilemap in tilemapsToUpdate)
        // {
        //     tilemap.material = ColourSwapMaterial;
        // }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
