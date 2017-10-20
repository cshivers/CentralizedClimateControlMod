﻿using System.Collections.Generic;
using System.Text;
using Verse;

namespace CentralizedClimateControl
{
    public class AirFlowNet
    {
        private int _intGridId = -2;
        private float _currentIntakeAir;
        private float _currentExhaustAir;

        public List<CompAirFlow> Connectors = new List<CompAirFlow>();
        public List<CompAirFlowProducer> Producers = new List<CompAirFlowProducer>();
        public List<CompAirFlowTempControl> TempControls = new List<CompAirFlowTempControl>();
        public List<CompAirFlowConsumer> Consumers = new List<CompAirFlowConsumer>();

        public AirFlowType FlowType;

        public float ThermalCapacity;
        public float ThermalEfficiency = 1.0f;
        public float FlowEfficiency = 1.0f;

        public float AverageIntakeTemperature;
        public float AverageConvertedTemperature;

        public int GridID
        {
            get { return this._intGridId; }
            set { this._intGridId = value; }
        }

        public float CurrentIntakeAir
        {
            get { return _currentIntakeAir; }
        }

        public float CurrentExhaustAir
        {
            get { return _currentExhaustAir; }
        }

        /// <summary>
        /// Tick the Producers of Air Flow.
        /// We calculate the Intake Temperature here and the Total Air generated by the network.
        /// </summary>
        private void TickProducers()
        {
            float airFlow = 0.0f;
            float tempSum = 0.0f;

            foreach (var producer in Producers)
            {
                if (!producer.IsOperating() || !producer.IsActive())
                {
                    continue;
                }

                airFlow += producer.CurrentAirFlow;
                tempSum += producer.IntakeTemperature;
            }

            AverageIntakeTemperature = tempSum / Producers.Count;
            _currentIntakeAir = airFlow;

            if (TempControls.Count == 0)
            {
                AverageConvertedTemperature = AverageIntakeTemperature;
            }
        }

        /// <summary>
        /// Process the Consumers for a Tick. Consumers are the ones who consume Air Flow. They can be Vents (for now).
        /// We calculate the total Exhaust capacity of the Network. This Exhaust Capacity is used by the Flow Efficiency Attribute.
        /// </summary>
        private void TickConsumers()
        {
            float airFlow = 0.0f;

            foreach (var consumer in Consumers)
            {
                if (!consumer.IsOperating())
                {
                    continue;
                }

                airFlow += consumer.ExhaustAirFlow;
            }

            _currentExhaustAir = airFlow;
        }

        /// <summary>
        /// Process the Buildings that Control Climate. Generally, the Climate Control Units.
        /// Here, we process variables to be used to Thermal Efficiency.
        /// </summary>
        private void TickTempControllers()
        {
            var tempSum = 0.0f;
            var thermalCapacity = 0.0f;

            foreach (var compAirFlowTempControl in TempControls)
            {
                if (!compAirFlowTempControl.IsOperating() || !compAirFlowTempControl.IsActive())
                {
                    continue;
                }

                tempSum += compAirFlowTempControl.ConvertedTemperature;
                thermalCapacity += compAirFlowTempControl.ThermalCapacity;
            }

            // No Temperature Controllers -> Then Use the Intake Temperature directly.
            if (TempControls.Count > 0)
            {
                ThermalCapacity = thermalCapacity;
                AverageConvertedTemperature = tempSum / TempControls.Count;
            }
        }

        /// <summary>
        /// Register a Producer of Air Flow in the Network.
        /// </summary>
        /// <param name="producer">The Producer's Component</param>
        public void RegisterProducer(CompAirFlowProducer producer)
        {
            if (Producers.Contains(producer))
            {
                Log.Error("AirFlowNet registered producer it already had: " + producer);
                return;
            }

            Producers.Add(producer);
        }

        /// <summary>
        /// De-register a Producer in the Network.
        /// </summary>
        /// <param name="producer">The Producer's Component</param>
        public void DeregisterProducer(CompAirFlowProducer producer)
        {
            Producers.Remove(producer);
        }

        /// <summary>
        /// Process one Tick of the Air Flow Network. Here we process the Producers, Consumers and Climate Controllers.
        /// We Calculate the Flow Efficiency (FE) and Thermal Efficiency (TE).
        /// FE & TEs are recorded for each individual network.
        /// </summary>
        public void AirFlowNetTick()
        {
            TickProducers();
            TickTempControllers();
            TickConsumers();

            if (CurrentIntakeAir > 0)
            {
                ThermalEfficiency = ThermalCapacity / CurrentIntakeAir;
            }
            else
            {
                ThermalEfficiency = 0.0f;
            }

            if (CurrentExhaustAir > 0)
            {
                FlowEfficiency = CurrentIntakeAir / CurrentExhaustAir;

                if (FlowEfficiency > 1.0f)
                {
                    FlowEfficiency = 1.0f;
                }
            }
            else
            {
                FlowEfficiency = 0.0f;
            }
        }

        /// <summary>
        /// Print the Debug String for this Network
        /// </summary>
        /// <returns>Multi-line String containing Output</returns>
        public string DebugString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------");
            stringBuilder.AppendLine("AIRFLOW NET:");
            stringBuilder.AppendLine("  Prodcued AirFlow: " + CurrentIntakeAir);
            stringBuilder.AppendLine("  AverageIntakeTemperature: " + AverageIntakeTemperature);
            stringBuilder.AppendLine("  AverageConvertedTemperature: " + AverageConvertedTemperature);

            stringBuilder.AppendLine("  Producers: ");
            foreach (var current in Producers)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("  TempControls: ");
            foreach (var current in TempControls)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("  Consumers: ");
            foreach (var current in Consumers)
            {
                stringBuilder.AppendLine("      " + current.parent);
            }

            stringBuilder.AppendLine("------------");
            return stringBuilder.ToString();
        }
    }
}
