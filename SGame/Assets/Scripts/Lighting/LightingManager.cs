using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Credit to Glynn Taylor (https://gist.github.com/Glynn-Taylor/08da28896147faa6ba8f9654057d38e6, https://www.youtube.com/watch?v=m9hj9PdO328)
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Light MoonLight;
    [SerializeField] private LightingPresets Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    public bool upEv;
    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            //(Replace with a reference to the game time)
           // TimeOfDay += Time.deltaTime;
            TimeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        if (TimeOfDay >= 0.25f || TimeOfDay <= 0.75f) {
            RenderSettings.sun = DirectionalLight;
        }
        else
        {
            RenderSettings.sun = MoonLight;
        }
        if (upEv)
        {
            TimeOfDay += 0.01f;
        }
        if (TimeOfDay % 1 == 0)
        {
            DynamicGI.UpdateEnvironment();
            
        }
        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            


            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
        

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}