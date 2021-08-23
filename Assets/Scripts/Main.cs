using SimpleFileBrowser;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TMP_InputField nameIF;
    public TextMeshProUGUI stateIF;
    private string textState = "IN ATTESA";

    public void GetFileToUpload()
    {
        FileBrowser.ShowLoadDialog(filePaths =>
        {
            var filePath = filePaths[0];
            GetComponent<AuthClient>().InitializeAuth(filePath + " " + nameIF.text, response =>
            {
                if (response.Contains("Denied"))
                    textState = "RICONOSCIMENTO FALLITO";
                else
                    textState = "UTENTE RICONOSCIUTO: " + response.Split(' ')[1];
            });
            
            textState = "RICONOSCIMENTO...";
            
        }, () => { }, FileBrowser.PickMode.Files);
    }

    private void Update() => stateIF.text = textState;
}