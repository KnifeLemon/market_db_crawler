using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mm_db_market.helper
{
    public static class coupang
    {
        private readonly static string user_agent = "Dalvik/2.1.0 (Linux; U; Android 7.1.2; SM-G975N Build/N2G48H)";
        private readonly static string web_user_agent = "Mozilla/5.0 (Linux; Android 7.1.2; SM-G975N Build/N2G48H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.102 Mobile Safari/537.36 Edg/104.0.100.0";

        // 검색결과 JSON =      1~
        // 판매자결과 JSON      2~
        //
        // { "컬럼명", "1~JSON경로" }
        //
        // { "컬럼명", "1~앞에붙일내용|1~JSON경로|뒤에붙일내용" }
        //
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용" }
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용$찾을내용2,변경내용2" }
        public readonly static JObject columns = new JObject()
        {
            { "상품명", "1~entity.displayItem.title" },
            { "상품 URL", "1~https://www.coupang.com/vp/products/|entity.displayItem.id|" },
            { "상품 가격", "1~entity.displayItem.originalPrice[0]" },
            { "상품 할인 판매가", "1~entity.displayItem.salesPrice[0]" },
            { "상품 최저 판매가", "1~entity.displayItem.lowestPrice[0]" },
            { "상품 할인율 (%)", "1~entity.displayItem.discountRate" },
            { "상품 썸네일 이미지", "1~entity.displayItem.thumbnailSquare" },
            { "상품 리뷰 수", "1~entity.displayItem.ratingCount$(,$)," },
            { "상품 평점", "1~entity.displayItem.ratingAverage" },

            { "사업자 상호명", "2~returnPolicyVo.sellerDetailInfo.vendorName" },
            { "사업자 대표자명", "2~returnPolicyVo.sellerDetailInfo.repPersonName" },
            { "사업자 주소", "2~returnPolicyVo.sellerDetailInfo.repAddress" },
            { "사업자 등록 번호", "2~returnPolicyVo.sellerDetailInfo.bizNum" },
            { "통신 판매업 번호", "2~returnPolicyVo.sellerDetailInfo.ecommReportNum" },
            { "대표 이메일", "2~returnPolicyVo.sellerDetailInfo.repEmail" },
            { "대표 전화 번호(-포함)", "2~returnPolicyVo.sellerDetailInfo.repPhoneNum" },
        };

        public readonly static List<string> default_columns = new List<string>()
        {
            "상품명",
            "상품 URL",
            "상품 가격",
            "상품 할인 판매가",
            "상품 할인율 (%)",
            "사업자 상호명",
            "사업자 대표자명",
            "사업자 주소",
            "사업자 등록 번호",
            "통신 판매업 번호",
            "대표 이메일",
            "대표 전화 번호(-포함)",
            "상품 리뷰 수",
            "상품 평점",
        };
        
        public readonly static string[] sort_string = new string[]
        {
            "STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 쿠팡 랭킹순
            "SORT_KEY:LOW_PRICE|STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 낮은 가격순
            "SORT_KEY:BEST_SELLING|STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 판매량순
            "SORT_KEY:HIGHEST_REVIEW_COUNT|STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 리뷰많은순
            "SORT_KEY:HIGH_PRICE|STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 높은 가격순
            "SORT_KEY:LATEST|STATIC_SERVICE:ROCKET_WOW_DELIVERY,OVERSEA_DELIVERY,TOP_BRAND,FREE_DELIVERY@SEARCH", // 최신순
        };

        public static JArray LoadCategory()
        {
            JArray main_categorys = new JArray();

            string result = curl_get_category();

            JObject json_result = JObject.Parse(result);

            // 메인 카테고리 파싱
            foreach (JToken cate in json_result["rData"]["categoryList"])
            {
                if (cate["id"].ToString().All(char.IsNumber) == false) continue; // 숫자 체크
                if (cate["name"].ToString() == "서비스") continue;

                // 서브 카테고리 파싱
                JArray sub_categorys = new JArray();
                foreach (JToken sub_cate in cate["children"])
                {
                    if (sub_cate["id"].ToString().All(char.IsNumber) == false) continue; // 숫자 체크
                    if (sub_cate["id"].ToString() == "0") // 와우회원할인
                    {
                        continue;
                    }

                    sub_categorys.Add(new JObject()
                    {
                        { "id", sub_cate["id"].ToString() },
                        { "name", sub_cate["name"].ToString() },
                    });
                }

                main_categorys.Add(new JObject()
                {
                    { "id", cate["id"].ToString() },
                    { "name", cate["name"].ToString() },
                    { "sub_category", sub_categorys },
                });
            }

            return main_categorys;
        }

        private static string curl_get_category()
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://cmapi.coupang.com/v3/categories/home");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Coupang-App"] = "COUPANG|Android|7.1.2|7.2.7|";
            req.Headers["X-Coupang-Target-Market"] = "KR";
            req.Headers["X-Coupang-Origin-Region"] = "KR";
            req.Headers["X-Coupang-Accept-Language"] = "ko-KR";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR");

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

        public static string curl_get_products(string keyword, string nextPageKey = "", int filterIndex = 0)
        {
            string result = "";

            string url = $"https://cmapi.coupang.com/v3/products?preventingRedirection=false";

            if (!string.IsNullOrEmpty(nextPageKey))
                url += $"&nextPageKey={System.Web.HttpUtility.UrlEncode(nextPageKey)}";
            if (filterIndex >= 0)
                url += $"&filter={System.Web.HttpUtility.UrlEncode($"KEYWORD:{keyword}|STATIC_SERVICE:ROCKET_WOW_DELIVERY,COUPANG_GLOBAL,ROCKET_FRESH_DELIVERY,SUBSCRIPTION_DELIVERY,TOP_BRAND,FREE_DELIVERY,FREE_RETURN,FRESH_DIRECT_DELIVERY|{sort_string[filterIndex]}")}";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Coupang-App"] = "COUPANG|Android|7.1.2|7.2.7|";
            req.Headers["X-Coupang-Target-Market"] = "KR";
            req.Headers["X-Coupang-Origin-Region"] = "KR";
            req.Headers["X-Coupang-Accept-Language"] = "ko-KR";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR");

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
        
        public static string curl_get_category_products(string category_id, string nextPageKey = "", int filterIndex = 0)
        {
            string result = "";

            string url = $"https://cmapi.coupang.com/modular/v1/pages/1/link-categories/components/{category_id}?cartSessionId=&entrySize=64&filterVersion=V2";
            if (!string.IsNullOrEmpty(nextPageKey))
                url += $"&nextPageKey={System.Web.HttpUtility.UrlEncode(nextPageKey)}";
            if (filterIndex >= 0)
                url += $"&filter{System.Web.HttpUtility.UrlEncode(sort_string[filterIndex])}";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers["Coupang-App"] = "COUPANG|Android|7.1.2|7.2.7|";
            req.Headers["X-Coupang-Target-Market"] = "KR";
            req.Headers["X-Coupang-Origin-Region"] = "KR";
            req.Headers["X-Coupang-Accept-Language"] = "ko-KR";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR");

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

        public static string curl_get_store(string productId, string itemId, string vendorItemId)
        {
            string result = "";


            string url = $"https://www.coupang.com/vp/products/{productId}/items/{itemId}/vendoritems/{vendorItemId}";
            Uri uri = new Uri(url);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Host = uri.Host;
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = web_user_agent;
            req.Referer = $"https://www.coupang.com/vp/products/{productId}";
            req.Accept = "*/*";
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
            req.KeepAlive = true;
            req.Timeout = 2000;
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                //GZipStream gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                //gzipStream.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }
    }
}
