using System;
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
        [Flags]
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
        [Flags]
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
        [Flags]
        public enum CommandType : byte
        {
            /// <summary>
            /// Regular button press
            /// </summary>
            Press                   = 0x00,
            /// <summary>
            /// Contains a /
            /// </summary>
            Hold                    = 0x01,
            /// <summary>
            /// Contains a ~
            /// </summary>
            Release                 = 0x02,
            /// <summary>
            /// Contains a $
            /// </summary>
            MultiDirectional        = 0x04,
            /// <summary>
            /// Contains a +
            /// </summary>
            Combination             = 0x08,
            /// <summary>
            /// Contains that fucking > thing
            /// </summary>
            Strict                  = 0x10
        }

        // Maximum number of commands that can fit in this system.
        public const int MAX_NUM_COMMANDS = 27;

        // String templates
        private const string CMD_HEADER         = "[State 10380, {0} Init]\r\ntype = Null\r\n";
        private const string INIT_TRIGGER       = "trigger1 = !Var({0}) {1}\r\n";
        private const string SUBSEQUENT_TRIGGER = "trigger{0} = (Var({1})&(2**{2} - 1)) > (2**{3}){4}\r\n";
        private const string SECONDARY_TRIGGER  = "trigger{0} = e||(var({1}) := {2} + (2**{3}))\r\n\r\n";
        private const string DEINIT_TRIGGER     = "trigger{0} = Var({1}) && (Var({1})&15) = 0\r\ntrigger{0} = e||(var({1}) := 0)\r\n";
        private const string IGNOREHITPAUSE     = "ignorehitpause = 1\r\n";
        private const string VARIABLE_TEMPLATE  = " {0} (var({1})&{2}) {3} {4}"; //TODO: Clean the rest of this crap up with this.
        private const string CMD_TRIGGER_1      = "triggerall = (helper(10371), Var({0})&(2**{1} - 1)) > (2**{2})\r\n";
        private const string CMD_TRIGGER_2      = "triggerall = {0}\r\n";

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
        public static Tuple<string,string> CMDToBufferingSystem(string title, string command, byte buttonBufferTime, byte directionBufferTime, byte elemBufferTime, byte commandVar)
        {
            // We're gonna be doing LOTS of appending, so let's use StringBuilder.
            StringBuilder bufferedCommand = new StringBuilder();
            StringBuilder cmdTriggers = new StringBuilder();

            // Add the title first
            bufferedCommand.AppendFormat(CMD_HEADER, title);

            // Replace all whitespaces
            command = Regex.Replace(command, @"\s", String.Empty);

            // I am not explaining this but let's just say if this shit matches, we're good to go... so far.
            Match firstCheck = Regex.Match(command, @"(((~|/)?(((\${0})(UB|DB|DF|UF)|(\$?)(U|D|B|F))|((a|b|c|x|y|z|s)\+?)*)))(,(((~|/|>)?(((\${0})(UB|DB|DF|UF)|(\$?)(U|D|B|F))|((a|b|c|x|y|z|s)\+?)+))+))+");
            if (firstCheck.ToString() == command)
            {

                // Separate by commas and +'s
                string[] tokens = command.Split(',');
                if (tokens.Length > MAX_NUM_COMMANDS)
                    throw new ArgumentException(String.Format("Command cannot contain more than {0} presses!", MAX_NUM_COMMANDS));

                // Triggers always start at 1
                int triggerNum = 1;

                // Used to properly build the trigger list when the command ends in a button.
                bool endsInButton = false;

                // Iterate through that shit!
                foreach (string token in tokens)
                {
                    // Split character-by-character.
                    char[] pieces = token.ToCharArray();

                    if (pieces.Length == 0)
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
                        else
                        {
                            if (pieces[i] == '$' && currCmdType == CommandType.Press)
                                currCmdType = CommandType.MultiDirectional;
                            else if (pieces[i] == '/')
                                currCmdType |= CommandType.Hold;
                            else if (pieces[i] == '~')
                                currCmdType |= CommandType.Release;
                            // Combinations require some more work
                            else if (pieces[i] == '+' && buttons.Count > 0)
                            {
                                // We can only have a+b+c+x+y+z+start at most
                                if (cmdTypes.Count > 6)
                                    throw new ArgumentException(String.Format("Malformed command token at token {0}, character {1}: number of buttons exceeded.", triggerNum, i));
                            }
                            //TODO
                            //else if (pieces[i] == '>')
                            //    currCmdType = CommandType.Strict;
                            // Or bullshit?
                            else
                                throw new ArgumentException(String.Format("Malformed command token at token {0}, character {1}: unexpected character.", triggerNum, i));

                            // Add to the command types array if we encounter a + or the start of the string.
                            if (pieces[i] == '+')
                            {
                                cmdTypes.Add(currCmdType);
                                currCmdType = CommandType.Press;
                            }
                        }
                    }

                    // Add the final command type for buttons
                    if (buttons.Count > 0)
                        cmdTypes.Add(currCmdType);

                    // Buffer the directions for as long as specified
                    int directionArray = (int)directions;
                    int maskArray = 0xF;
                    for (int i = 1; i < directionBufferTime; i++)
                    {
                        directionArray = (directionArray << 4);
                        maskArray = (maskArray << 4);
                    }

                    // Now create the command
                    if (triggerNum == 1)
                    {
                        // Contains a direction
                        if (directions > 0)
                        {
                            if ((currCmdType&CommandType.MultiDirectional) > 0)
                            {
                                string commandBuffer = String.Format(VARIABLE_TEMPLATE, "&&", (int)(currCmdType^CommandType.MultiDirectional) + 3, directionArray, ">", 0);
                                bufferedCommand.AppendFormat(INIT_TRIGGER, commandVar, commandBuffer);
                            }
                            else
                            {
                                // We're gonna have to build a special string for this.
                                StringBuilder directionBuilder = new StringBuilder(String.Format(VARIABLE_TEMPLATE, "&&", (int)(currCmdType+3), maskArray, "=", directionArray));
                                for (int i = 1; i < directionBufferTime; i++)
                                {
                                    directionArray = (directionArray >> 4);
                                    maskArray = (maskArray >> 4);
                                    directionBuilder.AppendFormat(VARIABLE_TEMPLATE, "||", (int)(currCmdType+3), maskArray, "=", directionArray);
                                }

                                if (directionBufferTime > 1)
                                {
                                    directionBuilder.Insert(4, "(");
                                    directionBuilder.Append(")");
                                }

                                bufferedCommand.AppendFormat(INIT_TRIGGER, commandVar, directionBuilder);
                            }
                        }
                        // Button
                        else if (buttons.Count > 0)
                        {
                            // We're gonna have to build a special string for this.
                            StringBuilder buttonBuilder = new StringBuilder();

                            // There's all sorts of bullshit here.
                            int[] buttonArrays = new int[(int)CommandType.Strict+1];
                            for (int i=cmdTypes.Count-1; i >= 0; i--)
                            {
                                buttonArrays[(int)cmdTypes[i]] |= (int)buttons[i];
                            }

                            // Iterate through the button arrays.
                            for (int i=0; i < cmdTypes.Count; i++)
                            {
                                // Buffer the buttons for as long as specified
                                int buttonArray = buttonArrays[(int)cmdTypes[i]];
                                for (int j = 1; j < buttonBufferTime; j++)
                                {
                                    buttonArray |= (buttonArray<<7);
                                }

                                if (buttonArray > 0)
                                    buttonBuilder.AppendFormat(VARIABLE_TEMPLATE, "&&", (int)cmdTypes[i], buttonArray, (cmdTypes[i]&CommandType.MultiDirectional) > 0 ? ">" : "=", (cmdTypes[i] & CommandType.MultiDirectional) > 0 ? 0 : buttonArray);
                            }

                            // Build the final string
                            bufferedCommand.AppendFormat(INIT_TRIGGER, commandVar, buttonBuilder);
                        }
                        else
                            throw new ArgumentException(String.Format("Malformed command token {0}: unexpected character.", currCmdType.ToString()));

                        // Append the end
                        bufferedCommand.AppendFormat(SECONDARY_TRIGGER, triggerNum, commandVar, elemBufferTime, 3+triggerNum, String.Empty);
                    }
                    else
                    {
                        // Contains a direction
                        if (directions > 0)
                        {

                            if ((currCmdType&CommandType.MultiDirectional) > 0)
                            {
                                int d = directionArray;
                                for (int i = 1; i < directionBufferTime; i++)
                                {
                                    d |= (directionArray << 4);
                                }
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER, triggerNum, commandVar, 3 + triggerNum, 2 + triggerNum, String.Format(VARIABLE_TEMPLATE, "&&", (int)(currCmdType+3), d, ">", 0), "");
                            }
                            else
                            {
                                // We're gonna have to build a special string for this.
                                StringBuilder directionBuilder = new StringBuilder(String.Format(VARIABLE_TEMPLATE, "&&", (int)(currCmdType+3), maskArray, "=", directionArray));
                                for (int i = 1; i < directionBufferTime; i++)
                                {
                                    directionArray = (directionArray >> 4);
                                    maskArray = (maskArray >> 4);
                                    directionBuilder.AppendFormat(VARIABLE_TEMPLATE, "||", (int)(currCmdType+3), maskArray, "=", directionArray);
                                }

                                if (directionBufferTime > 1)
                                {
                                    directionBuilder.Insert(4, "(");
                                    directionBuilder.Append(")");
                                }

                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER, triggerNum, commandVar, 3+triggerNum, 2+triggerNum, directionBuilder);
                            }

                        }
                        // Button
                        else if (buttons.Count > 0)
                        {
                            // We're gonna have to build a special string for this.
                            StringBuilder buttonBuilder = new StringBuilder();

                            // There's all sorts of bullshit here.
                            int[] buttonArrays = new int[8];
                            for (int i = cmdTypes.Count - 1; i >= 0; i--)
                            {
                                buttonArrays[(int)cmdTypes[i]] |= (int)buttons[i];
                            }

                            // Iterate through the button arrays.
                            for (int i = 0; i < cmdTypes.Count; i++)
                            {
                                // Buffer the buttons for as long as specified
                                int buttonArray = buttonArrays[(int)cmdTypes[i]];
                                for (int j = 1; j < buttonBufferTime; j++)
                                {
                                    buttonArray |= (buttonArray<<7);
                                }

                                if (buttonArray > 0)
                                    buttonBuilder.AppendFormat(VARIABLE_TEMPLATE, "&&", (int)cmdTypes[i], buttonArray, (cmdTypes[i]&CommandType.Strict) > 0 ? "=" : ">", cmdTypes[i] == CommandType.Strict ? buttonArray : 0);
                            }

                            // Build the final string
                            if (triggerNum < tokens.Length)
                                bufferedCommand.AppendFormat(SUBSEQUENT_TRIGGER, triggerNum, commandVar, 3+triggerNum, 2+triggerNum, buttonBuilder.ToString());
                            else if (buttons.Count > 0)
                            {
                                cmdTriggers.AppendFormat(CMD_TRIGGER_1, commandVar, 3+triggerNum, 2+triggerNum);

                                // Need to do a little work on the built buttons.

                                // Remove " &&" from the start
                                buttonBuilder.Remove(0, 3);

                                // Now add the helper redirection
                                buttonBuilder.Replace("var", "(helper(10371), var");
                                buttonBuilder.Replace(") > 0", ")) > 0");
                                buttonBuilder.Replace(") = 0", ")) = 0");

                                // Finally, build the command.
                                cmdTriggers.AppendFormat(CMD_TRIGGER_2, buttonBuilder); 
                                endsInButton = true;
                            }
                        }
                        else
                            throw new ArgumentException(String.Format("Malformed command token {0}: unexpected character.", currCmdType.ToString()));

                        // Append the end
                        if (!endsInButton)
                            bufferedCommand.AppendFormat(SECONDARY_TRIGGER, triggerNum, commandVar, elemBufferTime, 3+triggerNum);
                    }

                    triggerNum++;

                    // Append the end
                    if (triggerNum > tokens.Length)
                    {
                        bufferedCommand.AppendFormat(DEINIT_TRIGGER, triggerNum - Convert.ToInt32(endsInButton), commandVar);
                        bufferedCommand.Append(IGNOREHITPAUSE);
                    }
                }
            }
            else
                throw new ArgumentException("Malformed command string");


            return new Tuple<string, string>(bufferedCommand.ToString(), cmdTriggers.ToString());
        }
    }
}
