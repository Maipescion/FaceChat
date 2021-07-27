using UnityEngine;

public class TestClient : MonoBehaviour
{
    private TestRequester testRequester;

    private void Start()
    {
        testRequester = new TestRequester();
        testRequester.Start();
    }

    public void SendText(string text)
    {
        testRequester.SendText(text);
    }

    private void OnDestroy()
    {
        testRequester.Stop();
    }
}