using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace mm_db_market.forms
{
    public partial class frmColumnEditor : Form
    {
        JObject columnData;
        string extension;
        public string savePath;

        public frmColumnEditor(JObject _columnData, string _extension)
        {
            InitializeComponent();

            columnData = _columnData;
            extension = _extension;
        }

        private void frmColumnEditor_Load(object sender, EventArgs e)
        {
            foreach (var cols in columnData)
            {
                lvColumnList.Items.Add(cols.Key.ToString());
                Application.DoEvents();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lvColumnList.SelectedItems.Count <= 0)
                return;

            foreach (ListViewItem item in lvColumnList.SelectedItems)
            {
                ListViewItem lvi = new ListViewItem(item.Text);
                lvi.Name = item.Text;

                lvAddList.Items.Add(lvi);
                lvColumnList.Items.Remove(item);
                Application.DoEvents();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvAddList.SelectedItems.Count <= 0)
                return;

            // 재정렬을 위해 초기화 후 추가
            lvColumnList.Items.Clear();
            foreach (var cols in columnData)
            {
                string keyName = cols.Key.ToString();

                // 현재 항목이 선택 항목에 포함되어 있다면 선택항목 지움
                if (lvAddList.SelectedItems.ContainsKey(keyName))
                {
                    lvAddList.Items[keyName].Remove();
                }
                // 현재 항목이 이미 추가되어 있다면 건너뜀
                if (lvAddList.Items.ContainsKey(keyName))
                {
                    continue;
                }
                lvColumnList.Items.Add(keyName);
                Application.DoEvents();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "파일을 선택해주세요.";
            ofd.Filter = extension;

            if (DialogResult.OK == ofd.ShowDialog())
            {
                try
                {
                    List<string> jCol = helper.helper.ReadColumnsFromDisk(ofd.FileName);

                    // 초기화 후 추가
                    lvColumnList.Items.Clear();
                    lvAddList.Items.Clear();

                    // 열 항목부터 추가
                    foreach (var item in jCol)
                    {
                        ListViewItem lvi = new ListViewItem(item.ToString());
                        lvi.Name = item.ToString();

                        lvAddList.Items.Add(lvi);
                        Application.DoEvents();
                    }
                    foreach (var cols in columnData)
                    {
                        string keyName = cols.Key.ToString();

                        // 현재 항목이 선택 항목에 포함되어 있다면 선택항목 지움
                        if (lvAddList.SelectedItems.ContainsKey(keyName))
                        {
                            lvAddList.Items[keyName].Remove();
                        }
                        // 현재 항목이 이미 추가되어 있다면 건너뜀
                        if (lvAddList.Items.ContainsKey(keyName))
                        {
                            continue;
                        }
                        lvColumnList.Items.Add(keyName);
                        Application.DoEvents();
                    }
                    MessageBox.Show(this, "열 데이터를 불러왔습니다.");
                }
                catch
                {
                    MessageBox.Show(this, "열 설정 파일에 오류가 있습니다.\n다른 파일을 시도해주세요.");
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "저장 위치와 파일 이름을 선택해주세요.";
            sfd.Filter = extension;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    savePath = sfd.FileName;

                    List<string> jCol = new List<string>();
                    foreach (ListViewItem item in lvAddList.Items)
                    {
                        jCol.Add(item.Text);
                        Application.DoEvents();
                    }

                    helper.helper.WriteColumnsToDisk(savePath, jCol);

                    MessageBox.Show(this, $"저장이 완료되었습니다.\n저장경로 : {savePath}");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show(this, "저장중 오류가 발생했습니다.");
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lvAddList.Items.Clear();
            lvColumnList.Items.Clear();
            foreach (var cols in columnData)
            {
                lvColumnList.Items.Add(cols.Key.ToString());
                Application.DoEvents();
            }
        }
    }
}
