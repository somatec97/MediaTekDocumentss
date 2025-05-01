using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe indépendante d'accès à une api rest avec éventuellement une "basic authorization"
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// unique instance de la classe
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion à l'api
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentification</param>
        private ApiRest(String uriApi, String authenticationString="")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            // prise en compte dans l'url de l'authentificaiton (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentificatio (login:pwd)</param>
        /// <returns></returns>
        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {
            if(instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        /// <summary>
        /// Envoi une demande à l'API et récupère la réponse
        /// </summary>
        /// <typeparam name="T">type d'une des classes du model</typeparam>
        /// <param name="methode">verbe http (GET, POST, PUT, DELETE)</param>
        /// <param name="message">message à envoyer dans l'URL</param>
        /// <returns>liste d'objets (select) ou liste vide (ok) ou null si erreur</returns>
        //public JObject RecupDistant(string methode, string message)
        //{

        //    // envoi du message et attente de la réponse
        //    switch (methode)
        //    {
        //        case "GET":
        //            httpResponse = httpClient.GetAsync(message).Result;
        //            break;
        //        case "POST":
        //            httpResponse = httpClient.PostAsync(message, null).Result;
        //            break;
        //        case "PUT":
        //            httpResponse = httpClient.PutAsync(message, null).Result;
        //            break;
        //        case "DELETE":
        //            httpResponse = httpClient.DeleteAsync(message).Result;
        //            break;
        //        // methode incorrecte
        //        default:
        //            return new JObject();
        //    }
        //    Console.WriteLine("Contenu envoyé : " );

        //    // récupération de l'information retournée par l'api
        //    return httpResponse.Content.ReadAsAsync<JObject>().Result;
        //}

        //public JObject RecupDistant(string methode, string message)
        //{
        //    // Création d’un body vide mais valide
        //    HttpContent content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

        //    switch (methode)
        //    {
        //        case "GET":
        //            httpResponse = httpClient.GetAsync(message).Result;
        //            break;
        //        case "POST":
        //            httpResponse = httpClient.PostAsync(message, content).Result;
        //            break;
        //        case "PUT":
        //            httpResponse = httpClient.PutAsync(message, content).Result;
        //            break;
        //        case "DELETE":
        //            httpResponse = httpClient.DeleteAsync(message).Result;
        //            break;
        //        default:
        //            return new JObject();
        //    }

        //    Console.WriteLine("Contenu envoyé : " + message);

        //    // Lecture du contenu brut
        //    string responseText = httpResponse.Content.ReadAsStringAsync().Result;
        //    Console.WriteLine("Réponse brute reçue : " + responseText);

        //    // Nettoyage : on garde seulement la partie JSON à partir du premier '{'
        //    int index = responseText.IndexOf('{');
        //    if (index >= 0)
        //    {
        //        string jsonClean = responseText.Substring(index);
        //        return JObject.Parse(jsonClean);  // Parsing sans erreur
        //    }
        //    else
        //    {
        //        Console.WriteLine("Réponse invalide : pas de JSON détecté.");
        //        return new JObject();
        //    }
        //}
        public JObject RecupDistant(string methode, string message)
        {
            // Création d’un body vide mais valide
            HttpContent content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    httpResponse = httpClient.DeleteAsync(message).Result;
                    break;
                default:
                    return new JObject();
            }

            Console.WriteLine("Contenu envoyé : " + message);

            // ✅ Lecture du contenu brut
            string responseText = httpResponse.Content.ReadAsStringAsync().Result;
            Console.WriteLine("Réponse brute reçue : " + responseText);

            // ✅ Nettoyage pour extraire uniquement le JSON propre
            // Chercher l'indice du premier '{' qui marque le début du JSON
            int index = responseText.IndexOf('{');
            if (index >= 0)
            {
                string jsonClean = responseText.Substring(index);  // Extrait le JSON valide
                Console.WriteLine("JSON nettoyé : " + jsonClean);

                // Parse le JSON propre
                return JObject.Parse(jsonClean);
            }
            else
            {
                Console.WriteLine("Réponse invalide : pas de JSON détecté.");
                return new JObject();
            }
        }







    }
}
