using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mm_db_market.helper
{
    public static class auction

    {
        private readonly static string app_version = "8.5.51";
        private readonly static string app_info = $"Android; version={app_version}";
        private readonly static string user_agent = $"MobileApp/1.0 (Android; {app_version}; com.ebay.kr.auction)";
        // 옥션 Auth 값 - 고정
        private readonly static string auction_auth = "Android IQBnAG0AeQBIAEQAeQBwADMAdQBnAHEATgBsAGYAcQAyAEgALwAxAEsATwA1AEUAYQAxAHAAZgBNAGQAZgA2ACsAbgBKAE8AWQBiAFEAVQBqAHIAUgBtAEoAVAB0AEcAVgBCAFUAMwA2AG4ANABUAHAASQBYADMAaABnAHkATABrAFoAVQBhAGUASABLAHkAeQBlAGEASgBLAGUAYQBIAGUAMwBnAE8AQgBnAHkAegA5AFkAUgBVAGEAdgBoADUAdQBGADUAaABtAFoAWQBDAGYAcwBKAGsAMgA3ADcAcABhAFkANwBiAEEAegBxAHQAbgB1AG0ANQB2AGQATABkAEkAMgArADMASQA3AG8AMgBBAGEAeABzAEEARABQAFoAagBvAGIAWQA0AHgAZwA9AD0A";

        // 검색결과   JSON      1~
        // 스토어결과 JSON      2~
        // 
        // { "컬럼명", "1~JSON경로" }
        //
        // { "컬럼명", "1~앞에붙일내용|JSON경로|뒤에붙일내용" }
        //
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용" }
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용$찾을내용2,변경내용2" }
        public readonly static JObject columns = new JObject()
        {
            { "상점명", "1~viewModel.seller.text" },
            { "상점 URL", "1~viewModel.seller.link" },
            { "상품명", "1~viewModel.item.text" },
            { "상품 URL", "1~viewModel.item.link" },
            { "상품 가격", "1~viewModel.price.binPrice" },
            { "상품 할인 판매가", "1~viewModel.price.couponDiscountedBinPrice" },
            { "상품 할인율", "1~viewModel.price.discountRate" },
            { "상품 할인율 (%)", "1~|viewModel.price.discountRate|%" },
            { "상품 썸네일 이미지", "1~viewModel.itemImageHQ" },
            { "상품 만족도", "1~viewModel.score.satisfactionRate" },
            { "상품 평점", "1~viewModel.score.itemScore" },
            { "상품 후기 수", "1~viewModel.score.feedbackCount.text" },
            { "상품 구매 수", "1~viewModel.score.payCount.text" },

            { "사업자 상호명", "2~compName" },
            { "사업자 대표자명", "2~personName" },
            { "사업자 주소", "2~address" },
            { "사업자 등록 번호", "2~bizNum" },
            { "통신 판매업 번호", "2~ecommReportNum" },
            { "대표 이메일", "2~email" },
            { "대표 전화 번호(-포함)", "2~phone" },
        };

        public readonly static List<string> default_columns = new List<string>()
        {
            "상점명",
            "상점 URL",
            "상품명",
            "상품 URL",
            "상품 가격",
            "사업자 상호명",
            "사업자 등록 번호",
            "통신 판매업 번호",
            "사업자 대표자명",
            "사업자 주소",
            "대표 이메일",
            "대표 전화 번호(-포함)",
            "상품 만족도",
            "상품 후기 수",
            "상품 구매 수",
        };

        public static JArray LoadCategory()
        {
            JArray main_categorys = new JArray();

            string result = curl_get_category();

            JObject json_result = JObject.Parse(result);

            // 대분류 파싱
            foreach (JToken cate in json_result["Data"]["MobileAuctionCategoryService"])
            {
                string sub_result = curl_get_child_category(cate["Id"].ToString());

                JObject sub_json_result = JObject.Parse(sub_result);

                // 중분류 파싱
                JArray sub_categorys = new JArray();
                foreach (JToken sub_cate in sub_json_result["Data"])
                {
                    string min_result = curl_get_child_category(sub_cate["Id"].ToString());

                    JObject min_json_result = JObject.Parse(min_result);

                    // 소분류 파싱
                    JArray min_categorys = new JArray();
                    foreach (JToken min_cate in min_json_result["Data"])
                    {
                        min_categorys.Add(new JObject()
                        {
                            { "id", min_cate["Id"].ToString() },
                            { "name", min_cate["Name"].ToString() },
                        });
                    }

                    sub_categorys.Add(new JObject()
                    {
                        { "id", sub_cate["Id"].ToString() },
                        { "name", sub_cate["Name"].ToString() },
                        { "min_category", min_categorys },
                    });
                }

                main_categorys.Add(new JObject()
                {
                    { "id", cate["Id"].ToString() },
                    { "name", cate["Name"].ToString() },
                    { "sub_category", sub_categorys },
                });
            }

            return main_categorys;
        }

        private static string curl_get_category()
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://imapi.auction.co.kr/api/GNB/GetCategoryGroupAndCornersListV2");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Authorization"] = auction_auth;
            req.Headers["App-Info"] = app_version;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        private static string curl_get_child_category(string parent_category_no)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://imapi.auction.co.kr/api/Category/GetCategoryList?id={parent_category_no}&type=normal&sort=0");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Authorization"] = auction_auth;
            req.Headers["App-Info"] = app_version;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        public static string curl_get_products(string keyword, int page = 0)
        {
            string result = "";

            string rawUrl = $"api://search?keyword={System.Web.HttpUtility.UrlEncode(keyword)}";
            if (page >= 1)
                rawUrl += $"&p={page}";

            string post_data = "" +
               "{" +
                   "\"adOptOut\":\"0\"," +
                   "\"adtId\":\"" + Guid.NewGuid().ToString() + "\"," +
                   "\"clientSideImpressionLog\":false," +
                   "\"debugMode\":false," +
                   "\"rawUrl\":\"" + rawUrl + "\"," +
                   "\"requestId\":\"" + Guid.NewGuid().ToString().Replace("-", "") + "\"" +
               "}";

            byte[] data = Encoding.UTF8.GetBytes(post_data);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://imapi.auction.co.kr/api/SnowWhite/Browse");
            req.Method = WebRequestMethods.Http.Post;
            req.UserAgent = user_agent;
            req.ContentType = "application/json; charset=UTF-8";
            req.Headers["Authorization"] = auction_auth;
            req.Headers["App-Info"] = app_version;
            req.ContentLength = data.Length;

            Stream requestStream = req.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        public static string curl_get_category_products(string category_id, int page = 0)
        {
            string result = "";

            string rawUrl = $"api://list?category={category_id}";
            if (page >= 1)
                rawUrl += $"&p={page}";

            string post_data = "" +
               "{" +
                   "\"adOptOut\":\"0\"," +
                   "\"adtId\":\"" + Guid.NewGuid().ToString() + "\"," +
                   "\"clientSideImpressionLog\":false," +
                   "\"debugMode\":false," +
                   "\"rawUrl\":\"" + rawUrl + "\"," +
                   "\"requestId\":\"" + Guid.NewGuid().ToString().Replace("-", "") + "\"" +
               "}";

            byte[] data = Encoding.UTF8.GetBytes(post_data);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://imapi.auction.co.kr/api/SnowWhite/Browse");
            req.Method = WebRequestMethods.Http.Post;
            req.UserAgent = user_agent;
            req.ContentType = "application/json; charset=UTF-8";
            req.Headers["Authorization"] = auction_auth;
            req.Headers["App-Info"] = app_version;
            req.ContentLength = data.Length;

            Stream requestStream = req.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        public static string curl_get_store(string itemNo, string sellerID)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://imapi.auction.co.kr/api/api/ItemV2/GetItemStandardReturnExchangeInfo?itemNo={itemNo}&sellerID={sellerID}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Authorization"] = auction_auth;
            req.Headers["App-Info"] = app_version;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }
    }
}
