using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text;
using System.IO;

namespace WEB_API.IntegrationTest
{
    [TestFixture]
    public class WebEditorTests
    {
        private readonly HttpClient client = new WebApplicationFactory<Startup>().CreateClient();

        [Test]
        public async Task GetFiles_NoParam_CorrectCopyToStorage()
        {           
            HttpResponseMessage response1 = await client.GetAsync("/File/GetFiles");
           
            var multiForm = new MultipartFormDataContent();

            FileStream fs = File.OpenRead("myData.txt");
            
            multiForm.Add(new StreamContent(fs), "File", Path.GetFileName("myData.txt"));
                                      
            HttpResponseMessage response4 = await client.PutAsync("/File/PutFile/myData.txt", multiForm);

            HttpResponseMessage response5 = await client.GetAsync("/File/GetFiles");

            var result = await response1.Content.ReadAsStringAsync();

            var result2 = await response5.Content.ReadAsStringAsync();

            Assert.IsTrue(result2.Contains("myData.txt"));
        }

        [Test]
        public async Task GetFiles_NoParam_CorrectReplaceText()
        {                    
            HttpResponseMessage response1 = await client.GetAsync("/File/GetFile/MyStorage.txt?searchText=Praesent+at+aliquam+ex.");

            HttpResponseMessage response2 = await client.PostAsync("/File/PostFile/MyStorage.txt?searchText=at&replaceText=bt", null);

            HttpResponseMessage response3 = await client.GetAsync("/File/GetFile/MyStorage.txt?searchText=Praesent+at+aliquam+ex.");

            var result1 = await response1.Content.ReadAsStringAsync();

            var result2 = await response3.Content.ReadAsStringAsync();

            Assert.IsFalse(result2.Equals(result1));

            HttpResponseMessage response4 = await client.PostAsync("/File/PostFile/MyStorage.txt?searchText=bt&replaceText=at", null);
        }


    }
}