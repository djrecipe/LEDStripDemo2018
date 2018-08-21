#define WHEEL_SIZE 384 // wheel sizes > 384 will cause gaps in the rainbow, while < 384 will cause truncated rainbow

#define DISCOVERY_RESPONSE "BM2018"

#include <EEPROM.h>

#include "base64.hpp"
#include "LPD8806.h"
#include "SPI.h" // Comment out this line if using Trinket or Gemma

enum PatternModes
{
  Unknown,
  Chase,
  TheaterChase,
  Fill,
  Rain,
  Solid,
  Rider,
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

struct Settings
{
  int ColorIncrement;
  ColorSchemes ColorScheme;
  int PatternDelay;
  PatternModes PatternMode;
  int PatternSpacing;
};

struct State
{
  bool ColorIncrementing;
  int ColorIndex;
  int PatternDelayModified;
  int StateIncrement;
  int StateIndex;
};

// Number of RGB LEDs in strand
int countLEDs = 32;
// LED strip instance
LPD8806 strip = LPD8806(countLEDs);
// Current serial input string
String inputString = "";
Settings currentSettings = Settings();
State currentState = State();
bool toggleValue = false;

// Perform one-time start-up routines
void setup()
{
  pinMode(12, INPUT);
  // random seed
  randomSeed(analogRead(0));
  // initialize serial
  Serial.begin(9600);
  while(!Serial){;}
  // read settings from NVS
  currentSettings = ReadEEPROM();
  ValidateCurrentSettings();
  PrintSettings(currentSettings);
  // initialize strip
  strip.begin();
  ClearStrip();
  return;
}


void loop()
{
  // retrieve current color
  uint32_t color = GetColor(currentSettings.ColorScheme, currentState.ColorIndex);
  // various values for miscellaneous things
  int value = 0;
  
  // determine current pattern
  switch(currentSettings.PatternMode)
  {
    case Chase:
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = 1;
      LightChase(currentState.StateIndex, color, currentSettings.PatternSpacing);
      break;
    case TheaterChase:
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = 1;
      LightChaseTheater(currentState.StateIndex, color, currentSettings.PatternSpacing);
      break;
    case Fill:
      if(toggleValue)
        color = strip.Color(0,0,0);
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = 1;
      Light(currentState.StateIndex, color);
      if(currentState.StateIndex >= strip.numPixels() - currentState.StateIncrement)
        toggleValue = !toggleValue;
      break;
    case Rain:
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = random(-1 * currentSettings.PatternSpacing,currentSettings.PatternSpacing + 1);
      Light(currentState.StateIndex, color);
      break;
    case Solid:
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = 1;
      for(currentState.StateIndex=0; currentState.StateIndex<strip.numPixels(); currentState.StateIndex++)
      {
        Light(currentState.StateIndex, color);
      }
      break;
    case Rider:
      if(currentState.StateIndex >= strip.numPixels() - 1)
        currentState.StateIncrement = -1;
      else if(currentState.StateIndex <= currentSettings.PatternSpacing - 1) // bounce off edge without wrapping around
        currentState.StateIncrement = 1;
      value = ((int)strip.numPixels() +  (currentSettings.PatternSpacing - 1))/2 - currentState.StateIndex; // determine distance from middle
      currentState.PatternDelayModified = currentSettings.PatternDelay * max(abs(value) / 2, 1); // slower towards edges (multiply half of distance time delay)
      LightChase(currentState.StateIndex, color, currentSettings.PatternSpacing);
      break;
    default:
    case Unknown:
      currentState.PatternDelayModified = currentSettings.PatternDelay;
      currentState.StateIncrement = 1;
      ClearStrip();
      break;
  }
  
  // increment state index
  currentState.StateIndex = (currentState.StateIndex + currentState.StateIncrement)%strip.numPixels();
  IncrementColorIndex();
  // read serial bus
  ReadSerial();

  // wait for delay
  delay(currentState.PatternDelayModified);
}

/// <summary>
/// Turn off all LEDs
/// </summary>
void ClearStrip()
{
  currentState = State();
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
  byte primary_value = 0;
  byte secondary_value = 0;
  switch(scheme)
  {
    case SimpleRedColor:
      color = strip.Color(100, 0, 0);
      break;
    case PulsingRedColor:
      primary_value = ratio * 45.0;
      secondary_value = 1;
      color = strip.Color(primary_value, secondary_value, secondary_value);
      break;
    case SimpleGreenColor:
      color = strip.Color(0, 100, 0);
      break;
    case PulsingGreenColor:
      primary_value = ratio * 45.0;
      secondary_value = 1;
      color = strip.Color(secondary_value, primary_value, secondary_value);
      break;
    case SimpleBlueColor:
      color = strip.Color(0, 0, 100);
      break;
    case PulsingBlueColor:
      primary_value = ratio * 45.0;
      secondary_value = 1;
      color = strip.Color(secondary_value, secondary_value, primary_value);
      break;
    case RandomColor:
      color = strip.Color(random(0,127), random(0, 127), random(0, 127));
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
/// Advance the current color index according to the current settings
/// </summary>
void IncrementColorIndex()
{
  switch(currentSettings.ColorScheme)
  {
    // rainbow color goes in complete wheel/circle, so reset index to zero
    //   in order to complete the rainbow "wheel"
    case RainbowColor:
      currentState.ColorIndex += currentSettings.ColorIncrement;
      if(currentState.ColorIndex >= WHEEL_SIZE)
        currentState.ColorIndex = 0;
      break;
    // default behavior is to "bounce" back and forth between two extremes
    //   of the color cycle
    default:
      // determine color index direction
      if(currentState.ColorIndex >= WHEEL_SIZE)
        currentState.ColorIncrementing = false;
      else if(currentState.ColorIndex <=0)
        currentState.ColorIncrementing = true;
      // increment or decrement color index
      if(currentState.ColorIncrementing)
        currentState.ColorIndex += currentSettings.ColorIncrement;
      else
        currentState.ColorIndex -= currentSettings.ColorIncrement;
      break;
  }
  return;
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
  int target_index = 0;
  for(int j=0; j<count && ((int)index)-j<(int)strip.numPixels(); j++)
  {
    target_index = (((int)index) - j)%strip.numPixels();
    strip.setPixelColor(target_index, color);
  }

  // update lit pixels
  strip.show();
  
  // queue pixels off
  for(int j=0; j<count && ((int)index)-j<(int)strip.numPixels(); j++)
  {
    target_index = (((int)index) - j)%strip.numPixels();
    strip.setPixelColor(target_index, 0);
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
  int target_index = 0;
  for (int j=0; j<strip.numPixels(); j += spacing)
  {
    target_index = (j+index)%strip.numPixels();
    strip.setPixelColor(target_index, color);
  }
  // update lit pixels
  strip.show();

  // queue pixels off
  for (int j=0; j<strip.numPixels(); j += spacing)
  {
    target_index = (j+index)%strip.numPixels();
    strip.setPixelColor(target_index, 0);
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
/// Parse the specified base64 string as a firmware command
/// </summary>
/// <param name=command>Base64 string to parse</param>
Settings ParseSettings(String command)
{
  // create new settings object
  Settings settings = Settings();
  // decode base64 string into bytes
  unsigned int needed_length = sizeof(settings);
  unsigned char bytes[needed_length];
  decode_base64(command.c_str(), bytes);
  // copy raw bytes into settings object
  memcpy(&settings, bytes, sizeof(settings));
  return settings;
}

void PrintSettings(Settings settings)
{
  Serial.println("Pattern Mode: " + String(settings.PatternMode));
  Serial.println("Pattern delay: " + String(settings.PatternDelay));
  Serial.println("Color Scheme: " + String(settings.ColorScheme));
  return;
}

/// <summary>
/// Read settings from EEPROM
/// </summary>
/// <returns> Settings retrieved from EEPROM </returns>
Settings ReadEEPROM()
{
  Serial.println("> Reading EEPROM...");
  // create new settings object
  Settings settings = Settings();
  // aggregate EEPROM bytes into settings object
  for (unsigned int i=0; i<sizeof(settings); i++)
  {
    *((char*)&settings + i) = EEPROM.read(0 + i);
  }
  return settings;
}

/// <summary>
/// Read all available serial input
/// </summary>
/// <remarks> Sets a flag whenever a newline character is found </remarks>
void ReadSerial()
{
  while (Serial.available())
  {
    // get the new byte:
    char input_char = (char)Serial.read();
    // check for newline
    if (input_char == '\n')
    {
      // parse command and update settings
      if(inputString == "ID;")
      {
        SendDiscoveryResponse();
      }
      else if(inputString == "ES;")
      {
        SendSettingsResponse(currentSettings);
      }
      else
      {        
        currentSettings = ParseSettings(inputString);
        ValidateCurrentSettings();
        WriteEEPROM(currentSettings);
        ClearStrip();
      }
      inputString = "";
      Serial.flush();
      break;
    }
    // add it to the inputString:
    inputString += input_char;
  }
  return;
}


/// <summary>
/// Respond to a device discovery ping via serial communication
/// </summary>
void SendDiscoveryResponse()
{
  Serial.println(DISCOVERY_RESPONSE);
  return;
}

/// <summary>
/// Send specified settings via serial communication
/// </summary>
/// <param name=settings>Settings to send via serial</param>
void SendSettingsResponse(Settings settings)
{
  // retrieve raw bytes of settings object
  unsigned int original_length = sizeof(settings);
  unsigned char bytes[original_length];
  memcpy(bytes, &settings, original_length);
  // encode raw bytes as base-64 string
  unsigned int needed_length = encode_base64_length(original_length);
  char base64[needed_length];
  encode_base64(bytes, original_length, base64);
  // send base-64 string
  Serial.println(base64);
  Settings validate = ParseSettings(base64);
  if(settings.PatternDelay != validate.PatternDelay)
    Serial.println("Settings serialization error");
  return;
}

void ValidateCurrentSettings()
{
  if(currentSettings.PatternDelay < 10)
    currentSettings.PatternDelay = 10;
  return;
}

/// <summary>
/// Write specified settings to device EEPROM
/// </summary>
/// <param name=settings>Settings to write to EEPROM</param>
void WriteEEPROM(Settings settings)
{
  EEPROM.put(0, settings);
  return;
}

void serialEvent()
{
  ReadSerial();
  return;
}
