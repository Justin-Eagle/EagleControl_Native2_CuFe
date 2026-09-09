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
using System.Windows.Shapes;

namespace EagleControl_Native2_CuFe
{
    /// <summary>
    /// Interaction logic for UnLockView.xaml
    /// </summary>
    public partial class UnLockView : Window
    {
        public string UserInput { get; private set; }
        public UnLockView(String title)
        {
            InitializeComponent();

            Title.Text = title;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            UserInput = InputPasswordBox.Password;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool isRevealing = false;

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (isRevealing)
            {
                // 切回隱藏
                RevealPasswordBox.Visibility = Visibility.Collapsed;
                InputPasswordBox.Visibility = Visibility.Visible;
                isRevealing = false;
            }
            else
            {
                // 顯示密碼文字
                RevealPasswordBox.Text = InputPasswordBox.Password;
                RevealPasswordBox.Visibility = Visibility.Visible;
                InputPasswordBox.Visibility = Visibility.Collapsed;
                isRevealing = true;
            }
        }
    }
}
