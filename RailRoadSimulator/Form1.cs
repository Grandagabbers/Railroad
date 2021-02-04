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

        private Point cleanLoc { get; set; }


		public MainForm()
		{
			//set the tick frequency
			timer.Interval = 50;
            timerForMaids.Interval = 50;
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

            //if emergencyclean is called get the train at this station to stop and clean
            if (manager.emergencyClean.Value) {
                cleanLoc = new Point(manager.emergencyClean.Key.X, manager.emergencyClean.Key.Y);
                //set emergency clean back to false so it can be called again
                manager.emergencyClean = new KeyValuePair<Point, bool>(manager.emergencyClean.Key, false);
            }

            //if leaves is called
            if (manager.Leaves.Value) {
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
                    if (people != null && people.areaType.Contains("Person")) {
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
                    if (train.route != null && train.route.Count > 0 && !train.isCleaning)
                    {
                            ILayout check = fac.coordinates[train.X, train.Y];
                            if (check != null && fac.coordinates[check.X, check.Y] == check)
                            {
                                train.currentRoom = check;
                                if (train.currentRoom.areaType.Contains("Station") && train.personsInTrain.Count < train.capacity && manager.people.Count > 0 && !train.returning) {
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
                    if (train.isCleaning)
                    {
                        train.cleanWait++;
                    }
                    if (train.cleanWait == 20)
                    {
                        train.isCleaning = false;
                        train.cleanWait = 0;
                        //remove item in list
                        manager.AddPeopleBack(train);
                        Console.WriteLine("Train has been cleaned");
                    }
                    if (train.returning && train.currentRoom.areaType.Contains("Remise"))
                    {
                        manager.trains.Remove(train);
                    }

                }

            }
            foreach (IEntity people in manager.people.ToList())
            {
                if (people.areaType.Contains("Maid")) //checks if the person is from the maid class
                {
                    Maid currentMaid = (Maid)people;

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
                    //cast the variable in a train, since it is a train
                    var current = (Train)train;

                    //if cleanloc is not null emergency clean is called at this location check if train is at this location
                    if (cleanLoc != null && current.X == cleanLoc.X && current.Y == cleanLoc.Y)
                    {
                        //set is cleaning to true because train has to be cleaned
                        current.isCleaning = true;
                        //start cleaning
                        manager.StartCleaning(current);
                        //set cleanloc to nothing so it can be called again
                        cleanLoc = new Point();
                    }

                    //we want to save the endX and endY only once until end is reached
                    if (current.Xend == 0 && current.Yend == 0) {
                        current.Xend = current.firstX;
                        current.Yend = current.firstY;
                    }
                    if (current.currentRoom != null && current.eventQueue.Count == 0 && current.Xend == current.X && current.Yend == current.Y)
                    {
                        current.Xend = train.endX;
                        current.Yend = train.endY;

                        //train does not have a path anymore
                        current.hasPath = false;

                        //if there are person in the train
                        if (current.personsInTrain.Count > 0 && !current.isCleaning && !current.returning) {
                            //checkout persons if they are at their station
                            manager.CheckOutPeople(current);
                        }

                        //if the capacity of the train is not filled after checkout
                        if (current.personsInTrain.Count < current.capacity && !current.isCleaning && !current.returning) {
                            //check if there are persons add that station if so let them go in
                            manager.CheckIfPeopleAtStation(current);
                        }
                        if (!current.hasPath && !current.returning) {
                            manager.GoToNextStation(current);
                        }
                        //make a list to add only persons not maids, use this list to check if there are still people waiting
                        List<IEntity> onlyPeople = new List<IEntity>();
                        foreach (var item in manager.people.ToList()) {
                            if (item.areaType.Contains("Person")) {
                                onlyPeople.Add(item);
                            }
                        }
                        //since train is at endstation, check if people are in train or waiting at stations
                        if (current.personsInTrain.Count == 0 && !current.isCleaning && onlyPeople.Count == 0 && !current.hasBeenCleaned)
                        {
                            //set is cleaning to true because train will now be cleaned and cant move
                            current.isCleaning = true;
                            //start cleaning
                            manager.StartCleaning(current);
                            current.hasBeenCleaned = true;
                        }
                        
                        foreach (var item in manager.ReturnToRemisePair) {
                            if (current.personsInTrain.Count == 0 && item.Value && !current.isCleaning && !current.returning && current.startLocation == item.Key)// && current.currentRoom.Parent.whatIsIt.ToString() == item.Key &&) 
                            {
                                    current.returning = true;
                                    Console.WriteLine("Return to remise with this train");
                                    //this works but last train doesnt go to remise for some reason
                                    manager.ReturnToRemise(current);
                                    manager.ReturnToRemisePair.Remove(item);
                                    break;
                            }
                        }

                    }
                }

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
