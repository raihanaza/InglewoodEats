using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Using UDP networking to receive messages from MediaPipe Python script. 
// Qill need to run locally on one machine.

public class MediaPipeReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private int port = 5005;

    [Header("References")]
    [SerializeField] private DishManager dishManager;
    [SerializeField] private PlateSpawner plateSpawner;

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;
    private string latestMessage = null;
    private readonly object messageLock = new object();

    void Start()
    {
        if (dishManager == null)
            Debug.LogError("MediaPipeReceiver: DishManager not assigned!");

        StartListening();
    }

    void StartListening()
    {
        udpClient = new UdpClient(port);
        isRunning = true;
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"MediaPipeReceiver: Listening on port {port}");
    }

    void ReceiveLoop()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(data);
                lock (messageLock)
                {
                    latestMessage = message;
                }
            }
            catch (SocketException) { break; }
        }
    }

    void Update()
    {
        string message = null;
        lock (messageLock)
        {
            if (latestMessage != null)
            {
                message = latestMessage;
                latestMessage = null;
            }
        }

        if (message != null)
            ProcessMessage(message);
    }

    void ProcessMessage(string json)
    {
        Debug.Log($"MediaPipeReceiver: Received: {json}");

        try
        {
            var data = JsonUtility.FromJson<MediaPipeData>(json);

            if (data.gesture == "push")
            {
                dishManager.OnDishPushed();
            }
            else if (data.gesture == "cup_fork_detected")
            {
                Debug.Log($"Cup: {data.cupPosition}, Fork: {data.forkPosition}");
                if (plateSpawner != null)
                    plateSpawner.OnObjectsDetected(data.cupPosition, data.forkPosition);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MediaPipeReceiver: Parse error: {e.Message}");
        }
    }

    void OnDestroy()
    {
        isRunning = false;
        udpClient?.Close();
        receiveThread?.Abort();
    }
}

[System.Serializable]
public class MediaPipeData
{
    public string gesture;
    public SerializableVector3 cupPosition;
    public SerializableVector3 forkPosition;
}

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3() => new Vector3(x, y, z);
}