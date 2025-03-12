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

        DataManager.Instance.LoadData(0.5f);

        while (!dataManager.isDataLoad)
        {
            if (dataManager.task == DataManagerTask.LoadTable)
                SetLoadingText("���̺� �ҷ����� ��..");
            if (dataManager.task == DataManagerTask.CheckLogin)
                SetLoadingText("�α��� ��..");
            if (dataManager.task == DataManagerTask.LoadUserInformation)
                SetLoadingText("���� �ҷ����� ��..");
            yield return null;
        }

        SuccessLoading();
    }

    private void SuccessLoading()
    {
        LoadingSceneManager.SetNextScene("LobbyScene");
        SceneManager.LoadScene("LoadingScene");
    }

    private void SetLoadingText(string text)
    {
        dataLoadingSprite.SetActive(true);
        dataLoadingText.text = text;
    }
}
