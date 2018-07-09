#define ADR_ACTION_TYPE 0
#define ADR_DELAY 2

#define WHEEL_SIZE 384 // wheel sizes > 384 will cause gaps in the rainbow, while < 384 will cause truncated rainbow

#include <EEPROM.h>
#include "LPD8806.h"
#include "SPI.h" // Comment out this line if using Trinket or Gemma

enum ActionTypes {
  Unknown,
  Rainbow,
  RainbowCycle,
  TheaterRainbowCycle,
  Manual,


  
  Discovery,
  ExportSettings
};

// Number of RGB LEDs in strand
int countLEDs = 32;
// LED strip instance
LPD8806 strip = LPD8806(countLEDs);
// Current action type
ActionTypes currentActionType = Unknown;
int currentDelay = 10;
// Current serial input string
String inputString = "";
bool stringComplete = false;

// Perform one-time start-up routines
void setup()
{
  // Initialize serial
  Serial.begin(9600);
  while(!Serial){;}
  Serial.println("> Initializing Arduino...");
  // Read last settings
  ReadSettings();
  // Initialize the LED strip
  strip.begin();
  // Color the strip black
  LightFill(strip.Color(0,   0,   0), 50);  // Red
  // Turn all LEDs off
  strip.show();
  return;
}


void loop()
{
  /*
  // Send a simple pixel chase in...
  colorChase(strip.Color(127, 127, 127), 50); // White
  colorChase(strip.Color(127,   0,   0), 50); // Red
  colorChase(strip.Color(127, 127,   0), 50); // Yellow
  colorChase(strip.Color(  0, 127,   0), 50); // Green
  colorChase(strip.Color(  0, 127, 127), 50); // Cyan
  colorChase(strip.Color(  0,   0, 127), 50); // Blue
  colorChase(strip.Color(127,   0, 127), 50); // Violet

  // Send a theater pixel chase in...
  theaterChase(strip.Color(127, 127, 127), 50); // White
  theaterChase(strip.Color(127,   0,   0), 50); // Red
  theaterChase(strip.Color(127, 127,   0), 50); // Yellow
  theaterChase(strip.Color(  0, 127,   0), 50); // Green
  theaterChase(strip.Color(  0, 127, 127), 50); // Cyan
  theaterChase(strip.Color(  0,   0, 127), 50); // Blue
  theaterChase(strip.Color(127,   0, 127), 50); // Violet

  // Fill the entire strip with...
  colorWipe(strip.Color(127,   0,   0), 50);  // Red
  colorWipe(strip.Color(  0, 127,   0), 50);  // Green
  colorWipe(strip.Color(  0,   0, 127), 50);  // Blue
  */

  //rainbow(10);
  if(stringComplete)
  {
    ReadCommand();
    WriteSettings();
  }
  switch(currentActionType)
  {
    case Rainbow:
      LightRainbow(currentDelay);
      break;
    case RainbowCycle:
      LightRainbowCycle(currentDelay);
      break;
    case TheaterRainbowCycle:
      LightTheaterRainbowCycle(currentDelay);
      break;   
    case Manual:
      LightFill(strip.Color(0,   0,   0), currentDelay);
      break;
    default:
    case Unknown:
      LightFill(strip.Color(0,   0,   0), currentDelay);
      break;
      
  }
}

// Chase one dot down the full strip.
void colorChase(uint32_t c, uint8_t wait)
{
  int i;

  // Start by turning all pixels off:
  for(i=0; i<strip.numPixels() && !stringComplete; i++)
  {
    strip.setPixelColor(i, 0);
  }

  // Then display one pixel at a time:
  for(i=0; i<strip.numPixels() && !stringComplete; i++)
  {
    strip.setPixelColor(i, c); // Set new pixel 'on'
    strip.show();              // Refresh LED states
    strip.setPixelColor(i, 0); // Erase pixel, but don't refresh!
    
    delay(wait);
    ReadSerial();
  }

  strip.show(); // Refresh to turn off last pixel
}

//Theatre-style crawling lights.
void theaterChase(uint32_t c, uint8_t wait)
{
  for (int j=0; j<10 && !stringComplete; j++)
  {  //do 10 cycles of chasing
    for (int q=0; q < 3 && !stringComplete; q++)
    {
      for (int i=0; i < strip.numPixels() && !stringComplete; i=i+3)
      {
        strip.setPixelColor(i+q, c);    //turn every third pixel on
      }
      strip.show();
     
      delay(wait);
      ReadSerial();
     
      for (int i=0; i < strip.numPixels() && !stringComplete; i=i+3)
      {
        strip.setPixelColor(i+q, 0);        //turn every third pixel off
      }
    }
  }
}

/// <summary>
/// Retrieve a color associated with the specified wheel position
/// </summary>
/// <param name=position>Position on the color wheel</position>
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

// Fill the dots progressively along the strip.
void LightFill(uint32_t c, uint8_t wait) {
  int i;

  for (i=0; i < strip.numPixels() && !stringComplete; i++)
  {
      strip.setPixelColor(i, c);
      strip.show();
      
      delay(wait);
      ReadSerial();
  }
}


