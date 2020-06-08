using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataAbstract.Music;
using MuKai_Music.Service;
using Xunit;

namespace Test.Account
{
    public class Account_UnitTest : BaseTest
    {

        /// <summary>
        /// 测试读取Token信息，自动添加到Action参数列表
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AuthorizationTest()
        {
            var lid = "5edb54f26d8b632c60eb1cf1";
            var tokenProvider = ServiceProvider.GetService(typeof(TokenProvider)) as TokenProvider;
            var auth = new AuthenticationHeaderValue("Bearer", tokenProvider.CreateAccessToken("1"));
            Client.DefaultRequestHeaders.Authorization = auth;
            var result = await Client.GetAsync<TestResult<UserPlaylist>>
                ($"/api/playlist/user/detail?id={lid}");
            Assert.Equal(200, result.Code);
        }
    }
}
