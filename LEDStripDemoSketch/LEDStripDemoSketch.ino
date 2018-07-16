#define ADR_ACTION_TYPE 0
#define ADR_COLOR_SCHEME 2
#define ADR_DELAY 4
#define ADR_RAINBOW_INCR 8

#define WHEEL_SIZE 384 // wheel sizes > 384 will cause gaps in the rainbow, while < 384 will cause truncated rainbow

#include <EEPROM.h>
#include "LPD8806.h"
#include "SPI.h" // Comment out this line if using Trinket or Gemma

enum ActionTypes
{
  Unknown,
  Chase,
  TheaterChase,
  Solid,
  Rain,
  
  Discovery,
  ExportSettings
};

enum ColorSchemes
{
  UnknownColor,
  SimpleGreenColor,
  SimpleRedColor,
  SimpleBlueColor,
  PulsingGreenColor,
  PulsingRedColor,
  PulsingBlueColor,
  RainbowColor,
  RandomColor
};
// Number of RGB LEDs in strand
int countLEDs = 32;
// LED strip instance
LPD8806 strip = LPD8806(countLEDs);
// Current action type
ActionTypes currentActionType = Unknown;
// current color scheme
ColorSchemes currentColorScheme = UnknownColor;
uint32_t currentRainbowIncrement = 1;
uint32_t currentRainbowIndex = 0;
uint8_t currentStateIncrement = 1;
uint8_t currentStateIndex = 0;
// current delay value
int currentDelay = 10;
// Current serial input string
String inputString = "";

// Perform one-time start-up routines
void setup()
{
  // random seed
  randomSeed(analogRead(0));
  // initialize serial
  Serial.begin(9600);
  while(!Serial){;}
  Serial.println("> Initializing Device...");
  // read settings from NVS
  ReadSettings();
  // initialize strip
  strip.begin();
  ClearStrip();
  return;
}


void loop()
{
  // retrieve current color
  uint32_t color = GetColor(currentColorScheme, currentRainbowIndex);
  
  // determine current pattern
  switch(currentActionType)
  {
    case Chase:
      LightChase(currentStateIndex, color, 1);
      break;
    case TheaterChase:
      LightChaseTheater(currentStateIndex, color, 3);
      break;
    case Solid:
      Light(currentStateIndex, color);
      break;
    case Rain:
      currentStateIndex = random(0, strip.numPixels());
      Light(currentStateIndex, color);
      break;
    default:
    case Unknown:
      ClearStrip();
      break;
  }
  
  // increment state index
  currentStateIndex += currentStateIncrement;
  if(currentStateIndex >= strip.numPixels())
    currentStateIndex = 0;
  // increment rainbow index
  currentRainbowIndex += currentRainbowIncrement;
  if(currentRainbowIndex >= WHEEL_SIZE)
    currentRainbowIndex = 0;

  // read serial bus
  ReadSerial();

  // wait for delay
  delay(currentDelay);
}

/// <summary>
/// Turn off all LEDs
/// </summary>
void ClearStrip()
{
  for(int i=0; i<strip.numPixels(); i++)
  {
    // light pixel
    strip.setPixelColor(i, 0);
  }
  strip.show();
  return;
}

/// <summary>
/// Retrieve a color associated with the specified color scheme
/// </summary>
/// <param name=scheme>Color scheme</param>
/// <param name=seed>Optional color seed</param>
/// <returns>An unsigned integer representing an RGB color value</returns>
uint32_t GetColor(ColorSchemes scheme, uint16_t seed)
{
  uint32_t color = strip.Color(0, 0, 0);
  float ratio = ((float)seed)/((float)WHEEL_SIZE);
  switch(scheme)
  {
    case SimpleRedColor:
      color = strip.Color(255, 0, 0);
      break;
    case PulsingRedColor:
      color = strip.Color(ratio * 255.0, 0, 0);
      break;
    case SimpleGreenColor:
      color = strip.Color(0, 255, 0);
      break;
    case PulsingGreenColor:
      color = strip.Color(0, ratio * 255.0, 0);
      break;
    case SimpleBlueColor:
      color = strip.Color(0, 0, 255);
      break;
    case PulsingBlueColor:
      color = strip.Color(0, 0, ratio * 255.0);
      break;
    case RandomColor:
      color = strip.Color(random(0,256), random(0, 256), random(0, 256));
      break;
    case RainbowColor:
      color = GetWheelColor(seed);
      break;
    default:
    case UnknownColor:
      break;
  }
  return color;
}

