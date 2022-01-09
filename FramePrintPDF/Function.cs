using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FramePrintPDF
{
    public class LambdaRequest
    {
        [JsonProperty(PropertyName = "body")]
        public string body { get; set; }
    }

    public class LambdaResponse
    {
        [JsonProperty(PropertyName = "statusCode")]
        public HttpStatusCode statusCode { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string body { get; set; }
    }

    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public LambdaResponse FunctionHandler(LambdaRequest input, ILambdaContext context)
        {
            // base64���f�R�[�h
            byte[] a = Convert.FromBase64String(input.body);
            String stCsvData = Encoding.UTF8.GetString(a);

            // �J���}��؂�ŕ������Ĕz��Ɋi�[����
            string[] stArrayData = stCsvData.Split(',');

            // byte �z��ɕϊ�����
            byte[] b = new byte[stArrayData.Length];
            for (int i = 0; i < stArrayData.Length; i++)
                b[i] = Convert.ToByte(stArrayData[i]);

            // gzip��
            String line = Unzip(b);

            // �f�[�^�̓ǂݍ���
            var myPrintInput = new PrintInput(line);
            string base64str = myPrintInput.getPdfSource(); 

            // ���ʃf�[�^����������(json�`��) -------------------------------------------------------
            return new LambdaResponse
            {
                statusCode = HttpStatusCode.OK,
                headers = new Dictionary<string, string>() {
                    { "Content-Type", "text/plain" },
                    { "Access-Control-Allow-Origin", "*" }
                },
                body = base64str,
            };
        }

        /// <summary>
        /// ���k�f�[�^�𕶎���Ƃ��ĕ������܂��B
        /// </summary>


        public string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
 


    }
}
