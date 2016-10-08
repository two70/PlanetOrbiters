using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class MakePlayer : MonoBehaviour {

    public GameObject star = null;
    public GameObject station = null;

    public GameObject playerOne;
    public GameObject playerTwo;
    public ulong myID;

    private void Start() {
        myID = Networking.PrimarySocket.Me.NetworkId;
        playerOne = Instantiate(star, new Vector3(-1500, 0, 0), Quaternion.identity) as GameObject;
        playerTwo = Instantiate(star, new Vector3(1500, 0, 0), Quaternion.identity) as GameObject;

        Vector3 stationPosition;

        // Player one is on west and player two is on east
        // Only the active player's station is viewable
        if(myID == 0) {
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
    }
    /*private void Start() {
        if (NetworkingManager.Socket == null || NetworkingManager.Socket.Connected)
            Networking.Instantiate(star, NetworkReceivers.AllBuffered, PlayerSpawned);
        else {
            NetworkingManager.Instance.OwningNetWorker.connected += delegate ()
            {
                Networking.Instantiate(star, NetworkReceivers.AllBuffered, PlayerSpawned);
            };
        }

    }

    private void PlayerSpawned(SimpleNetworkedMonoBehavior playerObject) {
        if (Networking.PrimarySocket.IsServer) {
            playerObject.transform.position = new Vector3(-1500f, 0f, 0f);
            Vector3 newPosition = playerObject.transform.position;
            newPosition.y = 550;
            Instantiate(station, newPosition, Quaternion.Euler(0, 0, 90));
        }
        else {
            playerObject.transform.position = new Vector3(1500f, 0f, 0f);
            Camera.main.transform.position = new Vector3(0, 0, 3000);
            Camera.main.transform.rotation *= Quaternion.Euler(0, 180, 0);
            Vector3 newPosition = playerObject.transform.position;
            newPosition.y = 550;
            Instantiate(station, newPosition, Quaternion.Euler(0, 0, 90));
        }
        Debug.Log("The player object " + playerObject.name + " has spawned at " +
            "X: " + playerObject.transform.position.x +
            "Y: " + playerObject.transform.position.y +
            "Z: " + playerObject.transform.position.z);
    }*/

}
