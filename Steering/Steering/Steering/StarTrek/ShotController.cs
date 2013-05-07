using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering
{
    public class ShotController : Entity
    {
        private XNAGame Game { get; set; }

        private Fighter Chased { get; set; }
        private Fighter ChaserLeader { get; set; }
        private List<Fighter> ChaserFleet { get; set; }

        private List<Shot> ShotList { get; set; }
        private int CurrentShot { get; set; }

        public ShotController()
        {
            Game = XNAGame.Instance();

            InitShotList();
        }

        private void InitShotList()
        {
            ShotList = new List<Shot>();
            CurrentShot = 0;
            Shot shot = null;

            shot = new Shot();
            shot.StartTime = 3.91;
            shot.InitialAction = InitSpaceships;
            shot.Action = totalSeconds =>
            {
            };
            ShotList.Add(shot);

            shot = new Shot();
            shot.StartTime = 8.54;
            shot.Action = totalSeconds =>
            {
            };
            ShotList.Add(shot);

            shot = new Shot();
            shot.StartTime = 13.91;
            shot.Action = totalSeconds =>
            {
            };
            ShotList.Add(shot);

            ShotList.Sort();
        }

        private void InitSpaceships(double totalSeconds)
        {
            // chased
            Chased = new Fighter();
            Chased.ModelName = "Ship1";
            Chased.pos = new Vector3(0, 0, 0);
            Chased.targetPos = new Vector3(0, 200, -450);
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            Game.Children.Add(Chased);

            // constants for chasers
            const String chaserModelName = "fighter";
            const int chaserFleetSize = 3;

            // chaser leader
            ChaserLeader = new Fighter();
            ChaserLeader.ModelName = chaserModelName;
            ChaserLeader.pos = new Vector3(1000, 0, 0);
            ChaserLeader.Target = Chased;
            ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            Game.Children.Add(ChaserLeader);

            // chaser fleet
            ChaserFleet = new List<Fighter>(chaserFleetSize);
            var offset = new[]
            {
                new Vector3(50, 0, -50),
                new Vector3(50, 50, 0),
                new Vector3(50, 0, 50)
            };

            for (var i = 0; i < chaserFleetSize; i++)
            {
                var fighter = new Fighter();
                fighter.ModelName = chaserModelName;
                fighter.Leader = ChaserLeader;
                fighter.offset = offset[i];
                fighter.pos = fighter.Leader.pos + fighter.offset;
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);

                ChaserFleet[i] = fighter;

                Game.Children.Add(fighter);
            }
        }

        public override void LoadContent()
        {
            Chased.LoadContent();
            ChaserLeader.LoadContent();
            foreach (var fighter in ChaserFleet)
            {
                fighter.LoadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentShot >= ShotList.Count) return;

            var totalSeconds = gameTime.TotalGameTime.TotalSeconds;
            var elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (totalSeconds > ShotList[CurrentShot].StartTime)
            {
                ++CurrentShot;
                if (CurrentShot >= ShotList.Count) return;

                ShotList[CurrentShot].InitialAction(totalSeconds);
            }

            ShotList[CurrentShot].Action(elapsedSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void UnloadContent()
        {
        }
    }
}
