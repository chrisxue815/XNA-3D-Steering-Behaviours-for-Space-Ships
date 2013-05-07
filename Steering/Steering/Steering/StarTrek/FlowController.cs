using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Steering
{
    public class FlowController : Entity
    {
        private XNAGame Game { get; set; }

        private Fighter Chased { get; set; }
        private Fighter ChaserLeader { get; set; }
        private List<Fighter> ChaserFleet { get; set; }

        private static readonly double[] SceneStartTime = {3.91, 8.54, 13.91};
        private List<Action<double>> ActionList { get; set; }
        private int CurrentScene { get; set; }


        public FlowController()
        {
            Game = XNAGame.Instance();

            InitSpaceships();
            InitActionList();
        }

        private void InitObstacles()
        {
            
        }

        private void InitSpaceships()
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

        private void InitActionList()
        {
            CurrentScene = 0;

            ActionList = new List<Action<double>>(SceneStartTime.Length);

            ActionList.Add(totalSeconds =>
            {
            });
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentScene >= SceneStartTime.Length) return;

            var totalSeconds = gameTime.TotalGameTime.TotalSeconds;
            ActionList[CurrentScene](totalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void UnloadContent()
        {
        }
    }
}
