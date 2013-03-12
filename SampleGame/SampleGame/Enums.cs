using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleGame
{
    public class Enums
    {
        public enum SensorType
        {
            RangeFinder = 0,
            AgentSensor = 1,
            PieSliceSensor = 2
        }

        public enum AgentType
        {
            Wall = 0,
            NPC = 1
        }
    }
}
