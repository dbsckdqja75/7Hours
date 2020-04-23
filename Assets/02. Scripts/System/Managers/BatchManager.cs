using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatchManager : MonoBehaviour
{

    public int batchNumber;

    public Image batchIcon;
    public Text batchTitleText;
    public Text batchDescriptionText;

    public Button buyButton;

    public GameObject buildObj;

    public Sprite[] batchIcons;

    public GameObject[] batchObjects;

    public string[] batchTitle;
    public string[] batchDescription;

    public int[] batchPrice;

    public static bool isBuildable;

    void Update()
    {
        if(!GameManager.isWave)
        {
            batchIcon.sprite = batchIcons[batchNumber];
            batchTitleText.text = batchTitle[batchNumber].ToString();
            batchDescriptionText.text = batchDescription[batchNumber].ToString();

            if (Input.GetKeyDown(KeyCode.Q) && batchNumber > 0)
            {
                batchNumber--;

                if (buildObj)
                    Destroy(buildObj);
            }

            if (Input.GetKeyDown(KeyCode.E) && batchNumber < 1)
            {
                batchNumber++;

                if (buildObj)
                    Destroy(buildObj);
            }

            if(GameManager.money >= batchPrice[batchNumber])
            {
                buyButton.interactable = true;
                buyButton.GetComponentInChildren<Text>().text = "구매 & 배치";
            }
            else
            {
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<Text>().text = "금액 부족";
            }

            if(!buildObj)
                buildObj = Instantiate(batchObjects[batchNumber], batchObjects[batchNumber].transform.position, Quaternion.identity);
        }
        else
        {
            batchNumber = 0;

            if (buildObj)
                Destroy(buildObj);
        }
    }

    public void OnBuild()
    {
        if(isBuildable && GameManager.money >= batchPrice[batchNumber])
        {
            buildObj.SendMessage("BuildUp", SendMessageOptions.DontRequireReceiver);
            buildObj = null;

            GameManager.MoneyPlus(-batchPrice[batchNumber]);
        }
    }
}