/// <summary>
/// Retrieve a color associated with the specified wheel position
/// </summary>
/// <param name=position>Position on the color wheel</param>
/// <returns>An unsigned integer representing an RGB color value</returns>
/// <remarks>Wheel is a rainbow color wheel with 384 positions that transition from red to green to blue, then back to red</remarks>
uint32_t GetWheelColor(uint16_t position)
{
  // trim position
  position = position % WHEEL_SIZE;
  byte r=0, g=0, b=0;
  switch(position / 128)
  {
    case 0:
      r = 127 - position % 128;   //Red down
      g = position % 128;      // Green up
      b = 0;                  //blue off
      break; 
    case 1:
      g = 127 - position % 128;  //green down
      b = position % 128;      //blue up
      r = 0;                  //red off
      break; 
    case 2:
      b = 127 - position % 128;  //blue down 
      r = position % 128;      //red up
      g = 0;                  //green off
      break; 
  }
  return(strip.Color(r,g,b));
}

/// <summary>
/// Chase a set of sequential pixels down the strip
/// </summary>
/// <param name=index>State index</param>
/// <param name=color>Pixel color</param>
/// <param name=count>Number of lit pixels</param>
void LightChase(uint8_t index, uint32_t color, uint8_t count)
{
  // queue new pixels on
  for(int j=0; j<count && index+j<strip.numPixels(); j++)
  {
    strip.setPixelColor(index+j, color);
  }

  // update lit pixels
  strip.show();
  
  // queue pixels off
  for(int j=0; j<count && index+j<strip.numPixels(); j++)
  {
    strip.setPixelColor(index+j, 0);
  }
  return;
}

/// <summary>
/// Chase a set of evenly-spaced pixels down the strip
/// </summary>
/// <param name=index>State index</param>
/// <param name=color>Pixel color</param>
/// <param name=spacing>Spacing of lit pixels</param>
void LightChaseTheater(uint8_t index, uint32_t color, uint8_t spacing)
{
  // queue new pixels on
  for (int j=0; index+j<strip.numPixels(); j += spacing)
  {
    strip.setPixelColor(index+j, color);
  }

  // update lit pixels
  strip.show();

  // queue pixels off
  for (int j=0; index+j<strip.numPixels(); j += spacing)
  {
    strip.setPixelColor(index+j, 0);
  }
  return;
}

/// <summary>
/// Light a single pixel
/// </summary>
/// <param name=index>Pixel index</param>
/// <param name=color>Pixel color</param>
void Light(uint8_t index, uint32_t color)
{
  // light pixel
  strip.setPixelColor(index, color);
  strip.show();
  return;
}

