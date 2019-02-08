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

        RefreshTeamName();
        
    }

    public void RefreshTeamName()
    {
        originalPos = transform.position;
        var name = textContainer.transform.parent.gameObject.name;

        GetComponent<TextMesh>().text = new String(name.Take(15).ToArray());
        CenterText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.allCamerasCount == 0) return;
        var currentCamera = Camera.allCameras[0];
        textContainer.transform.LookAt(textContainer.transform.position + currentCamera.transform.rotation * Vector3.forward,
            currentCamera.transform.rotation*Vector3.up);

        var orthoCamController = currentCamera.transform.gameObject.GetComponent<OrthoCameraController>();
        var playerCamController = currentCamera.transform.gameObject.GetComponent<PlayerCamController>();

        float percentZoom;
        if(orthoCamController != null)
        {
            percentZoom = orthoCamController.GetPercentZoom();
        }
        else if (playerCamController != null) {
            percentZoom = 1f;
        }
        else
        {
            percentZoom = 0.75f; //for top down cam
        }

        int size = Mathf.FloorToInt(Mathf.Lerp(minTextSize, maxTextSize, percentZoom));
        GetComponent<TextMesh>().fontSize = size;
        CenterText();

    }

    void CenterText()
    {
        var size = GetComponent<MeshRenderer>().bounds.size;
        var current_position = transform.position;
        current_position = textContainer.transform.parent.position - size / 2f;
        transform.position = current_position;
    }
}
