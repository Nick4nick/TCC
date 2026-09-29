using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GameTemplate.Editor
{
    /// <summary>
    /// Gera um ID novo a cada build. Em runtime, GameConfig compara esse ID com o último
    /// salvo no PlayerPrefs: se for diferente, apaga o save antigo (evita que uma build nova
    /// abra já com jogo salvo de um teste/build anterior).
    /// </summary>
    public class BuildIdGenerator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private const string ResourcesDir = "Assets/Resources";
        private const string BuildIdAssetPath = ResourcesDir + "/BuildId.txt";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!Directory.Exists(ResourcesDir))
                Directory.CreateDirectory(ResourcesDir);

            File.WriteAllText(BuildIdAssetPath, Guid.NewGuid().ToString());
            AssetDatabase.ImportAsset(BuildIdAssetPath);
        }
    }
}
