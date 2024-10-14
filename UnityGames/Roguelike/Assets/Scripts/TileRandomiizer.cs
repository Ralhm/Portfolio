using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomiizer : MonoBehaviour
{

    public List<Sprite> Sprites = new List<Sprite>();
    public SpriteRenderer Renderer;

    // Start is called before the first frame update
    void Start()
    {
        int sprite = Random.Range(0, Sprites.Count);
        Renderer.sprite = Sprites[sprite];
    }


}
