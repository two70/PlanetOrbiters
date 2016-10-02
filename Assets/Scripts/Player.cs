using UnityEngine;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Player : NetworkedMonoBehavior {
    
    public float mass = 100;

    public static int currentPlanet = 0;
    public Color c1 = Color.yellow;
    public Color c2 = Color.blue;
    
    private LineRenderer lineRenderer;
    private Vector3 worldMousePositionStart;
    private Vector3 worldMousePositionStop;
    private bool mouseHeldDown = false;
    private bool multiTouchMode = false;
    private List<GameObject> planetPrefabs;

    // This is the resources directory where prefabs are loaded from
    public string resourcesDirectory = string.Empty;

    public static List<Player> players = new List<Player>();
    public static List<Planet> planets = new List<Planet>();

    
    private void Start() {
        players.Add(this);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(2f, 4f);
        lineRenderer.SetVertexCount(2);
        planetPrefabs = new List<GameObject>();

        if (!string.IsNullOrEmpty(resourcesDirectory)) {
            foreach (GameObject obj in Resources.LoadAll<GameObject>(resourcesDirectory)) {
                GameObject planet = new GameObject();
                planetPrefabs.Add(obj);
            }
        }
        else {
            Debug.Log(resourcesDirectory + " is incorrect");
        }
    }

    //protected override void OwnerUpdate() {
      //  base.OwnerUpdate();
    //}
    private void Update() { 
        if (Input.GetKeyDown(KeyCode.Escape)) {
            players.Remove(this);
            Networking.Disconnect();
        }

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
                if (IsOwner && Mathf.Abs(Vector3.Magnitude(worldMousePositionStart - worldMousePositionStop)) > 100) {
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
        thisMousePosition.z = Camera.main.transform.position.z * -1;
        return Camera.main.ScreenToWorldPoint(thisMousePosition);
    }

    private void ExitGame() {
        Networking.PrimarySocket.disconnected -= ExitGame;

        BeardedManStudios.Network.Unity.MainThreadManager.Run(() => {
            Debug.Log("Quit game");

            BeardedManStudios.Network.Unity.UnitySceneManager.LoadScene("Menu");

        });
    }

    public void SetPlanetToBeSpawned (int num) {
        currentPlanet = num;
        Debug.Log(currentPlanet);
    }

    public void SpawnPlanet(Vector3 start, Vector3 stop) {
        Vector3 initialVelocity = start - stop;
        float launchMagnitude = initialVelocity.magnitude / 100f; // the original magnitude of the vector is way too large so we cut it down
        initialVelocity.Normalize(); // we want the normalized vector for the initial launch velocity, and then we
        initialVelocity *= launchMagnitude; // multiply it by our cut down magnitude

        Networking.Instantiate(planetPrefabs[currentPlanet], start, Quaternion.identity,
            NetworkReceivers.AllBuffered, (go) => PlanetSpawned(go, initialVelocity));
        
    }

    private void PlanetSpawned(SimpleNetworkedMonoBehavior go, Vector3 initialVelocity) {
        Planet planet = go.GetComponent<Planet>();
        planet.initialVelocity = initialVelocity;
        
    }
}
