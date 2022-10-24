using Kuwo_API;
using Kuwo_API.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using Moq;

namespace ApiTest;

[TestClass]
public class UnitTest1
{
    private IHttpClientFactory _factory;
    private IMemoryCache _cache;

    [TestInitialize]
    public void Init()
    {
        var facMock = new Mock<IHttpClientFactory>();
        facMock.Setup(t => t.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient());
        _factory = facMock.Object;

        var cacheMock = new Mock<IMemoryCache>();

        _cache = cacheMock.Object;
    }

    [TestMethod]
    public async Task CookieTest()
    {
        var musicService = new MusicService(_factory, _cache);
        await musicService.GetKuwoToken();
    }

    [TestMethod]
    public async Task UrlTest()
    {
        var controller = new KuwoController(_factory, _cache, new MusicService(_factory, _cache));
        var res = await controller.UrlInfo(152275);
    }
}