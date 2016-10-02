using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public float perspectiveZoomSpeed = 0.3f;
    public float orthoZoomSpeed = 0.8f;

    private float currentRotation = 0;
    private float rotationSpeed = 0.5f;

    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        // Rotates the skybox for a dynamic feel
        currentRotation += rotationSpeed * Time.deltaTime;
        currentRotation %= 360;
        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);

        // Gets touches and acts on them to do a pinch zoom
        if (Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if(mainCamera.orthographic) {
                mainCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
                mainCamera.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
            }
            else {
                mainCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 20f, 100f);
            }
        }
    }
}
