using System;
using UnityEngine;

public class AuthClient : MonoBehaviour
{
    private AuthRequester authRequester;

    private void Start()
    {
        authRequester = new AuthRequester();
        authRequester.Start();
    }

    public void InitializeAuth(string initializationText, Action<string> onResponseReceived)
    {
        authRequester.SetOnTextReceivedListener(onResponseReceived);
        authRequester.SendText(initializationText);
    }

    private void OnDestroy()
    {
        authRequester.Stop();
    }
}