using System;
using System.Collections.Generic;
using StreamReader = System.IO.StreamReader;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using MegaFilesSharing.Models;
using MemoryStream = System.IO.MemoryStream;
using StreamWriter = System.IO.StreamWriter;

namespace MegaFilesSharing.Controllers
{
    public class MegaCoNz
    {
        public File getFile(string urlstr)
        {
            string title = "", fileSize = "", fname = "";
            string url = "https://eu.api.mega.co.nz/cs?id=1";

            try
            {
                //getting FileID and File KEY
                string[] stra = urlstr.Split('!');
                string key = stra[2];
                string fileID = stra[1];

                //Create HTTP Connection 
                WebClient client = new WebClient();

                string jsonStr = "[{\"a\":\"g\",\"ssl\":" + 1
                    + ",\"p\":\"" + fileID + "\"}]";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());

                streamWriter.Write(jsonStr);
                streamWriter.Flush();
                streamWriter.Close();
                //Sending JSON request
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());

                jsonStr = streamReader.ReadToEnd();
                //Getting file Size
                fileSize = Regex.Match(jsonStr, "\"s\"\\s*?:\\s*?(\\d+)").Groups[1].Value;

                if (String.IsNullOrWhiteSpace(fileSize))
                {
                    //if file not found 
                    return null;
                }
                else
                {
                    //Getting File name using AES No Pagging Decryption
                    fname = Regex.Match(jsonStr, "\"at\"\\s*?:\\s*?\"(.*?)\"").Groups[1].Value;
                    title = decrypt(fname, key);
                }

                System.Diagnostics.Debug.WriteLine(jsonStr);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            //Return file Object with File Size and File Name and Type
            File file = new File();
            file.FileSize = Math.Round((Convert.ToDouble(fileSize) / 1024 / 1024), 3);
            file.Name = title;
            file.FileType = getFileType(VirtualPathUtility.GetExtension(title));
             
            return file;
        }

        public string getFileType(string fex)
        {
            fex = fex.ToLower();
            string type = "Other";

            if(fex == ".mp4" || fex == ".mkv" || fex == ".rmvb" || 
                fex == ".flv" || fex == "avi" || fex == ".mov" ||  
                fex == ".rm" || fex == ".divx" || fex == ".mpeg" || 
                fex == ".mpeg-2" || fex == ".mpeg-4" || fex == ".ts" || 
                fex == ".mpeg-1" || fex == ".3gpp" || fex == ".avchd" )
                type = "Video";
            else if(fex == ".mp3" || fex == ".flac" || fex == ".ape" || 
                fex == ".wav" || fex == ".aiff" || fex == ".m4a" || 
                fex == ".m4b" || fex == ".m4p" || fex == ".mp4" ||
                fex == ".mid" || fex == ".midi" || fex == ".wma")
                type = "Audio";

            return type;
        }

        private byte[] b64decode(String data)
        {
            data = data.Replace(",", "");
            data += "==".Substring((2 - data.Length * 3) & 3);
            data = data.Replace("-", "+").Replace("_", "/");
            byte[] temp = Convert.FromBase64String(data);
            return temp;
        }

        private byte[] aInt_to_aByte(params int[] intKey)
        {
            byte[] result = new byte[intKey.Length * 4];
            int counter = 0;
            foreach (int k in intKey)
            {
                byte[] bytes = BitConverter.GetBytes(k);
                Array.Reverse(bytes);
                foreach (byte b in bytes)
                {
                    result[counter] = b;
                    counter++;
                }
            }
            return result;
        }

        private int[] aByte_to_aInt(params byte[] bytes)
        {
            int[] res = new int[bytes.Length / 4];

            for (int i = 0; i < res.Length; i++)
            {
                byte[] bs = new byte[4];
                for (int k = 0; k < 4; k++)
                {
                    bs[k] = bytes[(i * 4) + 3 - k];
                }
                res[i] = BitConverter.ToInt32(bs, 0);
            }
            return res;
        }

        private String decrypt(String input, String keyString)
        {
            string ret = "";
            //decode b64 string to byte array
            byte[] cipherBytes = b64decode(input);
            byte[] b64Dec = b64decode(keyString);

            //Convert byte array to int array
            int[] intKey = aByte_to_aInt(b64Dec);

            //Convert int array to byte array
            //Ex-OR btye
            byte[] key = aInt_to_aByte(intKey[0] ^ intKey[4],
                intKey[1] ^ intKey[5], intKey[2] ^ intKey[6], intKey[3]
                        ^ intKey[7]);
            byte[] iv = aInt_to_aByte(0, 0, 0, 0);

            //decrypt  
            try
            {
                RijndaelManaged rijAlg = new RijndaelManaged();
                rijAlg.Key = key;
                rijAlg.IV = iv;
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.None;

                ICryptoTransform decrpytor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                MemoryStream msDecrypt = new MemoryStream(cipherBytes);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decrpytor, CryptoStreamMode.Read);
                StreamReader srDecrypt = new StreamReader(csDecrypt);
                ret = srDecrypt.ReadToEnd();
            }
            catch (Exception e)
            {

            }


            if (ret != null && !ret.StartsWith("MEGA{"))
            {
                return "";
            }

            //getting the file name
            //format: \"n\":\"[FileName]\"
            String[] strar = ret.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
            ret = strar[1].Substring(5, strar[1].Length - 6);

            return ret;
        }

    }
}