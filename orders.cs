using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace OrderingSystem
{
    public partial class orders : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public orders()
        {
            InitializeComponent();
            LoadProducts();
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string ProdName { get; set; }
            public decimal ProdPrice { get; set; }

            public override string ToString()
            {
                return ProdName;
            }
        }

        private async void LoadProducts()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost/myapi/phpapi/apiOrders.php?action=getProducts");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a list of Product objects
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                comboBox1.Items.Clear();
                foreach (var product in products)
                {
                    comboBox1.Items.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        public class Order
        {
            public int OrderID { get; set; }
            public string CustName { get; set; }
            public string CustAddress { get; set; }
            public string PhoneNum { get; set; }
            public string ProdName { get; set; }
            public decimal ProdPrice { get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public double AmountDue { get; set; }
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private async void postBTN_Click(object sender, EventArgs e)
        {
            var selectedProduct = comboBox1.SelectedItem as Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Please select a product.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nameTXT.Text) ||
                string.IsNullOrWhiteSpace(addressTXT.Text) ||
                string.IsNullOrWhiteSpace(phoneNumTXT.Text) ||
                string.IsNullOrWhiteSpace(quantityTXT.Text))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var userData = new
            {
                custName = nameTXT.Text,
                custAddress = addressTXT.Text,
                phoneNum = phoneNumTXT.Text,
                productID = selectedProduct.ProductId,
                quantity = int.Parse(quantityTXT.Text),
                prodPrice = selectedProduct.ProdPrice,  

            };

            string json = JsonConvert.SerializeObject(userData);
            Console.WriteLine("JSON Sent: " + json); // Log the JSON payload
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost/myapi/phpapi/apiOrders.php", content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseBody);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Request error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            nameTXT.Text = "";
            addressTXT.Text = "";
            phoneNumTXT.Text = "";
            comboBox1.Text = "SELECT PRODUCT";
            quantityTXT.Text = "";
        }

        private async void getBTN_Click(object sender, EventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost/myapi/phpapi/apiOrders.php");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                dataGridView1.DataSource = orders;
                // Adjust columns to show only necessary data
                dataGridView1.Columns["ProductID"].Visible = false;
                dataGridView1.Columns["ProdName"].HeaderText = "Product Name";
                dataGridView1.Columns["ProdPrice"].HeaderText = "Product Price";
                dataGridView1.Columns["AmountDue"].HeaderText = "Amount Due";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
