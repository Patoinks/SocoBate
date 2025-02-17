using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TMP_Text nicknameText;

    void Start()
    {
      nicknameText.text = Context.UserContext.account.Nickname;
    }
}
