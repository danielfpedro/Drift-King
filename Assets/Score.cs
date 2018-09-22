using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public VehicleController vehicle;
    public Text scoreText;
    public Text comboText;
    public float currentScore;
    public float currentCombo;
    public float comboBreak = 3;
    public float comboTimer = 0;

    // Use this for initialization
    void Start () {
        currentScore = 0;
        currentCombo = 0;

    }
	
	// Update is called once per frame
	void Update () {
        if (comboTimer >= comboBreak) {
            currentScore += currentCombo;
            currentCombo = 0;
        }

        scoreText.text = currentScore.ToString();
        comboText.text = currentCombo.ToString();
    }
    void FixedUpdate()
    {
        if (vehicle.grounded && vehicle.sidewaysDeslocation > 30f)
        {
            currentCombo += (vehicle.sidewaysDeslocation * vehicle.speed) / 1000f;
            comboTimer = 0;
        } else
        {
            comboTimer += Time.deltaTime;
        }
    }
}
