using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using Newtonsoft.Json;

namespace tut_table_paging.Pages
{
    public class PostModel
    {
        public int Id { get; set; } 

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
    
    public class IndexController : Controller
    {
        // ist statisch, da Controller-Instanzen nach einem Aufruf verloren gehen (stateless web)
        private static IEnumerable<PostModel> data;

        // liefert eine View ohne Daten
        [Route("")]
        [HttpGet]
        public ViewResult Get()
        {
            return View("Index.View");
        }

        [HttpGet]
        [Route("data")]
        async public Task<JsonResult> GetData(Int32 skip, Int32 take)
        {
            // keine Daten am Start? Für's 1. Mal welche holen
            if (data == null) 
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");

                if (response.IsSuccessStatusCode)
                {
                    string rawJson = await response.Content.ReadAsStringAsync();

                    PostModel[] parsedJson = JsonConvert.DeserializeObject<PostModel[]>(rawJson);

                    data = new List<PostModel>(parsedJson);
                }
                else
                {
                    // todo: error-page anzeigen oder so
                    throw new Exception("something got wrong here");
                }
            }

            var returnData = data.Skip(skip).Take(take).ToList();

            return new JsonResult(returnData);
        }
    }
}