void LightRainbow(uint8_t wait)
{
  int i, j;
   
  for (j=0; j < WHEEL_SIZE && !stringComplete; j++)
  {
    for (i=0; i < strip.numPixels() && !stringComplete; i++)
    {
      strip.setPixelColor(i, GetWheelColor( (i + j) % WHEEL_SIZE));
    }  
    strip.show();   // write all the pixels out
    
    delay(wait);
    ReadSerial();
  }
}

// Slightly different, this one makes the rainbow wheel equally distributed 
// along the chain
void LightRainbowCycle(uint8_t wait)
{
  uint16_t i, j;
    
  for (j=0; j < WHEEL_SIZE * 5 && !stringComplete; j++)
  {     // 5 cycles of all 384 colors in the wheel
    for (i=0; i < strip.numPixels() && !stringComplete; i++)
    {
      // tricky math! we use each pixel as a fraction of the full 384-color wheel
      // (thats the i / strip.numPixels() part)
      // Then add in j which makes the colors go around per pixel
      // the % 384 is to make the wheel cycle around
      strip.setPixelColor(i, GetWheelColor( ((i * 384 / strip.numPixels()) + j) % WHEEL_SIZE) );
    }  
    strip.show();   // write all the pixels out
    
    delay(wait);
    ReadSerial();
  }
}

//Theatre-style crawling lights with rainbow effect
void LightTheaterRainbowCycle(uint8_t wait)
{
  for (int j=0; j < WHEEL_SIZE && !stringComplete; j++)
  {     // cycle all 384 colors in the wheel
    for (int q=0; q < 3 && !stringComplete; q++)
    {
        for (int i=0; i < strip.numPixels(); i=i+3)
        {
          strip.setPixelColor(i+q, GetWheelColor( (i+j) % WHEEL_SIZE));    //turn every third pixel on
        }
        strip.show();
       
        delay(wait);
        ReadSerial();
       
        for (int i=0; i < strip.numPixels() && !stringComplete; i=i+3)
        {
          strip.setPixelColor(i+q, 0);        //turn every third pixel off
        }
    }
  }
}

void ReadCommand()
{
  String temp_str = "";
  Serial.println("> Reading Command... ");
  // clear input string and mark as read
  String input_string = inputString;
  stringComplete = false;
  inputString = "";
  // determine input string length
  int str_length = input_string.length();
  // parse action type
  if(str_length < 1)
    return;
  int semicolon_index = input_string.indexOf(';', 0);
  int old_semicolon_index = 0;
  if (semicolon_index < 1)
  {
    Serial.println("! Command Warning: First Semicolon Not Found, Cannot Parse Action Type");
    return;
  }
  temp_str = input_string.substring(old_semicolon_index, semicolon_index);
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
    // parse delay
    old_semicolon_index = semicolon_index;
    semicolon_index = input_string.indexOf(';', old_semicolon_index+1);
    if (semicolon_index < 1)
    {
      Serial.println("! Command Warning: Second Semicolon Not Found, Cannot Parse Delay Value");
      return;
    }
    temp_str = input_string.substring(old_semicolon_index+1, semicolon_index);
    Serial.println("> Parsing Command String to Delay Value: "+temp_str);
    int delay_value = temp_str.toInt();
    if(delay_value != 0)
       currentDelay = delay_value;
    Serial.println("> Delay Value: "+String(currentDelay));
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
      stringComplete = true;
    }
    return;
  }
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
  // read delay value
  value = EEPROM.read(ADR_DELAY);
  if(value == 255)
    currentDelay = 10;
  else
    currentDelay = value;
  Serial.println("> Delay: "+String(currentDelay));
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
  // .. add current delay value
  text += String(currentDelay) + ";";
  // .. add suffix
  text += ":ES:";
  // send string
  Serial.println(text);
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
    case Manual:
      return "Manual";
    case Rainbow:
      return "Basic Rainbow";
    case RainbowCycle:
      return "Rainbow Cycle";
    case TheaterRainbowCycle:
      return "Theater Rainbow Cycle";
    case Unknown:
      return "Unknown";
    default:
      return "Undefined!";
  }
}

String ActionTypeToCommandString(ActionTypes action_type)
{
  switch(action_type)
  {
    case Manual:
      return "m";
    case Rainbow:
      return "b";
    case RainbowCycle:
      return "r";
    case TheaterRainbowCycle:
      return "t";
    case Unknown:
    default:
      return "?";
  }
}

ActionTypes CommandStringToActionType(String text)
{
  Serial.println("> Parsing Command String to Action Type: "+text);
  // LED Actions
  if (text == "b")
      return Rainbow;
  if (text== "r")
      return RainbowCycle;
  if (text == "t")
      return TheaterRainbowCycle;
  if (text == "m")
      return Manual;

  // Serial Interface Actions
  if (text == "ID")
      return Discovery;
  if (text == "ES")
      return ExportSettings;
  return Unknown;
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

void WriteSettings()
{
  EEPROM.write(ADR_ACTION_TYPE, currentActionType);
  EEPROM.write(ADR_DELAY, currentDelay);
  return;
}

void serialEvent()
{
  ReadSerial();
  return;
}
