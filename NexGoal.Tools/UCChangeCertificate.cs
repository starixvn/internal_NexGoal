namespace NexGoal.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows.Forms;

    public partial class UCChangeCertificate : UserControl
    {
        private readonly ICertificateService certificateService;
        private readonly ICommonService commonService;

        public UCChangeCertificate()
        {
            InitializeComponent();

            certificateService = new CertificateService();
            commonService = new CommonService();
        }

        private void btnOpenOldFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose pfx file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "pfx file (*.pfx)|*.pfx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtOldCertificate.Text = openFileDialog.FileName;
            }
        }

        private void btnOpenNewFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose pfx file";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "pfx file (*.pfx)|*.pfx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtNewCertificate.Text = openFileDialog.FileName;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            IList<EPAModel> epaInfos = new List<EPAModel>();
            X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;

            try
            {
                X509Certificate2 oldCert2 = new X509Certificate2(txtOldCertificate.Text, txtOldPassword.Text, flags);
                var criteria = new GetEPAInfoToChangeCertificateCriteria();
                epaInfos = commonService.GetEPAInfoToChangeCertificate(criteria);
                DecryptScore(epaInfos, oldCert2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                X509Certificate2 newCert2 = new X509Certificate2(txtNewCertificate.Text, txtNewPassword.Text, flags);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void DecryptScore(IList<EPAModel> epaInfos, X509Certificate2 cert2)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = certificateService.Decrypt(cert2, epaInfo.FinalScoreEncode);
                epaInfo.Grade = certificateService.Decrypt(cert2, epaInfo.GradeEncode);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    process.TotalScore = certificateService.Decrypt(cert2, process.TotalScoreEncode);
                    process.AdjustScore = certificateService.Decrypt(cert2, process.AdjustScoreEncode);

                    foreach (EPAGroupDetailModel groupDetail in process.EPAGroupDetails)
                    {
                        groupDetail.Score = certificateService.Decrypt(cert2, groupDetail.ScoreEncode);

                        foreach (EPADetailModel detail in groupDetail.EPADetails)
                        {
                            detail.Score = certificateService.Decrypt(cert2, detail.ScoreEncode);
                            detail.Rate = certificateService.Decrypt(cert2, detail.RateEncode);
                        }
                    }
                }
            }
        }
    }
}