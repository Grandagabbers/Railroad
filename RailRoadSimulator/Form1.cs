using RailroadEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadSimulator
{
	public partial class MainForm : Form
	{
		public Timer timer { get; } = new Timer();

		private int timerTickCount { get; set; }

		//background where to draw the layout on
		private Bitmap background;
		//background where to draw the trains on
		private Bitmap trainLayout;
		//Make Manager class with dll and its methods
		private Manager manager;
        private Draw draw = new Draw();
        private LayoutFactory fac = new LayoutFactory();

        public Dictionary<IEntity, Tile> peopleToDraw = new Dictionary<IEntity, Tile>();
        //private Factory


        //Form atributes
        private int buttonCounter { get; set; }
		public Button pauseButton = new Button();
		public Button speedButton = new Button();
		public Button stopButton = new Button();

		public MainForm()
		{
			//set the tick frequency
			timer.Interval = 1000;
            fac.GenerateEntity();
            manager = new Manager(fac.coordinates, this);
            background = draw.DrawLayout(fac.coordinates);
            trainLayout = new Bitmap(background.Width, background.Height);

			InitializeComponent();
            railRoadMap.BackgroundImage = background;

            //Setup buttons
            int distanceButtons = 100;
            //pauseButton.Location = new Point(background.Width + distanceButtons, railRoadMap.Location.Y + distanceButtons);
            //pauseButton.Size = new Size(250, 100);
            //pauseButton.Text = "Pauze";
            //pauseButton.Click += new EventHandler(PauseButton_Click);
            //speedButton.Location = new Point(background.Width + distanceButtons, pauseButton.Height + distanceButtons);
            //speedButton.Size = pauseButton.Size;
            //speedButton.Text = "Versnellen";
            //speedButton.Click += new EventHandler(SpeedButton_Click);
            //stopButton.Location = new Point(background.Width + distanceButtons, speedButton.Location.Y + distanceButtons);
            //stopButton.Size = pauseButton.Size;
            //stopButton.Text = "Stop";
            //stopButton.Click += new EventHandler(StopButton_Click);
            //Controls.Add(pauseButton);
            //Controls.Add(speedButton);
            //Controls.Add(stopButton);


            //Start the hotel events 
            RailroadEventManager.Start();
			timer.Start();

            //call the update function after each timer tick.
            timer.Tick += new EventHandler(UpdateImage);
        }

        /// <summary>
        /// update the displayed image in the picturebox of the form so there can be animations displayed.
        /// </summary>
        /// <param name="sender">interval</param>
        /// <param name="e"></param>
        private void UpdateImage(object sender, EventArgs e)
        {
            trainLayout.Dispose();//empty the Bitmap
            trainLayout = new Bitmap(background.Width, background.Height); //create new one

            //if there are changes in the simulation
            //update personlayout 
            if (peopleToDraw != manager.people)
            {
                peopleToDraw.Clear();//empty the list 

                //foreach person in manager.people add it to peopleToDraw
                foreach (KeyValuePair<IEntity, Tile> person in manager.people)
                {
                    IEntity last = manager.people.Keys.Last();
                    peopleToDraw.Add(person.Key, person.Value);

                    //if person has a path to walk. go walk
                    if (person.Key.route != null && person.Key.route.Count > 0)
                    {
                        bool wait = false;

                        if (wait == false)//if wait is false. The person is not in a elevator so it can continue moving. 
                        {
                            person.Key.WalkTo();
                            ILayout check = fac.coordinates[person.Key.X, person.Key.Y];
                            if (check != null && fac.coordinates[check.X, check.Y] == check)
                            {
                                person.Key.currentRoom = check;
                            }

                        }
                        wait = false;
                    }

                }

            }
            //draw the new personLayout and background
            background.Dispose();
            background = draw.DrawLayout(fac.coordinates);
            trainLayout = draw.DrawPersonLayout(trainLayout, peopleToDraw);

            Console.WriteLine("DISPLAYED NEW IMAGE");
            //dislay new persenLayout and background
            railRoadMap.BackgroundImage = background;
            railRoadMap.Image = trainLayout;


        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void railRoadMap_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Exit the app 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Pause the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (buttonCounter % 2 == 0 || buttonCounter == 0)
            {
                RailroadEventManager.Pauze();
                timer.Stop();
                pauseButton.Text = "Hervat";
                buttonCounter++;
            }
            else
            {
                RailroadEventManager.Start();
                timer.Start();
                pauseButton.Text = "Pauze";
                buttonCounter++;
            }
        }

        /// <summary>
        /// Speed up or slow down the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeedButton_Click(object sender, EventArgs e)
        {
            if (buttonCounter % 2 == 0 || buttonCounter == 0)//speed up
            {
                RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor * 2f;
                timer.Interval = timer.Interval / 2;
                speedButton.Text = "Vertragen";
                buttonCounter++;
            }
            else//slow down
            {
                RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor / 2f;
                timer.Interval = timer.Interval * 2;
                speedButton.Text = "Versnellen";
                buttonCounter++;
            }
        }

        /// <summary>
        /// Stop the simulation and go back to setting menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, EventArgs e)
        {
            RailroadEventManager.Stop();
            this.Hide();
            //settings.Show();
        }
    }
}
