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
    public static class gmarket
    {
        private readonly static string user_agent = "Mozilla/5.0 (Linux; Android 9; SM-G973N Build/PI; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/121.0.6167.180 Mobile Safari/537.36 MobileApp/1.1 (Android; 10.5.7; com.ebay.kr.gmarket; SM-G973N)";

        // 검색결과   JSON      1~
        // 스토어결과 JSON      2~
        // 스토어결과_섹션 JSON      3~
        // 사업자결과 JSON      4~
        // 
        // { "컬럼명", "1~JSON경로" }
        //
        // { "컬럼명", "1~앞에붙일내용|JSON경로|뒤에붙일내용" }
        //
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용" }
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용$찾을내용2,변경내용2" }
        public readonly static JObject columns = new JObject()
        {
            { "상품명", "3~sectionData.itemInfo.goodsName" },
            { "상품 URL", "2~http://item.gmarket.co.kr/Item?goodscode=|data.extraData.order.orderRequest.goodsCode|" },
            { "상품 썸네일 이미지", "3~sectionData.itemInfo.images[0]" },
            { "상품 가격", "3~sectionData.itemInfo.originalPrice" },
            { "상품 판매가", "3~sectionData.itemInfo.price" },
            { "상품 할인금액", "2~data.extraData.discount.discountPrice" },
            { "상품 리뷰 수", "3~sectionData.itemInfo.reviewCount" },
            { "상품 판매 수", "3~sectionData.itemInfo.sellCount" },
            { "상품 평점", "3~sectionData.itemInfo.review.value" },

            { "상점명", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.sellerName" },
            { "상점 URL", "4~http://minishop.gmarket.co.kr/|pageProps.initialStates.sellerInfoPage.minish.shopAlias|" },
            { "사업자 상호명", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.companyName" },
            { "사업자 대표자명", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.manager" },
            { "사업자 대표 메일", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.emailAddress" },
            { "사업장 주소", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.streetAddress" },
            { "사업자 등록 번호", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.displayedCorpNo" },
            { "통신 판매업 번호", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.ecommerceNo" },
            { "대표 전화 번호", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.helpdeskTelNo" },
            { "대표 팩스 번호", "4~pageProps.initialStates.sellerInfoPage.sellerInfo.faxNo" },
        };

        public readonly static List<string> default_columns = new List<string>()
        {
            "상품명",
            "상품 URL",
            "상품 가격",
            "상품 판매가",
            "상품 할인금액",
            "상점명",
            "상점 URL",
            "사업자 상호명",
            "사업자 등록 번호",
            "통신 판매업 번호",
            "사업자 대표자명",
            "사업자 대표 메일",
            "대표 전화 번호",
            "대표 팩스 번호",
            "사업장 주소",
            "상품 평점",
            "상품 리뷰 수",
            "상품 판매 수",
        };

        public readonly static string[] sort_string = new string[]
        {
            "s=7", // G마켓 랭크 순
            "s=8", // 판매 인기 순
            "s=1", // 낮은 가격 순
            "s=2", // 높은 가격 순
            "s=13", // 상품평 많은 순
            "s=3", // 신규 상품 순
        };


        public static JArray LoadCategory()
        {
            JArray main_categorys = new JArray();

            string result = curl_get_category();

            JObject json_result = JObject.Parse(result);

            // 대분류 파싱
            foreach (JToken cate in json_result["data"]["cppCategoryInfos"])
            {
                // 중분류 파싱
                JArray sub_categorys = new JArray();
                foreach (JToken sub_cate in cate["categoryTabs"])
                {
                    if (string.IsNullOrEmpty(sub_cate["categoryCode"].ToString())) continue;

                    // 소분류 파싱
                    JArray min_categorys = new JArray();
                    foreach (JToken min_cate in sub_cate["categoryElements"])
                    {
                        if (string.IsNullOrEmpty(min_cate["categoryCode"].ToString())) continue;

                        min_categorys.Add(new JObject()
                        {
                            { "id", min_cate["categoryCode"].ToString() },
                            { "name", min_cate["name"].ToString() },
                        });
                    }

                    sub_categorys.Add(new JObject()
                    {
                        { "id", sub_cate["categoryCode"].ToString() },
                        { "name", sub_cate["name"].ToString() },
                        { "min_category", min_categorys },
                    });
                }

                main_categorys.Add(new JObject()
                {
                    { "id", cate["seq"].ToString() },
                    { "name", cate["name"].ToString() },
                    { "sub_category", sub_categorys },
                });
            }

            return main_categorys;
        }

        private static string curl_get_category()
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://elsa-fe.gmarket.co.kr/api/gnb-search");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = "https://m.gmarket.co.kr/";
            req.Accept = "*/*";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

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

        public static string curl_get_token()
        {
            string result = "";

            byte[] data = Encoding.ASCII.GetBytes("");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://spacegate.gmarket.co.kr/hermes/auth/v1/na/token/create-token");
            req.Method = WebRequestMethods.Http.Post;
            req.Headers["Apikey"] = "7fa9de183ab00f5db0520737a27e61e5";
            curl_default_header(ref req);
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

                JObject json = JObject.Parse(result);
                result = json["data"]["accessToken"].ToString();
            }

            return result;
        }

        public static string curl_get_products(string access_token, string keyword, int page = 0, int sortIndex = 0)
        {
            string result = "";

            string postdata = "{" +
                "\"requestId\":\"" + Guid.NewGuid().ToString().Replace("-", "") + "\"," +
                "\"rawUrl\":\"api://search?keyword=" + System.Web.HttpUtility.UrlEncode(keyword) + "&p=" + page.ToString() + "&" + sort_string[sortIndex] + "\"," +
                "\"adId\":\"\"," +
                "\"adOptOut\":\"0\"," +
                "\"clientSideImpressionLog\":false," +
                "\"debugMode\":false" +
            "}";
            
            byte[] data = Encoding.ASCII.GetBytes(postdata);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://stargate.gmarket.co.kr/hermes/facade/v1/snowwhite/browse");
            req.Method = WebRequestMethods.Http.Post;
            req.Headers["Authorization"] = $"Bearer {access_token}";
            req.Headers["Apikey"] = "610e2d2071015ec7ff27371e7feb6368";
            curl_default_header(ref req);
            req.ContentType = "application/json; charset=UTF-8";
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

        public static string curl_get_category_products(string access_token, string category_id, int page = 1, int sortIndex = 0)
        {
            string result = "";

            string postdata = "{" +
                "\"requestId\":\"" + Guid.NewGuid().ToString().Replace("-", "") + "\"," +
                "\"rawUrl\":\"api://list?category=" + category_id + "&p=" + page.ToString() + "&" + sort_string[sortIndex] + "\"," +
                "\"adId\":\"\"," +
                "\"adOptOut\":\"0\"," +
                "\"clientSideImpressionLog\":false," +
                "\"debugMode\":false" +
            "}";

            byte[] data = Encoding.ASCII.GetBytes(postdata);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://stargate.gmarket.co.kr/hermes/facade/v1/snowwhite/browse");
            req.Method = WebRequestMethods.Http.Post;
            req.Headers["Authorization"] = $"Bearer {access_token}";
            req.Headers["Apikey"] = "610e2d2071015ec7ff27371e7feb6368";
            curl_default_header(ref req);
            req.ContentType = "application/json; charset=UTF-8";
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

        public static string curl_get_store(string access_token, string goodsCode)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://stargate.gmarket.co.kr/hermes/facade/v1/item/get-item-detail-view/{goodsCode}");
            req.Method = WebRequestMethods.Http.Get;
            req.Headers["Authorization"] = $"Bearer {access_token}";
            req.Headers["Apikey"] = "610e2d2071015ec7ff27371e7feb6368";
            curl_default_header(ref req);

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

        public static string curl_get_biz(string shopId)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://m.gmarket.co.kr/n/minishop/_next/data/I4TG1nfSuF-5o2sz1tobo/n/minishop/{shopId}/sellerinfo.json?shopId={shopId}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = $"https://m.gmarket.co.kr/n/minishop/{shopId}";
            req.Accept = "*/*";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

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

        private static void curl_default_header(ref HttpWebRequest req)
        {
            req.Headers["Hermes-Apptype"] = "C";
            req.Headers["Hermes-Appversion"] = "10.5.7";
            req.Headers["Hermes-Ostype"] = "A";
            req.Headers["Hermes-Duid"] = "6481ebb5-4028-42ea-834c-9be62eaaff43";
            req.Headers["Hermes-Cguid"] = "11714444495797000032000000";
            req.Headers["Hermes-Pguid"] = "21714444495797000032020000";
            req.Headers["Hermes-Sguid"] = "31714444495797000032510000";
            req.Headers["Hermes-Osversion"] = "28";
            req.Headers["Hermes-Gp"] = "adtid=;adoptout=;jaehuid=200003602";
            req.UserAgent = "okhttp/3.14.9";
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
    }
}
