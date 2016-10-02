using UnityEngine;
using System.Collections;

using BeardedManStudios.Network;

public class UniverseManager : SimpleNetworkedMonoBehavior {

    public GameObject[] stars;
    public GameObject[] planets;
    public GameObject planetPrefab;
    
    public float launchMagnitude;

    private Vector3 initialVelocity;
    private LineRenderer lineRenderer;
    private Vector3 worldMousePositionStart;
    private Vector3 worldMousePositionStop;
    public bool mouseHeldDown = false;
    private bool multiTouchMode = false;

    void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
    }
    protected override void NetworkStart() {
        base.NetworkStart();
    }
    
    void Update() {
        stars = GameObject.FindGameObjectsWithTag("Star");
        planets = GameObject.FindGameObjectsWithTag("Planet");

        if (Input.touchCount > 1) {
            mouseHeldDown = false;
            multiTouchMode = true;
            lineRenderer.SetPosition(0, new Vector3());
            lineRenderer.SetPosition(1, new Vector3());
        }

        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            worldMousePositionStart = GetWorldMousePosition();
        }
        else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
            lineRenderer.SetPosition(0, new Vector3());
            lineRenderer.SetPosition(1, new Vector3());
            worldMousePositionStop = GetWorldMousePosition();

            if (!multiTouchMode) {
                RPC("LaunchPlanet", worldMousePositionStart, worldMousePositionStop);
            }
            else {
                multiTouchMode = false;
            }
        }

        if (mouseHeldDown) {
            Vector3 worldMousePositionCurrent = GetWorldMousePosition();
            initialVelocity = worldMousePositionStart - worldMousePositionCurrent;
            launchMagnitude = initialVelocity.magnitude / 100f; // the original magnitude of the vector is way too large so we cut it down
            lineRenderer.SetPosition(0, worldMousePositionStart);
            lineRenderer.SetPosition(1, worldMousePositionCurrent);
        }
    }
    protected override void OwnerUpdate() {
        base.OwnerUpdate();
        
    }
    
    [BRPC]
    void LaunchPlanet(Vector3 start, Vector3 stop) {
        initialVelocity = start - stop; 
        launchMagnitude = initialVelocity.magnitude / 100f; // the original magnitude of the vector is way too large so we cut it down
        initialVelocity.Normalize(); // we want the normalized vector for the initial launch velocity, and then we
        initialVelocity *= launchMagnitude; // multiply it by our cut down magnitude
        GameObject planet = Instantiate(planetPrefab, start, Quaternion.identity) as GameObject;
        planet.GetComponent<Planet>().initialVelocity = initialVelocity;

        Debug.Log("Launched a planet: " + worldMousePositionStart + " " + worldMousePositionStop);
    }


    Vector3 GetWorldMousePosition() {
        Vector3 thisMousePosition = Input.mousePosition;
        thisMousePosition.z = Camera.main.transform.position.z * -1;
        return Camera.main.ScreenToWorldPoint(thisMousePosition);
    }
}
