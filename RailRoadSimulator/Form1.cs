using RailroadEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private Timer timerForMaids { get; } = new Timer();
        private Timer slowTimer { get; } = new Timer();
        private int timerTickCount { get; set; }
        private bool slowBool { get; set; } = false;
        private int amount { get; set; } = 0;

		//background where to draw the layout on
		private Bitmap background;
		//background where to draw the trains on
		private Bitmap trainLayout;
		//Make Manager class with dll and its methods
		private Manager manager;
        private Draw draw = new Draw();
        private LayoutFactory fac = new LayoutFactory();

        public List<IEntity> thingsToDraw = new List<IEntity>();
        public List<IEntity> newPeople = new List<IEntity>();
        //private Factory

        int endX = 0;
        int endY = 0;

        //Form atributes
        private int buttonCounter { get; set; }
		public Button pauseButton = new Button();
		public Button speedButton = new Button();
		public Button stopButton = new Button();

		public MainForm()
		{
			//set the tick frequency
			timer.Interval = 150;
            timerForMaids.Interval = 150;
            slowTimer.Enabled = false;
            fac.GenerateEntity();
            manager = new Manager(fac.coordinates, this);
            background = draw.DrawLayout(fac.coordinates);
            trainLayout = new Bitmap(background.Width, background.Height);

			InitializeComponent();
            railRoadMap.BackgroundImage = background;


            //Start the railroad events 
            RailroadEventManager.Start();
			timer.Start();
            timerForMaids.Start();

            //call the update function after each timer tick.
            timer.Tick += new EventHandler(UpdateImage);
            timerForMaids.Tick += (UpdateEvents);
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

            //if leaves is called
            if (manager.Leaves.Value == true) {
                manager.Leaves = new KeyValuePair<int, bool>(manager.Leaves.Key, false);
                //multiply by 1000 key is given in seconds
                slowTimer.Interval = manager.Leaves.Key * 1000;
                RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor / 2f;
                timer.Interval = timer.Interval * 2;
                slowTimer.Start();
                //set amount and enabled back to 0 to make sure this event can happen again
                slowTimer.Enabled = true;
                amount = 0;
            }

            //check amount to ensure it only happens one time
            //check enabled to ensure it doesnt go at the start of the application
            if (slowTimer != null && amount == 0 && slowTimer.Enabled == true) {

                slowTimer.Tick += new EventHandler(SpeedUp);
                amount++;

            }

            //we use to list because else the list may be updated while this function is still running
            if (newPeople != manager.people.ToList())
            {
                newPeople.Clear();

                foreach (var people in manager.people.ToList())
                {
                    newPeople.Add(people);
                    if (people.areaType.Contains("Person")) {
                        var pers = (Person)people;
                        //check if person has died, if so remove it
                        bool hasDied = pers.isWaiting();
                        if (hasDied) {
                            manager.people.Remove(pers);
                            newPeople.Remove(pers);
                        }
                    }

                }

            }

            //if there are changes in the simulation
            //update personlayout 
            if (thingsToDraw != manager.trains.ToList())
            {
                thingsToDraw.Clear();//empty the list 

                //foreach person in manager.people add it to peopleToDraw
                foreach (Train train in manager.trains.ToList())
                {
                    //IEntity last = manager.trains.Last();
                    thingsToDraw.Add(train);

                    //if person has a path to walk. go walk
                    if (train.route != null && train.route.Count > 0)
                    {
                        bool wait = false;

                        if (wait == false)//if wait is false. The person is not in a elevator so it can continue moving. 
                        {
                            ILayout check = fac.coordinates[train.X, train.Y];
                            if (check != null && fac.coordinates[check.X, check.Y] == check)
                            {
                                train.currentRoom = check;
                                if (train.currentRoom.areaType.Contains("Station") && train.personsInTrain.Count < train.capacity && manager.people.Count > 0) {
                                    //this works now but if no one checks in we need to continue
                                    manager.CheckIfPeopleAtStation((Train)train);
                                    train.waitCount = true;
                                }
                                if (train.waitCount) {
                                    train.waitAmount++;
                                }
                                if (train.waitAmount == 5)
                                {
                                    //Console.WriteLine("Train has waited 5 ticks");
                                    train.waitAmount = 0;
                                    train.waitCount = false;
                                }
                                if (!train.waitCount)
                                {
                                    train.WalkTo(train.currentRoom);
                                }
                            }

                        }
                        wait = false;
                    }

                }

            }
            foreach (IEntity people in manager.people.ToList())
            {
                if (people.areaType.Contains("Maid")) //checks if the person is from the maid class
                {
                    Maid currentMaid = (Maid)people;

                    currentMaid.eventStarted = true;
                    if (timerTickCount - currentMaid.timeBusyEvent >= 250 && manager.evacuation == false) //after around 4.8 seconds the maid is done with cleaning room she is in.
                    {
                        currentMaid.eventStarted = false;
                        //manager.FinishCleaning(currentMaid, currentMaid.room);//finish cleaning the room
                    }
                    //add to the to draw list
                    thingsToDraw.Add(currentMaid);
                }
                if (people.areaType.Contains("Person")) {
                    Person current = (Person)people;
                    thingsToDraw.Add(current);
                }
            }
            //draw the new personLayout and background
            background.Dispose();
            background = draw.DrawLayout(fac.coordinates);
            trainLayout = draw.DrawPersonLayout(trainLayout, thingsToDraw);

            //Console.WriteLine("DISPLAYED NEW IMAGE");
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
            foreach (IEntity train in thingsToDraw.ToList())
            {
                if (train.areaType.Contains("Train")) //checks if the person is from the customer class
                {
                    var current = (Train)train;
                    //we want to save the endX and endY only once until end is reached
                    if (current.Xend == 0 && current.Yend == 0) {
                        current.Xend = current.firstX;
                        current.Yend = current.firstY;
                    }
                    if (train.currentRoom != null && train.eventQueue.Count == 0 && current.Xend == train.X && current.Yend == train.Y)
                    {
                        current.Xend = train.endX;
                        current.Yend = train.endY;


                        current.hasPath = false;
                        if (current.personsInTrain.Count > 0) {
                            //checkout persons because they are at their station
                            manager.CheckOutPeople(current);
                        }

                        //if the capacity of the train is not filled after checkout
                        if (current.capacity > current.personsInTrain.Count) {
                            //check if there are persons add that station if so let them go in
                            manager.CheckIfPeopleAtStation(current);
                            //when this is called train teleports for some reason
                            //if (current.personsInTrain.Count == 0 && manager.ReturnToRemisePair.First().Value == true)
                            //{
                            //    if (current.currentRoom.areaType.Contains("Remise"))
                            //    {
                            //        manager.trains.Remove(current);
                            //    }
                            //    manager.ReturnToRemise(current);
                            //}
                        }
                        if (!current.hasPath) {
                            manager.GoToNextStation(current);
                        }
                        if (current.personsInTrain.Count == 0 && manager.people.Count == 0) {
                            //current.isCleaning = true;
                            //start cleaning
                        }
                        foreach (var item in manager.ReturnToRemisePair) {
                            if (current.personsInTrain.Count == 0 && item.Value && current.startLocation == item.Key && !current.isCleaning) {
                                Console.WriteLine("Return to remise with this train");
                                //this works but last train doesnt go to remise for some reason
                                manager.ReturnToRemise(current);
                                manager.ReturnToRemisePair.Remove(item);
                                break;
                            }
                        }
                        if (current.currentRoom.areaType.Contains("Remise")) {
                            manager.trains.Remove(current);
                            thingsToDraw.Remove(current);
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
            foreach (IEntity person in manager.trains)// if a event hasnt started, set timertickcount equal to person time.
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

        private void SpeedButton_Click(object sender, EventArgs e)
        {


                RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor / 2f;
                timer.Interval = timer.Interval * 2;
                speedButton.Text = "Versnellen";
                buttonCounter++;
            
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

        /// <summary>
        /// Speeds up the event by 2 times the speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SpeedUp(object sender, EventArgs e)
        {
            //Set slowtimer.enabled it to false again so the event can be fired again
            slowTimer.Enabled = false;
            RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor * 2f;
            timer.Interval = timer.Interval / 2;
            Console.WriteLine("sneller");
        }
    }
}
