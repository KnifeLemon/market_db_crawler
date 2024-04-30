using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mm_db_market.helper
{
    public static class _11st
    {
        private readonly static string user_agent = "Mozilla/5.0 (Linux; Android 9; SM-G973N Build/PI; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/121.0.6167.180 Mobile Safari/537.36 CP_ELEVENST (01; 9.7.6; playstore; 225; c29885f466388121818af4502c806fd3; 93ba8d34-bebc-42a5-82b8-196502c34a2c) CP_SESSION_ID (34b7401b-c91c-3de3-a816-1b6b4f9f146c_1714466428885; 34b7401b-c91c-3de3-a816-1b6b4f9f146c_1714466428886) SKpay/1.8.1 11pay/1.8.1 (Android 9; plugin-mode) com.elevenst/9.7.6";

        // 검색결과   JSON      1~
        // 스토어결과 JSON      2~
        // 상점결과   JSON      3~
        // 
        // { "컬럼명", "1~JSON경로" }
        //
        // { "컬럼명", "1~앞에붙일내용|JSON경로|뒤에붙일내용" }
        //
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용" }
        // { "컬럼명", "1~JSON경로$찾을내용,변경내용$찾을내용2,변경내용2" }
        public readonly static JObject columns = new JObject()
        {
            { "상품명", "2~appDetail.prdNm" },
            { "상품설명", "2~appDetail.prdOptDescription" },
            { "상품 URL", "2~https://11st.co.kr/products/|appDetail.prdNo|" },
            { "대분류", "2~appDetail.prdLogData.large_category_name" },
            { "중분류", "2~appDetail.prdLogData.middle_category_name" },
            { "소분류", "2~appDetail.prdLogData.small_category_name" },
            { "상품 썸네일 이미지", "2~appDetail.prdImg.headerImgUrl" },
            { "상품 가격", "2~appDetail.prdPrice.selPrc" },
            { "상품 판매가", "2~appDetail.prdPrice.finalDscPrc" },
            { "상품 회원가", "2~appDetail.prdPrice.memberFinalDscPrc" },
            { "상품 쿠폰적용가", "2~appDetail.cupnDownloadInfo.cupnApplyPrice" },
            { "상품 리뷰 수", "2~appDetail.prdReviewPost.totalCount" },
            { "상품 평점", "2~appDetail.prdSatisfyStr" },
            { "상품 좋아요 수", "2~appDetail.prdLike.likeCount" },

            { "상점아이디", "2~appDetail.miniMall.selMemId" },
            { "상점명", "2~appDetail.miniMall.selNickNm" },
            { "상점 URL", "2~https://shop.11st.co.kr/stores/|appDetail.maxDiscountPriceInfo.requestBody.sellerStoreNo|" },
            { "사업자 종류", "3~data[0].businessClassification" },
            { "사업자 상호명", "3~data[0].businessName" },
            { "사업자 대표자명", "3~data[0].representative" },
            { "사업자 대표 메일", "3~data[0].email" },
            { "사업자 등록 번호", "3~data[0].businessClassification" },
            { "통신 판매업 번호", "3~data[0].onlineSalesDeclaration" },
            { "사업장 주소", "3~data[0].businessAddress" },
            { "대표 전화 번호(-포함)", "3~data[0].phoneNumber" },
        };

        public readonly static List<string> default_columns = new List<string>()
        {
            "상품명",
            "상품 URL",
            "상품 가격",
            "상품 판매가",
            "상품 평점",
            "상품 리뷰 수",
            "상품 좋아요 수",
            "사업자 종류",
            "사업자 상호명",
            "사업자 대표자명",
            "사업자 대표 메일",
            "사업자 등록 번호",
            "통신 판매업 번호",
            "대표 전화 번호(-포함)",
            "사업장 주소",
        };

        private static CookieContainer tempCookie;


        public static JArray LoadCategory(string param = "pageId=SIDEMENU_V3", bool isLastCategory = false)
        {
            String url = "http://m.11st.co.kr/";
            Uri uri = new Uri(url);
            tempCookie = new CookieContainer();
            tempCookie.Add(new Cookie("WMONID", "5MHizauLATP") { Domain = uri.Host });
            tempCookie.Add(new Cookie("appVCA", "976") { Domain = uri.Host });
            tempCookie.Add(new Cookie("appType", "appmw") { Domain = uri.Host });
            tempCookie.Add(new Cookie("screenDensity", "225") { Domain = uri.Host });
            tempCookie.Add(new Cookie("appEmbweb", "Y") { Domain = uri.Host });
            tempCookie.Add(new Cookie("deviceID", Guid.NewGuid().ToString()) { Domain = uri.Host });
            tempCookie.Add(new Cookie("MT", "notiShpEvtBnftInstYn|Y") { Domain = uri.Host });

            JArray main_categorys = new JArray();

            string result = "";
            if (isLastCategory == false) result = curl_get_category(param);
            else result = curl_get_category_last(param);

            if (string.IsNullOrEmpty(result))
                return main_categorys;

            JObject json_result = JObject.Parse(result);

            // 대분류 파싱
            foreach (JToken blockMain in json_result["data"].Where(a => a["type"].ToString() == "CaptionCarrier" || a["type"].ToString() == "BlankCarrier"))
            {

                foreach (JToken blockItem in blockMain["blockList"])
                {
                    if (blockItem["type"].ToString().Contains("GridList_ImgTextCard") == false) continue;

                    foreach (JToken cate in blockItem["list"])
                    {
                        bool isSubCategory = (cate["dispObjNo"] != null);
                        string cate_no = "";
                        if (cate["dispCtgrNo"] != null) cate_no = cate["dispCtgrNo"].ToString();
                        if (cate["dispObjNo"] != null) cate_no = cate["dispObjNo"].ToString();

                        string link = "";
                        if (cate["link"] != null) link = cate["link"].ToString();
                        if (cate["linkUrl1"] != null) link = cate["linkUrl1"].ToString();
                        if (string.IsNullOrEmpty(link)) continue;

                        string sub_param = link.Substring(link.IndexOf("?") + 1);

                        JArray sub_categorys = new JArray();
                        if (isSubCategory == false && link.Contains("pageId"))
                            sub_categorys = LoadCategory(sub_param);
                        else if (isSubCategory)
                            sub_categorys = LoadCategory("metaCtgrNo=" + cate_no, true);

                        JObject obj = new JObject()
                        {
                            { "id", cate_no },
                            { "name", cate["title1"].ToString() },
                        };

                        if (sub_categorys.Count > 0)
                            obj.Add("sub_category", sub_categorys);

                        main_categorys.Add(obj);
                    }
                }
            }

            return main_categorys;
        }

        private static string curl_get_category(string param)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://m.11st.co.kr/MW/CMS/PageDataAjax.tmall?" + param);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("EUC-KR"));

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        private static string curl_get_category_last(string param)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://apis.11st.co.kr/display-api/display/category?pageId=MOCATEGORYDEFAULT&appId=01&appType=appmw&appVCA=976&deviceID=" + Guid.NewGuid().ToString() + "&tStoreYN=N&deviceType=android&" + param);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));

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

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"http://apis.11st.co.kr/search/api/tab/total-search/more/common?poc=app&searchKeyword={System.Web.HttpUtility.UrlEncode(keyword)}&tier=B&mergeCollections=CATALOG_COMPARE,AMAZON_PRODUCT&pageNo={page}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = $"http://search.11st.co.kr/";
            req.Accept = "application/json, text/plain, */*";
            req.Headers["X-Requested-With"] = "com.elevenst";
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

        public static string curl_get_category_products(string category_id, int page = 1)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"http://m.11st.co.kr/MW/api/app/elevenst/category/getMore.tmall?dispCtgrLevel=2&dispCtgrNo={category_id}&pageNo={page}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("EUC-KR"));

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();
            }

            return result;
        }

        public static string curl_get_detail(string prdNo)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"http://m.11st.co.kr/products/v1/app/products/{prdNo}/detail");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
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

        public static string curl_get_store(string storeNo)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"http://apis.11st.co.kr/app-store/client/stores/{storeNo}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Accept = "application/json, text/plain, */*";
            req.Referer = "http://shop.11st.co.kr/";
            req.Headers["X-Requested-With"] = "com.elevenst";
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

        public static string curl_get_store_bizInfo(string storeNo, string encSellerNo)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"http://apis.11st.co.kr/app-store/client/store/data/seller?storeNo={storeNo}&encSellerNo={System.Web.HttpUtility.UrlEncode(encSellerNo)}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Accept = "application/json, text/plain, */*";
            req.Referer = "http://shop.11st.co.kr/";
            req.Headers["X-Requested-With"] = "com.elevenst";
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
    }
}
