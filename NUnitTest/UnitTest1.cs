using Data;
using Newtonsoft.Json;
using NUnit.Framework;
using Repositories;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NUnitTest
{
    public class Tests
    {
        string _baseUrl = "http://localhost:51404";
        HttpClient client;
        State _state;
      

        [SetUp]
        public void Setup()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
        }

        [Test]
        public async Task Get_ShouldReturn_OKAsync()
        {
            //Arrange
            var url = "statemanager";
            //Act
            var response = await client.GetAsync(url);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task ShouldReturn_NotSameAsync()
        {
            //Arrange
            var url = "statemanager";
            //Act
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Stopwatch delay = new Stopwatch();
            delay.Start();
            
            if (delay.ElapsedMilliseconds == 10000) 
            {
                delay.Stop();
                var response2 = await client.GetAsync(url);
                var content2 = await response2.Content.ReadAsStringAsync();
            //Assert
            Assert.AreNotEqual(content, content2);
            }          
        }

        [Test]
        public async Task ShouldReturn_DifferentPlanesAsync()
        {
            //Arrange
            var url = "statemanager";
            //Act
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _state = JsonConvert.DeserializeObject<State>(content);
            Stopwatch delay2 = new Stopwatch();
            delay2.Start();

            if (delay2.ElapsedMilliseconds == 12000)
            {
                delay2.Stop();
                var response2 = await client.GetAsync(url);
                var content2 = await response2.Content.ReadAsStringAsync();
                State state2 = JsonConvert.DeserializeObject<State>(content2);
                //Assert
                Assert.AreNotEqual(_state.Planes, state2.Planes);
            }
        }
    }
}