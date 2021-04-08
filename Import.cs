using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Json.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Threading;
using RestSharp;

namespace JsonParserGTIN
{
    /// <summary>
    /// Коллекция для получения данных с помощью RestAPI
    /// </summary>
    public static class Import
    {
       /// <summary>
       /// Метод, который считывает данные с файла
       /// </summary>
       /// <param name="path">Путь к файлу</param>
       /// <returns></returns>
       public static List<string> getGtins(string path)
        {
            try
            {
                List<string> gtins = new List<string>();
                string text = File.ReadAllText(path);
                string[] temp = text.Split('\n');
                for (int i = 0; i < temp.Length; i++)
                {
                    //Подведение всех GTIN под единый формат
                    if (temp[i].Any(e => (e == '\r') || (e == '\n')))
                    {
                        temp[i] = temp[i].Remove(temp[i].Length - 1, 1);
                    }
                    while (temp[i].Length < 14)
                    {
                        temp[i] = temp[i].Insert(0, "0");
                    }
                }
                gtins = temp.ToList();
                return gtins;
            }
            catch
            {
                MessageBox.Show("Выбран некорректный файл");
                return null;
            }
        }

        
        /// <summary>
        /// Получение токена прода
        /// </summary>
        /// <returns></returns>
        public static string getTokenProd()
        {
            try
            {
                string login = File.ReadAllText(@"tokenProd\login.txt");
                string password = File.ReadAllText(@"tokenProd\password.txt");
                string host = File.ReadAllText(@"tokenNew\host.txt");
                var client = new RestClient($"{host}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", "Basic Y3JwdC1zZXJ2aWNlOnNlY3JldA==");
                request.AddParameter("grant_type", "password");
                request.AddParameter("username", $"{login}");
                request.AddParameter("password", $"{password}");
                IRestResponse response = client.Execute(request);
                string token = JObject.Parse(response.Content)["access_token"].ToObject<string>();
                return token;
            }
            catch
            {
                MessageBox.Show("Не удалось получить токен Prod");
                return null;
            }
        }
        /// <summary>
        /// Получение токена демо
        /// </summary>
        /// <returns></returns>
        public static string getTokenNew()
        {
            try
            {
                string login = File.ReadAllText(@"tokenNew\login.txt");
                string password = File.ReadAllText(@"tokenNew\password.txt");
                string host = File.ReadAllText(@"tokenNew\host.txt");
                var client = new RestClient($"{host}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "JSESSIONID=782D3598E0618D506DF515BF2BF93B16");
                request.AddParameter("application/json", "{\"login\":\""+ $"{login}" + "\",\"password\":\""+$"{password}"+"\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                string token = JObject.Parse(response.Content)["token"].ToObject<string>();
                return token;
            }
            catch
            {
                MessageBox.Show("Не удалось получить токен Demo");
                return null;
            }
            
        }
        /// <summary>
        /// Получение токена препрода
        /// </summary>
        /// <returns></returns>
        public static string getTokenPreProd()
        {
            string token = File.ReadAllText(@"tokenPreprod\token.txt");
            return null;
        }

        /// <summary>
        /// Отправка API запроса на сервер
        /// </summary>
        /// <param name="token">Токен</param>
        /// <param name="body">Тело запроса</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="host">url</param>
        /// <returns></returns>
        private static string foiv(string token, string body, int page,string host)
        {
            try
            {
                var client = new RestClient($"{host}?pageSize=10000&page={page}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", $"bearer {token}");
                request.AddHeader("pageSize", "10000");
                request.AddHeader("page", $"{page}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", $"{body}", ParameterType.RequestBody);
                var restResponse = client.Execute(request);
                return restResponse.Content;
            }
            catch
            {
                MessageBox.Show("Не удалось отправить метод foiv");
                return null;
            }
            
        }
        /// <summary>
        /// Создание тела запроса по коллекции GTIN
        /// </summary>
        /// <param name="gtins">Коллекция GTIN</param>
        /// <returns></returns>
        private static string getBodystring(List<string> gtins)
        {
            try
            {
                //Создание тела запроса
                string bodyString = "[\r\n";
                for (int i = 0; i < gtins.Count - 1; i++)
                {
                    bodyString += '"' + gtins[i] + '"' + ",\r\n";
                }
                bodyString += '"' + gtins[gtins.Count - 1] + '"' + "\r\n]";
                return bodyString;
            }
            catch
            {
                MessageBox.Show("Некорректный файл, неудалось создать body для метода foiv");
                return null;
            }
        }
        /// <summary>
        /// Получение коллекции КМ по API запросу
        /// </summary>
        /// <param name="gtins">Коллекция GTIN</param>
        /// <param name="token">Токен</param>
        /// <param name="host">url</param>
        /// <param name="gtinCountOneOperation">Лимит на кол-во GTIN, обработанных за один запрос</param>
        /// <returns></returns>
        public static List<Kiz> getResponce(List<string> gtins, string token,string host, int gtinCountOneOperation)
        {
            try
            {
                string bodyString = getBodystring(gtins);
                List<Kiz> kizs = new List<Kiz>();
                int countOperation = 0;
                string foivResultCount = "";
                string foivResult;
                List<string> gtinsMini = new List<string>();
                List<List<Kiz>> ListOfKiz = new List<List<Kiz>>();
                //Считывание кол-ва КМ
                try
                {
                    foivResultCount = foiv(token, bodyString, 0, host);
                    countOperation = JObject.Parse(foivResultCount)["count"].ToObject<int>() / 10000;
                    MessageBox.Show($"Кол-во КМ: {JObject.Parse(foivResultCount)["count"].ToObject<int>()}\nКол-во страниц: {countOperation + 1}");
                }
                catch { }
                //Если в коллекции больше GTIN, чем лимит
                if (gtins.Count > gtinCountOneOperation)
                {
                    //Количество GTIN за один запрос
                    int gtinsOneOperationCount = gtinCountOneOperation;
                    //Общее количество GTIN
                    int gtinsCount = gtins.Count;
                    //Количество запросов
                    int iterationCount = gtinsCount / gtinsOneOperationCount;
                    //Остаток GTIN в последнем запросе
                    int Ostatok = gtinsCount % gtinsOneOperationCount;
                    //Проходка по всем данным, кроме остатка
                    for (int i = 0; i < iterationCount; i++)
                    {
                        //Заполнение порции данных
                        for (int j = i * gtinsOneOperationCount; j < (i+1) * gtinsOneOperationCount; j++)
                        {
                            gtinsMini.Add(gtins[j]);
                        }
                        bodyString = getBodystring(gtinsMini);
                        foivResultCount = foiv(token, bodyString, 0, host);
                        //Количество операций, при 10_000 КМ на одной странице
                        countOperation = JObject.Parse(foivResultCount)["count"].ToObject<int>() / 10000;
                        for (int k = 0; k <= countOperation; k++)
                        {
                            try
                            {
                                foivResult = foiv(token, bodyString, k, host);
                                if(foivResult.Contains("Gateway Time-out"))
                                {
                                    MessageBox.Show($"Падение по Gateway Time-out на шаге {i}. {i * gtinsOneOperationCount} GTINS, Страница {k} из {countOperation}");
                                    return null;
                                }
                                else
                                {
                                    kizs.AddRange(new List<Kiz>(JsonSerializetion.Parse(foivResult)));
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Программа не смогла обработать на шаге {i}. {i * gtinsOneOperationCount} GTINS, Страница {k} из {countOperation}");
                                return null;
                            }
                            
                        }
                        //Очистка порции данных
                        gtinsMini.Clear();
                    }
                    //Проходка по данным остатка
                    try
                    {
                        for (int j = iterationCount * gtinsOneOperationCount; j < (iterationCount * gtinsOneOperationCount) + Ostatok; j++)
                        {
                            gtinsMini.Add(gtins[j]);
                        }
                        bodyString = getBodystring(gtinsMini);
                        foivResultCount = foiv(token, bodyString, 0, host);
                        countOperation = JObject.Parse(foivResultCount)["count"].ToObject<int>() / 10000;
                        for (int k = 0; k <= countOperation; k++)
                        {
                            try
                            {
                                foivResult = foiv(token, bodyString, k, host);
                                if (foivResult.Contains("Gateway Time-out"))
                                {
                                    MessageBox.Show($"Падение по Gateway Time-out на шаге Остаток, Страница {k} из {countOperation}");
                                    return null;
                                }
                                else
                                {
                                    kizs.AddRange(new List<Kiz>(JsonSerializetion.Parse(foivResult)));
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Программа не смогла обработать на шаге Остаток");
                                return null;
                            }

                        }
                    }
                    catch
                    {
                        MessageBox.Show($"Программа не смогла обработать на шаге 'Остаток'.");
                        return null;
                    }
                    
                    return kizs;
                }
                else
                {// Меньше лимита
                    for (int k = 0; k <= countOperation; k++)
                    {
                        try
                        {
                            foivResult = foiv(token, bodyString, k, host);
                            if (foivResult.Contains("Gateway Time-out"))
                            {
                                MessageBox.Show($"Падение по Gateway Time-out, Страница {k} из {countOperation}");
                                return null;
                            }
                            else
                            {
                                kizs.AddRange(new List<Kiz>(JsonSerializetion.Parse(foivResult)));
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"Программа не смогла обработать на шаге: страница {k} из {countOperation}");
                            return null;
                        }
                    }
                    return kizs;
                }
            }
            catch
            {
                MessageBox.Show("Некорректный ответ метода foiv");
                return null;
            }
            
        }
        
    }
}
