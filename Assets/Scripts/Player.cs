using UnityEngine;
using System.Collections.Generic;
using BeardedManStudios.Network;

public class Player : MonoBehaviour {

    public GameObject starPrefab = null;
    public GameObject stationPrefab = null;
    public List<GameObject> planetPrefabs;
    public GameObject sun;
    public ulong myID;
    public int currentPlanet = 0;
    public static List<Star> stars = new List<Star>();
    public static List<Planet> planets = new List<Planet>();
    
    private GameObject tempPlanet;
    private Station station = null;

    private void Start() {

        if (NetworkingManager.IsOnline) {
            myID = Networking.PrimarySocket.Me.NetworkId;
            Debug.Log("Connected");
        }
        else {
            myID = 0;
            Debug.Log("Not connected");
        }

        Vector3 stationPosition;
        
        sun = Instantiate(starPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        stars.Add(sun.GetComponent<Star>());
        //stars.Add(otherStar.GetComponent<Star>());

        stationPosition = sun.transform.position;
        stationPosition.y = 550f;
        stationPrefab = Instantiate(stationPrefab, stationPosition, Quaternion.Euler(0, 0, 90)) as GameObject;
        station = stationPrefab.GetComponent<Station>();
        GetPrefabs();
        
    }

    private void Update() { 
        
    }

    public void SetPlanetToBeLaunched (int num) {
        currentPlanet = num;
        station.isLauncherLoaded = true;
        if (tempPlanet)
            Destroy(tempPlanet);
        tempPlanet = Instantiate(planetPrefabs[currentPlanet], station.transform.position, Quaternion.identity) as GameObject;
        tempPlanet.GetComponent<Planet>().isTemporary = true;
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
        station.isLauncherLoaded = false;
    }

    private void PlanetSpawned(SimpleNetworkedMonoBehavior go, Vector3 initialVelocity) {
        Planet planet = go.GetComponent<Planet>();
        planet.initialVelocity = initialVelocity;
        planet.owner = myID;
    }

    private void GetPrefabs () {
        planetPrefabs = new List<GameObject>();

        foreach (GameObject obj in NetworkingManager.Instance.NetworkInstantiates) {
            planetPrefabs.Add(obj);
        }
    }
}
