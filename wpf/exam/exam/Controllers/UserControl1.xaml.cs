using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace exam.Controllers
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
           
        }

        public void setTile(string name)
        {
            title.Text = name;
        }

        public delegate void ButtonClickCallback(string inputValue);
        public event ButtonClickCallback OnButtonClick;

        private void onButtonCrearClick(object sender, RoutedEventArgs e)
        {
            string textBoxValue = nom.Text;
            OnButtonClick?.Invoke(textBoxValue);
        }
    }
}
