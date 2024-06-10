using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Transform player; 
    public float heightThreshold = 10f; // Die Höhe, bei der sich die Skybox ändert.
    public Material lowSkybox; // Die Skybox für niedrige Höhen.
    public Material highSkybox; // Die Skybox für hohe Höhen.

    private bool isHighSkyboxActive = false;

    void Update()
    {
        if (player.position.y >= heightThreshold && !isHighSkyboxActive)
        {
            ChangeSkybox(highSkybox);
            isHighSkyboxActive = true;
        }
        else if (player.position.y < heightThreshold && isHighSkyboxActive)
        {
            ChangeSkybox(lowSkybox);
            isHighSkyboxActive = false;
        }
    }

    void ChangeSkybox(Material skybox)
    {
        RenderSettings.skybox = skybox;
        DynamicGI.UpdateEnvironment(); // Aktualisiert die Beleuchtung für die neue Skybox.
    }
}
