using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NBtcPayClient.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CanCreateInstanceOfClient()
        {
            new BtcPayClient("http://localhost:14142");
            new BtcPayClient("http://gozo.com");
        }
        [Theory]
        [InlineData("@%%^&(")]
        [InlineData("http://")]
        [InlineData("ftp://dsddwq")]
        [InlineData("://dsddwq")]
        [InlineData(":<>//dsddddwq")]
        public void ThrowsOnInvalidUrl(string url)
        {
            Assert.ThrowsAny<Exception>(() => new BtcPayClient(url));
        }

        [Fact]
        public async Task AuthenticateViaPasswordFlow_CallsCorrectUrl()
        {
            var httpClient = new MockHttpClient();
            httpClient.BaseAddress = new Uri("http://notarealhost:14142");
           
            var callCount = 0;
            httpClient.SendAsyncCalled += (sender, args) =>
            {
                var typedArgs = args as MockHttpClient.SendAsyncCalledEventArgs;
                Assert.Contains("connect/token", typedArgs.Message.RequestUri.ToString());
                callCount++;
            };
            var client = new BtcPayClient(httpClient);
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.AuthenticateViaPasswordFlow("user", "password"));
            Assert.Equal(1, callCount);
        }
    }

    public class MockHttpClient : HttpClient
    {
        public event EventHandler SendAsyncCalled;

        
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var handler = SendAsyncCalled;
            handler?.Invoke(this, new SendAsyncCalledEventArgs()
            {
                Message = request
            });
            return base.SendAsync(request, cancellationToken);
        }
        
        
        public class SendAsyncCalledEventArgs : EventArgs
        {
            public HttpRequestMessage Message { get; set; }
        }
    }
}