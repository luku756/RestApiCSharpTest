using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace RestApiCSharpTest
{
    class Program
    {

        //static string targetURL = "http://data.ex.co.kr/openapi/safeDriving/forecast?key=test&type=json";
        static string targetURL = "http://localhost:8000";

        static void Main(string[] args)
        {
            string problem = "0", user = "tane", count = "2";
            var uri = targetURL + "/start/" + user + "/" + problem + '/' + count;
            Console.WriteLine(uri);
            var url = "http://localhost:8000/start/tester/0/2";
           string res = POST(uri);
            Console.WriteLine(res);

            //1.WebClient 클래스 활용
           //string webClientResult = callWebClient();

            var r = JObject.Parse(res);
            Console.WriteLine(r["token"]);

            res = oncalls(targetURL, r["token"].ToString());
            var a = JObject.Parse(res);
            Console.WriteLine(a);
            Console.WriteLine(a["timestamp"]);

            var command = new JObject();
            var order = new JArray();
            var ev1 = new JObject();
            ev1.Add("elevator_id",0);
            ev1.Add("command", "UP");
            var ev2 = new JObject();
            ev2.Add("elevator_id", 1);
            ev2.Add("command", "STOP");
            order.Add(ev1);
            order.Add(ev2);
            command.Add("commands",order);
            res = actions(targetURL, r["token"].ToString(), command.ToString(Formatting.None));
            Console.WriteLine(res);

            //res = oncalls(targetURL, r["token"].ToString());
            //a = JObject.Parse(res);
            //Console.WriteLine(a["timestamp"]);

            //var list = r["residents"];

            //Console.WriteLine(r);
            //foreach (var o in list)
            //{
            //    Console.WriteLine(string.Format("{0} : {1}", "날짜", o));
            //   //Console.WriteLine(string.Format("{0} : {1}", "전국교통량", o[1]));
            //   //Console.WriteLine(string.Format("{0} : {1}", "지방교통량", o["cjibangDir"]));
            //   //Console.WriteLine(string.Format("{0} : {1}", "서울->대전 소요시간", o["csudj"]));
            //   //Console.WriteLine(string.Format("{0} : {1}", "서울->대구 소요시간", o["csudg"]));
            //   //Console.WriteLine(string.Format("{0} : {1}", "서울->울산 소요시간", o["csuus"]));
            //}

            //Console.WriteLine("*************************************************************");

            //string wbRequestResult = callWebRequest();

            //var r2 = JObject.Parse(webClientResult);

            //var list2 = r["list"];

            //Console.WriteLine(r2);
            //foreach (var o in list2)
            //{
            //    Console.WriteLine(string.Format("{0} : {1}", "날짜", o["sdate"]));
            //    Console.WriteLine(string.Format("{0} : {1}", "전국교통량", o["cjunkook"]));
            //    Console.WriteLine(string.Format("{0} : {1}", "지방교통량", o["cjibangDir"]));
            //    Console.WriteLine(string.Format("{0} : {1}", "대전->서울 버스 소요시간", o["cdjsu_bus"]));
            //    Console.WriteLine(string.Format("{0} : {1}", "대구->서울 버스 소요시간", o["cdgsu_bus"]));
            //    Console.WriteLine(string.Format("{0} : {1}", "울산->서울 버스 소요시간", o["cussu_bus"]));
            //}


        }

        public static string POST(string url)
        {
            string responseFromServer = string.Empty;

            try
            {

                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return responseFromServer;

        }


        public static string oncalls(string uri, string token)
        {
            string url = uri + "/oncalls";
            Console.WriteLine(url);
            string responseFromServer = string.Empty;

            try
            {

                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("X-Auth-Token", token);

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return responseFromServer;

        }

        public static string actions(string uri, string token, string command)
        {
            string url = uri + "/action";
            
            string responseFromServer = string.Empty;

            Console.WriteLine(command);
            Console.WriteLine(url);
            Console.WriteLine(command);

            try
            {

                byte[] byteArray = Encoding.UTF8.GetBytes(command);

                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("X-Auth-Token", token);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                Stream dataStreams = request.GetRequestStream();

                dataStreams.Write(byteArray, 0, byteArray.Length);

                dataStreams.Close();

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return responseFromServer;

        }


        public static string callWebClient()
        {
            string result = string.Empty;
            try
            {
                WebClient client = new WebClient();

                //특정 요청 헤더값을 추가해준다. 
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                using (Stream data = client.OpenRead(targetURL))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        string s = reader.ReadToEnd();
                        result = s;

                        reader.Close();
                        data.Close();
                    }
                }

            }
            catch (Exception e)
            {
                //통신 실패시 처리로직
                Console.WriteLine(e.ToString());
            }
            return result;
        }


        public static string callWebRequest()
        {
            string responseFromServer = string.Empty;

            try
            {

                WebRequest request = WebRequest.Create(targetURL);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (WebResponse response = request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return responseFromServer;
        }

    }
}

