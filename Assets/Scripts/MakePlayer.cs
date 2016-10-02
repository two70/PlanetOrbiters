using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

public class MakePlayer : MonoBehaviour {

    public GameObject objectToSpawn = null;

    private void Start() {
        if (NetworkingManager.Socket == null || NetworkingManager.Socket.Connected)
            Networking.Instantiate(objectToSpawn, NetworkReceivers.AllBuffered, PlayerSpawned);
        else {
            NetworkingManager.Instance.OwningNetWorker.connected += delegate ()
            {
                Networking.Instantiate(objectToSpawn, NetworkReceivers.AllBuffered, PlayerSpawned);
            };
        }

    }

    private void PlayerSpawned(SimpleNetworkedMonoBehavior playerObject) {
        if (!Networking.PrimarySocket.IsServer) {
            playerObject.transform.position = new Vector3(800f, -150f, 0f);
        }
        /*Debug.Log("The player object " + playerObject.name + " has spawned at " +
            "X: " + playerObject.transform.position.x +
            "Y: " + playerObject.transform.position.y +
            "Z: " + playerObject.transform.position.z);*/
    }
}
