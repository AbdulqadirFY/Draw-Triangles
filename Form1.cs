﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;



namespace triangles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        #region drawGraphics
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
            Random rnd = new Random();

            // Draw triangle if draw button is clicked
            if (drawListbox || drawBtn)
            {
                // Draw trianle with different colors
                Color Color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                SolidBrush Brush = new SolidBrush(Color);

                // Calculate triangle coordinates from speified lengths
                // Initial const position to start at center - there must be a better way around this :(
                const int x1 = 300;
                const int y1 = 100;
                int x2 = x1 - a;
                int y2 = y1 + a;
                int x3 = x2 + b;
                int y3 = y1 + c;

                // Create points that define triangle.
                Point point1 = new Point(x1, y1);
                Point point2 = new Point(x2, y2);
                Point point3 = new Point(x3, y3);

                Point[] trianglePoints =
                         {
                 point1,
                 point2,
                 point3
                };

                e.Graphics.FillPolygon(Brush, trianglePoints);
            }
            
        }
        #endregion

        #region drawButton
        private void drawButton_Click(object sender, EventArgs e)
        {
            
            drawBtn = true;

            if (drawBtn) 
            {
                drawListbox = false;
            }
            

            // Create new triangle object
            Triangle triangle = new Triangle();
            a = triangle.A;
            b = triangle.B;
            c = triangle.C;
            
            try 
            {
                a = Convert.ToInt32(atextBox.Text);
                b = Convert.ToInt32(btextBox.Text);
                c = Convert.ToInt32(ctextBox.Text);
            
            

                // Only accept lengths greater than 0
                if (a > 0 && b > 0 && c > 0)
                {

                    panel1.Refresh();
                
                }
                else
                {

                    MessageBox.Show("ALERT: The values of a, b, c must be greater than 0");  
                    return;
                }

            }
            catch
            {
                MessageBox.Show("a, b, c must be numbers!");
                return;
            }
            

            // hold a copy of triangleList vales for validation
            string val = ("a: " + a + "  b: " + b + " c: " + c);

            bool duplicate = false;

            // loop around all elements in triangs
            foreach (var item in triangleList)
            {

                // check in loop if item.ToString() is equal to the above string or not
                if (String.Equals(item.ToString(), val))
                {

                    duplicate = true;
                    break;

                }       
                
            }
            if (!duplicate)
            {


                // then add triangle to list and add list to listbox
                triangleList.Add(new Triangle() { A = a, B = b, C = c });
                listBox.Items.Add(triangleList.Last());

            }


        }
        #endregion

        #region deleteTriangle
        private void delete_Click(object sender, EventArgs e)
        {
            // delete triangle from triangleList and listbox
            if (listBox.SelectedIndex > -1)
            {

                string val = listBox.SelectedItem.ToString();

                foreach (var item in triangleList)
                {
                   
                    if (String.Equals(item.ToString(), val))
                    {

                        triangleList.Remove(item);
                        MessageBox.Show("Succesfully deleted triangle  " + val);
                        break;

                    }
                }

                listBox.Items.RemoveAt(listBox.SelectedIndex);
                
            }
            else if (listBox.SelectedIndex == -1)
            {

                MessageBox.Show("ALERT: Triangle must be selected from the list!");

            }
        }
        #endregion

        #region unusedEvents
        private void label1_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            drawListbox = true;

            if (drawListbox) 
            {
                drawBtn = false;
            }
            
            // get a,b,c values from the index of triangelelist that correspomds to index of selected listbox item

            string listBoxIndex;
            string triangleIitem;

            if (listBox.SelectedIndex >= 0)
            {

                listBoxIndex = Convert.ToString(listBox.SelectedIndex);
                triangleIitem = Convert.ToString(triangleList[listBox.SelectedIndex]);
                
                a = triangleList[listBox.SelectedIndex].A;
                b = triangleList[listBox.SelectedIndex].B;
                c = triangleList[listBox.SelectedIndex].B;

                panel1.Refresh();
                
            }
            
        }

        #region importExport
        private void export_button_Click(object sender, EventArgs e)
        {
            // Get the app's local folder.
           //appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
           string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TriangleApp";

           Directory.CreateDirectory(path);

           // !dt.Columns.Contains("a") && !dt.Columns.Contains("b") && !dt.Columns.Contains("c")
            
           if (!dt.TableName.Contains("Triangle"))
           {
               dt.TableName = "Triangle";
               dt.Columns.Add("a");
               dt.Columns.Add("b");
               dt.Columns.Add("c");
         
           }
            

           foreach (var item in triangleList)
           {

               dt.Rows.Add();
               dt.Rows[dt.Rows.Count - 1]["a"] = item.A;
               dt.Rows[dt.Rows.Count - 1]["b"] = item.B;
               dt.Rows[dt.Rows.Count - 1]["c"] = item.C;

           }            

           dt.WriteXml(path + @"\triangles.Xml");

           MessageBox.Show("Triangles saved to: " + path + @"\triangles.Xml" + "\nUse IMPORT button to load saved triangles again.");
           
           listBox.Items.Clear();
           triangleList.Clear();
           dt.Rows.Clear();
        }

        //Load saved triangles
        private void import_Button_Click(object sender, EventArgs e)
        {
            // XML handler to read from xml file and extract a,b,c values to listbox
            listBox.Items.Clear();
            try 
            {
                XmlTextReader reader = new XmlTextReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TriangleApp\triangles.Xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
            

            
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode child in node)
	            {
                    if (child.NodeType == XmlNodeType.Element && child.Name == "a")
                    {
                    
                            aXml = child.InnerText;       
            
                    }
                    else if (child.NodeType == XmlNodeType.Element && child.Name == "b")
                    {
                    
                            bXml = child.InnerText;
                
                    }
                    else if (child.NodeType == XmlNodeType.Element && child.Name == "c")
                    {
                    
                            cXml = child.InnerText;  
                
                    }
	            }

                a = Convert.ToInt32(aXml);
                b = Convert.ToInt32(bXml);
                c = Convert.ToInt32(cXml);

                listBox.Items.Add("a: " + aXml + "  b: " + bXml + " c: " + cXml);    
                triangleList.Add(new Triangle() { A = a, B = b, C = c});

            }

            reader.Close();

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("No saved triangles found! Please add and export some triangles!");
            }
        }

        #endregion
        /*
        private void atextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char chr = e.KeyChar;
            if (!Char.IsDigit(chr))
                {
                e.Handled = true;
                MessageBox.Show("Please enter only ins");
            }
        }
        */
        #region formVariables
        bool drawListbox = false;
        bool drawBtn = false;

        #endregion

        #region triangleClass
        class Triangle
        {

            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }

            public override string ToString()
            {
                return "a: " + A + "  b: " + B +" c: "+ C;
            }
        }

        // List and datatable of triangles for drawing and xmlHandling
        List<Triangle> triangleList = new List<Triangle>();
        DataTable dt = new DataTable();
        #endregion

        #region properties
        private int a { get; set; }
        private int b { get; set; }
        private int c { get; set; }
        private string aXml { get; set; }
        private string bXml { get; set; }
        private string cXml { get; set; }        
        

        #endregion
    }
    
}
