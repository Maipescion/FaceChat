using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TMP_InputField textToSend;

    public void SendText() => GetComponent<TestClient>().SendText(textToSend.text);
}
