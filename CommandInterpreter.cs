﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DudeMugen
{
    /// <summary>
    /// This class is responsible for converting string tokens into Vans's command buffering system commands.
    /// </summary>
    static class CommandInterpreter
    {
        /// <summary>
        /// Represents a direction.
        /// </summary>
        [FlagsAttribute]
        public enum Direction : byte
        {
            None = 0x0,
            U = 0x1,
            D = 0x2,
            B = 0x4,
            F = 0x8
        }

        /// <summary>
        /// Represents a button.
        /// </summary>
        [FlagsAttribute]
        public enum Button : byte
        {
            None = 0x0,
            x = 0x01,
            y = 0x02,
            z = 0x04,
            s = 0x08,
            a = 0x10,
            b = 0x20,
            c = 0x40
        }

        /// <summary>
        /// The type of command
        /// </summary>
        public enum CommandType : byte
        {
            /// <summary>
            /// Regular button press
            /// </summary>
            Press                   = 0x0,
            /// <summary>
            /// Contains a /
            /// </summary>
            Hold                    = 0x1,
            /// <summary>
            /// Contains a ~
            /// </summary>
            Release                 = 0x2,
            /// <summary>
            /// Contains a $
            /// </summary>
            MultiDirectional        = 0x3,
            /// <summary>
            /// Contains a $ and a ~
            /// </summary>
            ReleaseMultiDirectional = 0x4,
            /// <summary>
            /// Contains a $ and a /
            /// </summary>
            HoldMultiDirectional    = 0x5,
            /// <summary>
            /// Contains a +
            /// </summary>
            Combination             = 0x6,
            /// <summary>
            /// Contains that fucking > thing
            /// </summary>
            StupidShit              = 0x7
        }

        // Maximum number of commands that can fit in this system.
        public const int MAX_NUM_COMMANDS = 28;

        // String templates
        private const string CMD_HEADER            = "[State 10380, {0} Init]\r\ntype = Null\r\n";
        private const string INIT_TRIGGER_BF       = "trigger1 = !Var({0}) && cond(P2Dist X >= 0, (var({1})&{2}) {3}, (var({1})&{4}) {3})\r\n";
        private const string INIT_TRIGGER_UD       = "trigger1 = !Var({0}) && (var({1})&{2}) {3}\r\n";
        private const string INIT_TRIGGER_BT       = "trigger1 = !Var({0}) {1}";
        private const string SUBSEQUENT_TRIGGER_BF = "trigger{0} = (Var({1}) & (2**{2} - 1)) > (2**{3}) && cond(p2dist X >= 0, (var({4})&{5}) {6}, (var({4})&{7}) {8})\r\n";
        private const string SUBSEQUENT_TRIGGER_UD = "trigger{0} = (Var({1}) & (2**{2} - 1)) > (2**{3}) && (var({4})&{5}) {6}\r\n";
        private const string SUBSEQUENT_TRIGGER_BT = "trigger{0} = (Var({1}) & (2**{2} - 1)) > (2**{3}) {4}\r\n";
        private const string SECONDARY_TRIGGER     = "trigger{0} = e||(var({1}) := {2} + (2**{3}))\r\n\r\n";
        private const string DEINIT_TRIGGER        = "trigger{0} = Var({1}) && (Var({1})&15) = 0\r\ntrigger{0} = e||(var({1}) := 0)\r\n";
        private const string IGNOREHITPAUSE        = "ignorehitpause = 1\r\n";
        private const string VARIABLE_TEMPLATE     = "&& (var({0})&{1}) {2} {3}"; //TODO: Clean the rest of this crap up with this.
        private const string CMD_TRIGGER_1         = "triggerall = (helper(10371), Var({0})&(2**{1} - 1) > (2**{2})\r\n";
        private const string CMD_TRIGGER_2         = "triggerall = {0}\r\n";

        /// <summary>
        /// Converts the MUGEN .CMD notation to buffering system notation.
        /// </summary>
        /// <param name="title">The title to give to the command.</param>
        /// <param name="command">The command string.</param>
        /// <param name="buttonBufferTime">The time to buffer each button.</param>
        /// <param name="directionBufferTime">The time to buffer each direction.</param>
        /// <param name="elemBufferTime">The time to buffer each element in the command array.</param>
        /// <param name="commandVar">The variable in which to store the command.</param>
        /// <returns></returns>
        public static string CMDToBufferingSystem(string title, string command, byte buttonBufferTime, byte directionBufferTime, byte elemBufferTime, byte commandVar)
        {
            // We're gonna be doing LOTS of appending, so let's use a StringBuilder.
            StringBuilder bufferedCommand = new StringBuilder();

            // Add the title first
            bufferedCommand.AppendFormat(CMD_HEADER, title);

            // I am not explaining this but let's just say if this shit matches, we're good to go... so far.
            Match firstCheck = Regex.Match(command, @"(((~|/)?(((\${0})(UB|DB|DF|UF)|(\$?)(U|D|B|F))|((a|b|c|x|y|z)\+?)*)))(,(((~|/|>)?(((\${0})(UB|DB|DF|UF)|(\$?)(U|D|B|F))|(((a|b|c|x|y|z)\+?)*)))))+");
            if (firstCheck.ToString() == command)
            {

                // Separate by commas and +'s
                string[] tokens = command.Split(',');
                if (tokens.Length > MAX_NUM_COMMANDS)
                    throw new ArgumentException("Command cannot contain more than 28 presses!");

                // Triggers always start at 1
                int triggerNum = 1;

                // Iterate through that shit!
                foreach (string token in tokens)
                {
                    // Split character-by-character.
                    char[] pieces = token.ToCharArray();

                    if (pieces.Length == 0 || pieces.Length > 3)
                        throw new ArgumentException("Malformed command string: unexpected length");

                    // Variables for each piece
                    CommandType currCmdType = CommandType.Press;
                    List<CommandType> cmdTypes = new List<CommandType>(); 
                    Direction directions = Direction.None;
                    List<Button> buttons = new List<Button>();

                    // Iterate through the pieces
                    for (int i = pieces.Length - 1; i >= 0; i--)
                    {
                        Direction tryDir = Direction.None;
                        Button tryButton = Button.None;

                        // Is it a direction?
                        if (Enum.TryParse(pieces[i] + "", out tryDir))
                            directions |= tryDir;
                        // Or a button?
                        else if (Enum.TryParse(pieces[i] + "", out tryButton))
                        {
                            if (buttons.Contains(tryButton))
                                throw new ArgumentException(String.Format("Malformed command token at token {0}, character {1}: button appeared more than once.", triggerNum, i));
                            else
                                buttons.Add(tryButton);
                        }
                        // Or a modifier?
                        else if (pieces[i] == '$' && currCmdType == CommandType.Press)
                            currCmdType = CommandType.MultiDirectional;
                        else if (pieces[i] == '/')
                            currCmdType = currCmdType == CommandType.MultiDirectional ? CommandType.HoldMultiDirectional : CommandType.Hold;
                        else if (pieces[i] == '~')
                            currCmdType = currCmdType == CommandType.MultiDirectional ? CommandType.ReleaseMultiDirectional : CommandType.Release;
                        // Combinations require some more work
                        else if (pieces[i] == '+' && buttons.Count > 0)
                        {
                            // We can only have a+b+c+x+y+z at most
                            if (cmdTypes.Count > 5)
                                throw new ArgumentException(String.Format("Malformed command token at token {0}, character {1}: number of buttons exceeded.", triggerNum, i));
                            else
                                cmdTypes.Add(currCmdType);
                        }
                        // Or bullshit?
                        else
                            throw new ArgumentException(String.Format("Malformed command token at token {0}, character {1}: unexpected character.", triggerNum, i));

                        // Add to the command types array
                        cmdTypes.Add(currCmdType);
                    }

                    // Buffer the directions for as long as specified
                    int directionArray = (int)directions;
                    int xorArray = (int)(Direction.B | Direction.F);
                    for (int i = 1; i < directionBufferTime; i++)
                    {
                        directionArray |= (directionArray << 4);
                        xorArray |= (xorArray << 4);
                    }

                    // Now create the command
                    if (triggerNum == 1)
                    {
                        // Contains F/B
                        if ((directions & (Direction.F | Direction.B)) > 0)
                        {
                            if (currCmdType == CommandType.StupidShit)
                                bufferedCommand.AppendFormat(INIT_TRIGGER_BF, commandVar, (int)currCmdType + 3, -1, "= " + directionArray, directionArray ^ xorArray);
                            else
                                bufferedCommand.AppendFormat(INIT_TRIGGER_BF, commandVar, (int)(currCmdType == CommandType.MultiDirectional ? CommandType.Press : currCmdType) + 3, directionArray, "> 0", directionArray ^ xorArray);
                        }
                        // Contains U/D
                        else if ((directions & (Direction.U | Direction.D)) > 0)
                        {
                            if (currCmdType == CommandType.StupidShit)
                                bufferedCommand.AppendFormat(INIT_TRIGGER_UD, commandVar, (int)currCmdType + 3, -1, "= " + directionArray);
                            else
                                bufferedCommand.AppendFormat(INIT_TRIGGER_UD, commandVar, (int)(currCmdType == CommandType.MultiDirectional ? CommandType.Press : currCmdType) + 3, directionArray, "> 0");
                        }
                        // Button
                        else if (buttons.Count > 0)
                        {
                            // We're gonna have to build a special string for this.
                            StringBuilder buttonBuilder = new StringBuilder();

                            // There's all sorts of bullshit here.
                            int[] buttonArrays = new int[(int)CommandType.StupidShit+1];
                            for (int i=buttons.Count-1; i >= 0; i--)
                            {
                                buttonArrays[(int)cmdTypes[i]] |= (int)buttons[i];
                            }

                            // Iterate through the button arrays.
                            for (int i=0; i < cmdTypes.Count; i++)
                            {
                                // Buffer the buttons for as long as specified
                                int buttonArray = buttonArrays[i];
                                for (int j = 1; j < buttonBufferTime; j++)
                                {
                                    buttonArray |= (buttonArray << 7);
                                }

                                bufferedCommand.AppendFormat(VARIABLE_TEMPLATE, i, buttonArray, cmdTypes[i] == CommandType.StupidShit ? "=" : ">", cmdTypes[i] == CommandType.StupidShit ? buttonArray : 0);
                            }

                            // Build the final string
                            bufferedCommand.AppendFormat(INIT_TRIGGER_BT, commandVar, buttonBuilder.ToString());
                        }
                        else
                            throw new ArgumentException(String.Format("Malformed command token {0}: unexpected character.", currCmdType.ToString()));

                        // Append the end
                        bufferedCommand.AppendFormat(SECONDARY_TRIGGER, triggerNum, commandVar, elemBufferTime, 3 + triggerNum);
                    }
                    else
                    {
                        // Contains F/B
                        if ((directions & (Direction.F | Direction.B)) > 0)
                        {
                            if (currCmdType == CommandType.StupidShit)
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER_BF, triggerNum, commandVar, 3 + triggerNum, 2 + triggerNum, (int)CommandType.Press + 3, -1, "= " + directionArray, -1, "= " + (directionArray^xorArray));
                            else
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER_BF, triggerNum, commandVar, 3 + triggerNum, 2 + triggerNum, (int)(currCmdType == CommandType.MultiDirectional ? CommandType.Press : currCmdType) + 3, directionArray, "> 0", directionArray ^ xorArray, "> 0");
                        }
                        // Contains U/D
                        else if ((directions & (Direction.U | Direction.D)) > 0)
                        {
                            if (currCmdType == CommandType.StupidShit)
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER_UD, triggerNum, commandVar, 3 + triggerNum, 2 + triggerNum, (int)CommandType.Press+3, -1, "= " + directionArray);
                            else
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER_UD, triggerNum, commandVar, 3 + triggerNum, 2 + triggerNum, (int)(currCmdType == CommandType.MultiDirectional ? CommandType.Press : currCmdType) + 3, directionArray, "> 0");
                        }
                        // Button
                        else if (buttons.Count > 0)
                        {
                            // We're gonna have to build a special string for this.
                            StringBuilder buttonBuilder = new StringBuilder();

                            // There's all sorts of bullshit here.
                            int[] buttonArrays = new int[(int)CommandType.StupidShit + 1];
                            for (int i = buttons.Count - 1; i >= 0; i--)
                            {
                                buttonArrays[(int)cmdTypes[i]] |= (int)buttons[i];
                            }

                            // Iterate through the button arrays.
                            for (int i = 0; i < cmdTypes.Count; i++)
                            {
                                // Buffer the buttons for as long as specified
                                int buttonArray = buttonArrays[i];
                                for (int j = 1; j < buttonBufferTime; j++)
                                {
                                    buttonArray |= (buttonArray << 7);
                                }

                                bufferedCommand.AppendFormat(VARIABLE_TEMPLATE, i, buttonArray, cmdTypes[i] == CommandType.StupidShit ? "=" : ">", cmdTypes[i] == CommandType.StupidShit ? buttonArray : 0);
                            }

                            // Build the final string
                            bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER_BT, triggerNum, commandVar, 3+triggerNum, 2+triggerNum, buttonBuilder.ToString());
                        }
                        else
                            throw new ArgumentException(String.Format("Malformed command token {0}: unexpected character.", currCmdType.ToString()));

                        // Append the end
                        bufferedCommand.AppendFormat(SECONDARY_TRIGGER, triggerNum, commandVar, elemBufferTime, 3 + triggerNum);
                    }

                    triggerNum++;

                    // Append the end
                    if (triggerNum > tokens.Length)
                    {
                        bufferedCommand.AppendFormat(DEINIT_TRIGGER, triggerNum, commandVar);
                        bufferedCommand.Append(IGNOREHITPAUSE);
                    }
                }
            }
            else
                throw new ArgumentException("Malformed command string");


            return bufferedCommand.ToString();
        }
    }
}
