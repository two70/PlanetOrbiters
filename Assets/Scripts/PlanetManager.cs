using UnityEngine;
using System.Collections;

public class PlanetManager : MonoBehaviour {

    public GameObject[] stars;
    public GameObject planetPrefab;
    public Vector3 worldMousePositionStart;
    public Vector3 worldMousePositionStop;
    public bool mouseHeldDown = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        stars = GameObject.FindGameObjectsWithTag("Star");

        if (Input.GetMouseButtonDown(0)) {
            mouseHeldDown = true;
            Vector3 thisMousePosition = Input.mousePosition;
            thisMousePosition.z = 100f;
            worldMousePositionStart = Camera.main.ScreenToWorldPoint(thisMousePosition);
            Debug.Log(worldMousePositionStart);
        }
        else if (Input.GetMouseButtonUp(0)) {
            mouseHeldDown = false;
            Vector3 thisMousePosition = Input.mousePosition;
            thisMousePosition.z = 100f;
            worldMousePositionStop = Camera.main.ScreenToWorldPoint(thisMousePosition);
        }
    }
}
