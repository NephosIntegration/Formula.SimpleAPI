using System;
using System.IO;
using System.Text.Json;
using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    public class CorsConfigLoader : ConfigLoader<CorsConfigDefinition>, ICorsConfig
    {
        public string[] GetOrigins()
        {
            return this.instance.Origins;
        }

        public static new CorsConfigLoader Get(String fileName, GetDefaults getDefaults = null)
        {
            var output = new CorsConfigLoader();

            output.LoadFromFile(fileName, getDefaults);

            return output;
        }

    }
}