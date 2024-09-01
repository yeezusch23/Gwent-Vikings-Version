using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class DSLManager : MonoBehaviour
{      
    public GameObject input;
    private TMP_InputField inputField;

    public GameObject Console;
    public TMP_InputField Text;

    void Start()
    {
        // inputField = input.GetComponent<TMP_InputField>();
        // Text = Console.GetComponent<TMP_InputField>();
    }

    //Se llama al presionar el boton Compilar
    public void DSLCompiler(){
        inputField = input.GetComponent<TMP_InputField>();
        Text = Console.GetComponent<TMP_InputField>();
        DSL.Compile(inputField.text);
    }

    public void MenuInicial()
    {
        SceneManager.LoadScene("MenuInicial");
    }
}