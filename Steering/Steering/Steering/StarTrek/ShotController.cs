using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Particle3DSample;

namespace Steering
{
    public class ShotController : Entity
    {
        private XNAGame Game { get; set; }
        private Camera Camera { get { return Game.Camera; } }
        private GameTime GameTime { get; set; }
        private float ElapsedSeconds { get { return (float)GameTime.ElapsedGameTime.TotalSeconds; } }

        private Fighter Chased { get; set; }
        private Fighter ChaserLeader { get; set; }
        private List<Fighter> ChaserFleet { get; set; }

        private List<Shot> ShotList { get; set; }
        private int CurrentShot { get; set; }

        private MoviePlayer MoviePlayer { get; set; }
        private double PlayPosition { get; set; }

        private SkySphere SkyBox { get; set; }

        private List<Projectile> Projectiles { get; set; }
        private ParticleSystem ExplosionParticles { get; set; }
        private ParticleSystem ExplosionSmokeParticles { get; set; }
        private ParticleSystem ProjectileTrailParticles { get; set; }
        private double TimeToNextProjectile { get; set; }

        public ShotController()
        {
            Game = XNAGame.Instance();

            InitScenario();
            InitShotList();

            MoviePlayer = new MoviePlayer();
            Game.Children.Add(MoviePlayer);

            SkyBox = new SkySphere();
            Game.Children.Add(SkyBox);

            Projectiles = new List<Projectile>();
            ExplosionParticles = new ExplosionParticleSystem(Game, Game.Content);
            ExplosionSmokeParticles = new ExplosionSmokeParticleSystem(Game, Game.Content);
            ProjectileTrailParticles = new ProjectileTrailParticleSystem(Game, Game.Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            ExplosionSmokeParticles.DrawOrder = 200;
            ProjectileTrailParticles.DrawOrder = 300;
            ExplosionParticles.DrawOrder = 400;

            Game.Components.Add(ExplosionParticles);
            Game.Components.Add(ExplosionSmokeParticles);
            Game.Components.Add(ProjectileTrailParticles);
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
                    up = Vector3.Up,
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
                Action = () =>
                {
                    Camera.look = ChaserLeader.pos + new Vector3(10, 0, 0) - Camera.pos;
                    Camera.up = Vector3.Up;
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 5.9,
                InitialAction = () =>
                {
                    Camera.pos = Chased.pos + new Vector3(0, 2, 10);
                    Camera.look = Chased.pos + new Vector3(-800, 0, -100) - Camera.pos;
                    Camera.up = Vector3.Up;
                },
                Action = () =>
                {
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 8.54,
                InitialAction = () =>
                {
                },
                Action = () =>
                {
                    Camera.pos += ElapsedSeconds * new Vector3(-50, 0, 0);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 13.91,
                InitialAction = () =>
                {
                    var asteroidList = new List<Asteroid>
                    {
                        new Asteroid(10) {pos = Chased.pos + new Vector3(-500, 20, 50)},
                        new Asteroid(10) {pos = Chased.pos + new Vector3(-500, -10, -50)},
                        new Asteroid(10) {pos = Chased.pos + new Vector3(-450, -50, -10)},
                        new Asteroid(10) {pos = Chased.pos + new Vector3(-550, 30, -10)},
                        new Asteroid(10) {pos = Chased.pos + new Vector3(-550, -10, 50)}
                    };

                    foreach (var asteroid in asteroidList)
                    {
                        asteroid.LoadContent();
                        Game.Children.Add(asteroid);
                    }

                    Camera.pos = Chased.pos + new Vector3(-100, -20, 20);
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);

                    SetChaserPursuit(false);
                },
                Action = () =>
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
                    Camera.pos = ChaserLeader.pos + new Vector3(-100, 8, 5);
                    Camera.look = ChaserLeader.pos - Camera.pos;
                    Camera.up = Vector3.Up;

                    SetChaserPursuit(true);
                },
                Action = () =>
                {
                    Camera.pos += ElapsedSeconds * new Vector3(-20, 0, 0);
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
                        new Asteroid(30) {pos = Chased.pos + new Vector3(-60, 0, -5)},
                        new Asteroid(30) {pos = Chased.pos + new Vector3(-60, 0, -70)}
                    };

                    foreach (var asteroid in asteroidList)
                    {
                        asteroid.LoadContent();
                        Game.Children.Add(asteroid);
                    }

                    SetChaserPursuit(false);

                    Chased.targetPos = Chased.pos + new Vector3(-100, 0, -35);
                    Chased.look = Vector3.Normalize(new Vector3(-1, 0, 1.5f));
                    Chased.velocity = Chased.look * Chased.velocity.Length();

                    Camera.pos = Chased.pos + new Vector3(-60, 0, 40);
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                },
                Action = () =>
                {
                    Camera.pos += ElapsedSeconds * new Vector3(-50, 0, -1);
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
                Action = () =>
                {
                    Camera.look = Chased.pos - Camera.pos;
                    var cameraRight = Vector3.Cross(look, Vector3.Up);
                    Camera.up = Vector3.Cross(cameraRight, look);
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 33.5,
                InitialAction = () =>
                {
                    Chased.pos = Chased.pos + new Vector3(-200, 0, 0);
                    Chased.look = Vector3.Left;
                    Chased.targetPos = Chased.pos + new Vector3(-300, 0, 0);

                    var asteroidList = new List<Asteroid>
                    {
                        new Asteroid(5) {pos = Chased.pos + new Vector3(-100, -20, 50)},
                        new Asteroid(5) {pos = Chased.pos + new Vector3(-120, 0, -60)},
                        new Asteroid(5) {pos = Chased.pos + new Vector3(-150, 20, 40)},
                        new Asteroid(5) {pos = Chased.pos + new Vector3(-190, -20, -30)},
                        new Asteroid(5) {pos = Chased.pos + new Vector3(-230, 30, -20)}
                    };

                    foreach (var asteroid in asteroidList)
                    {
                        asteroid.LoadContent();
                        Game.Children.Add(asteroid);
                    }
                },
                Action = () =>
                {
                    Camera.pos = Chased.pos + new Vector3(20, 5, 0);
                    Camera.look = Chased.pos - Camera.pos;
                    Camera.up = Vector3.Up;
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 36,
                InitialAction = () =>
                {
                    Chased.SteeringBehaviours.turnOff(SteeringBehaviours.behaviour_type.arrive);
                    Chased.velocity.Normalize();
                    Chased.velocity *= 0.1f;
                },
                Action = () =>
                {
                    Chased.velocity += ElapsedSeconds * new Vector3(0.5f, 0, 0.5f);
                    Chased.velocity.Normalize();
                    Chased.velocity *= 0.1f;
                }
            });

            ShotList.Add(new Shot
            {
                EndTime = 42,
                InitialAction = () =>
                {
                },
                Action = () =>
                {

                }
            });

            ShotList.Sort();
        }

        /// <summary>
        /// Helper for updating the explosions effect.
        /// </summary>
        void UpdateExplosions(GameTime gameTime)
        {
            TimeToNextProjectile -= gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeToNextProjectile <= 0)
            {
                // Create a new projectile once per second. The real work of moving
                // and creating particles is handled inside the Projectile class.
                Projectiles.Add(new Projectile(ExplosionParticles,
                                               ExplosionSmokeParticles,
                                               ProjectileTrailParticles));

                TimeToNextProjectile += 1;
            }
        }

        /// <summary>
        /// Helper for updating the list of active projectiles.
        /// </summary>
        void UpdateProjectiles(GameTime gameTime)
        {
            int i = 0;

            while (i < Projectiles.Count)
            {
                if (!Projectiles[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    Projectiles.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        private void SetChaserPursuit(bool pursuit)
        {
            if (pursuit)
            {
                ChaserLeader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
                foreach (var fighter in ChaserFleet)
                {
                    fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                }
            }
            else
            {
                ChaserLeader.SteeringBehaviours.turnOff(SteeringBehaviours.behaviour_type.pursuit);
                foreach (var fighter in ChaserFleet)
                {
                    fighter.SteeringBehaviours.turnOff(SteeringBehaviours.behaviour_type.offset_pursuit);
                }
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

            GameTime = gameTime;

            PlayPosition += ElapsedSeconds;

            if (totalSeconds > ShotList[CurrentShot].EndTime)
            {
                ++CurrentShot;
                if (CurrentShot >= ShotList.Count) return;

                ShotList[CurrentShot].InitialAction();
            }

            ShotList[CurrentShot].Action();
            //UpdateExplosions(GameTime);
            //UpdateProjectiles(GameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Pass camera matrices through to the particle system components.
            ExplosionParticles.SetCamera(Camera.view, Camera.projection);
            ExplosionSmokeParticles.SetCamera(Camera.view, Camera.projection);
            ProjectileTrailParticles.SetCamera(Camera.view, Camera.projection);
        }

        public override void UnloadContent()
        {
        }
    }
}
