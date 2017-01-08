using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MtaTrainTime
{
    public partial class Form1 : Form
    {
        DataTable TrainTable;
        public Form1()
        {
            InitializeComponent();
            TrainTable = SetupTable();
            UpdateTable(TrainTable);
        }

        void UpdateTable(DataTable oldDT)
        {
            DataTable updatedTable = TrainTimeFunctions.UpdateTime(oldDT);
            oldDT.Dispose();

            dataGridView1.DataSource = updatedTable;
            dataGridView1.Update();
        }

        DataTable SetupTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Station ID");
            dt.Columns.Add("Uptown");
            dt.Columns.Add("Downtown");

            dt.Rows.Add("401"); //Woodlawn
            dt.Rows.Add("402"); //Moshulo Pwky
            dt.Rows.Add("405"); //Bedford Park Blvd - Lehman College
            dt.Rows.Add("406"); //Kingsbridge
            dt.Rows.Add("407"); //Fordham
            dt.Rows.Add("408"); //183
            dt.Rows.Add("409"); //burnside
            dt.Rows.Add("410"); //176
            dt.Rows.Add("411"); //mt eden
            dt.Rows.Add("412"); //170
            dt.Rows.Add("413"); //167
            dt.Rows.Add("414"); //161
            dt.Rows.Add("415"); //149
            dt.Rows.Add("416"); //138
            return dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateTable((DataTable)dataGridView1.DataSource);
        }
    }
}
