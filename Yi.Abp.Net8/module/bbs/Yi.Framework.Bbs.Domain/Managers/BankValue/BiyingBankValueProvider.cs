using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;

namespace Yi.Framework.Bbs.Domain.Managers.BankValue
{
    [Dependency(TryRegister = true)]
    public class BiyingBankValueProvider : IBankValueProvider, ITransientDependency
    {
        //官网地址：www.biyingapi.com
        private const string Url = "https://api.biyingapi.com/hsrl/ssjy/600519/5579aa4b391945678";

        public decimal StandardValue => 1700;


        public async Task<decimal> GetValueAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var reponse = await client.GetAsync(Url);
                    reponse.EnsureSuccessStatusCode();
                    var dataStr = await reponse.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(dataStr);
                    return jsonObject["p"].Value<decimal>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BiyingBank获取数据异常", ex);

            }

        }
    }
}