/// <summary>
/// Parse the specified string as a firmware command
/// </summary>
/// <param name=command>Command string to parse</param>
void ParseCommand(String command)
{
  String temp_str = "";
  Serial.println("> Reading Command... ");
  // determine input string length
  int str_length = command.length();
  // parse action type
  if(str_length < 1)
    return;
  int semicolon_index = command.indexOf(';', 0);
  int old_semicolon_index = 0;
  if (semicolon_index < 1)
  {
    Serial.println("! Command Warning: Semicolon Not Found, Cannot Parse Action Type");
    return;
  }
  temp_str = command.substring(old_semicolon_index, semicolon_index);
  ActionTypes action_type = CommandStringToActionType(temp_str);
  Serial.println("> Action Type: "+ActionTypeToString(action_type));
  if(IsSerialActionType(action_type))
  {
    // Serial Action Type
    switch(action_type)
    {
      case Discovery:
        SendDiscoveryResponse();
        break;
      case ExportSettings:
        SendSettingsResponse();
        break;
    }
  }
  else
  {
    // LED Action Type
    currentActionType = action_type;
    // parse color scheme
    old_semicolon_index = semicolon_index;
    semicolon_index = command.indexOf(';', old_semicolon_index+1);
    if (semicolon_index < 1)
    {
      Serial.println("! Command Warning: Semicolon Not Found, Cannot Parse Color Scheme");
      return;
    }
    temp_str = command.substring(old_semicolon_index+1, semicolon_index);
    ColorSchemes color_scheme = CommandStringToColorScheme(temp_str);
    Serial.println("> Color Scheme: "+ColorSchemeToString(color_scheme));
    currentColorScheme = color_scheme;
    // parse delay
    old_semicolon_index = semicolon_index;
    semicolon_index = command.indexOf(';', old_semicolon_index+1);
    if (semicolon_index < 1)
    {
      Serial.println("! Command Warning: Semicolon Not Found, Cannot Parse Delay Value");
      return;
    }
    temp_str = command.substring(old_semicolon_index+1, semicolon_index);
    int delay_value = temp_str.toInt();
    if(delay_value != 0)
       currentDelay = delay_value;
    Serial.println("> Delay Value: "+String(currentDelay));
    // parse rainbow increment
    old_semicolon_index = semicolon_index;
    semicolon_index = command.indexOf(';', old_semicolon_index+1);
    if (semicolon_index < 1)
    {
      Serial.println("! Command Warning: Semicolon Not Found, Cannot Parse Rainbow Increment");
      return;
    }
    temp_str = command.substring(old_semicolon_index+1, semicolon_index);
    int rainbow_increment = temp_str.toInt();
    if(rainbow_increment != 0)
       currentRainbowIncrement = rainbow_increment;
    Serial.println("> Rainbow Increment: "+String(currentRainbowIncrement));
  }
  return;
}

void ReadSerial()
{
  while (Serial.available())
  {
    // get the new byte:
    char input_char = (char)Serial.read();
    // add it to the inputString:
    inputString += input_char;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (input_char == '\n')
    {
      // parse command and update settings
      ParseCommand(inputString);
      WriteSettings();
      ClearStrip();
      currentStateIndex = 0;
      inputString = "";
      break;
    }
  }
  return;
}

void ReadSettings()
{
  Serial.println("> Reading Settings...");
  // read action type
  int value = EEPROM.read(ADR_ACTION_TYPE);
  if(value == 255)
    currentActionType = Unknown;
  else
    currentActionType = value;
  Serial.println("> Action Type: "+ActionTypeToString(currentActionType));
  // read color scheme
  value = EEPROM.read(ADR_COLOR_SCHEME);
  if(value == 255)
    currentColorScheme = UnknownColor;
  else
    currentColorScheme = value;
  Serial.println("> Color Scheme: "+ColorSchemeToString(currentColorScheme));
  // read delay value
  value = EEPROM.read(ADR_DELAY);
  if(value == 255)
    currentDelay = 10;
  else
    currentDelay = value;
  Serial.println("> Delay: "+String(currentDelay));
  // read rainbow increment
  value = EEPROM.read(ADR_RAINBOW_INCR);
  if(value == 255)
    currentRainbowIncrement = 1;
  else
    currentRainbowIncrement = value;
  Serial.println("> Rainbow Increment: "+String(currentRainbowIncrement));
  return;
}

void SendDiscoveryResponse()
{
  Serial.println("BM2018");
  return;
}

