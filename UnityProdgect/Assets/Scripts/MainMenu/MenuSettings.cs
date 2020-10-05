using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    private static int columsCount;
    private static int rowsCount;
    
    public static int Colums
    {
        get { return columsCount; }
        set { columsCount = value; }
    }

    
    public static int Rows
    {
        get { return rowsCount; }
        set { rowsCount = value; }
    }

    public void PushStart()
    {
        if (columsCount != 0 && rowsCount != 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
    }
}