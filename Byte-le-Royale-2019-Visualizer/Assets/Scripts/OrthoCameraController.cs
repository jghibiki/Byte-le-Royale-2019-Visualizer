
using UnityEngine;

public class OrthoCameraController : MonoBehaviour
{

    public float panSpeedLow = 20f;
    public float panSpeedHigh = 100f;
    public float rotateSpeed = 8f;
    public Vector2 panLimit;

    public float dragSpeed = 2;

    private float X;
    private float Y;

    public float scrollSpeed = 8f;

    private float maxCameraHeight;
    private Vector3 defaultCamPos;
    private Vector3 defaultCamRot;

    void Start()
    {
        maxCameraHeight = transform.position.y;
        defaultCamPos = transform.position;
        defaultCamRot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameObject.activeInHierarchy) return;

        Vector3 pos = transform.position;
        var percentScroll = pos.y / maxCameraHeight;

        var panSpeed = Mathf.Lerp(panSpeedLow, panSpeedHigh, percentScroll);


        if (Input.GetKey("w"))
        {
            float y = transform.position.y;
            transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.Self);
            Vector3 new_pos = transform.position;
            new_pos.y = y;
            transform.position = new_pos;
        }
        if (Input.GetKey("s"))
        {
            float y = transform.position.y;
            transform.Translate(-Vector3.forward * panSpeed * Time.deltaTime, Space.Self);
            Vector3 new_pos = transform.position;
            new_pos.y = y;
            transform.position = new_pos;

        }

        if (Input.GetKey("d"))
        {
            float y = transform.position.y;
            transform.Translate(-Vector3.left * panSpeed * Time.deltaTime, Space.Self);
            Vector3 new_pos = transform.position;
            new_pos.y = y;
            transform.position = new_pos;
        }
        if (Input.GetKey("a"))
        {
            float y = transform.position.y;
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.Self);
            Vector3 new_pos = transform.position;
            new_pos.y = y;
            transform.position = new_pos;
        }

        if (Input.GetKey("e"))
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("q"))
        {
            transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            transform.Translate(Vector3.up * scroll * scrollSpeed *3f * 100f * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector3.up * scroll * scrollSpeed * 100f * Time.deltaTime, Space.World);
        }
        
        var verticalPos = transform.position;
        verticalPos.y = Mathf.Clamp(verticalPos.y, 0, maxCameraHeight);
        transform.position = verticalPos;

        pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, 1000+panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, 1000+panLimit.y);
        transform.position = pos;

        UpdateCameraDrag();

        // listen for mouse pad keys
        if (Input.GetKey("[5]"))
        {
            transform.position = defaultCamPos;
            transform.eulerAngles = defaultCamRot;
        }
        
    }

    void UpdateCameraDrag()
    {
        var speed = 1f;
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }

    }

    public float GetPercentZoom()
    {
        return transform.position.y / maxCameraHeight;
    }
}
