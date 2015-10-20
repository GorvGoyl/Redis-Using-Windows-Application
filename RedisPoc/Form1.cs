using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedisPoc
{
    public partial class Form1 : Form
    {
        private static string _PersonalizationHost = "redis.leadsquared.co:6379";
        Tag rc = new Tag(_PersonalizationHost);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void get_btn_Click(object sender, EventArgs e)
        {
            this.info_lbl.Text = "";
            string key = this.textBox1.Text.ToString();
            this.info_lbl.Text = rc.GetValueFromCache(key);
        }

        private void put_btn_Click(object sender, EventArgs e)
        {
            this.info_lbl.Text = "";
            string key = this.textBox1.Text.ToString();
            string Value = this.textBox2.Text.ToString();
            if (key != "" && Value != "")
            {
                rc.AddValueToCache(key, Value);
            }
            else
            {
                this.info_lbl.Text = "field can't be empty";
            }
        }
    }
}