using StackExchange.Redis;
using System;
using System.Windows.Forms;

namespace MyRedisDemo
{
    public partial class Form1 : Form
    {
        public ConnectionMultiplexer connection = null;
        public IDatabase database = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            connection = ConnectionMultiplexer.Connect("127.0.0.1:6379");//初始化
            database = connection.GetDatabase(0);//指定连接库 0
            this.listView1.Items.Clear();
        }
        /// <summary>
        /// set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string key = this.textBox1.Text;
            string value = this.textBox2.Text;
            int usefulTime = Convert.ToInt32(this.numericUpDown1.Value);
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("key或者value不能为空！", "MyRedisDemo");
                return;
            }
            bool isSuccess = false;
            try
            {
                isSuccess = database.StringSet(key, value, TimeSpan.FromSeconds(usefulTime));
                ShowKeyValue(key);
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// get
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string key = this.textBox3.Text;
            string value = string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                //得到所有的Key
                //Dictionary<string, string> dicList = RedisHelper.HGetAll(key);
            }
            else
            {
                ShowKeyValue(key);
            }
        }

        public void ShowKeyValue(string key)
        {
            RedisValueWithExpiry redisValueWithExpiry = database.StringGetWithExpiry(key);
            string value = redisValueWithExpiry.Value;
            string remainingTime = redisValueWithExpiry.Expiry.ToString();
            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show($"{key} 不存在", "MyRedisDemo");
                return;
            }
            this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            ListViewItem lvi = new ListViewItem();
            // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标
            lvi.Text = key;
            lvi.SubItems.Add(value);
            lvi.SubItems.Add(remainingTime);
            this.listView1.Items.Add(lvi);
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            this.listView1.EnsureVisible(this.listView1.Items.Count - 1);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveByKey(string key)
        {
            bool isDelete = false;
            try
            {
                isDelete = database.KeyDelete(key);
            }
            catch (Exception)
            {

            }
            if (isDelete)
                MessageBox.Show($"{key} 删除成功！", "MyRedisDemo");
            else
                MessageBox.Show($"{key} 删除失败！", "MyRedisDemo");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string key = this.textBox4.Text;
            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("key不能为空！", "MyRedisDemo");
                return;
            }
            RemoveByKey(key);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = this.listView1.SelectedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem item = this.listView1.SelectedItems[i];
                this.listView1.Items.Remove(item);
            }
        }
    }
}
