using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordUIManager : MonoBehaviour
{
    public Text uppercaseText;
    public Text symbolText;
    public Text digitText;
    public Text charText;
    public Text lengthText;
    [SerializeField] public InputField senhaInput;

    void Update()
    {
        string password = senhaInput.text; 

        uppercaseText.color = Validator.HasUppercase(password) ? Color.green : Color.red;
        symbolText.color = Validator.HasSymbol(password) ? Color.green : Color.red;
        digitText.color = Validator.HasDigit(password) ? Color.green : Color.red;
        charText.color = Validator.HasChar(password) ? Color.green : Color.red;
        lengthText.color = Validator.HasMinimumLength(password) ? Color.green : Color.red;

        Success();

    }

    void Success()
    {
        string password = senhaInput.text;

        if (Validator.HasUppercase(password) && Validator.HasDigit(password) && Validator.HasSymbol(password) && Validator.HasChar(password) && Validator.HasMinimumLength(password))
        {
            uppercaseText.CrossFadeAlpha(0, 0.04f, false);
            symbolText.CrossFadeAlpha(0, 0.04f, false);
            digitText.CrossFadeAlpha(0, 0.04f, false);
            charText.CrossFadeAlpha(0, 0.04f, false);
            lengthText.CrossFadeAlpha(0, 0.04f, false);
        }
        else
        {
            uppercaseText.CrossFadeAlpha(1, 0.08f, false);
            symbolText.CrossFadeAlpha(1, 0.08f, false);
            digitText.CrossFadeAlpha(1, 0.08f, false);
            charText.CrossFadeAlpha(1, 0.08f, false);
            lengthText.CrossFadeAlpha(1, 0.08f, false);
        }
    }
}