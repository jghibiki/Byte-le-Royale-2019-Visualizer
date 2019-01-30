using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextController : MonoBehaviour

    
{
    public GameObject textContainer;
    public int maxTextSize;
    public int minTextSize;

    private Vector3 originalPos;

    // Start is called before the first frame update
    void Start()
    {

        originalPos = transform.position;
        var name = textContainer.transform.parent.gameObject.name;

        GetComponent<TextMesh>().text = new String(name.Take(15).ToArray());
        CenterText();
        
    }

    // Update is called once per frame
    void Update()
    {
        textContainer.transform.LookAt(textContainer.transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation*Vector3.up);


        var percentZoom = Camera.main.transform.gameObject.GetComponent<OrthoCameraController>().GetPercentZoom();
        int size = Mathf.FloorToInt(Mathf.Lerp(maxTextSize, minTextSize, percentZoom));
        GetComponent<TextMesh>().fontSize = size;
        //CenterText();

    }

    void CenterText()
    {
        var width = GetComponent<MeshRenderer>().bounds.size.x;
        var current_position = transform.position;
        current_position.x += width / 2f;
        transform.position = current_position;
    }
}
