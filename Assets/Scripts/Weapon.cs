using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    public int bulletInMagazine = 20;


    public Vector3 stockPos;
    public Vector3 zoomPos;
    public Vector3 shotPos;

    public float shotPower = 1f;
    public float shotMultipler = 0.5f;

    public GameObject bulletHole;

    private Camera playerCamera;
    private TextMeshProUGUI pointText;
    private TextMeshProUGUI scoreText;
    private int score = 0;

    public void Awake()
    {
        scoreText = GameObject.Find("Score_Text").GetComponent<TextMeshProUGUI>();

        playerCamera = Camera.main;

        pointText = GameObject.Find("Point_Text").GetComponent<TextMeshProUGUI>();
    }
    public void BulletHole()
    {
        pointText.fontSize = 20;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 100))
        {
            var obj = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
            obj.transform.position += obj.transform.forward / 1000; 
            Destroy(obj, 5);

            switch (hit.transform.gameObject.tag)
            {
                case "2":
                    StartCoroutine(Point(2));
                    score += 2;
                    scoreText.text = "Score = " + score;
                    break;
                case "7":
                    StartCoroutine(Point(7));
                    score += 7;
                    scoreText.text = "Score = " + score;
                    break;
                case "8":
                    StartCoroutine(Point(8));
                    score += 8;
                    scoreText.text = "Score = " + score;
                    break;
                case "9":
                    StartCoroutine(Point(9));
                    score += 9;
                    scoreText.text = "Score = " + score;
                    break;
                case "10":
                    StartCoroutine(Point(10));
                    score += 10;
                    scoreText.text = "Score = " + score;
                    break;
                case "12":
                    StartCoroutine(Point(12));
                    score += 12;
                    scoreText.text = "Score = " + score;
                    break;
            }
        }
    }

    IEnumerator Point(int point)
    {
        pointText.text = "+ " + point;

        while (pointText.fontSize < 49)
        {
            pointText.fontSize = Mathf.Lerp(pointText.fontSize, 50, 0.03f);

            yield return null;
        }

        pointText.fontSize = 20;
        pointText.text = "";
    }
}
