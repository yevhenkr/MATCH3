using UnityEngine;
using UnityEngine.UI;

public class InputCount : MonoBehaviour
{
    public InputField colums;
    public InputField rows;

    public char[] validValue;
    public char[] noValidValue;

    private void Update()
    {
        if (colums.text != "")
        {
            CheckEnterValue(colums, true);
        }

        if (rows.text != "")
        {
            
            CheckEnterValue(rows,false);
        }
    }

    private void CheckEnterValue(InputField enterValue,bool column)
    {
        for (int i = 0; i < noValidValue.Length; i++)
        {
            if (enterValue.text == noValidValue[i].ToString())
            {
                enterValue.text = "";
                i = noValidValue.Length;
            }
        }

        for (int i = 0; i < validValue.Length; i++)
        {
            if (enterValue.text == validValue[i].ToString())
            {
                if (column)
                {
                    MenuSettings.Colums = int.Parse(enterValue.text);
                }
                else
                {
                    MenuSettings.Rows = int.Parse(enterValue.text);
                }
               
                i = validValue.Length;
            }
        }
    }
}