using UnityEngine;
using System.Collections;

public class Station : MonoBehaviour {

    public Player player;
    public Color lineColor1 = Color.yellow;
    public Color lineColor2 = Color.blue;
    public bool isLauncherLoaded = false;

    private GameObject tempPlanet;
    private LineRenderer lineRenderer;

    private Vector3 mousePositionStart;
    private Vector3 mousePositionStop;
    private float xMin, xMax;
    private float yMax = 700;
    private float yMin = -700;

    void Start () {
        player = FindObjectOfType<Player>();
        xMin = player.sun.transform.position.x - 1000;
        xMax = player.sun.transform.position.x + 1000;
        CreateLineRenderer();
	}
	
	void Update () {

    }

    private void OnMouseDown() {
        mousePositionStart = GetWorldMousePosition();
        mousePositionStart.x = transform.position.x;
        mousePositionStart.y = transform.position.y;
    }

    private void OnMouseDrag() {
        Vector3 mousePositionCurrent = GetWorldMousePosition();
        if (isLauncherLoaded) {
            lineRenderer.SetPosition(0, mousePositionStart);
            lineRenderer.SetPosition(1, mousePositionCurrent);
        }
        else {
            Vector3 newPosition = mousePositionCurrent;
            transform.position = newPosition;
            float newX = Mathf.Clamp(transform.position.x, xMin, xMax);
            float newY = Mathf.Clamp(transform.position.y, yMin, yMax);
            transform.position = new Vector3(newX, newY, 0f);
        }
    }

    private void OnMouseUp() {
        mousePositionStop = GetWorldMousePosition();
        if (isLauncherLoaded && Mathf.Abs(Vector3.Magnitude(mousePositionStart - mousePositionStop)) > 100) {
            player.SpawnPlanet(transform.position, mousePositionStop);
        }
        lineRenderer.SetPosition(0, new Vector3());
        lineRenderer.SetPosition(1, new Vector3());
    }

    private Vector3 GetWorldMousePosition() {
        Vector3 thisMousePosition = Input.mousePosition;
        thisMousePosition.z = Camera.main.transform.position.z * -1;
        return Camera.main.ScreenToWorldPoint(thisMousePosition);
    }

    private void CreateLineRenderer() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.SetColors(lineColor1, lineColor2);
        lineRenderer.SetWidth(2f, 4f);
        lineRenderer.SetVertexCount(2);
    }
}
