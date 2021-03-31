# sensor-opc-demo

This is a example project to demonstrate an Industrial IoT scenario step by step.

## Scenarios
This chapter will describe the scenarios that should be demonstrated.
### Visualize Air Quality Data


## Repostory Overview

* [sensor-sketch](./sensor-sketch/README.md) - Contains all artifacts for simple IoT Sensor, that pushes data via serial (over USB)
* [sensor-opc-server](./sensor-opc-server/README.md) - Contains all artifacts for server that reads data from serial (over USB) and distributes the data on different ways; as initial version it stores the data on local hosted influxdb 
* [sensor-dashboard](./sensor-dashboard/README.md) - Contains all artifacts to visualize the sensor values (using influxdb and grafana)