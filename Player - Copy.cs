using UnityEngine;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Player : MonoBehaviour {
    
    public GameObject star = null;
    public GameObject station = null;
    public List<GameObject> planetPrefabs;
    public GameObject playerOne;
    public GameObject playerTwo;

    public ulong myID;
    public int currentPlanet = 0;
    public Color lineColor1 = Color.yellow;
    public Color lineColor2 = Color.blue;

    private GameObject tempPlanet;
    private LineRenderer lineRenderer;
    private Vector3 worldMousePositionStart;
    private Vector3 worldMousePositionStop;
    private bool mouseHeldDown = false;
    private bool multiTouchMode = false;
    private bool isLauncherLoaded = false;

    // This is the resources directory where prefabs are loaded from
    public string resourcesDirectory = string.Empty;

    public static List<Star> stars = new List<Star>();

    
    private void Start() {
        //players.Add(this);
        if (NetworkingManager.IsOnline) {
            myID = Networking.PrimarySocket.Me.NetworkId;
            Debug.Log("Connected");
        }
        else {
            myID = 0;
            Debug.Log("Not connected");
        }
        playerOne = Instantiate(star, new Vector3(-1500, 0, 0), Quaternion.identity) as GameObject;
        stars.Add(playerOne.GetComponent<Star>());
      
        playerTwo = Instantiate(star, new Vector3(1500, 0, 0), Quaternion.identity) as GameObject;
        stars.Add(playerTwo.GetComponent<Star>());
        Vector3 stationPosition;

        // Player one is on west and player two is on east
        // Only the active player's station is viewable
        if (myID == 0) {
            stationPosition = playerOne.transform.position;
        }
        // Move camera so that no matter which player you are, your position is on the left
        else {
            stationPosition = playerTwo.transform.position;
            Camera.main.transform.position = new Vector3(0, 0, 3000);
            Camera.main.transform.rotation *= Quaternion.Euler(0, 180, 0);
        }
        stationPosition.y = 550f;
        Instantiate(station, stationPosition, Quaternion.Euler(0, 0, 90));

        CreateLineRenderer();

        GetPrefabs();
        
    }

    private void Update() { 
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
                // only spawn if this is the owner and the user did more than just click on screen
                if (Mathf.Abs(Vector3.Magnitude(worldMousePositionStart - worldMousePositionStop)) > 100) {
                    SpawnPlanet(worldMousePositionStart, worldMousePositionStop);
                }
            }
            else {
                multiTouchMode = false;
            }
        }
        if (mouseHeldDown) {
            Vector3 worldMousePositionCurrent = GetWorldMousePosition();
            lineRenderer.SetPosition(0, worldMousePositionStart);
            lineRenderer.SetPosition(1, worldMousePositionCurrent);
        }
    }

    private Vector3 GetWorldMousePosition() {
        Vector3 thisMousePosition = Input.mousePosition;
        if (myID == 0) {
            thisMousePosition.z = Camera.main.transform.position.z * -1;
        }
        else {
            thisMousePosition.z = Camera.main.transform.position.z;
        }
        return Camera.main.ScreenToWorldPoint(thisMousePosition);
    }

    public void SetPlanetToBeSpawned (int num) {
        currentPlanet = num;
        if (tempPlanet)
            Destroy(tempPlanet);
        tempPlanet = Instantiate(planetPrefabs[currentPlanet], station.transform.position, Quaternion.identity) as GameObject;
        tempPlanet.GetComponent<Planet>().isTemporary = true;
        isLauncherLoaded = true;
    }

    public void SpawnPlanet(Vector3 start, Vector3 stop) {
        Vector3 initialVelocity = start - stop;
        float launchMagnitude = initialVelocity.magnitude / 100f; // the original magnitude of the vector is way too large so we cut it down
        initialVelocity.Normalize(); // we want the normalized vector for the initial launch velocity, and then we
        initialVelocity *= launchMagnitude; // multiply it by our cut down magnitude

        if (tempPlanet)
            Destroy(tempPlanet);
        Networking.Instantiate(planetPrefabs[currentPlanet], start, Quaternion.identity,
            NetworkReceivers.AllBuffered, (go) => PlanetSpawned(go, initialVelocity));
    }

    private void PlanetSpawned(SimpleNetworkedMonoBehavior go, Vector3 initialVelocity) {
        Planet planet = go.GetComponent<Planet>();
        planet.initialVelocity = initialVelocity;
    }

    private void CreateLineRenderer () {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.SetColors(lineColor1, lineColor2);
        lineRenderer.SetWidth(2f, 4f);
        lineRenderer.SetVertexCount(2);
    }

    private void GetPrefabs () {
        planetPrefabs = new List<GameObject>();

        /*if (!string.IsNullOrEmpty(resourcesDirectory)) {
            foreach (GameObject obj in Resources.LoadAll<GameObject>(resourcesDirectory)) {
                GameObject planet = new GameObject();
                planetPrefabs.Add(obj);
            }
        }
        else {
            Debug.Log(resourcesDirectory + " is incorrect");
        }*/
        foreach (GameObject obj in NetworkingManager.Instance.NetworkInstantiates) {
            //GameObject planet = new GameObject();
            planetPrefabs.Add(obj);
        }
    }
}
