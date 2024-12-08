using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitButton : MonoBehaviour
{
    public Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick(){
        Debug.Log("Quitting...");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
