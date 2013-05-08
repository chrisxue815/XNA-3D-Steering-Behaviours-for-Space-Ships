﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Steering
{
    class Scenario
    {
        static Random random = new Random(DateTime.Now.Millisecond);

        static Vector3 randomPosition(float range)
        {
            Vector3 pos = new Vector3();
            pos.X = (random.Next() % range) - (range / 2);
            pos.Y = (random.Next() % range) - (range / 2);
            pos.Z = (random.Next() % range) - (range / 2);
            return pos;
        }

        public static void setUpFlockingDemo()
        {
            Params.Load("flocking.properties");
            List<Entity> children = XNAGame.Instance().Children;
            //Ground ground = new Ground();
            //children.Add(ground);
            //XNAGame.Instance().Ground = ground;
            Fighter bigFighter = new EliteFighter();
            bigFighter.ModelName = "python";
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            bigFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
            bigFighter.scale = 10.0f;
            children.Add(bigFighter);        

            float range = Params.GetFloat("world_range");
            Fighter fighter = null;
            for (int i = 0; i < Params.GetFloat("num_boids"); i++)
            {
                Vector3 pos = randomPosition(range);
                
                fighter = new EliteFighter();
                fighter.ModelName = "ferdelance";
                fighter.pos = pos;
                fighter.Target = bigFighter;
                fighter.SteeringBehaviours.turnOffAll();
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.separation);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.cohesion);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.alignment);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
                fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                children.Add(fighter);                
            }

            int numObstacles = 0;
            float dist = (range * 2) / numObstacles;
            for (float x = - range ; x < range ; x+= dist)
            {
                for (float z = - range ; z < range ; z += dist)
                {
                    Obstacle o = new Obstacle(20);
                    o.pos = new Vector3(x, 0, z);
                    o.Color = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                    o.ShouldDraw = true;
                    children.Add(o);
                }
            }

            bigFighter.Target = fighter;

            Fighter camFighter = new EliteFighter();
            Vector3 offset = new Vector3(0, 0, 10);
            fighter.ModelName = "cobramk3";
            camFighter.pos = fighter.pos + offset;
            camFighter.offset = offset;
            camFighter.Leader = fighter;
            camFighter.SteeringBehaviours.turnOffAll();
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.sphere_constrain);
            XNAGame.Instance().Children.Add(camFighter);
    
            XNAGame.Instance().CamFighter = camFighter;
            Camera camera = XNAGame.Instance().Camera;
            camera.pos = new Vector3(0.0f, 60.0f, 200.0f);

            foreach (Entity child in children)
            {
                child.LoadContent();
            }
        }

        public static void setUpStateMachineDemo()
        {
            Params.Load("avoidance.properties");
            List<Entity> children = XNAGame.Instance().Children;            
            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance().Ground = ground;            
            AIFighter aiFighter = new AIFighter();
            aiFighter.pos = new Vector3(-20, 50, 50);
            aiFighter.maxSpeed = 16.0f;
            aiFighter.SwicthState(new IdleState(aiFighter));
            aiFighter.Path.DrawPath = true;
            children.Add(aiFighter);

            Fighter fighter = new Fighter();
            fighter.ModelName = "ship2";
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            fighter.pos = new Vector3(10, 50, 0);
            fighter.targetPos = aiFighter.pos + new Vector3(-50, 0, -80);
            children.Add(fighter);

            Fighter camFighter = new Fighter();
            camFighter.Leader = fighter;            
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.pos = fighter.pos + camFighter.offset;
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance().CamFighter = camFighter;
            children.Add(camFighter);

            XNAGame.Instance().Leader = fighter;
            Camera camera = XNAGame.Instance().Camera;
            camera.pos = new Vector3(0.0f, 60.0f, 100.0f);

            Obstacle o = new Obstacle(4);
            o.pos = new Vector3(0, 50, -10);
            children.Add(o);

            o = new Obstacle(4);
            o.pos = new Vector3(50, 0, -90) + aiFighter.pos;
            children.Add(o);
            foreach (Entity child in children)
            {
                child.LoadContent();
            }
        }

        public static void setUpPursuit()
        {
            Params.Load("avoidance.properties");
            List<Entity> children = XNAGame.Instance().Children;

            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance().Ground = ground;            

            Fighter fighter = new Fighter();
            fighter.ModelName = "ship1";
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            fighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            fighter.pos = new Vector3(2, 20, -50);
            fighter.targetPos = fighter.pos * 2;
            XNAGame.Instance().Leader = fighter;
            children.Add(fighter);

            Fighter fighter1 = new Fighter();
            fighter1.ModelName = "ship2";
            fighter1.Target = fighter;
            fighter1.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.pursuit);
            fighter1.pos = new Vector3(-20, 20, -20);
            children.Add(fighter1);
            foreach (Entity child in children)
            {
                child.LoadContent();
            }  
        }

        public static void setUpWander()
        {
            Params.Load("avoidance.properties");
            List<Entity> children = XNAGame.Instance().Children;
            Fighter leader = new Fighter();
            leader.pos = new Vector3(10, 120, 20);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wander);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            children.Add(leader);

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.pos = new Vector3(10, 120, 0);
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance().CamFighter = camFighter;
            children.Add(camFighter);

            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance().Ground = ground;

            XNAGame.Instance().Camera.pos = new Vector3(10, 120, 50);
            foreach (Entity child in children)
            {
                child.LoadContent();
            }
      
        }

        public static void setUpPathFindingDemo()
        {
            List<Entity> children = XNAGame.Instance().Children;

            Fighter fighter = new Fighter();
            fighter.ModelName = "fighter";
            fighter.pos = new Vector3(10, 0, 20);
            children.Add(fighter);
            Obstacle obstacle = new Obstacle(10);
            obstacle.pos = new Vector3(15, 0, -10);
            children.Add(obstacle);

            obstacle = new Obstacle(12);
            obstacle.pos = new Vector3(5, 0, -50);
            children.Add(obstacle);

            obstacle = new Obstacle(5);
            obstacle.pos = new Vector3(10, 0, -70);
            children.Add(obstacle);
            
            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance().Ground = ground;
            PathFinder pathFinder = new PathFinder();
            Path path = pathFinder.findPath(fighter.pos, new Vector3(20, 0, -150));
            fighter.targetPos = new Vector3(20, 0, -150);
            path.DrawPath = true;
            //dalek.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.follow_path);
            path.Looped = false;
            fighter.Path = path;
            XNAGame.Instance().Leader = fighter;

            XNAGame.Instance().Ground = ground;
            foreach (Entity child in children)
            {
                child.LoadContent();
            }

        }


        public static void setUpArrive()
        {
            Params.Load("avoidance.properties");
            List<Entity> children = XNAGame.Instance().Children;
            Fighter leader = new Fighter();
            leader.pos = new Vector3(10, 120, 20);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.targetPos = new Vector3(0, 200, -450);
            children.Add(leader);
            XNAGame.Instance().Leader = leader;

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.pos = new Vector3(10, 120, 0);
            camFighter.offset = new Vector3(0, 5, 10);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance().CamFighter = camFighter;
            children.Add(camFighter);

            Ground ground = new Ground();
            children.Add(ground);

            XNAGame.Instance().Ground = ground;
            foreach (Entity child in children)
            {
                child.LoadContent();
            }

        }
        

        public static void setUpBuckRogersDemo()
        {
            Params.Load("avoidance.properties");
            List<Entity> children = XNAGame.Instance().Children;
            Fighter leader = new Fighter();
            leader.pos = new Vector3(10, 20, 20);            
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.arrive);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            leader.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            leader.targetPos = new Vector3(0, 100, -450);
            children.Add(leader);
            XNAGame.Instance().Leader = leader;

            // Add some Obstacles

            Obstacle o = new Obstacle(4);
            o.pos = new Vector3(0, 10, -10);
            children.Add(o);

            o = new Obstacle(17);
            o.pos = new Vector3(-10, 16, -80);
            children.Add(o);

            o = new Obstacle(10);
            o.pos = new Vector3(10, 15, -120);
            children.Add(o);

            o = new Obstacle(12);
            o.pos = new Vector3(5, -10, -150);
            children.Add(o);

            o = new Obstacle(20);
            o.pos = new Vector3(-2, 5, -200);
            children.Add(o);

            o = new Obstacle(10);
            o.pos = new Vector3(-25, -20, -250);
            children.Add(o);

            o = new Obstacle(10);
            o.pos = new Vector3(20, -20, -250);
            children.Add(o);

            o = new Obstacle(35);
            o.pos = new Vector3(-10, -30, -300);
            children.Add(o);

            // Now make a fleet
            int fleetSize = 5;
            float xOff = 6;
            float zOff = 6;
            for (int i = 2; i < fleetSize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    float z = (i - 1) * +zOff;
                    Fighter fleet = new Fighter();
                    fleet.Leader = leader;
                    fleet.offset = new Vector3((xOff * (-i / 2.0f)) + (j * xOff), 0, z);
                    fleet.pos = leader.pos + fleet.offset;
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
                    fleet.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
                    children.Add(fleet);
                }
            }

            Fighter camFighter = new Fighter();
            camFighter.Leader = leader;
            camFighter.pos = new Vector3(0, 15, fleetSize * zOff);
            camFighter.offset = new Vector3(0, 5, fleetSize * zOff);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.offset_pursuit);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.wall_avoidance);
            camFighter.SteeringBehaviours.turnOn(SteeringBehaviours.behaviour_type.obstacle_avoidance);
            XNAGame.Instance().CamFighter = camFighter;
            children.Add(camFighter);


            Ground ground = new Ground();
            children.Add(ground);
            XNAGame.Instance().Ground = ground;
            foreach (Entity child in children)
            {
                child.pos.Y += 100;
                child.LoadContent();
            }
        }

        public static void SetUpStarTrekDemo()
        {
            Params.Load("StarTrek/startrek.properties");
            XNAGame.Instance().Children.Add(new ShotController());
        }
    }
}
