using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);

    public TMP_InputField BetAmount;
    public TMP_InputField TotalAmount;
    public TMP_Text Alert;
    public TMP_Text SlideButtonValue;
    public TMP_Text SlideValue;
    public TMP_Text RandomValue;
    public TMP_Text Chance;
    public TMP_Text Reward;
    public TMP_Text EarnAmount;
    private float betAmount;
    private float totalAmount;
    public Button disable_BET;
    public Button disable_increase;
    public Button disable_decrease;
    public GameObject RollUnder;
    public GameObject RollOver;
    public GameObject TopBackground;
    public GameObject Slider;
    public GameObject Slider_Fill;
    public GameObject Slider_background;
    public static ReceiveJsonObject apiform;
    private int flag;
    private bool roll;
    private string BaseUrl = "http://83.136.219.243:3306";
    BetPlayer __player;
    void Start()
    {
        betAmount = 10.00f;
        BetAmount.text = betAmount.ToString("F2");
        __player = new BetPlayer();
        flag = 0;
        roll = false;
        Chance.text = "50%";
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameReady("Ready");
#endif
    }
    void Update()
    {

    }
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        __player.token = usersInfo["token"];
        totalAmount = float.Parse(usersInfo["amount"]);
        TotalAmount.text = totalAmount.ToString("F2");
    }
    public void double_increase()
    {
        StartCoroutine(_double_increase());
    }
    IEnumerator _double_increase()
    {
        if (flag == 0)
        {
            disable_increase.interactable = false;
            betAmount = float.Parse(BetAmount.text);
            if (totalAmount >= 2 * betAmount)
            {
                betAmount = 2 * betAmount;
                BetAmount.text = betAmount.ToString("F2");
            }
            else if (totalAmount < 10f)
            {
                Alert.text = "";
                Alert.text = "NOT ENOUGH BALANCE!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_increase.interactable = true;
            }
            else
            {
                betAmount = (int)Math.Floor(totalAmount);
                BetAmount.text = betAmount.ToString("F2");
                Alert.text = "";
                Alert.text = "MAXIMUM BET LIMIT " + betAmount.ToString("F2");
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_increase.interactable = true;

            }
            disable_increase.interactable = true;
        }

    }
    public void double_decrease()
    {
        StartCoroutine(_double_decrease());
    }
    IEnumerator _double_decrease()
    {
        if (flag == 0)
        {
            disable_decrease.interactable = false;
            betAmount = float.Parse(BetAmount.text);
            if (totalAmount >= betAmount / 2)
            {
                if (betAmount / 2 >= 10f)
                {
                    betAmount = betAmount / 2;
                    BetAmount.text = betAmount.ToString("F2");
                }
                else
                {
                    betAmount = 10f;
                    BetAmount.text = betAmount.ToString("F2");
                    Alert.text = "";
                    Alert.text = "MINIMUM BET LIMIT 10.00!";
                    yield return new WaitForSeconds(2f);
                    Alert.text = "";
                    disable_decrease.interactable = true;

                }
            }
            else if (totalAmount < 10f)
            {
                betAmount = 10f;
                BetAmount.text = betAmount.ToString("F2");
                Alert.text = "";
                Alert.text = "MINIMUM BET LIMIT 10.00!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_decrease.interactable = true;
            }
            else
            {
                betAmount = totalAmount;
            }
            BetAmount.text = betAmount.ToString("F2");
            disable_decrease.interactable = true;
        }
    }
    public void MAX()
    {
        betAmount = (int)Math.Floor(totalAmount);
        BetAmount.text = betAmount.ToString("F2");
    }
    public void MIN()
    {
        betAmount = 10f;
        BetAmount.text = betAmount.ToString("F2");
    }
    public void roll_under()
    {
        if (roll)
        {
            Slider_background.GetComponent<Image>().color = new Color32(146, 79, 158, 255);
            Slider_Fill.GetComponent<Image>().color = new Color32(123, 161, 78, 255);

            int value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            if (value > 95)
            {
                Slider.GetComponent<Slider>().value = 0.95f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            Chance.text = value.ToString() + "%";
            Reward.text = "X" + (98f / (value)).ToString("F2");
            roll = false;
        }
    }
    public void roll_over()
    {
        if (!roll)
        {
            Slider_background.GetComponent<Image>().color = new Color32(123, 161, 78, 255);
            Slider_Fill.GetComponent<Image>().color = new Color32(146, 79, 158, 255);

            int value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            if (value < 4)
            {
                Slider.GetComponent<Slider>().value = 0.04f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            Chance.text = (99 - value).ToString() + "%";
            Reward.text = "X" + (98f / (99 - value)).ToString("F2");
            roll = true;
        }
    }
    public void slide_value()
    {
        int value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
        SlideButtonValue.text = value.ToString();
        if (value > 10)
        {
            SlideValue.text = value.ToString();
        }
        else
        {
            SlideValue.text = "0" + value.ToString();
        }
        if (!roll)
        {
            if (value > 95)
            {
                Slider.GetComponent<Slider>().value = 0.95f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            else if (value <= 0)
            {
                Slider.GetComponent<Slider>().value = 0.01f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            Chance.text = value.ToString() + "%";
            float reward_value = 98f / value;
            Reward.text = "X" + reward_value.ToString("F2");
        }
        else
        {
            if (value > 98)
            {
                Slider.GetComponent<Slider>().value = 0.98f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            else if (value < 4)
            {
                Slider.GetComponent<Slider>().value = 0.04f;
                value = Convert.ToInt32(Slider.GetComponent<Slider>().value * 100);
            }
            Chance.text = (99 - value).ToString() + "%";
            Reward.text = "X" + (98f / (99 - value)).ToString("F2");
        }
    }
    public void PlayGame()
    {
        StartCoroutine(_PlayGame());
    }
    IEnumerator _PlayGame()
    {
        disable_BET.interactable = false;
        flag = 1;
        betAmount = float.Parse(BetAmount.text);
        if (betAmount < 10)
        {
            betAmount = 10f;
            Alert.text = "";
            Alert.text = "MINIMUM BET LIMIT 10.00!";
            yield return new WaitForSeconds(2f);
            Alert.text = "";
            flag = 0;
            disable_BET.interactable = true;
        }
        else
        {
            if (totalAmount >= betAmount)
            {
                StartCoroutine(Server());
            }
            else
            {
                Alert.text = "";
                Alert.text = "NOT ENOUGH BALANCE!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                flag = 0;
                disable_BET.interactable = true;
            }
        }
    }
    private IEnumerator Server()
    {
        betAmount = float.Parse(BetAmount.text);
        WWWForm form = new WWWForm();
        form.AddField("token", __player.token);
        form.AddField("betAmount", betAmount.ToString("F2"));
        form.AddField("SlideValue", SlideValue.text);
        form.AddField("roll", roll.ToString());

        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/BET", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            yield return new WaitForSeconds(2f);
            Alert.text = "";
            flag = 0;
            disable_BET.interactable = true;
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                if (apiform.randomNumber >= 10)
                {
                    RandomValue.text = apiform.randomNumber.ToString();
                }
                else
                {
                    RandomValue.text = "0" + apiform.randomNumber.ToString();
                }
                if (apiform.earnAmount > 0f)
                {
                    totalAmount += apiform.earnAmount - betAmount;
                    TotalAmount.text = totalAmount.ToString("F2");
                    EarnAmount.text = apiform.earnAmount.ToString("F2");
                    Alert.text = "";
                    Alert.text = "YOU WIN!";
                    yield return new WaitForSeconds(2f);
                    Alert.text = "";
                }
                else
                {
                    totalAmount -= betAmount;
                    TotalAmount.text = totalAmount.ToString("F2");
                    EarnAmount.text = "0.00";
                    Alert.text = "";
                    Alert.text = "BETTER LUCK NEXT TIME!";
                    yield return new WaitForSeconds(2f);
                    Alert.text = "";
                }
                flag = 0;
                disable_BET.interactable = true;
            }
            else if (apiform.Message == "BET ERROR!")
            {
                Alert.text = "";
                Alert.text = "BET ERROR!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                flag = 0;
                disable_BET.interactable = true;
            }
            else if (apiform.Message == "SERVER ERROR!")
            {
                Alert.text = "";
                Alert.text = "SERVER ERROR!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                flag = 0;
                disable_BET.interactable = true;
            }
        }
    }
}
public class BetPlayer
{
    public string token;
}
