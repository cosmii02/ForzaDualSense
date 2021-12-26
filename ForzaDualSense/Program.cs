﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ForzaDualSense
{
    class Program
    {
        //This sends the data to DualSenseX based on the input parsed data from Forza.
        //See DataPacket.cs for more details about what forza parameters can be accessed.
        //See the Enums at the bottom of this file for details about commands that can be sent to DualSenseX
        //Also see the Test Function below to see examples about those commands
        static void SendData(DataPacket data)
        {
            Packet p = new Packet();

            //Set the controller to do this for
            int controllerIndex = 0;

            //Initialize our array of instructions
            p.instructions = new Instruction[4];

            //Update the left(Brake) trigger
            p.instructions[0].type = InstructionType.TriggerUpdate;
            //By default, it should be uniform hard resistance
            p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Resistance, 0, 8 };

            //Get average tire slippage. This value runs from 0.0 upwards with a value of 1.0 or greater meaning total loss of grip.
            float combinedTireSlip = (data.TireCombinedSlipFrontLeft + data.TireCombinedSlipFrontRight + data.TireCombinedSlipRearLeft + data.TireCombinedSlipRearRight) / 4;

            //All grip lost, trigger should be loose
            if (combinedTireSlip > 1)
            {
                //Set left trigger to normal mode(i.e no resistance)
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Normal, 0, 0 };

            }
            //Some grip lost, begin to vibrate according to the amount of grip lost
            else if (combinedTireSlip > 0.25)
            {
                int freq = 35 - (int)Math.Floor(Map(combinedTireSlip, 0.25f, 1, 0, 35));
                //Set left trigger to the custom mode VibrateResitance with values of Frequency = freq, Stiffness = 104, startPostion = 76. 
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.VibrateResistance, freq, 104, 76, 0, 0, 0, 0 };

            }

            //Set the updates for the right Trigger(Throttle)
            p.instructions[2].type = InstructionType.TriggerUpdate;
            //It should probably always be uniformly stiff
            p.instructions[2].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.Resistance, 0, 8 };

            // if (combinedTireSlip > 1)
            // {
            //     p.instructions[2].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.Normal, 0, 0 };

            // }
            // else if (combinedTireSlip > 0.25)
            // {
            //     int freq = 35 - (int)Math.Floor(Map(combinedTireSlip, 0.25f, 1, 0, 35));
            //     p.instructions[2].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.VibrateResistance, freq, 104, 76, 0, 0, 0, 0 };

            // }

            //Update the light bar
            p.instructions[1].type = InstructionType.RGBUpdate;
            //Currently registers intensity on the green channel based on engnine RPM as a percantage of the maxium. 
            p.instructions[1].parameters = new object[] { controllerIndex, 0, (int)Math.Floor((data.CurrentEngineRpm / data.EngineMaxRpm) * 255), 0 };

            //Send the commands to DualSenseX
            Send(p);


        }
        //This is the same test method from the UDPExample in DualSenseX. It just provides a basic overview of the different commands that can be used with DualSenseX.
        static void test(string[] args)
        {


            while (true)
            {
                Packet p = new Packet();

                int controllerIndex = 0;

                p.instructions = new Instruction[4];

                // ----------------------------------------------------------------------------------------------------------------------------

                //Normal:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Normal };

                //GameCube:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.GameCube };

                //VerySoft:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.VerySoft };

                //Soft:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Soft };

                //Hard:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Hard };

                //VeryHard:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.VeryHard };

                //Hardest:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Hardest };

                //Rigid:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Rigid };

                //VibrateTrigger needs 1 param of value from 0-255:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.VibrateTrigger, 10 };

                //Choppy:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Choppy };

                //Medium:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Medium };

                //VibrateTriggerPulse:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.VibrateTriggerPulse };

                //CustomTriggerValue with CustomTriggerValueMode:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.PulseAB, 0, 101, 255, 255, 0, 0, 0 };

                //Resistance needs 2 params Start: 0-9 Force:0-8:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Resistance, 0, 8 };

                //Bow needs 4 params Start: 0-8 End:0-8 Force:0-8 SnapForce:0-8:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Bow, 0, 8, 2, 5 };

                //Galloping needs 5 params Start: 0-8 End:0-9 FirstFoot:0-6 SecondFoot:0-7 Frequency:0-255:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Galloping, 0, 9, 2, 4, 10 };

                //SemiAutomaticGun needs 3 params Start: 2-7 End:0-8 Force:0-8:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.SemiAutomaticGun, 2, 7, 8 };

                //AutomaticGun needs 3 params Start: 0-8 End:0-9 StrengthA:0-7 StrengthB:0-7 Frequency:0-255 Period 0-2:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.AutomaticGun, 0, 8, 10 };

                //AutomaticGun needs 6 params Start: 0-9 Strength:0-8 Frequency:0-255:
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.Machine, 0, 9, 7, 7, 10, 0 };

                // ----------------------------------------------------------------------------------------------------------------------------

                p.instructions[1].type = InstructionType.RGBUpdate;
                p.instructions[1].parameters = new object[] { controllerIndex, 0, 255, 0 };

                // ----------------------------------------------------------------------------------------------------------------------------

                // PLAYER LED 1-5 true/false state
                p.instructions[2].type = InstructionType.PlayerLED;
                p.instructions[2].parameters = new object[] { controllerIndex, true, false, true, false, true };

                // ----------------------------------------------------------------------------------------------------------------------------

                // TriggerThreshold needs 2 params LeftTrigger:0-255 RightTrigger:0-255
                p.instructions[3].type = InstructionType.TriggerThreshold;
                p.instructions[3].parameters = new object[] { controllerIndex, Trigger.Right, 0 };

                // ----------------------------------------------------------------------------------------------------------------------------

                Send(p);

                Console.WriteLine("Press any key to send again");
                Console.ReadKey();
            }
        }
        //Maps floats from one range to another.
        public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private static DataPacket data = new DataPacket();
        private const int FORZA_DATA_OUT_PORT = 5300;
        static UdpClient senderClient;
        static IPEndPoint endPoint;
        //Connect to DualSenseX
        static void Connect()
        {
            senderClient = new UdpClient();
            endPoint = new IPEndPoint(Triggers.localhost, 6750);
        }
        //Send Data to DualSenseX
        static void Send(Packet data)
        {
            var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
            senderClient.Send(RequestData, RequestData.Length, endPoint);
        }

        //Main running thread of program.
        static async Task Main(string[] args)
        {
            #region udp stuff
            //Connect to DualSenseX
            Connect();

            //Connect to Forza
            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, FORZA_DATA_OUT_PORT);
            var client = new UdpClient(FORZA_DATA_OUT_PORT);

            Console.WriteLine("The Program is running. Please set the Forza data out to 127.0.0.1, port 5300 and the DualSenseX UDP Port to 6750");

            //Main loop, go until killed
            while (true)
            {
                //If Forza sends an update
                await client.ReceiveAsync().ContinueWith(receive =>
                {
                    //parse data
                    var resultBuffer = receive.Result.Buffer;
                    if (!AdjustToBufferType(resultBuffer.Length))
                    {
                        //  return;
                    }
                    data = ParseData(resultBuffer);

                    //Process and send data to DualSenseX
                    SendData(data);

                });
            }


            #endregion

        }

        //Parses data from Forza into a DataPacket
        static DataPacket ParseData(byte[] packet)
        {
            DataPacket data = new DataPacket();

            // sled
            data.IsRaceOn = packet.IsRaceOn();
            data.TimestampMS = packet.TimestampMs();
            data.EngineMaxRpm = packet.EngineMaxRpm();
            data.EngineIdleRpm = packet.EngineIdleRpm();
            data.CurrentEngineRpm = packet.CurrentEngineRpm();
            data.AccelerationX = packet.AccelerationX();
            data.AccelerationY = packet.AccelerationY();
            data.AccelerationZ = packet.AccelerationZ();
            data.VelocityX = packet.VelocityX();
            data.VelocityY = packet.VelocityY();
            data.VelocityZ = packet.VelocityZ();
            data.AngularVelocityX = packet.AngularVelocityX();
            data.AngularVelocityY = packet.AngularVelocityY();
            data.AngularVelocityZ = packet.AngularVelocityZ();
            data.Yaw = packet.Yaw();
            data.Pitch = packet.Pitch();
            data.Roll = packet.Roll();
            data.NormalizedSuspensionTravelFrontLeft = packet.NormSuspensionTravelFl();
            data.NormalizedSuspensionTravelFrontRight = packet.NormSuspensionTravelFr();
            data.NormalizedSuspensionTravelRearLeft = packet.NormSuspensionTravelRl();
            data.NormalizedSuspensionTravelRearRight = packet.NormSuspensionTravelRr();
            data.TireSlipRatioFrontLeft = packet.TireSlipRatioFl();
            data.TireSlipRatioFrontRight = packet.TireSlipRatioFr();
            data.TireSlipRatioRearLeft = packet.TireSlipRatioRl();
            data.TireSlipRatioRearRight = packet.TireSlipRatioRr();
            data.WheelRotationSpeedFrontLeft = packet.WheelRotationSpeedFl();
            data.WheelRotationSpeedFrontRight = packet.WheelRotationSpeedFr();
            data.WheelRotationSpeedRearLeft = packet.WheelRotationSpeedRl();
            data.WheelRotationSpeedRearRight = packet.WheelRotationSpeedRr();
            data.WheelOnRumbleStripFrontLeft = packet.WheelOnRumbleStripFl();
            data.WheelOnRumbleStripFrontRight = packet.WheelOnRumbleStripFr();
            data.WheelOnRumbleStripRearLeft = packet.WheelOnRumbleStripRl();
            data.WheelOnRumbleStripRearRight = packet.WheelOnRumbleStripRr();
            data.WheelInPuddleDepthFrontLeft = packet.WheelInPuddleFl();
            data.WheelInPuddleDepthFrontRight = packet.WheelInPuddleFr();
            data.WheelInPuddleDepthRearLeft = packet.WheelInPuddleRl();
            data.WheelInPuddleDepthRearRight = packet.WheelInPuddleRr();
            data.SurfaceRumbleFrontLeft = packet.SurfaceRumbleFl();
            data.SurfaceRumbleFrontRight = packet.SurfaceRumbleFr();
            data.SurfaceRumbleRearLeft = packet.SurfaceRumbleRl();
            data.SurfaceRumbleRearRight = packet.SurfaceRumbleRr();
            data.TireSlipAngleFrontLeft = packet.TireSlipAngleFl();
            data.TireSlipAngleFrontRight = packet.TireSlipAngleFr();
            data.TireSlipAngleRearLeft = packet.TireSlipAngleRl();
            data.TireSlipAngleRearRight = packet.TireSlipAngleRr();
            data.TireCombinedSlipFrontLeft = packet.TireCombinedSlipFl();
            data.TireCombinedSlipFrontRight = packet.TireCombinedSlipFr();
            data.TireCombinedSlipRearLeft = packet.TireCombinedSlipRl();
            data.TireCombinedSlipRearRight = packet.TireCombinedSlipRr();
            data.SuspensionTravelMetersFrontLeft = packet.SuspensionTravelMetersFl();
            data.SuspensionTravelMetersFrontRight = packet.SuspensionTravelMetersFr();
            data.SuspensionTravelMetersRearLeft = packet.SuspensionTravelMetersRl();
            data.SuspensionTravelMetersRearRight = packet.SuspensionTravelMetersRr();
            data.CarOrdinal = packet.CarOrdinal();
            data.CarClass = packet.CarClass();
            data.CarPerformanceIndex = packet.CarPerformanceIndex();
            data.DrivetrainType = packet.DriveTrain();
            data.NumCylinders = packet.NumCylinders();

            // dash
            data.PositionX = packet.PositionX();
            data.PositionY = packet.PositionY();
            data.PositionZ = packet.PositionZ();
            data.Speed = packet.Speed();
            data.Power = packet.Power();
            data.Torque = packet.Torque();
            data.TireTempFl = packet.TireTempFl();
            data.TireTempFr = packet.TireTempFr();
            data.TireTempRl = packet.TireTempRl();
            data.TireTempRr = packet.TireTempRr();
            data.Boost = packet.Boost();
            data.Fuel = packet.Fuel();
            data.Distance = packet.Distance();
            data.BestLapTime = packet.BestLapTime();
            data.LastLapTime = packet.LastLapTime();
            data.CurrentLapTime = packet.CurrentLapTime();
            data.CurrentRaceTime = packet.CurrentRaceTime();
            data.Lap = packet.Lap();
            data.RacePosition = packet.RacePosition();
            data.Accelerator = packet.Accelerator();
            data.Brake = packet.Brake();
            data.Clutch = packet.Clutch();
            data.Handbrake = packet.Handbrake();
            data.Gear = packet.Gear();
            data.Steer = packet.Steer();
            data.NormalDrivingLine = packet.NormalDrivingLine();
            data.NormalAiBrakeDifference = packet.NormalAiBrakeDifference();

            return data;
        }

        //Support different standards
        static bool AdjustToBufferType(int bufferLength)
        {
            switch (bufferLength)
            {
                case 232: // FM7 sled
                    return false;
                case 311: // FM7 dash
                    FMData.BufferOffset = 0;
                    return true;
                case 324: // FH4
                    FMData.BufferOffset = 12;
                    return true;
                default:
                    return false;
            }
        }


    }

    //Needed to communicate with DualSenseX
    public static class Triggers
    {
        public static IPAddress localhost = new IPAddress(new byte[] { 127, 0, 0, 1 });

        public static string PacketToJson(Packet packet)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(packet);
        }

        public static Packet JsonToPacket(string json)
        {
            return JsonConvert.DeserializeObject<Packet>(json);
        }
    }

    //The different trigger Modes. These correlate the values in the DualSenseX UI
    public enum TriggerMode
    {
        Normal = 0,
        GameCube = 1,
        VerySoft = 2,
        Soft = 3,
        Hard = 4,
        VeryHard = 5,
        Hardest = 6,
        Rigid = 7,
        VibrateTrigger = 8,
        Choppy = 9,
        Medium = 10,
        VibrateTriggerPulse = 11,
        CustomTriggerValue = 12,
        Resistance = 13,
        Bow = 14,
        Galloping = 15,
        SemiAutomaticGun = 16,
        AutomaticGun = 17,
        Machine = 18
    }

    //Custom Trigger Values. These correspond to the values in the DualSenseX UI
    public enum CustomTriggerValueMode
    {
        OFF = 0,
        Rigid = 1,
        RigidA = 2,
        RigidB = 3,
        RigidAB = 4,
        Pulse = 5,
        PulseA = 6,
        PulseB = 7,
        PulseAB = 8,
        VibrateResistance = 9,
        VibrateResistanceA = 10,
        VibrateResistanceB = 11,
        VibrateResistanceAB = 12,
        VibratePulse = 13,
        VibratePulseA = 14,
        VibratePulsB = 15,
        VibratePulseAB = 16
    }

    public enum Trigger
    {
        Invalid,
        Left,
        Right
    }

    public enum InstructionType
    {
        Invalid,
        TriggerUpdate,
        RGBUpdate,
        PlayerLED,
        TriggerThreshold
    }

    public struct Instruction
    {
        public InstructionType type;
        public object[] parameters;
    }

    public class Packet
    {
        public Instruction[] instructions;
    }
}
