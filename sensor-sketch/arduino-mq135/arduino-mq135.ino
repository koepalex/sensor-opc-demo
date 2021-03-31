#include <MQUnifiedsensor.h>

//Definitions
/*
 * MQ135 sensor
 */
#define Board "Arduino NANO"
#define Voltage_Resolution 5
#define Pin A0                          // MQ135 connected to PIN A0 of Arduino
#define MQType "MQ-135"                 // Kind of MQ sensor
#define ADC_Bit_Resolution 10           // For arduino UNO/MEGA/NANO
#define RatioMQ135CleanAir 3.6          // RS / R0 = 3.6 ppm  
/*
 * Input API
 */
#define CommandCalibrate 42                // control id for "calibrate" command
/*
 * Output API
 */
#define TelemetryTelegramType "t"       // type of telegram for telemetry messages
#define TelemetryTelegramVersion "v1.0" // version of telegram for telemetry messages
#define TelemetryIdentifierCO "CO"
#define TelemetryIdentifierAlcohol "Alcohol"
#define TelemetryIdentifierCO2 "CO2"
#define TelemetryIdentifierToluen "Toluen"
#define TelemetryIdentifierAmmonia "Ammonia"
#define TelemetryIdentifierAcetonia "Acetonia"
#define DiagnosticTelegramType "d"      // type of telegram for diagnostic messages
#define DiagnosticTelegramVersion "v1.0" // version of telegram for telemetry messages
#define DiagnosticLevelError "error"

//Global Variables
MQUnifiedsensor MQ135(Board, Voltage_Resolution, ADC_Bit_Resolution, Pin, MQType);
int incomingByte = 0;
unsigned long elapsedMilliseconds;

/*
 * Read from serial to see if the command was send to call clibration
 */
bool shouldCalibrate() {
  if (Serial.available() > 0) {
    incomingByte = Serial.read();    
    return (incomingByte == CommandCalibrate);
  }
  return false;
}

/*
 * Send sensor data via serial
 * format
 *  <type>|version>|<timestamp>|<identifier>|<value>|\n
 */
void sendTelemetry(String identifier, float value) {

  elapsedMilliseconds = millis();
  
  Serial.print(TelemetryTelegramType);
  Serial.print("|");
  Serial.print(TelemetryTelegramVersion);
  Serial.print("|");
  Serial.print(elapsedMilliseconds);
  Serial.print("|");
  Serial.print(identifier);
  Serial.print("|");
  Serial.print(value);
  Serial.println("|");
}

/*
 * Send diagnostic data via serial
 * format
 *  <type>|version>|<timestamp>|<level>|<message>|\n
 */
void sendDiagnostic(String diagonsticLevel, String message) {
  elapsedMilliseconds = millis();
  Serial.print(DiagnosticTelegramType);
  Serial.print("|");
  Serial.print(DiagnosticTelegramVersion);
  Serial.print("|");
  Serial.print(elapsedMilliseconds);
  Serial.print("|");
  Serial.print("diagonsticLevel");
  Serial.print("|");
  Serial.print("message");
  Serial.println("|");
}

/*
 * Calibrate the MQ135 sensor 
 * parameter: applyEpromValueOnly 
 *  true: read value from EPROM and apply
 *  false: active calibrate the sensor (requires sensor in fresh air)
 */
void calibrate(bool applyEpromValueOnly) {
  // TODO Check EEPROM if calibration value for R0 is available
  // https://www.arduino.cc/en/Tutorial/LibraryExamples/EEPROMRead
  
  float calcR0 = 0;
  for (int i = 1; i<=10; i++)
  {
    MQ135.update(); 
    calcR0 += MQ135.calibrate(RatioMQ135CleanAir);
  }
  MQ135.setR0(calcR0/10);
  
  if(isinf(calcR0)) {
    sendDiagnostic(DiagnosticLevelError, "Conection issue founded, R0 is infite (Open circuit detected) please check your wiring and supply"); 
    exit(-1);
  }
  if(calcR0 == 0){
    sendDiagnostic(DiagnosticLevelError, "Conection issue founded, R0 is zero (Analog pin with short circuit to ground) please check your wiring and supply");
    exit(-2);
  }

  // TODO store R0 value in EEPROM 
  // https://www.arduino.cc/en/Tutorial/LibraryExamples/EEPROMWrite
}

void setup() {
  //Init the serial port communication - to debug the library
  Serial.begin(9600); 

  //Set math model to calculate the PPM concentration and the value of constants
  MQ135.setRegressionMethod(1); //_PPM =  a*ratio^b
  
  // pinMode(Pin, INPUT); 
  MQ135.init(); 

  //depending on MQ135 type, the RL may different from 10K, 
  // e.g. 202 for smd resistor defines 2KOhm
  MQ135.setRL(2); 

  calibrate(true);
}

void loop() {
  /*
    Required values to calculate GAS from voltage of Analog Input
    GAS      | a      | b
    CO       | 605.18 | -3.937  
    Alcohol  | 77.255 | -3.18 
    CO2      | 110.47 | -2.862
    Tolueno  | 44.947 | -3.445
    NH4      | 102.2  | -2.473
    Acetona  | 34.668 | -3.369

    Calculation details see
    * https://jayconsystems.com/blog/understanding-a-gas-sensor
    * https://components101.com/sensors/mq135-gas-sensor-for-air-quality
  */
  if (shouldCalibrate()) {
    calibrate(false);
  }
  
  // analogRead(Pin) + calculate voltage
  MQ135.update(); 

  // CO
  MQ135.setA(605.18); 
  MQ135.setB(-3.937); 
  float COppm = MQ135.readSensor(); 
  sendTelemetry(TelemetryIdentifierCO, COppm);

  // Alcohol
  MQ135.setA(77.255); 
  MQ135.setB(-3.18); 
  float Alcoholppm = MQ135.readSensor();
  sendTelemetry(TelemetryIdentifierAlcohol, Alcoholppm);

  // CO2
  MQ135.setA(110.47); 
  MQ135.setB(-2.862); 
  float CO2ppm = MQ135.readSensor();
  CO2ppm += 400; //calibrated sensor assumes the current state of the air as 0 PPM, and it is considered today that the CO2 present in the atmosphere is around 400 PPM
  sendTelemetry(TelemetryIdentifierCO2, CO2ppm);
  
  // Tolueno (german: Methylbenzol)
  MQ135.setA(44.947);
  MQ135.setB(-3.445); 
  float Toluenoppm = MQ135.readSensor();
  sendTelemetry(TelemetryIdentifierToluen, Toluenoppm);

  // NH4 (german: Ammonium)
  MQ135.setA(102.2 );
  MQ135.setB(-2.473); 
  float NH4ppm = MQ135.readSensor();
  sendTelemetry(TelemetryIdentifierAmmonia, NH4ppm);

  // Acetona 
  MQ135.setA(34.668);
  MQ135.setB(-3.369);
  float Acetonappm = MQ135.readSensor();
  sendTelemetry(TelemetryIdentifierAcetonia, Acetonappm);


  delay(500); //Sampling frequency
}
