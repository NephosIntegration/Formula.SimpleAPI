using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    public class CorsConfigLoader : ConfigLoader<CorsConfigDefinition>, ICorsConfig
    {
        public string[] GetOrigins()
        {
            return instance.Origins;
        }

        public static new CorsConfigLoader Get(string fileName, GetDefaults getDefaults = null)
        {
            var output = new CorsConfigLoader();

            output.LoadFromFile(fileName, getDefaults);

            return output;
        }

    }
}