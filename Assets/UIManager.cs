using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text Speed;
    [SerializeField] private TMP_Text Buoyancy;
    [SerializeField] private TMP_Text Mass;
    [SerializeField] private TMP_Text Drag;
    [SerializeField] private TMP_Text Resistance;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image LeftArrow;
    [SerializeField] private Image RightArrow;
    [SerializeField] private CreatureMovementPhyiscs player;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Color originalColor;


    private void Update()
    {
        float fillAmount = player.buoyancyForce / player.maxBuoyancyForce;
        fillImage.fillAmount = fillAmount;


        if (Input.GetKey(KeyCode.Q))
            RightArrow.color = pressedColor;
        
        else if (Input.GetKey(KeyCode.E))
            LeftArrow.color = pressedColor;
        else
        {
            LeftArrow.color = originalColor;
            RightArrow.color = originalColor;
        }

        
        //Updating Player's States to UI
        Speed.text = "Speed : " + player.moveSpeed;
        Buoyancy.text = "Buoyancy : " + player.buoyancyForce;
        Mass.text = "Mass : " + player.mass;
        Drag.text = "Drag : " + player.drag;
        Resistance.text = "Resistance : " + player.resistance;
    }
}
