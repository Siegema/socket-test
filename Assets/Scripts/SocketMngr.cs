using System;
using UnityEngine;
using UnityEngine.Events;

using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using System.Collections.Generic;

[Serializable]
public class ConnectionResponse : UnityEvent<ConnectResponse> 
{
}; 

[Serializable]
public class CustomError : UnityEvent<Error> 
{
};


class MessageData
{
    public System.Object id = DateTimeOffset.Now.ToUnixTimeSeconds();
    public System.Object action = "allseated";
};

[Serializable]
public class SocketMngr : MonoBehaviour
{
    public string address = "";

    public ConnectionResponse OnConnect;
    public UnityEvent OnDisconnect;
    public CustomError OnError;

    public bool sending;

    private string workAroundNamespace = "/5f85a298cdb604306224db80,";
    private string classNamespace = "/5f85a298cdb604306224db80";
    private SocketManager Manager; 

    private Socket root;
    private Socket customNamespace;
    private Socket messageSocket;

    void Start()
    {
        SocketOptions options = new SocketOptions(); 
        options.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;

        Manager = new SocketManager(new Uri(this.address), options);

        root = Manager.Socket; 
        customNamespace = Manager.GetSocket(classNamespace); 
        messageSocket = Manager.GetSocket(workAroundNamespace);

        customNamespace.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnect.Invoke);
        customNamespace.On<Error>(SocketIOEventTypes.Error, OnError.Invoke);
        customNamespace.On(SocketIOEventTypes.Disconnect, OnDisconnect.Invoke);
        customNamespace.On<Dictionary<string, System.Object>>("message", OnMessage);
    }

    public void Send()
    {
        messageSocket.Emit("message", "message", new MessageData());
    }

    void OnMessage(Dictionary<string, System.Object> msg)
    {
        Debug.Log(msg["id"]);
        Debug.Log(msg["action"]);
    }

    private void Update()
    {
        if (sending)
        {
            sending = false;
            Send();
        }
    }

}
