using System;
using System.Collections;
using UnityEngine;
using NativeWebSocket;

// We'll need the NativeWebSocket plugin for this | package must be added via Unity Package Manager or downloaded from GitHub 
// Github link: https://github.com/endel/NativeWebSocket

public class WebSocketManager : MonoBehaviour
{
    private WebSocket websocket;

    // Event to notify other scripts of received action
    public event Action<int> OnActionReceived;

    async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        // Connect to the Python server running separately
        websocket = new WebSocket("ws://localhost:8000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection opened");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received message: " + message);

            try
            {
                // Assuming the Python server sends: { "next_action": int }
                var data = JsonUtility.FromJson<ActionResponse>(message);
                OnActionReceived?.Invoke(data.next_action);
            }
            catch (Exception ex)
            {
                Debug.Log("Failed to parse JSON: " + ex.Message);
            }
        };

        await websocket.Connect();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    // Send data to Python server
    public async void SendMessageToServer(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
        }
    }

    [Serializable]
    public class ActionResponse
    {
        public int next_action;
    }
}
