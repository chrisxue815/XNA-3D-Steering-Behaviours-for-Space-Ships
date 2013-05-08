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

            InitScenario();
            InitShotList();
        }

        private void InitScenario()
        {
            // chased
            Chased = new Fighter();
            Chased.ModelName = "Ship1";
            Chased.pos = Vector3.Zero;
            Chased.look = Vector3.Left;
            Chased.up = Vector3.Up;
            Chased.targetPos = new Vector3(-1000, 0, 0);
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            Chased.LoadContent();
            Game.Children.Add(Chased);

            // constants for chasers
            const String chaserModelName = "fighter";

            // chaser leader
            ChaserLeader = new Fighter();
            ChaserLeader.ModelName = chaserModelName;
            ChaserLeader.pos = Chased.pos + new Vector3(200, 0, 0);
            ChaserLeader.look = Vector3.Left;
            ChaserLeader.up = Vector3.Up;
            ChaserLeader.Target = Chased;
            ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            ChaserLeader.LoadContent();
            Game.Children.Add(ChaserLeader);

            // chaser fleet
            var fleetOffset = new[]
            {
                new Vector3(5, 0, -5),
                new Vector3(5, 5, 0),
                new Vector3(5, 0, 5)
            };

            ChaserFleet = new List<Fighter>(fleetOffset.Length);

            foreach (var offset in fleetOffset)
            {
                var fighter = new Fighter();
                fighter.ModelName = chaserModelName;
                fighter.Leader = ChaserLeader;
                fighter.offset = offset;
                fighter.pos = fighter.Leader.pos + fighter.offset;
                fighter.look = Vector3.Left;
                fighter.up = Vector3.Up;
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                fighter.LoadContent();

                ChaserFleet.Add(fighter);

                Game.Children.Add(fighter);
            }

            // camera
            var camera = Game.Camera;
            camera.ControlledByUser = false;
            camera.pos = Chased.pos + new Vector3(-40, 2, 5);
            camera.look = Chased.pos - camera.pos;
            camera.up = Vector3.Up;
        }

        private void InitShotList()
        {
            ShotList = new List<Shot>();
            CurrentShot = 0;
            Shot shot;

            shot = new Shot();
            shot.EndTime = 3.91;
            shot.Action = totalSeconds =>
            {
            };
            ShotList.Add(shot);

            //shot = new Shot();
            //shot.EndTime = 8.54;
            //shot.Action = totalSeconds =>
            //{
            //};
            //ShotList.Add(shot);

            //shot = new Shot();
            //shot.EndTime = 13.91;
            //shot.Action = totalSeconds =>
            //{
            //};
            //ShotList.Add(shot);

            ShotList.Sort();
        }

        private void InitAsteroids()
        {
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentShot >= ShotList.Count) return;

            var totalSeconds = gameTime.TotalGameTime.TotalSeconds;
            var elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (totalSeconds > ShotList[CurrentShot].EndTime)
            {
                ++CurrentShot;
                if (CurrentShot >= ShotList.Count) return;

                ShotList[CurrentShot].InitialAction(totalSeconds);

                elapsedSeconds = totalSeconds - ShotList[CurrentShot - 1].EndTime;
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
