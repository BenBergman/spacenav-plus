/*
serialcommunication - serial communication for spaceball/magellan device.
Copyright (C) 2010 Thomas Anderson <ta@nextgenengineering.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/  

#ifndef SERIAL_COMMUNICATION_H_
#define SERIAL_COMMUNICATION_H_

int openFile(const char *devFile);
int setPortSpaceball(int fileDescriptor);
int setPortMagellan(int fileDescriptor);
void longWait();
void shortWait();
void serialWriteString(int fileDescriptor, char *string, int count);
int serialRead(int fileDescriptor, char *buffer, int bufferSize);
int serialWaitRead(int fileDescriptor, char *buffer, int bufferSize, int waitSeconds);

#endif
