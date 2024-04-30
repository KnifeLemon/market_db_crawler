using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mm_db_market.helper;
using Newtonsoft.Json.Linq;

namespace mm_db_market.forms
{
    public partial class ucGmarket : UserControl
    {
        bool workstatus = false;

        JArray categorys;
        Thread th;
        List<string> columns = new List<string>();

        public ucGmarket()
        {
            InitializeComponent();
        }

        private void ucNShop_Load(object sender, EventArgs e)
        {
            cmbSort.SelectedIndex = 0;
            LoadCategorys();
            LoadColumns(helper.gmarket.default_columns);
        }

        void LoadCategorys()
        {
            new Thread(() =>
            {
                cmbMainCategory.Items.Add("로딩중...");
                cmbMainCategory.SelectedIndex = 0;

                string cachePath = Environment.CurrentDirectory + @"\cache";
                string cacheFile = cachePath + @"\gmarket_category.json";

                if (Directory.Exists(cachePath) == false)
                    Directory.CreateDirectory(cachePath);

                if (File.Exists(cacheFile))
                {
                    if (new FileInfo(cacheFile).LastWriteTime < DateTime.Now.AddDays(-1))
                    {
                        categorys = helper.gmarket.LoadCategory();
                        File.WriteAllText(cacheFile, categorys.ToString());
                    }
                    else
                    {
                        categorys = JArray.Parse(File.ReadAllText(cacheFile));
                    }
                }
                else
                {
                    categorys = helper.gmarket.LoadCategory();
                    File.WriteAllText(cacheFile, categorys.ToString());
                }

                cmbMainCategory.Items.Clear();
                cmbSubCategory.Items.Clear();
                cmbMinCategory.Items.Clear();

                cmbMainCategory.Enabled = true;
                cmbSubCategory.Enabled = false;
                cmbMinCategory.Enabled = false;

                cmbMainCategory.Items.Add("전체");
                cmbMainCategory.SelectedIndex = 0;

                foreach (JObject cate in categorys)
                {
                    cmbMainCategory.Items.Add(cate["name"].ToString());
                }
            }).Start();
        }

        void LoadColumns(List<string> _columns)
        {
            new Thread(() =>
            {
                columns = _columns;
                lvList.Columns.Clear();

                foreach (string col in _columns)
                {
                    lvList.Columns.Add(col);
                }
            }).Start();
        }

        private void cmbMainCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIdx = cmbMainCategory.SelectedIndex;

            cmbSubCategory.Items.Clear();

            if (selectedIdx <= 0)
            {
                cmbSubCategory.Enabled = false;
                return;
            }

            new Thread(() =>
            {
                if (categorys.ElementAt(selectedIdx - 1)["sub_category"].Count() == 0)
                {
                    return;
                }

                cmbSubCategory.Enabled = true;

                cmbSubCategory.Items.Add("전체");
                cmbSubCategory.SelectedIndex = 0;

                foreach (JObject sub_cate in categorys.ElementAt(selectedIdx - 1)["sub_category"])
                {
                    cmbSubCategory.Items.Add(sub_cate["name"].ToString());
                }
            }).Start();
        }

        private void cmbSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int main_selectedIdx = cmbMainCategory.SelectedIndex;
            int selectedIdx = cmbSubCategory.SelectedIndex;

            cmbMinCategory.Items.Clear();

            if (selectedIdx <= 0)
            {
                cmbMinCategory.Enabled = false;
                return;
            }

