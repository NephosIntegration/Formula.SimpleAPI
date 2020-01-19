using System;
using System.IO;
using System.Text.Json;

namespace Formula.SimpleAPI
{
    public class CorsConfigLoader : ICorsConfig
    {
        protected CorsConfigDefinition instance = null;

        public CorsConfigLoader LoadFromFile(String fileName)
        {
            var json = File.ReadAllText(fileName);
            this.instance = JsonSerializer.Deserialize<CorsConfigDefinition>(json);
            return this;
        }

        public CorsConfigLoader SaveToFile(String fileName)
        {
            if (this.InstanceValid())
            {
                var json = JsonSerializer.Serialize(this.instance);
                var fileStream = File.Open(fileName, FileMode.Append, FileAccess.Write);
                var fileWriter = new StreamWriter(fileStream);
                fileWriter.Write(json);
                fileWriter.Flush();
                fileWriter.Close();
            }

            return this;
        }

        protected Boolean InstanceValid(Boolean throwIfNot = true)
        {
            Boolean output = false;

            if (this.instance == null)
            {
                if (throwIfNot)
                {
                    throw new Exception("Cors Configuration not found");
                }
            }
            else
            {
                output = true;
            }

            return output;
        }

        public string[] GetOrigins()
        {
            return this.instance.Origins;
        }

    }
}