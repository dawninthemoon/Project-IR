#pragma warning disable 0649
using System.IO;
using System.Text;
using UnityEngine;
using Aroma;

public static class JsonHelper {
    static string SlashString = "/";
    static string JsonString = ".json";
    public static T[] LoadJsonFile<T>(string loadPath, string fileName) {
        FileStream fileStream = new FileStream(StringUtils.MergeStrings(loadPath, SlashString, fileName, JsonString), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        Wrapper<T> wrapper = new Wrapper<T>();
        return JsonUtility.FromJson<Wrapper<T>>(jsonData).items;
    }

    [System.Serializable]
    class Wrapper<T> {
        public T[] items;
    }
}