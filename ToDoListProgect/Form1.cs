using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;
using ToolTip = System.Windows.Forms.ToolTip;
using Tulpep.NotificationWindow;

namespace ToDoListProgect
{
    public partial class Form1 : Form
    {
        SqlConnection ToDoListDB_connect = null;
        private PopupNotifier popupinfo = null;
        List<int> list_sekTime = new List<int>();
        List<OutBase> outBase = new List<OutBase>();
        public Form1()
        {
            InitializeComponent();
        }
       

        private void Form1_Load(object sender, EventArgs e)
        {
            string Connect_path = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ToDoListBD;" +
                "Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
                "TrustServerCertificate=False;ApplicationIntent=ReadWrite;" +
                "MultiSubnetFailover=False";
            ToDoListDB_connect =new SqlConnection(Connect_path);
            ToDoListDB_connect.Open();
            dataGridView1.Refresh();
            
          

        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            Add();
            Refresh_();
            
        }

        private void Refresh_()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter
                ("SELECT * FROM [dbo].[Table] ", ToDoListDB_connect);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;            
            dataGridView1.Columns[0].Width = 25;
            dataGridView1.Columns[2].Width =100;
            dataGridView1.Columns[3].Width = 80;
           dataGridView1.Columns[3].Visible = false; 
            dataGridView1.Columns[1].HeaderText = "          СПИСОК ДЕЛ,КОТОРЫЕ НЕОБХОДИМО СДЕЛАТЬ:";
            dataGridView1.Columns[2].HeaderText = "СРОК ВЫПОЛНЕНИЯ:";
            dataGridView1.Columns[3].HeaderText = "Время в секундах";
            dataGridView1.Refresh();




        }

        private void Add()
        {
            string DateTime_ = maskedTextBox_Time.Text;
            var timeTmp = DateTime.Parse(DateTime_);
            var timeTmpSek = new DateTimeOffset(timeTmp);
            var timeSek_str = timeTmpSek.ToUnixTimeSeconds().ToString();
            var resultLong = Int64.Parse(timeSek_str);
            long TimeSek = resultLong;
            
            SqlCommand command = new SqlCommand($"INSERT INTO [dbo].[Table]([Task],[Time],[TimeSek])" +
               $"VALUES(@Task,@Time,@TimeSek)", ToDoListDB_connect);
            command.Parameters.AddWithValue("Task", textBox_Task.Text);
             command.Parameters.AddWithValue("Time", maskedTextBox_Time.Text);
            command.Parameters.AddWithValue("TimeSek", TimeSek);

            command.ExecuteNonQuery();


        }
        private void Update_Task()
        {
            SqlCommand command = new SqlCommand($"Update [dbo].[Table] set [Task]=@Task,Time=[Time] where id=@id;", ToDoListDB_connect);
            command.Parameters.AddWithValue("Task", textBox_Task.Text);
            command.Parameters.AddWithValue("id", Convert.ToInt32(textBox_ID.Text));           
            command.ExecuteNonQuery();
        }
        private void Update_Time()
        {
            SqlCommand command = new SqlCommand($"Update [dbo].[Table] set [Task]=[Task] ,[Time]=@Time where id=@id;", ToDoListDB_connect);
            command.Parameters.AddWithValue("Time",maskedTextBox_Time.Text);
            command.Parameters.AddWithValue("id", Convert.ToInt32(textBox_ID.Text));
            command.ExecuteNonQuery();
        }

        private void Update_Full()
        {
            SqlCommand command = new SqlCommand($"Update [dbo].[Table] set [Task]=@Task,[Time]=@Time where id=@id;", ToDoListDB_connect);
            command.Parameters.AddWithValue("Task", textBox_Task.Text);
            command.Parameters.AddWithValue("Time", maskedTextBox_Time.Text);
            command.Parameters.AddWithValue("id", Convert.ToInt32(textBox_ID.Text));
            command.ExecuteNonQuery();

        }
        private void delite()
        {
            SqlCommand command = new SqlCommand($"delete [dbo].[Table]  where id=@id;",ToDoListDB_connect);           
            command.Parameters.AddWithValue("id", Convert.ToInt32(textBox_ID.Text));
            command.ExecuteNonQuery();

        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            Refresh_();
        }

        private void button_change_Click(object sender, EventArgs e)
        {
            
            Update_Task();
            Refresh_();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Update_Time();
            Refresh_();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Update_Full();
            Refresh_();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            delite();
            Refresh_();
        }
       
        private void Sek_reader()
        {
            SqlDataReader reader = null;
          outBase.Clear();
            
                SqlCommand commands = new SqlCommand($"select [TimeSek],[Task] from  [dbo].[Table];", ToDoListDB_connect);
                reader = commands.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    outBase.Add(new OutBase(reader.GetInt32(0), reader.GetString(1))) ;

                }
                reader.Close();
            }
           
                

            
        }
        private int TimeNow_in_Sek()
        {
            var timeNowsek = new DateTimeOffset(DateTime.Now);
            var timeNowsek_str = timeNowsek.ToUnixTimeSeconds().ToString();
            var resultNowsek = Int32.Parse(timeNowsek_str.ToString());
            return resultNowsek;
        }
       
        private void timer1_Tick(object sender, EventArgs e)
        {
            popupinfo = new PopupNotifier();
            popupinfo.Image = Properties.Resources.putin;
            popupinfo.ImageSize = new Size(200,300);
            popupinfo.TitleText = "Напоминание!!";
            popupinfo.Size = new Size(400, 320);

            Sek_reader();
            foreach (var item in outBase)
            {
                int tmpTime = item.TimeSek;
                int res = tmpTime-TimeNow_in_Sek();
               
                if (res ==300||res<300&&res>120)
                {
                    
                    popupinfo.ContentText = ($"{item.Task}");
                    popupinfo.HeaderColor = Color.Blue;
                    popupinfo.TitleColor = Color.Blue;
                    this.popupinfo.Popup();
                }

            }
        }

        
    }
}