            new Thread(() =>
            {
                if (categorys.ElementAt(main_selectedIdx - 1)["sub_category"].ElementAt(selectedIdx - 1)["min_category"].Count() == 0)
                {
                    return;
                }

                cmbMinCategory.Enabled = true;

                cmbMinCategory.Items.Add("전체");
                cmbMinCategory.SelectedIndex = 0;

                foreach (JObject sub_cate in categorys.ElementAt(main_selectedIdx - 1)["sub_category"].ElementAt(selectedIdx - 1)["min_category"])
                {
                    cmbMinCategory.Items.Add(sub_cate["name"].ToString());
                }
            }).Start();
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            forms.frmColumnEditor frm = new frmColumnEditor(helper.gmarket.columns, "G마켓 열 데이터|*.dbcol_gmarket");
            frm.Text = "G마켓 열 관리자";
            frm.ShowDialog();
        }

        private void btnLoadColumn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "파일을 선택해주세요.";
            ofd.Filter = "G마켓 열 데이터|*.dbcol_gmarket";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lvList.Items.Clear();

                    List<string> _cols = helper.helper.ReadColumnsFromDisk(ofd.FileName);

                    LoadColumns(_cols);
                    MessageBox.Show(this, "열 데이터를 불러왔습니다.");
                }
                catch
                {
                    MessageBox.Show(this, "열 데이터를 불러오는데 오류가 발생했습니다.");
                }
            }
        }

        private void btnExtractStart_Click(object sender, EventArgs e)
        {
            if (workstatus)
            {
                MessageBox.Show(this, "이미 추출중입니다.");
                return;
            }
            if (lvList.Items.Count > 0 && MessageBox.Show(this, "추출 목록을 초기화 후 추출하시겠습니까?", "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lvList.Items.Clear();
            }

            workstatus = true;

            cmbMainCategory.Enabled = false;
            cmbSubCategory.Enabled = false;
            cmbMinCategory.Enabled = false;
            txtSearchText.Enabled = false;

            btnAddColumn.Enabled = false;
            btnLoadColumn.Enabled = false;
            btnExtractStart.Enabled = false;
            btnExtractStop.Enabled = true;
            btnReset.Enabled = false;
            btnSaveList.Enabled = false;

            th = new Thread(WorkProgram);
            th.Start();
        }

        private void btnExtractStop_Click(object sender, EventArgs e)
        {
            if (!workstatus)
            {
                MessageBox.Show(this, "추출중이 아닙니다.");
                return;
            }

            try
            {
                workstatus = false;

                cmbMainCategory.Enabled = true;
                cmbSubCategory.Enabled = true;
                cmbMinCategory.Enabled = true;
                txtSearchText.Enabled = true;

                btnAddColumn.Enabled = true;
                btnLoadColumn.Enabled = true;
                btnExtractStart.Enabled = true;
                btnExtractStop.Enabled = false;
                btnReset.Enabled = true;
                btnSaveList.Enabled = true;

                th.Abort();
            }
            catch { }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (lvList.Items.Count == 0)
            {
                MessageBox.Show(this, "추출을 먼저 진행해주세요.");
                return;
            }
            if (MessageBox.Show(this, "정말로 목록을 초기화 하시겠습니까?", "경고", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            lvList.Items.Clear();
            lblStatus.Text = "총 0 개 추출완료";
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            if (lvList.Items.Count == 0)
            {
                MessageBox.Show(this, "추출을 먼저 진행해주세요.");
                return;
            }

            cmbMainCategory.Enabled = false;
            cmbSubCategory.Enabled = false;
            cmbMinCategory.Enabled = false;
            txtSearchText.Enabled = false;
            cmbSort.Enabled = false;

            btnAddColumn.Enabled = false;
            btnLoadColumn.Enabled = false;
            btnExtractStart.Enabled = false;
            btnExtractStop.Enabled = false;
            btnReset.Enabled = false;
            btnSaveList.Enabled = false;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "저장할 경로와 파일명을 설정해주세요.";
            sfd.Filter = "CSV파일|*.csv";

            if (DialogResult.OK == sfd.ShowDialog())
            {
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);

                using (var file = File.Exists(sfd.FileName) ? File.Open(sfd.FileName, FileMode.Append) : File.Open(sfd.FileName, FileMode.CreateNew))
                {
                    using (var stream = new StreamWriter(file, Encoding.UTF8))
                    {
                        List<string> column = new List<string>();
                        foreach (ColumnHeader item in lvList.Columns)
                            column.Add(item.Text);
                        stream.WriteLine($"\"{string.Join("\",\"", column.ToArray())}\"");

                        foreach (ListViewItem item in lvList.Items)
                        {
                            List<string> list_item = new List<string>();
                            for (int i = 0; i < item.SubItems.Count; i++)
                            {
                                list_item.Add(item.SubItems[i].Text);
                            }

                            stream.WriteLine($"\"{string.Join("\",\"", list_item.ToArray())}\"");
                        }
                        //stream.WriteLine(logtext);
                    }
                }

                MessageBox.Show($"총 {lvList.Items.Count}개 항목이 저장되었습니다. [{sfd.FileName}]");
            }

            cmbMainCategory.Enabled = true;
            cmbSubCategory.Enabled = true;
            cmbMinCategory.Enabled = true;
            txtSearchText.Enabled = true;
            cmbSort.Enabled = true;

            btnAddColumn.Enabled = true;
            btnLoadColumn.Enabled = true;
            btnExtractStart.Enabled = true;
            btnExtractStop.Enabled = false;
            btnReset.Enabled = true;
            btnSaveList.Enabled = true;
        }

        void WorkProgram()
        {
            string search_text = txtSearchText.Text.Trim();

            if (!string.IsNullOrEmpty(search_text))
            {
                ParseData(search_text, "");
            }
            else
            {
                // 카테고리 구하기
                if (cmbMinCategory.SelectedIndex > 0)
                {
                    string category_no = categorys.ElementAt(cmbMainCategory.SelectedIndex - 1)["sub_category"].ElementAt(cmbSubCategory.SelectedIndex - 1)["min_category"].ElementAt(cmbMinCategory.SelectedIndex - 1)["id"].ToString();

                    ParseData(search_text, category_no);
                }
                else if (cmbSubCategory.SelectedIndex > 0)
                {
                    string category_no = categorys.ElementAt(cmbMainCategory.SelectedIndex - 1)["sub_category"].ElementAt(cmbSubCategory.SelectedIndex - 1)["id"].ToString();

                    ParseData(search_text, category_no);
                }
                else if (cmbMainCategory.SelectedIndex > 0)
                {
                    string category_no = categorys.ElementAt(cmbMainCategory.SelectedIndex - 1)["id"].ToString();

                    ParseData(search_text, category_no);
                }
                else if (cmbMinCategory.SelectedIndex == 0)
                {
                    foreach (var min_category in categorys.ElementAt(cmbMainCategory.SelectedIndex - 1)["sub_category"].ElementAt(cmbSubCategory.SelectedIndex - 1))
                    {
                        string category_no = min_category["id"].ToString();

                        ParseData(search_text, category_no);
                    }
                }
                else if (cmbSubCategory.SelectedIndex == 0)
                {
                    foreach (var sub_category in categorys.ElementAt(cmbMainCategory.SelectedIndex - 1))
                    {
                        string category_no = sub_category["id"].ToString();

                        ParseData(search_text, category_no);
                    }
                }
                else if (cmbMainCategory.SelectedIndex == 0)
                {
                    foreach (var category in categorys)
                    {
                        if (category["sub_category"].Count() > 0)
                        {
                            foreach (var sub_category in category["sub_category"])
                            {
                                if (sub_category["min_category"].Count() > 0)
                                {
                                    foreach (var min_category in sub_category["min_category"])
                                    {
                                        string category_no = min_category["id"].ToString();

                                        ParseData(search_text, category_no);
                                    }
                                }
                                else
                                {
                                    string category_no = sub_category["id"].ToString();

                                    ParseData(search_text, category_no);
                                }
                            }
                        }
                        else
                        {
                            string category_no = category["id"].ToString();

                            ParseData(search_text, category_no);
                        }
                    }
                }
            }


            workstatus = false;

            cmbMainCategory.Enabled = true;
            cmbSubCategory.Enabled = true;
            cmbMinCategory.Enabled = true;
            txtSearchText.Enabled = true;

            btnAddColumn.Enabled = true;
            btnLoadColumn.Enabled = true;
            btnExtractStart.Enabled = true;
            btnExtractStop.Enabled = false;
            btnReset.Enabled = true;
            btnSaveList.Enabled = true;
        }

        void ParseData(string search_text, string category_no)
        {
            string access_token = helper.gmarket.curl_get_token();

            int page = 0;
            do
            {
                if (!workstatus)
                    break;

                JObject json;

                // 카테고리만 검색
                if (string.IsNullOrEmpty(search_text))
                {
                    string jsonResult = helper.gmarket.curl_get_category_products(access_token, category_no, ++page, cmbSort.SelectedIndex);
                    json = JObject.Parse(jsonResult);
                }
                else
                {
                    string jsonResult = helper.gmarket.curl_get_products(access_token, search_text, ++page, cmbSort.SelectedIndex);
                    json = JObject.Parse(jsonResult);
                }

                JToken content_body = json["data"]["regions"].Where(a => a["name"].ToString() == "content_body").First();

                List<JToken> products = new List<JToken>();
                foreach (JToken module in content_body["modules"])
                {
                    products.AddRange(module["rows"].Where(a => a["designName"].ToString() == "A_ItemCardGeneralGalleryCell").ToList());
                }
                
                // 항목이 없을경우 탈출 | 오류일 경우도 탈출
                try
                {
                    if (products.Count() == 0)
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }

                foreach (JToken product in products)
                {
                        string itemNo = product["viewModel"]["itemNo"].ToString();

                        string store_json_result = helper.gmarket.curl_get_store(access_token, itemNo);
                        JObject store_json = JObject.Parse(store_json_result);
                        JToken store_section_json = store_json["data"]["sections"].Where(a => a["sectionType"].ToString() == "ItemInfo").First();
                        JObject biz_json = null;

                        List<string> parse_ary = new List<string>();

                            Console.WriteLine();
                        foreach (string key in columns)
                        {
                            Console.WriteLine(key);
                            string parse_info = helper.gmarket.columns[key].ToString();

                            string parseJSON = parse_info.Split('~')[0];
                            parse_info = parse_info.Split('~')[1];

                            JToken parse_json;
                            if (parseJSON == "1")
                                parse_json = product;
                            else if (parseJSON == "2")
                                parse_json = store_json;
                            else if (parseJSON == "3")
                                parse_json = store_section_json;
                            else
                            {
                                if (biz_json == null)
                                {
                                    string shopURL = store_section_json["sectionData"]["itemInfo"]["miniShopName"]["landingUrl"].ToString();
                                    string shopId = shopURL.Split('/').Last();

                                    string biz_json_result = helper.gmarket.curl_get_biz(shopId);
                                    biz_json = JObject.Parse(biz_json_result);
                                }

                                parse_json = biz_json;
                            }
                            string parse_str = "";
                            if (parse_info.Contains("|"))
                            {
                                string[] sp = parse_info.Split('|');
                                parse_str = sp[0] + (string)parse_json.SelectToken(sp[1]) + sp[2];
                            }
                            else if (parse_info.Contains("$"))
                            {
                                string[] sp = parse_info.Split('$');

                                parse_str = (string)parse_json.SelectToken(sp[0]);

                                for (int i = 1; i < sp.Count(); i++)
                                {
                                    string[] sp2 = sp[i].Split(',');

                                    parse_str = parse_str.Replace(sp2[0], sp2[1]);
                                }
                            }
                            else
                            {
                                parse_str = (string)parse_json.SelectToken(parse_info);
                            }

                            parse_ary.Add(parse_str);
                        }

                        lvList.Items.Add(new ListViewItem(parse_ary.ToArray()));
                        lvList.Items[lvList.Items.Count - 1].EnsureVisible();
                        lblStatus.Text = $"총 {lvList.Items.Count} 개 추출완료";
                    try
                    {
                    }
                    catch { }
                }
            } while (true);
        }
    }
}
