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
        private double PlayPosition { get; set; }

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
                targetPos = new Vector3(-2000, 0, 0)
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
                pos = Chased.pos + new Vector3(110, 0, 0),
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
            Camera.pos = Chased.pos + new Vector3(-70, 2, 5);
            Camera.look = ChaserLeader.pos + new Vector3(10, 0, 0) - Camera.pos;
            Camera.up = Vector3.Up;
        }

        private void InitShotList()
        {
            ShotList = new List<Shot>();
            CurrentShot = 0;

            ShotList.Add(new Shot
            {
                EndTime = 3.91,
                Action = elapsedSeconds =>
                {
                    Camera.look = ChaserLeader.pos + new Vector3(10, 0, 0) - Camera.pos;
                    Camera.up = Vector3.Up;
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 8.54,
                InitialAction = () =>
                {
                    Camera.pos = Chased.pos + new Vector3(0, 2, 10);
                    Camera.look = Chased.pos + new Vector3(-800, 0, -100) - Camera.pos;
                    Camera.up = Vector3.Up;
                },
                Action = elapsedSeconds =>
                {
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 13.91,
                InitialAction = () =>
                {
                    InitAsteroids();

                    Camera.pos = Chased.pos + new Vector3(-100, -20, 20);
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);

                    ChaserLeader.SteeringBehaviours.turnOff(SteeringBehaviours.behaviour_type.pursuit);
                },
                Action = elapsedSeconds =>
                {
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 20.98,
                InitialAction = () =>
                {
                    Camera.pos = ChaserLeader.pos + new Vector3(-200, 8, 5);
                    Camera.look = ChaserLeader.pos - Camera.pos;
                    Camera.up = Vector3.Up;

                    ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
                },
                Action = elapsedSeconds =>
                {
                    Camera.look = ChaserLeader.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 24,
                InitialAction = () =>
                {
                    var asteroidList = new List<Asteroid>
                    {
                        new Asteroid(30)
                        {
                            pos = Chased.pos + new Vector3(-60, 0, -5)
                        },
                        new Asteroid(30)
                        {
                            pos = Chased.pos + new Vector3(-60, 0, -70)
                        }
                    };

                    foreach (var asteroid in asteroidList)
                    {
                        asteroid.LoadContent();
                        Game.Children.Add(asteroid);
                    }

                    ChaserLeader.SteeringBehaviours.turnOff(SteeringBehaviours.behaviour_type.pursuit);

                    Chased.targetPos = Chased.pos + new Vector3(-100, 0, -35);
                    Chased.look = Vector3.Normalize(new Vector3(-1, 0, 1.5f));
                    Chased.velocity = Chased.look * Chased.velocity.Length();

                    Camera.pos = Chased.pos + new Vector3(-60, 0, 40);
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                },
                Action = elapsedSeconds =>
                {
                    Camera.pos += elapsedSeconds * new Vector3(-50, 0, -1);
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 30,
                InitialAction = () =>
                {
                    Chased.targetPos = Chased.pos + new Vector3(75, 10, -10);
                },
                Action = elapsedSeconds =>
                {
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 33,
                InitialAction = () =>
                {
                    Chased.pos = Chased.pos + new Vector3(-200, 0, 0);
                    Chased.look = Vector3.Left;
                    Chased.targetPos = Chased.pos + new Vector3(-200, 0, 0);
                },
                Action = elapsedSeconds =>
                {
                    Camera.pos = Chased.pos + new Vector3(20, 5, 0);
                    Camera.look = Chased.pos - Camera.pos;
                    Camera.up = Vector3.Up;
                }
            });

            ShotList.Sort();
        }

        private void InitAsteroids()
        {
            var entryPos = Chased.pos;

            var asteroidList = new List<Asteroid>
            {
                new Asteroid(10)
                {
                    pos = entryPos + new Vector3(-500, 20, 50)
                },
                new Asteroid(10)
                {
                    pos = entryPos + new Vector3(-500, -10, -50)
                },
                new Asteroid(10)
                {
                    pos = entryPos + new Vector3(-450, -50, -10)
                },
                new Asteroid(10)
                {
                    pos = entryPos + new Vector3(-550, 30, -10)
                },
                new Asteroid(10)
                {
                    pos = entryPos + new Vector3(-550, -10, 50)
                }
            };

            foreach (var asteroid in asteroidList)
            {
                asteroid.LoadContent();
                Game.Children.Add(asteroid);
            }
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentShot >= ShotList.Count) return;

            var totalSeconds = MoviePlayer.PlayPosition.TotalSeconds;

            if (totalSeconds < PlayPosition) return;

            var elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            PlayPosition += elapsedSeconds;

            if (totalSeconds > ShotList[CurrentShot].EndTime)
            {
                ++CurrentShot;
                if (CurrentShot >= ShotList.Count) return;

                ShotList[CurrentShot].InitialAction();
            }

            ShotList[CurrentShot].Action((float)elapsedSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void UnloadContent()
        {
        }
    }
}
