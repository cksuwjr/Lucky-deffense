using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject dataLoadingSprite;
    [SerializeField] private TextMeshProUGUI dataLoadingText;

    private void Start()
    {
        SetLoadingText("");
        dataLoadingSprite.SetActive(false);

        Invoke("Init", 2.5f);
    }

    public void Init()
    {
        dataLoadingSprite.SetActive(false);

        StartCoroutine("Loading");
    }

    private IEnumerator Loading()
    {
        var dataManager = DataManager.Instance;

        DataManager.Instance.LoadData(2f);

        while (!dataManager.isDataLoad)
        {
            if (dataManager.task == DataManagerTask.LoadTable)
                SetLoadingText("테이블 불러오는 중..");
            if (dataManager.task == DataManagerTask.CheckLogin)
                SetLoadingText("로그인 중..");
            if (dataManager.task == DataManagerTask.LoadUserInformation)
                SetLoadingText("유저 불러오는 중..");
            yield return null;
        }

        SceneManager.LoadScene("GameScene");
    }

    private void SetLoadingText(string text)
    {
        dataLoadingSprite.SetActive(true);
        dataLoadingText.text = text;
    }
}
