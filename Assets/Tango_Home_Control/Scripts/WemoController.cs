using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WemoController : MonoBehaviour
{
    public string apiUrlTemplate = "{server}/wemo/{id}/{power}";
    public string server = "http://192.168.1.2:3000";
    public Dropdown target;
    public bool on;

    private string apiUrl
    {
        get 
        { 
            return apiUrlTemplate
                .Replace("{server}", server)
                .Replace("{id}", (target.value).ToString())
                .Replace("{power}", on ? "1" : "0");
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
        var req = UnityWebRequest.Get(apiUrl);
        yield return req.Send();

        if (req.isError) {
            Debug.LogError(req.error);
        }
    }
}