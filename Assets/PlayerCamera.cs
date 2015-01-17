using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 lastMousePosition;
    private Camera theCamera;

    public float KeysMoveSpeed = 10f;
    public float MouseMoveSpeed = 1f;
    public float KeysZoomSpeed = 30f;
    public float MouseZoomSpeed = 60f;
    public float MinCameraHeight = 3.5f;
    public float MaxCameraHeight = 20f;

    private static PlayerCamera pc;
    public static PlayerCamera Get() { return pc; }

	void Start()
    {
        pc = this;
        theCamera = GetComponent<Camera>();
	}
	
	void Update()
    {
        if (Input.GetMouseButtonDown(2)) // MMB
        {
            lastMousePosition = Input.mousePosition;
        }

        if (!Input.GetMouseButtonDown(2) && Input.GetMouseButton(2))
        {
            // cast rays to last, current pos
            var lastPos = Grid.Get().GetFloatCellFromRay(theCamera.ScreenPointToRay(lastMousePosition));
            var pos = Grid.Get().GetFloatCellFromRay(theCamera.ScreenPointToRay(Input.mousePosition));
            var delta = -(pos - lastPos) * MouseMoveSpeed;

            // apply delta
            var p = theCamera.transform.position;
            p.x += delta.x;
            p.z += delta.y;
            theCamera.transform.position = p;

            //theCamera.transform

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            var delta = Vector3.zero;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                delta.x += KeysMoveSpeed * Time.smoothDeltaTime;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                delta.x -= KeysMoveSpeed * Time.smoothDeltaTime;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                delta.z -= KeysMoveSpeed * Time.smoothDeltaTime;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                delta.z += KeysMoveSpeed * Time.smoothDeltaTime;

            if (delta != Vector3.zero)
                theCamera.transform.position = theCamera.transform.position + delta;
        }

        var mouseZoomDir = theCamera.ScreenPointToRay(Input.mousePosition).direction;
        var keyZoomDir = theCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)).direction;
        var scr = MouseZoomSpeed * Input.mouseScrollDelta.y;
        mouseZoomDir.Normalize();
        keyZoomDir.Normalize();

        if ((scr > 0 && theCamera.transform.position.y > MinCameraHeight) || (scr < 0 && theCamera.transform.position.y < MaxCameraHeight))
            theCamera.transform.position += scr * mouseZoomDir * Time.smoothDeltaTime;

        if ((Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus)) && theCamera.transform.position.y > MinCameraHeight)
            theCamera.transform.position += KeysZoomSpeed * keyZoomDir * Time.smoothDeltaTime;
        if ((Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Underscore) || Input.GetKey(KeyCode.KeypadMinus)) && theCamera.transform.position.y < MaxCameraHeight)
            theCamera.transform.position -= KeysZoomSpeed * keyZoomDir * Time.smoothDeltaTime;

        if (theCamera.transform.position.y < MinCameraHeight)
            theCamera.transform.position = new Vector3(theCamera.transform.position.x, MinCameraHeight, theCamera.transform.position.z);
        if (theCamera.transform.position.y > MaxCameraHeight)
            theCamera.transform.position = new Vector3(theCamera.transform.position.x, MaxCameraHeight, theCamera.transform.position.z);
	}
}
