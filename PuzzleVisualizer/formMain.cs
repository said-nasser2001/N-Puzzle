using N_Puzzle_Solver;

namespace PuzzleVisualizer
{
    public partial class formMain : Form
    {
        private State state;
        List<State> reversedPath;
        int stateCounter;

        string filePath, fileName;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        Point tileSize = new Point(85, 85);
        HeuristicFunction distanceFunction;

        public formMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void loadPuzzleStrip_Click(object sender, EventArgs e)
        {
            var fileInfo = browse();
            filePath = fileInfo[0]; // full path
            fileName = fileInfo[1]; // file name

            if (filePath is null) return;

            comboFunction.Enabled = true;
            comboFunction.SelectedIndex = 0;
        }

        String[] browse()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Choose a puzzle file";
            dialog.Multiselect = false;
            dialog.Filter = "txt files (*.txt)|*.txt";
            dialog.InitialDirectory =
                Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\N-Puzzle-Tester\Testcases")); ;

            string filePath, fileName;
            if (dialog.ShowDialog() == DialogResult.OK)
            {

                filePath = dialog.FileName;
                fileName = dialog.SafeFileName;

                labelStatus.Text = fileName;

                return new string[] { filePath, fileName };
            }

            return null;
        }

        private void comboFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!btnSolve.Enabled) btnSolve.Enabled = true;
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            state = Utils.FetchPuzzle(filePath, 0);

            if (!state.Solvable())
            {
                MessageBox.Show("Puzzle is not solvable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            hideControls();

            ThreadStart starter = delegate { Solver.PrioritySolve(ref state); };
            Thread thread = new Thread(starter);
            thread.Start();

            Task.Run(() =>
            {
                thread.Join();

                showControls();

                showPuzzleForm();
            });
        }

        private Size setFormSize() =>
            new Size(tileSize.X * state.RowsCount + 60,
                tileSize.Y * state.RowsCount + 110);

        void showPuzzleForm()
        {
            if (btnSolve.InvokeRequired)
            {
                btnSolve.Invoke(new Action(showPuzzleForm));
                return;
            }

            var formPuzzle = new FormPuzzle(state, tileSize);
            formPuzzle.Text = $"{fileName}({state.RowsCount}x{state.RowsCount})";
            formPuzzle.Size = setFormSize();

            formPuzzle.ShowDialog(this);
        }

        void showControls()
        {
            if (btnSolve.InvokeRequired)
            {
                btnSolve.Invoke(new Action(showControls));
                return;
            }

            panelLoading.Visible = false;
            panelControls.Visible = true;
            loadPuzzleStrip.Enabled = true;
        }

        void hideControls()
        {
            panelLoading.Dock = DockStyle.Fill;
            panelControls.Visible = false;
            panelLoading.Visible = true;
            loadPuzzleStrip.Enabled = false;
        }
    }
}