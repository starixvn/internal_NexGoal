namespace NexGoal.Tools
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows.Forms;

    public partial class UCEncryptor : UserControl
    {
        private readonly ICertificateService certificateService;

        public UCEncryptor()
        {
            InitializeComponent();

            certificateService = new CertificateService();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose pfx file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "pfx file (*.pfx)|*.pfx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtCertificate.Text = openFileDialog.FileName;
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPlainText.Text))
            {
                try
                {
                    var certificate = new X509Certificate2(txtCertificate.Text, txtPassword.Text, X509KeyStorageFlags.MachineKeySet);
                    txtCipherText.Text = certificateService.Encrypt(certificate, txtPlainText.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }
}