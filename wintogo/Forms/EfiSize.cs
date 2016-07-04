using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{
    public partial class EfiSize : Form
    {
        private string efisize = "350";

        public string EfiSz
        {
            get { return efisize; }
            set { efisize = value; }
        }
        
        public EfiSize()
        {
            InitializeComponent();
        }
        public EfiSize(string efisz)
            : this()
        {
            this.efisize = efisz;
        }

        private void Efisize_Load(object sender, EventArgs e)
        {
            textBox1.Text = efisize;
        }
        public bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
            !objTwoDotPattern.IsMatch(strNumber) &&
            !objTwoMinusPattern.IsMatch(strNumber) &&
            objNumberPattern.IsMatch(strNumber);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsNumber(textBox1.Text)) { MessageBox.Show("Error!"); return; }
            efisize = textBox1.Text;
            this.Close();
        }
    }
}
