using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class News_Manager : MonoBehaviour
{
    [SerializeField] TMP_Text newsTxt;
    [SerializeField] List<string> News = new();
    [SerializeField] string[] chosedNews = new string[3];
    private void OnEnable()
    {
        Game_Manager.OnDay += RandomNews;
    }
    private void OnDisable()
    {
        Game_Manager.OnDay -= RandomNews;
    }

    void RandomNews()
    {
        for (int i = 0; i < 3; i++)
        {
            string selNews = "";
            bool alreadyChoosen;
            do
            {
                int news = Random.Range(0, News.Count);
                selNews = News[news];
                alreadyChoosen = false;
                for (int j = 0; j < i; j++)
                {
                    if (chosedNews[j] == selNews)
                    {
                        alreadyChoosen = true;
                        break;
                    }
                }
            } while (alreadyChoosen);
            chosedNews[i] = selNews;
        }
        newsTxt.text = $" NEWS:\n{chosedNews[0]}\n-------------\n{chosedNews[1]}\n-------------\n{chosedNews[2]}\n-------------";
    }
}

