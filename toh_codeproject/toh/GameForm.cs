using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace toh
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
            this.AllowDrop = true;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            RestartGame();
        }

        void thisBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void thisBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            Disk disk = (Disk)sender;
            disk.Location = new Point(Cursor.Position.X - this.Location.X - (disk.Size.Height / 2),
                                             Cursor.Position.Y - this.Location.Y - (disk.Size.Width / 2));

            if (e.Action == DragAction.Drop)
            {
                int destinationPoleNumber = DeterminePoleFromCursorPosition();
                Pole currentPole = GameState.FindDisk(disk);
                Move move = new Move(currentPole, GameState.Poles[destinationPoleNumber]);

                if (move.IsValid())
                {
                    MakeMove(move);
                }
                else
                {
                    Move moveBack = new Move(currentPole, currentPole);
                    GameState.MakeMove(moveBack);
                }
            }
        }

        private void MakeMove(Move move)
        {
            int moveCount = GameState.MakeMove(move);
            moveCounter.Text = moveCount.ToString();
            if (GameState.IsSolved())
            {   
                possibleToSolve.Text = "Solved :) ";
            }
        }

        private void disk_MouseDown(object sender, MouseEventArgs e)
        {
            Disk disk = (Disk)sender;
            Pole pole = GameState.FindDisk(disk);
            if (!pole.GetTopDisk().Equals(disk))
            {
                return;
            }
            disk.DoDragDrop(disk, DragDropEffects.All);
        }

        private Point GetMousePosition()  {
            return new Point(Cursor.Position.X - this.Location.X, Cursor.Position.Y - this.Location.Y);
        }
        private int DeterminePoleFromCursorPosition()
        {
            Point MousePosition = GetMousePosition();
            if (MousePosition.X < GameState.Poles[0].Location.X)
                return 0;
            if (MousePosition.X > GameState.Poles[1].Location.X - 60 && MousePosition.X < GameState.Poles[2].Location.X - 60)
                return 1;
            if (MousePosition.X > GameState.Poles[2].Location.X - 60 && MousePosition.X < GameState.Poles[2].Location.X + GameConstants.SpaceBetweenPoles)
                return 2;
            return 0;
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem Item = sender as ToolStripDropDownItem;
        }

        private void Form2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void RestartGame()
        {
            RemoveAllDisks();
            GameState.RestartGame();
            AddComponents();
            hints.Text = "";
            hints.Visible = false;
            moveCounter.Text = GameState.MoveCount.ToString();
            possibleToSolve.Text = "Это возможно сделать за " + MoveCalculator.GetMoveCount(GameState.NumberOfDisks).ToString() + " ходов.";
        }

        private void RemoveAllDisks()
        {
            foreach (Pole pole in GameState.Poles)
            {
                foreach (Disk disk in pole.Disks.Values)
                {
                    this.Controls.Remove(disk);
                }
            }
        }

        private void AddComponents()
        {
            PictureBox _base = new PictureBox();
            _base.Image = toh.Properties.Resources._base;
            _base.Size = toh.Properties.Resources._base.Size;
            _base.BackColor = SystemColors.ControlDarkDark;
            _base.Location = new Point(GameConstants.BaseStartPositionX, GameConstants.BaseStartPositionY);

            this.Controls.Add(_base);
            
            moveCounter.Text = GameState.MoveCount.ToString();
            
            foreach (Pole pole in GameState.Poles)
            {
                InitPole(pole);
                foreach (Disk disk in pole.Disks.Values)
                {
                    InitDisk(disk);
                }
            }
        }

        private void InitPole(Pole pole)
        {
            if (!this.Controls.Contains(pole))
            {
                this.Controls.Add(pole);
            }
        }

        private void InitDisk(Disk disk)
        {
            if (!this.Controls.Contains(disk))
            {
                disk.MouseDown += new MouseEventHandler(disk_MouseDown);
                disk.QueryContinueDrag += new QueryContinueDragEventHandler(thisBox_QueryContinueDrag);
                disk.DragOver += new DragEventHandler(thisBox_DragOver);
                this.Controls.Add(disk);
                this.Controls.SetChildIndex(disk, 0);
            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            usernameTextbox.Text = "";
        }
    }
}