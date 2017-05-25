using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HueColorChanger : MonoBehaviour
{
    public string apiUrlTemplate = "{server}/hue/{id}/{api}/{color}";
    public string server = "http://192.168.1.2:3000";
    public Dropdown target;
    public string api = "rgb";

    private Color32 color
    {
        get
        {
            var image = GetComponent<Image>();
            return image ? image.color : Color.white;
        }
    }

    private string apiUrl
    {
        get 
        { 
            var colorStr = string.Format("{0},{1},{2}", color.r, color.g, color.b);
            return apiUrlTemplate
                .Replace("{server}", server)
                .Replace("{id}", (target.value + 1).ToString())
                .Replace("{api}", api)
                .Replace("{color}", colorStr);
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