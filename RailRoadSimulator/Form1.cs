﻿using RailroadEvents;
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
            //int distanceButtons = 100;
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
            timer.Tick += (UpdateEvents);
            trainLayout.Dispose();//empty the Bitmap
            trainLayout = new Bitmap(background.Width, background.Height); //create new one

            //if there are changes in the simulation
            //update personlayout 
            if (peopleToDraw != manager.trains)
            {
                peopleToDraw.Clear();//empty the list 

                //foreach person in manager.people add it to peopleToDraw
                foreach (KeyValuePair<IEntity, Tile> train in manager.trains)
                {
                    IEntity last = manager.trains.Keys.Last();
                    peopleToDraw.Add(train.Key, train.Value);

                    //if person has a path to walk. go walk
                    if (train.Key.route != null && train.Key.route.Count > 0)
                    {
                        bool wait = false;

                        if (wait == false)//if wait is false. The person is not in a elevator so it can continue moving. 
                        {
                            train.Key.WalkTo();
                            ILayout check = fac.coordinates[train.Key.X, train.Key.Y];
                            if (check != null && fac.coordinates[check.X, check.Y] == check)
                            {
                                train.Key.currentRoom = check;
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

        /// <summary>
        /// Updates the queue for all the persons in the hotel to see what they can do next
        /// </summary>
        /// <param name="sender">/Interval</param>
        /// <param name="e"></param>
        public void UpdateEvents(object sender, EventArgs e)
        {
            IEntity last = manager.people.Keys.LastOrDefault();
            foreach (IEntity person in peopleToDraw.Keys)
            {

                //if (person.id.Contains("Maid")) //checks if the person is from the maid class
                //{
                //    Maid currentMaid = (Maid)person;

                //    if (currentMaid.eventQueue.Count > 0 && currentMaid.eventQueue.FirstOrDefault().LastOrDefault().position == currentMaid.position) //checks if the maid is in the room she needs to clean.
                //    {
                //        currentMaid.eventStarted = true;
                //        if (timerTickCount - currentMaid.timeBusyEvent >= 250 && manager.evacuation == false) //after around 4.8 seconds the maid is done with cleaning room she is in.
                //        {
                //            IRoom start = currentMaid.eventQueue.FirstOrDefault().Last();
                //            if (currentMaid.eventQueue.Count > 0) //removes the room the maid was cleaning out of her toclean list.
                //            {
                //                currentMaid.eventQueue.RemoveFirst();
                //                currentMaid.route.Clear();
                //            }

                //            currentMaid.eventStarted = false;
                //            manager.FinishCleaning(currentMaid, currentMaid.room);//finish cleaning the room

                //        }

                //    }
                //}
                //else
                if (person.areaType.Contains("Person")) //checks if the person is from the customer class
                {

                    if ((person.eventQueue.Count > 0 && person.eventQueue.FirstOrDefault().LastOrDefault().X == person.X && person.eventQueue.FirstOrDefault().LastOrDefault().Y == person.Y))//selects shortest path to the final destination of its event.
                    {

                        person.eventStarted = true;


                        if (timerTickCount - person.timeBusyEvent >= 250 && manager.evacuation == false) //after around 4.8 seconds goes to next event in the queue (LinkedList).
                        {

                            if (manager.evacuation == false)
                            {

                                ILayout start = person.currentRoom;

                                if (person.eventQueue.Count > 0) //Removes first element in the list.
                                {
                                    person.eventQueue.RemoveFirst();
                                    person.route.Clear();
                                }
                                //find new path
                                //if (person.eventQueue.Count == 0 && person.position != person.room.position)
                                //{
                                //    IRoom end = current.room;
                                //    manager.FindPath(start, end, current);
                                //}
                                //if (person.eventQueue.Count > 0)
                                //{
                                //    IRoom end = person.eventQueue.First().Last();
                                //    manager.FindPath(start, end, person);
                                //}
                            }

                            person.eventStarted = false;

                        }
                    }
                }
                ///check if a evac is going on, if so set person.evac to true 
                ///MAYBE USE THIS FOR CLEAN EMERGENCY
                ///if there is no evac start new the new path
                //if (person.eventQueue.Count > 0 && person.route.Count == 0 && person.eventQueue.FirstOrDefault().Count > 1 || manager.evacuation == true && person.evac == false)
                //{
                //    if (manager.evacuation == true)
                //    {
                //        person.evac = true;
                //    }

                //    person.SavePath(person.eventQueue.First());
                //}

            }
            //if (manager.evacuation == true) // if evac is started
            //{

            //    if (manager.hotelEmpty == false)
            //    {
            //        //query to check if all persons are in the lobby
            //        var amount = from person in manager.people.Keys
            //                     where person.position != new Point(1, 0)
            //                     select person;
            //        List<IPerson> check = amount.ToList<IPerson>();

            //        //if all are in lobby start the event
            //        if (check.Count == 0)
            //        {
            //            manager.hotelEmpty = true;
            //            foreach (IPerson person in manager.people.Keys)
            //            {
            //                person.eventStarted = true;
            //                person.timeBusyEvent = timerTickCount;
            //            }
            //        }
            //    }

            //    ///foreach person check if the evac is over
            //    ///if yes, continue where they left off.
            //    foreach (IPerson person in manager.people.Keys)
            //    {
            //        if (manager.evacuation == true && manager.hotelEmpty == true && person.checkedIn == true)
            //        {
            //            //check if evac is over
            //            if (timerTickCount - person.timeBusyEvent >= 250)//check if waitingt time is over, if so return to rooms
            //            {
            //                //check if the last person in the loop, if yes stop the evac
            //                if (last == person)
            //                {
            //                    manager.evacuation = false;
            //                }

            //                person.evac = false;
            //                //remove the evac event from eventqueue
            //                if (person.eventQueue.Count > 0)
            //                {
            //                    person.eventQueue.RemoveFirst();
            //                }

            //                //find path to event that the person was doing before the evac
            //                if (person.eventQueue.Count > 0 && person.eventQueue.FirstOrDefault().Count > 2)
            //                {
            //                    int x = person.eventQueue.First().Last().position.X;
            //                    int y = person.eventQueue.First().Last().position.Y;
            //                    IRoom end = roomFactory.coordinates[x, y];
            //                    person.eventQueue.RemoveFirst();
            //                    if (end.position != new Point(1, 0))
            //                    {

            //                        manager.FindPathEvacuation(person.currentRoom, end, person, false);
            //                        person.SavePath(person.eventQueue.First());
            //                    }

            //                }
            //                ///if the person had no events.
            //                ///customer returns to room.
            //                ///maid stays in lobby
            //                else if (person.room.position != new Point(1, 0))
            //                {
            //                    manager.FindPathEvacuation(person.currentRoom, person.room, person, false);

            //                    person.SavePath(person.eventQueue.First());

            //                }
            //            }
            //        }


            //    }
            //}
            foreach (IEntity person in manager.people.Keys)// if a event hasnt started, set timertickcount equal to person time.
            {

                if (person.eventStarted == false)
                {
                    person.timeBusyEvent = timerTickCount;
                }
            }

            timerTickCount++;
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
