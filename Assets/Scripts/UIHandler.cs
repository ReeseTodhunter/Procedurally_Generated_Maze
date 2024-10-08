using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text widthText, heightText, toleranceText, scaleText, xOffsetText, yOffsetText, cameraText;
    public Slider width, height, perlinTolerance, perlinScale, perlinXOffset, perlinYOffset, cameraHeight;
    public Toggle useSeed, usePerlin, randomiseOffset;
    public InputField seed;

    public MazeGenerator mazeGen;

    // Start is called before the first frame update
    void Start()
    {
        width.value = mazeGen.width;
        height.value = mazeGen.height;
        perlinTolerance.value = mazeGen.perlinTolerance;
        perlinScale.value = mazeGen.perlinScale;
        perlinXOffset.value = mazeGen.perlinOffset.x;
        perlinYOffset.value = mazeGen.perlinOffset.y;
        cameraHeight.value = mazeGen.camHeight;
        useSeed.isOn = mazeGen.useSeed;
        usePerlin.isOn = mazeGen.usePerlinNoise;
        randomiseOffset.isOn = mazeGen.randomiseOffset;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        UpdateValues();
    }

    public void UpdateText()
    {
        widthText.text = "Width: " + width.value;
        heightText.text = "Height: " + height.value;
        toleranceText.text = "Perlin Tolerance: " + perlinTolerance.value;
        scaleText.text = "Perlin Scale: " + perlinScale.value;
        xOffsetText.text = "Perlin X Offset: " + perlinXOffset.value;
        yOffsetText.text = "Perlin Y Offset: " + perlinYOffset.value;
        cameraText.text = "CameraHeight: " + cameraHeight.value;
    }

    public void UpdateValues()
    {
        mazeGen.width = (int)width.value;
        mazeGen.height = (int)height.value;
        mazeGen.perlinTolerance = perlinTolerance.value;
        mazeGen.perlinScale = perlinScale.value;
        mazeGen.perlinOffset.x = perlinXOffset.value;
        mazeGen.perlinOffset.y = perlinYOffset.value;
        mazeGen.camHeight = cameraHeight.value;
        mazeGen.useSeed = useSeed.isOn;
        mazeGen.usePerlinNoise = usePerlin.isOn;
        mazeGen.randomiseOffset = randomiseOffset.isOn;

        var SubmitEvent = new InputField.EndEditEvent();
        SubmitEvent.AddListener(inputSeed);
        seed.onEndEdit = SubmitEvent;
    }

    private void inputSeed(string seed)
    {
        mazeGen.seed = int.Parse(seed);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
