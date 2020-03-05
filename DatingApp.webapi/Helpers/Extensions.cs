using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.webapi.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError( this HttpResponse httpResp, string message)
        {
            httpResp.Headers.Add("Application-Error", message);
            httpResp.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            httpResp.Headers.Add("Access-Control-Allow-Origin", "*");
        }   

        public static void AddPaginationHeader(this HttpResponse httpResponse, PaginationHeaders paginationHeader)  
        {
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

            httpResponse.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));            
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static int CalculateAge( this DateTime theDateTime)   
        {
            var age = DateTime.Today.Year - theDateTime.Year;

            if(DateTime.Now > theDateTime)
                age--;

            return age;
        }

        public static void AddCreatedAtLocation(this HttpResponse httpResponse)
        {
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Location");
        }
    }
}