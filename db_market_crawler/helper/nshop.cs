using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mm_db_market.helper
{
    public static class nshop
    {
        private readonly static string user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
        private readonly static string sec_ch_ua = "\"Microsoft Edge\";v=\"123\", \"Not:A-Brand\";v=\"8\", \"Chromium\";v=\"123\"";

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
            { "상품명", "1~productName" },
            { "상품제목", "1~productTitle" },
            { "상품 카테고리 1", "1~category1Name" },
            { "상품 카테고리 2", "1~category2Name" },
            { "상품 카테고리 3", "1~category3Name" },
            { "상품 카테고리 4", "1~category4Name" },
            { "상품 썸네일 이미지", "1~imageUrl" },
            { "상품 가격", "1~price" },
            { "상품 최저 판매가", "1~lowPrice" },
            { "상품 가격(Mobile)", "1~mobilePrice" },
            { "상품 최저 판매가(Mobile)", "1~mobileLowPrice" },
            { "상품 리뷰 수", "1~reviewCount" },
            { "상품 평점", "1~scoreInfo" },
            { "상품 브랜드", "1~brand" },
            { "상품 제조사", "1~maker" },

            { "쇼핑몰명", "2~smartStoreV2.channel.channelName" },
            { "쇼핑몰 URL", "2~https://smartstore.naver.com/|smartStoreV2.channel.url|" },
            { "쇼핑몰 로고 URL", "2~smartStoreV2.channel.representativeImageUrl" },
            { "톡톡 URL", "2~https://talk.naver.com/ct/|smartStoreV2.channel.talkAccountId|" },
            { "쇼핑몰 설명", "2~smartStoreV2.channel.description" },
            { "쇼핑몰 연락처 국가", "2~smartStoreV2.channel.contactInfo.telNo.countryCode" },
            { "쇼핑몰 연락처", "2~smartStoreV2.channel.contactInfo.telNo.phoneNo" },
            { "쇼핑몰 연락처(-포함)", "2~smartStoreV2.channel.contactInfo.telNo.formattedNumber" },
            { "쇼핑몰해외 연락처 국가", "2~smartStoreV2.channel.contactInfo.overseaTelNo.countryCode" },
            { "쇼핑몰해외 연락처", "2~smartStoreV2.channel.contactInfo.overseaTelNo.phoneNo" },
            { "쇼핑몰해외 연락처(-포함)", "2~smartStoreV2.channel.contactInfo.overseaTelNo.formattedNumber" },
            { "쇼핑몰 생성일시", "2~smartStoreV2.displayConfig.createdDate" },
            { "사업자 상호명", "2~smartStoreV2.channel.representName" },
            { "사업자 대표자명", "2~smartStoreV2.channel.representativeName" },
            { "사업자 법인유무", "2~smartStoreV2.channel.businessType$PRIVATE,개인$CORPORATION,법인$SIMPLE,간이" },
            { "사업자 주소", "2~smartStoreV2.channel.businessAddressInfo.fullAddressInfo" },
            { "사업자 위도", "2~smartStoreV2.channel.businessAddressInfo.latitude" },
            { "사업자 경도", "2~smartStoreV2.channel.businessAddressInfo.longitude" },
            { "사업자 등록 번호", "2~smartStoreV2.channel.identity" },
            { "통신 판매업 번호", "2~smartStoreV2.channel.declaredToOnlineMarkettingNumber" },
            { "대표 이메일", "2~smartStoreV2.channel.chrgrEmail" },
            { "대표 전화 번호(-포함)", "2~smartStoreV2.channel.representTelephoneNumber" },
            { "대표 생일", "2~smartStoreV2.channel.representativeBirthDay" },
            { "최근 판매횟수", "2~smartStoreV2.channel.saleCount" },
            { "문의 응답 지수", "2~smartStoreV2.channel.csResponseRatio" },
            { "2일 이내 배송완료 지수", "2~smartStoreV2.channel.in2DaysDeliveryCompleteRatio" },
            { "평균 구매 만족 지수", "2~smartStoreV2.channel.averageSaleSatificationScore" },
            { "판매중인 상품 수", "2~smartStoreV2.productCount" },
            { "오늘 방문자 수", "2~visit.A.today" },
            { "총 방문자 수", "2~visit.A.total" },
            { "쇼핑몰 찜 수", "2~storeKeep.A.zzimCount" },
        };

        public readonly static List<string> default_columns = new List<string>()
        {
            "쇼핑몰명",
            "쇼핑몰 URL",
            "사업자 법인유무",
            "사업자 상호명",
            "사업자 등록 번호",
            "통신 판매업 번호",
            "사업자 상호명",
            "사업자 대표자명",
            "쇼핑몰 연락처(-포함)",
            "사업자 주소",
            "대표 이메일",
            "대표 전화 번호(-포함)",
            "최근 판매횟수",
            "쇼핑몰 찜 수",
            "문의 응답 지수",
            "2일 이내 배송완료 지수",
            "평균 구매 만족 지수",
        };

        public static JArray LoadCategory()
        {
            JArray main_categorys = new JArray();

            string result = curl_get_category();

            JObject json_result = JObject.Parse(result);

            // 메인 카테고리 파싱
            foreach (JToken cate in json_result["hamburger"]["categories"])
            {
                // 서브 카테고리 파싱
                JArray sub_categorys = new JArray();
                foreach (JToken sub_cate in cate["categories"])
                {
                    sub_categorys.Add(new JObject()
                    {
                        { "id", sub_cate["catId"].ToString() },
                        { "name", sub_cate["catNm"].ToString() },
                    });
                }

                main_categorys.Add(new JObject()
                {
                    { "id", cate["catId"].ToString() },
                    { "name", cate["catNm"].ToString() },
                    { "sub_category", sub_categorys },
                });
            }

            return main_categorys;
        }

        private static string curl_get_category()
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://m.shopping.naver.com/menu?vertical=HOME");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = "https://m.shopping.naver.com/home";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");

            HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();

                result = Regex.Split(Regex.Split(result, @"__PRELOADED_STATE__=")[1], @"</script>")[0];
            }

            return result;
        }

        public static string curl_get_products(string keyword, string catId, int page = 1)
        {
            string result = "";

            string url = $"https://search.shopping.naver.com/api/search/all?sort=rel&pagingIndex={page}&viewType=list&productSet=checkout&pagingSize=80";

            if (!string.IsNullOrEmpty(catId))
                url += $"&catId={catId}";

            if (!string.IsNullOrEmpty(keyword))
                url += $"&query={System.Web.HttpUtility.UrlEncode(keyword)}";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = "https://msearch.shopping.naver.com/search/all";
            req.Accept = "application/json, text/plain, */*";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko,en;q=0.9,en-US;q=0.8");
            req.Headers["logic"] = "PART";
            req.Headers["sbth"] = make_sbth();
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

        public static string curl_get_catId(string category_id)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://search.shopping.naver.com/search/category/{category_id}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = "https://shopping.naver.com/home";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9\r\n";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko,en;q=0.9,en-US;q=0.8");
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

                result = Regex.Match(result, "catId=([0-9]+)").Groups[1].Value;
            }

            return result;
        }

        public static string curl_get_category_products(string category_id, string catId, int page = 1)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://search.shopping.naver.com/api/search/category/{category_id}?sort=rel&pagingIndex={page}&pagingSize=80&viewType=list&productSet=checkout&catId={catId}");
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = $"https://search.shopping.naver.com/search/category/{category_id}";
            req.Accept = "application/json, text/plain, */*";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko,en;q=0.9,en-US;q=0.8");
            req.Headers["logic"] = "PART";
            req.Headers["sbth"] = make_sbth();
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

        public static string curl_get_store(string store_url)
        {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(store_url);
            req.Method = WebRequestMethods.Http.Get;
            req.UserAgent = user_agent;
            req.Referer = "https://smartstore.naver.com/";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            req.Headers.Add(HttpRequestHeader.AcceptLanguage, "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
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

                result = Regex.Split(Regex.Split(result, @"__PRELOADED_STATE__=")[1], @"</script>")[0];
            }

            return result;
        }

        private static void curl_default_header(ref HttpWebRequest req)
        {
            req.UserAgent = user_agent;
            req.Headers["Sec-Ch-Ua"] = sec_ch_ua;
        }

        private static string make_sbth()
        {
            //string RFC1123_time = DateTime.Now.ToUniversalTime().ToString("r"); ;
            string RFC1123_time = DateTime.Now.ToString("r"); ;

            string encText = $"sb{RFC1123_time}th";
            string encryptedText = AESEncryption.Encrypt(encText);
            return encryptedText;
            Console.WriteLine("Encrypted text: " + encryptedText);

            string decText = "42b92cd0e819a98a70aea55898de80add7f3643cc08058227e9aba6d2c1acf2bd59e542267412702341dafa3ce26de3a";
            string decryptedText = AESEncryption.Decrypt(decText);
            Console.WriteLine("Decrypted text: " + decryptedText);
        }

        public class AESEncryption
        {
            /*
             * 네이버 원본 스크립트
             52647: function(e, t, n) {
                "use strict";
                n.d(t, {
                    cB: function() {
                        return l
                    }
                });
                var r = n(71941)
                  , o = n.n(r)
                  , i = function(e, t) {
                    var n = o().createCipheriv(t.algorithm, t.key, t.iv)
                      , r = n.update(e, t.inputEncoding, t.outputEncoding);
                    return r += n.final(t.outputEncoding)
                }
                  , a = {
                    algorithm: "aes-256-cbc",
                    key: "12501986019234170293715203984170",
                    iv: "6269036102394823",
                    inputEncoding: "utf8",
                    outputEncoding: "hex"
                }
                  , s = "sb"
                  , c = "th"
                  , l = function() {
                    var e = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : new Date;
                    return i("".concat(s).concat(e.toUTCString()).concat(c), a)
                }
            },
            */
            const string Key = "12501986019234170293715203984170";
            const string IV = "6269036102394823";
            public static string Encrypt(string message)
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = UTF8Encoding.UTF8.GetBytes(IV);
                aes.Key = UTF8Encoding.UTF8.GetBytes(Key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] data = Encoding.UTF8.GetBytes(message);
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(data, 0, data.Length);
                    return ToHexString(dest).ToLower();
                }
            }

            public static string Decrypt(string encryptedText)
            {
                string plaintext = null;
                using (AesManaged aes = new AesManaged())
                {
                    byte[] cipherText = FromHexString(encryptedText);
                    byte[] aesIV = UTF8Encoding.UTF8.GetBytes(IV);
                    byte[] aesKey = UTF8Encoding.UTF8.GetBytes(Key);
                    ICryptoTransform decryptor = aes.CreateDecryptor(aesKey, aesIV);
                    using (MemoryStream ms = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                                plaintext = reader.ReadToEnd();
                        }
                    }
                }
                return plaintext;
            }

            private static string ToHexString(byte[] str)
            {
                var sb = new StringBuilder();

                var bytes = str;
                foreach (var t in bytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString();
            }

            private static byte[] FromHexString(string hexString)
            {
                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return bytes;
            }
        }
    }
}
