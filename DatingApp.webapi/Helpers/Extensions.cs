using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.webapi.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError( this HttpResponse httpResp, string message)
        {
            httpResp.Headers.Add("Application-Error", message);
            httpResp.Headers.Add("Access-Control-Expose-Header", "Application-Error");
            httpResp.Headers.Add("Access-Control-Allow-Origin", "*");
        }     

        public static int CalculateAge( this DateTime theDateTime)   
        {
            var age = DateTime.Today.Year - theDateTime.Year;

            if(DateTime.Now > theDateTime)
                age--;

            return age;
        }
    }
}