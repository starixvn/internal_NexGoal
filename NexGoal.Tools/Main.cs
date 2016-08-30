namespace NexGoal.Tools
{
    using System.Windows.Forms;

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            UCResetScore resetScore = new UCResetScore();
            resetScore.Dock = DockStyle.Fill;

            UCChangeCertificate changeCertificate = new UCChangeCertificate();
            changeCertificate.Dock = DockStyle.Fill;

            tabControl.TabPages[0].Controls.Add(resetScore);
            tabControl.TabPages[1].Controls.Add(changeCertificate);
        }
    }
}