void SendSettingsResponse()
{
  // construct settings string
  // .. add prefix
  String text = ":ES:";
  // .. add current action type
  text += ActionTypeToCommandString(currentActionType) + ";";
  // .. add current color scheme
  text += ColorSchemeToCommandString(currentColorScheme) + ";";
  // .. add current delay value
  text += String(currentDelay) + ";";
  // .. add current rainbow increment
  text += String(currentRainbowIncrement) + ";";
  // .. add suffix
  text += ":ES:";
  // send string
  Serial.println(text);
  return;
}

void WriteSettings()
{
  EEPROM.write(ADR_ACTION_TYPE, currentActionType);
  EEPROM.write(ADR_COLOR_SCHEME, currentColorScheme);
  EEPROM.write(ADR_DELAY, currentDelay);
  EEPROM.write(ADR_RAINBOW_INCR, currentRainbowIncrement);
  return;
}

String ActionTypeToString(ActionTypes action_type)
{
  switch(action_type)
  {
    case Discovery:
      return "Discovery";
    case ExportSettings:
      return "Export Settings";
      
    case Unknown:
      return "Unknown";
    case Chase:
      return "Chase";
    case TheaterChase:
      return "Theater Chase";
    case Solid:
      return "Solid";
    case Rain:
      return "Rain";
    default:
      return "Undefined!";
  }
}

String ActionTypeToCommandString(ActionTypes action_type)
{
  switch(action_type)
  {
    case Chase:
      return "c";
    case TheaterChase:
      return "h";
    case Solid:
      return "s";
    case Rain:
      return "r";
    case Unknown:
    default:
      return "?";
  }
}

String ColorSchemeToCommandString(ColorSchemes scheme)
{
  switch(scheme)
  {
    case PulsingRedColor:
      return "pr";
    case PulsingGreenColor:
      return "pg";
    case PulsingBlueColor:
      return "pb";
    case RainbowColor:
      return "r";
    case RandomColor:
      return "x";
    case SimpleRedColor:
      return "sr";
    case SimpleGreenColor:
      return "sg";
    case SimpleBlueColor:
      return "sb";
    default:
      return "?";
  }
}

String ColorSchemeToString(ColorSchemes scheme)
{
  switch(scheme)
  {
    case PulsingRedColor:
      return "Pulsing Red";
    case PulsingGreenColor:
      return "Pulsing Green";
    case PulsingBlueColor:
      return "Pulsing Blue";
    case RainbowColor:
      return "Rainbow";
    case RandomColor:
      return "Randomn";
    case SimpleRedColor:
      return "Simple Red";
    case SimpleGreenColor:
      return "Simple Green";
    case SimpleBlueColor:
      return "Simple Blue";
    default:
      return "Undefined!";
  }
}

ActionTypes CommandStringToActionType(String text)
{
  // LED Actions
  if (text == "c")
      return Chase;
  if (text == "h")
      return TheaterChase;
  if (text == "s")
      return Solid;
  if (text == "r")
      return Rain;

  // Serial Interface Actions
  if (text == "ID")
      return Discovery;
  if (text == "ES")
      return ExportSettings;
  return Unknown;
}

ColorSchemes CommandStringToColorScheme(String text)
{
  if (text == "pr")
    return PulsingRedColor;
  if (text == "pg")
    return PulsingGreenColor;
  if (text == "pb")
    return PulsingBlueColor;
  if (text == "r")
    return RainbowColor;
  if (text == "x")
    return RandomColor;
  if (text == "sr")
    return SimpleRedColor;
  if (text == "sg")
    return SimpleGreenColor;
  if (text == "sb")
    return SimpleBlueColor;
  return UnknownColor;
}

bool IsSerialActionType(ActionTypes action_type)
{
  switch(action_type)
  {
    case ExportSettings:
    case Discovery:
      return true;
    default:
      return false;
  }
}

void serialEvent()
{
  ReadSerial();
  return;
}
