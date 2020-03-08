using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioManager
{
    class Program
    {
        static void Main(string[] args)
        {
            MMDeviceEnumerator deviceEnumerator;
            MMDevice defaultRecordingDevice, defaultPlaybackDevice;

            FetchDevices(out deviceEnumerator, out defaultRecordingDevice, out defaultPlaybackDevice);

            if (args.Length < 1)
            {
                while (true)
                {
                    List<MMDevice> devicesPresent = ShowDeviceStatus(deviceEnumerator, defaultRecordingDevice, defaultPlaybackDevice);

                    Console.WriteLine( "Welcome to AudioDeviceManager" );
                    Console.WriteLine( @"USAGE: First Argument is the task to perform, MUTE or UNMUTE, Second Argument is Device Number" );
                    Console.WriteLine( "Example: MUTE DEFAULT" );
                    Console.WriteLine( "To refresh devices list type REFRESH" );
                    Console.WriteLine( "To exit type EXIT" );

                    var commands = Console.ReadLine().Split(' ');

                    int.TryParse( commands[1], out int deviceNumber );

                    if ( commands.Length < 1 ){
                        InvalidCommand();
                        continue;
                    }


                    else if ( commands[0].Equals( "REFRESH", StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        Console.Clear();
                        FetchDevices(out deviceEnumerator, out defaultRecordingDevice, out defaultPlaybackDevice);
                    }

                    else if ( commands[0].Equals("EXIT", StringComparison.InvariantCultureIgnoreCase))
                        return;

                    else if ( commands[0].Equals( "MUTE", StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        if ( commands[1].Equals( "ALL", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            if ( !commands[2].Equals("MIC", StringComparison.InvariantCultureIgnoreCase ) 
                                && !commands[2].Equals( "SPEAKER", StringComparison.InvariantCultureIgnoreCase ) 
                                    && !commands[2].Equals( "BOTH", StringComparison.InvariantCultureIgnoreCase ) )
                            {
                                InvalidCommand();
                            }

                            else
                                MuteAll( deviceEnumerator, true, commands[2].ToUpper() );
                        }

                        else if ( commands[1].Equals( "DEFAULT", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            MuteDevice( deviceEnumerator.GetDefaultAudioEndpoint( DataFlow.Capture, Role.Communications ), true );
                        }

                        else if ( deviceNumber > 0 && deviceNumber <=
                            deviceEnumerator.EnumerateAudioEndPoints( DataFlow.All, DeviceState.Active ).Count )
                        {
                            MuteDevice( devicesPresent[deviceNumber - 1], true );
                        }

                        else
                        {
                            Console.WriteLine( "------------------------------------" );
                            Console.WriteLine( "***INVALID DEVICE***" );
                            Console.WriteLine( "------------------------------------" );
                        }

                        continue;
                    }


                    else if ( commands[0].Equals( "UNMUTE", StringComparison.InvariantCultureIgnoreCase ) )
                    {
                        if ( commands[1].Equals( "ALL", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            if ( !commands[2].Equals( "MIC", StringComparison.InvariantCultureIgnoreCase )
                                && !commands[2].Equals( "SPEAKER", StringComparison.InvariantCultureIgnoreCase )
                                    && !commands[2].Equals( "BOTH", StringComparison.InvariantCultureIgnoreCase ) )
                            {
                                InvalidCommand();
                            }

                            else
                                MuteAll( deviceEnumerator, false, commands[2].ToUpper() );
                        }

                        else if ( commands[1].Equals( "DEFAULT", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            MuteDevice( deviceEnumerator.GetDefaultAudioEndpoint( DataFlow.Capture, Role.Communications ), false );
                        }

                        else if ( deviceNumber > 0 && deviceNumber <=
                            deviceEnumerator.EnumerateAudioEndPoints( DataFlow.All, DeviceState.Active ).Count )
                        {
                            MuteDevice( devicesPresent[deviceNumber - 1], false );
                        }

                        else
                        {
                            Console.WriteLine( "------------------------------------" );
                            Console.WriteLine( "***INVALID DEVICE***" );
                            Console.WriteLine( "------------------------------------" );
                        }

                        continue;
                    }

                    else
                        InvalidCommand();

                }

            }
            else
            {
                if ( args[0].ToUpper() == "COMMANDS" )
                {
                    StringBuilder commandsList = new StringBuilder();

                    commandsList.AppendLine( "INFO -> Displays active devices Information." );
                    commandsList.AppendLine( "MUTE -> Allows to Mute Devices. Options: DEFAULT MIC(Default Microphone Device) ALL MIC(All Microphone Devices) " );
                    commandsList.AppendLine( "UNMUTE -> Allows to Unmute Devices. Options: DEFAULT MIC(Default Microphone Device) ALL MIC(All Microphone Devices)" );

                    Console.WriteLine( commandsList.ToString() );
                }
                else if ( args[0].ToUpper() == "INFO" )
                {
                    var returnedList = ShowDeviceStatus( deviceEnumerator, defaultRecordingDevice, defaultPlaybackDevice );
                }
                else if ( args[0].ToUpper() == "MUTE" && args.Length > 2 )
                {
                    if ( args[1].ToUpper() == "DEFAULT" && args[2].ToUpper() == "MIC" )
                    {
                        MuteDevice( deviceEnumerator.GetDefaultAudioEndpoint( DataFlow.Capture, Role.Communications ), true );
                    }
                    else if ( args[1].ToUpper() == "ALL" && args[2].ToUpper() == "MIC" )
                    {
                        MuteAll( deviceEnumerator, true, "MIC" );
                    }
                    else
                        InvalidCommand();
                }

                else if ( args[0].ToUpper() == "UNMUTE" && args.Length > 2 )
                {
                    if ( args[1].ToUpper() == "DEFAULT" && args[2].ToUpper() == "MIC" )
                    {
                        MuteDevice( deviceEnumerator.GetDefaultAudioEndpoint( DataFlow.Capture, Role.Communications ), false );
                    }
                    else if ( args[1].ToUpper() == "ALL" && args[2].ToUpper() == "MIC" )
                    {
                        MuteAll( deviceEnumerator, false, "MIC" );
                    }
                    else
                        InvalidCommand();
                }

                else
                    InvalidCommand();
            }
        }

        private static void InvalidCommand()
        {
            Console.WriteLine( "Invalid Command. Type COMMANDS for help." );
        }

        private static List<MMDevice> ShowDeviceStatus(MMDeviceEnumerator deviceEnumerator, MMDevice defaultRecordingDevice, MMDevice defaultPlaybackDevice)
        {
            var devicesPresent = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).ToList();
            Console.WriteLine("Devices Present:");
            int i = 1;
            foreach (var item in devicesPresent)
            {
                Console.WriteLine("Device {0} - {1} : Status: {2}", i, item.FriendlyName, item.AudioEndpointVolume.Mute == true ? "MUTED" : "UNMUTED");
                i++;
            }
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Default Recording Device:");
            Console.WriteLine(defaultRecordingDevice.FriendlyName);
            Console.WriteLine("Status: {0}", defaultRecordingDevice.AudioEndpointVolume.Mute == true ? "MUTED" : "UNMUTED");
            Console.WriteLine("**************");

            Console.WriteLine("Default Playback Device:");
            Console.WriteLine(defaultPlaybackDevice.FriendlyName);
            Console.WriteLine("Status: {0}", defaultPlaybackDevice.AudioEndpointVolume.Mute == true ? "MUTED" : "UNMUTED");
            Console.WriteLine( "------------------------------------" );
            return devicesPresent;
        }

        private static void FetchDevices(out MMDeviceEnumerator deviceEnumerator, out MMDevice defaultRecordingDevice, out MMDevice defaultPlaybackDevice)
        {
            deviceEnumerator = new MMDeviceEnumerator();
            defaultRecordingDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            defaultPlaybackDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        private static void MuteDevice(MMDevice deviceToMute, bool newStatus)
        {
            deviceToMute.AudioEndpointVolume.Mute = newStatus;
            Console.WriteLine("Microphone {0}", newStatus == true ? "MUTED" : "UNMUTED" );
        }

        private static void MuteAll( MMDeviceEnumerator deviceEnumerator, bool newStatus, string deviceType )
        {
            DataFlow dataFlow = DataFlow.All;

            if ( deviceType == "MIC" )
                dataFlow = DataFlow.Capture;

            else if ( deviceType == "SPEAKER" )
                dataFlow = DataFlow.Render;


            foreach (var item in deviceEnumerator.EnumerateAudioEndPoints( dataFlow, DeviceState.Active))
            {
                if ( !item.DeviceTopology.GetConnector( 0 ).ConnectedToDeviceId.Contains( "rtstereomixtopo" ) )
                    item.AudioEndpointVolume.Mute = newStatus;
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*********ALL MICROPHONES ARE {0}*********", newStatus == true ? "MUTED" : "UNMUTED" );
            Console.WriteLine("");
            Console.WriteLine("");
        }

    }
}
