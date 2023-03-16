using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IESManager : MonoBehaviour
{
    private const string IES_RESOURCES_FOLDER = "IES";
    private List<IESLight> IESs;

    void Awake()
    {
        IESs = new List<IESLight>();
        string IESDirectory = Path.Combine(Application.persistentDataPath, IES_RESOURCES_FOLDER);
        System.IO.Directory.CreateDirectory(IESDirectory);

        Object[] IES = Resources.LoadAll(IES_RESOURCES_FOLDER);
        foreach (Object ies in IES) {
            string path = Path.Combine(IESDirectory, ies.name + ".ies");
            if (!System.IO.File.Exists(path)) {
                TextAsset iesResource = UnityEngine.Resources.Load<TextAsset>(IES_RESOURCES_FOLDER + "/" + ies.name);
                using (FileStream file = File.Create(path))
                {
                    AddText(file, iesResource.text);
                }
            }

            IESs.Add(new IESLight(ies.name, LoadCookie(path), GetIntensity(path)));
        }
    }

    public IESLight GetIESLightFromName(string name)
    {
        IESLight IES = IESs.Find(iesLight => iesLight.Name == name);
        if (IES == null) {
            IES = IESs[0];
        }
        return IES;
    }

    private void AddText(FileStream file, string value)
    {
        byte[] info = new System.Text.UTF8Encoding(true).GetBytes(value);
        file.Write(info, 0, info.Length);
    }

    private Cubemap LoadCookie(string path)
    {
        return IESLights.RuntimeIESImporter.ImportPointLightCookie(path, 128, false);
    }

    private LightIntensity GetIntensity(string path)
    {
        IESReader iesReader = new IESReader();
        iesReader.ReadFile(path);

        // Sometimes Lumens are not specified in the IES file, so we use Candela in such case
        if (iesReader.TotalLumens == -1) {
            return new LightIntensity(iesReader.MaxCandelas, UnityEngine.Rendering.HighDefinition.LightUnit.Candela);
        } else {
            return new LightIntensity(iesReader.TotalLumens, UnityEngine.Rendering.HighDefinition.LightUnit.Lumen);
        }
    }
}