using N_Puzzle_Solver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleVisualizer
{
    public partial class FormPuzzle : Form
    {
        List<State> reversedPath;
        int stateCounter;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Point tileSize = new Point(85, 85);

        public FormPuzzle(State state, Point tileSize)
        {
            InitializeComponent();

            this.tileSize = tileSize;

            this.reversedPath = state.GetPath().ToList();
            stateCounter = reversedPath.Count - 1;

        }

        private void FormPuzzle_Load(object sender, EventArgs e)
        {
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 500;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (stateCounter < 0)
            {
                timer.Stop();
                return;
            }

            renderPuzzle(reversedPath[stateCounter]);
            stateCounter--;
        }

        private void renderPuzzle(State puzzle)
        {
            clearOldRender();

            for (int i = 0; i < puzzle.Size; i++)
            {
                if (puzzle.Content[i] == 0) continue;

                int x = i % puzzle.RowsCount;
                int y = i / puzzle.RowsCount;

                Label label = new TileLabel()
                {
                    Name = $"tile{i}",
                    Size = new Size(tileSize),
                    Location = new Point(x * tileSize.X + 20, y * tileSize.Y + 20),
                    Text = puzzle.Content[i].ToString()
                };

                Controls.Add(label);

            }
        }

        private void clearOldRender()
        {
            Controls.Clear();
        }
    }

    internal class TileLabel : Label
    {
        public TileLabel()
        {
            BackColor = BackColor = Color.FromKnownColor(KnownColor.ControlLight);
            Font = new Font(new FontFamily("Segoe UI"), 16.2f, FontStyle.Bold);
            TextAlign = ContentAlignment.MiddleCenter;
            BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
