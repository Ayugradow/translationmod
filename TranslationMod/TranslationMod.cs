
using System.Collections.Generic;

using System.IO;

using System.Reflection;

using Modding;


namespace TranslationMod
{
    public class TranslationMod : Mod
    {
        public const string version = "1.0.0";

        public override string GetVersion()
        {
            return version;
        }

        public override void Initialize()
        {
            Log("Initializing Poorly Translated Mod");

            InitializeDictionaries();
            ModHooks.Instance.LanguageGetHook += Instance_LanguageGetHook;

            Log("Initialized Poorly Translated Mod");
        }

        private void InitializeDictionaries()
        {
            translationDict = new Dictionary<string, Dictionary<string, string>>();
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                LogDebug(resourceName);
                if (!resourceName.EndsWith("txt")) continue;
                Dictionary<string, string> tempDict = new Dictionary<string, string>();
                StreamReader text = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                string line;
                while ((line = text.ReadLine()) != null)
                {
                    tempDict.Add(line.Split(':')[0].Trim(), line.Split(':')[1].Trim());
                }
                text.Close();
                translationDict.Add(Path.GetFileNameWithoutExtension(resourceName).Remove(0,25), tempDict);
            }
        }

        private string Instance_LanguageGetHook(string key, string sheetTitle)
        {
            string ret = Language.Language.GetInternal(key, sheetTitle);
            return !translationDict.ContainsKey(sheetTitle) ? ret : translationDict[sheetTitle][key];
        }

        public Dictionary<string, Dictionary<string,string>> translationDict;
    }
}
