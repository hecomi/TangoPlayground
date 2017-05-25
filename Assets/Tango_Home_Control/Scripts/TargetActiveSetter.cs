using UnityEngine;
using UnityEngine.UI;

public class TargetActiveSetter : MonoBehaviour
{
    [SerializeField]
    GameObject[] activeTargets;

    [SerializeField]
    GameObject[] inactiveTargets;

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        foreach (var target in activeTargets) {
            target.SetActive(true);
        }
        foreach (var target in inactiveTargets) {
            target.SetActive(false);
        }
    }
}