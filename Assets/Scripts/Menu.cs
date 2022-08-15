using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public TMP_InputField InputField;
    public GameObject pressText;


    private void Update()
    {
        if (InputField.text != "")
        {
            pressText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                LoadLevel();
            }
        }
        else
        {
            pressText.SetActive(false);
        }
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
