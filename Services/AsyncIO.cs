using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Services
{
    public static class AsyncIO
    {
        /// <summary>
        /// Write into a file asynchronously
        /// </summary>
        /// <param name="pluginName">The plugin's name</param>
        /// <param name="text">The text to write</param>
        /// <param name="extension">The file extension</param>
        static public async void ProcessWrite(string pluginName, string fileName, string text)
        {
            await WriteTextAsync("Plugins/" + pluginName + "/" + fileName, text);
        }

        /// <summary>
        /// Main method to write a file asynchrounously
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        static private async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }


        static public async Task<string> ProcessRead(string pluginName, string fileName)
        {
            string filePath = "Plugins/" + pluginName + "/" + fileName;
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
            {
                try
                {
                    string text = await ReadTextAsync(filePath);
                    return text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return "File not found.";
        }

        static private async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }
    }
}
