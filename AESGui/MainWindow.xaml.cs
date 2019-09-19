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

using AESEncryptor;

namespace AESGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            var encryptUtil = new AesUtil(Password.Text, Salt.Text);
            EncryptedText.Text = encryptUtil.Encrypt(PlainText.Text);
        }

        private void DecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            var encryptUtil = new AesUtil(Password.Text, Salt.Text);
            PlainText.Text = encryptUtil.Decrypt(EncryptedText.Text);
        }
    }
}
