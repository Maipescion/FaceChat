using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class AuthRequester : RunAbleThread
{
    private RequestSocket client;

    private Action<string> onTextReceived;
    
    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            while (Running)
            {
                string message = null;
                bool gotMessage = false;
                while (Running)
                {
                    try
                    {
                        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                        if (gotMessage) break;
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (gotMessage)
                {
                    Debug.Log("Messaggio ricevuto: " + message);
                    onTextReceived?.Invoke(message);
                }
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    public void SendText(string text)
    {
        client.SendFrame(text);
        Debug.Log($"Messaggio inviato: {text}");
    }

    public void SetOnTextReceivedListener(Action<string> onTextReceived) => this.onTextReceived = onTextReceived;
}