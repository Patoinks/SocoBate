using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class UI : MonoBehaviour
{
    public GameObject Player1HealthBar;
    public GameObject Player2HealthBar;

    public GameObject Nickname1;
    public GameObject Nickname2;

    public void Start()
    {
        Player1HealthBar.GetComponent<Slider>().value = 1;
        Player2HealthBar.GetComponent<Slider>().value = 1;
        Nickname1.GetComponent<TMP_Text>().text = Context.UserContext.account.Nickname;
        Nickname2.GetComponent<TMP_Text>().text = Context.FightContext.OpponentNickName;
    }


    public void SetPlayer1HealthBar(float health)
    {
        Player1HealthBar.GetComponent<Slider>().value = health;
    }

    public void SetPlayer2HealthBar(float health)
    {
        Player2HealthBar.GetComponent<Slider>().value = health;
    }

    public void SetNickname1(string nickname)
    {
        Nickname1.GetComponent<Text>().text = nickname;
    }

    public void SetNickname2(string nickname)
    {
        Nickname2.GetComponent<Text>().text = nickname;
    }

}
