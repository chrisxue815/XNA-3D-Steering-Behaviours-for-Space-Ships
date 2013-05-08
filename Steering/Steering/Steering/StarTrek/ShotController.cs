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
        private Camera Camera { get { return Game.Camera; } }

        private Fighter Chased { get; set; }
        private Fighter ChaserLeader { get; set; }
        private List<Fighter> ChaserFleet { get; set; }

        private List<Shot> ShotList { get; set; }
        private int CurrentShot { get; set; }

        private MoviePlayer MoviePlayer { get; set; }

        private SkyBox SkyBox { get; set; }        

        public ShotController()
        {
            Game = XNAGame.Instance();

            InitScenario();
            InitShotList();

            MoviePlayer = new MoviePlayer();
            Game.Children.Add(MoviePlayer);

            SkyBox = new SkyBox();
            Game.Children.Add(SkyBox);
        }

        private void InitScenario()
        {
            // chased
            Chased = new Fighter
            {
                ModelName = "Ship1",
                pos = Vector3.Zero,
                look = Vector3.Left,
                up = Vector3.Up,
                targetPos = new Vector3(-1000, 0, 0)
            };
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            Chased.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            Chased.LoadContent();
            Game.Children.Add(Chased);

            // constants for chasers
            const String chaserModelName = "fighter";

            // chaser leader
            ChaserLeader = new Fighter
            {
                ModelName = chaserModelName,
                pos = Chased.pos + new Vector3(100, 0, 0),
                look = Vector3.Left,
                up = Vector3.Up,
                Target = Chased
            };
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
                var fighter = new Fighter
                {
                    ModelName = chaserModelName,
                    Leader = ChaserLeader,
                    offset = offset,
                    pos = ChaserLeader.pos + offset,
                    look = Vector3.Left,
                    up = Vector3.Up
                };
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                fighter.LoadContent();

                ChaserFleet.Add(fighter);

                Game.Children.Add(fighter);
            }

            // Camera
            Camera.ControlledByUser = false;
            Camera.pos = Chased.pos + new Vector3(-40, 2, 5);
            Camera.look = Chased.pos - Camera.pos;
            Camera.up = Vector3.Up;
        }

        private void InitShotList()
        {
            ShotList = new List<Shot>();
            CurrentShot = 0;

            ShotList.Add(new Shot
            {
                EndTime = 3.91,
                Action = totalSeconds =>
                {
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 8.54,
                InitialAction = totalSeconds =>
                {
                    Camera.pos = Chased.pos + new Vector3(0, 2, 10);
                    Camera.look = Chased.pos + new Vector3(-800, 0, 0) - Camera.pos;
                    Camera.up = Vector3.Up;
                },
                Action = totalSeconds =>
                {
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 13.91,
                InitialAction = totalSeconds =>
                {
                    InitAsteroids();
                    Camera.pos = Chased.pos + new Vector3(-100, -20, 20);
                    Camera.look = Chased.pos - Camera.pos;
                    Camera.up = Vector3.Up;
                },
                Action = totalSeconds =>
                {
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 20.98,
                InitialAction = totalSeconds =>
                {
                    Camera.pos = Chased.pos + new Vector3(-40, 2, 5);
                    Camera.look = ChaserLeader.pos - Camera.pos;
                    Camera.up = Vector3.Up;
                },
                Action = totalSeconds =>
                {
                }
            });

            ShotList.Sort();
        }

        private void InitAsteroids()
        {
            var pos = Chased.pos;

            Asteroid asteroid;

            const float minRadius = 2f;
            const float maxRadius = 10f;
            const float maxDiff = maxRadius - minRadius;
            var random = new Random();

            float size;
            size = minRadius + (float)random.NextDouble() % maxDiff;
            asteroid = new Asteroid(size);

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
