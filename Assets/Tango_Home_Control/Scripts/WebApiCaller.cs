using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WebApiCaller : MonoBehaviour
{
    public string apiUrlTemplate = "{server}/{api}";
    public string server = "http://192.168.1.2:3000";
    public string api = "iremocon/ir001";
    public float delay = 0f;

    private string apiUrl
    {
        get 
        { 
            return apiUrlTemplate
                .Replace("{server}", server)
                .Replace("{api}", api);
        }
    }

    void Start()
    {
        var button = GetComponent<Button>();
        if (button) {
            button.onClick.AddListener(OnClicked);
        }
    }

    public void OnClicked()
    {
        StartCoroutine(CallApi());
    }

    IEnumerator CallApi()
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        var req = UnityWebRequest.Get(apiUrl);
        yield return req.Send();

        if (req.isError) {
            Debug.LogError(req.error);
        }
    }
}